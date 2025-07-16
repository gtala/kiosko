using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using KioskoMicroservice.Services;
using KioskoMicroservice.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace KioskoMicroservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MercadoLibreController : ControllerBase
    {
        private readonly IMercadoLibreService _mercadoLibreService;
        private readonly ILogger<MercadoLibreController> _logger;

        public MercadoLibreController(IMercadoLibreService mercadoLibreService, ILogger<MercadoLibreController> logger)
        {
            _mercadoLibreService = mercadoLibreService;
            _logger = logger;
        }

        /// <summary>
        /// Genera un code_verifier para PKCE
        /// </summary>
        /// <returns>Code verifier</returns>
        private string GenerateCodeVerifier()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
            }
            return Convert.ToBase64String(random)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        /// <summary>
        /// Genera un code_challenge para PKCE
        /// </summary>
        /// <param name="codeVerifier">Code verifier</param>
        /// <returns>Code challenge</returns>
        private string GenerateCodeChallenge(string codeVerifier)
        {
            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                return Convert.ToBase64String(challengeBytes)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
            }
        }

        /// <summary>
        /// Obtiene todos los productos de un usuario de MercadoLibre usando token de acceso
        /// </summary>
        /// <param name="accessToken">Token de acceso</param>
        /// <returns>Lista de productos del usuario</returns>
        [HttpGet("products/with-token")]
        public async Task<ActionResult<UserProductsResponse>> GetUserProductsWithToken([FromQuery] string accessToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    return BadRequest("El token de acceso es requerido");
                }

                _logger.LogInformation("Obteniendo productos reales usando token de acceso");
                
                var result = await _mercadoLibreService.GetUserProductsWithTokenAsync(accessToken);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo productos con token");
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene información del usuario usando token de acceso
        /// </summary>
        /// <param name="accessToken">Token de acceso</param>
        /// <returns>Información del usuario</returns>
        [HttpGet("user/with-token")]
        public async Task<ActionResult<MercadoLibreUser>> GetUserWithToken([FromQuery] string accessToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    return BadRequest("El token de acceso es requerido");
                }

                _logger.LogInformation("Obteniendo información del usuario usando token");
                
                var user = await _mercadoLibreService.GetUserWithTokenAsync(accessToken);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuario con token");
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todos los productos de un usuario de MercadoLibre
        /// </summary>
        /// <param name="userId">ID del usuario de MercadoLibre</param>
        /// <returns>Lista de productos del usuario</returns>
        [HttpGet("user/{userId}/products")]
        public async Task<ActionResult<UserProductsResponse>> GetUserProducts(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest("El ID del usuario es requerido");
                }

                _logger.LogInformation("Obteniendo productos del usuario {UserId}", userId);
                
                var result = await _mercadoLibreService.GetUserProductsAsync(userId);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo productos del usuario {UserId}", userId);
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene información de un usuario de MercadoLibre
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Información del usuario</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<MercadoLibreUser>> GetUser(string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return BadRequest("El ID del usuario es requerido");
                }

                var user = await _mercadoLibreService.GetUserAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuario {UserId}", userId);
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        /// <summary>
        /// Inicia el flujo de autenticación OAuth con MercadoLibre
        /// </summary>
        /// <returns>URL de autorización</returns>
        [HttpGet("auth")]
        public IActionResult StartAuth()
        {
            try
            {
                var config = HttpContext.RequestServices.GetRequiredService<IOptions<MercadoLibreConfig>>().Value;
                
                // Generar PKCE parameters
                var codeVerifier = GenerateCodeVerifier();
                var codeChallenge = GenerateCodeChallenge(codeVerifier);
                
                // Guardar code_verifier en session para usarlo en el callback
                HttpContext.Session.SetString("code_verifier", codeVerifier);
                
                // Según la documentación de MercadoLibre, necesitamos incluir el scope y PKCE
                var authUrl = $"{config.AuthUrl}?response_type=code&client_id={config.ClientId}&redirect_uri={Uri.EscapeDataString(config.RedirectUri)}&scope=read&code_challenge={codeChallenge}&code_challenge_method=S256";
                
                return Ok(new { authUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error iniciando autenticación");
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        /// <summary>
        /// Callback para recibir el código de autorización de MercadoLibre
        /// </summary>
        /// <param name="code">Código de autorización</param>
        /// <returns>Token de acceso</returns>
        [HttpGet("callback")]
        public async Task<IActionResult> AuthCallback([FromQuery] string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return BadRequest(new AuthCallbackResponse
                    {
                        Success = false,
                        Error = "Código de autorización requerido"
                    });
                }

                _logger.LogInformation("Recibido código de autorización: {Code}", code);
                
                // Obtener code_verifier de la session
                var codeVerifier = HttpContext.Session.GetString("code_verifier");
                if (string.IsNullOrEmpty(codeVerifier))
                {
                    return BadRequest(new AuthCallbackResponse
                    {
                        Success = false,
                        Error = "Code verifier no encontrado en la sesión"
                    });
                }
                
                // Intercambiar código por token de acceso con PKCE
                var tokenResponse = await _mercadoLibreService.ExchangeCodeForTokenAsync(code, codeVerifier);
                
                return Ok(new AuthCallbackResponse
                {
                    Success = true,
                    AccessToken = tokenResponse.AccessToken,
                    UserId = tokenResponse.UserId,
                    Message = "Autenticación exitosa"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en callback de autenticación");
                return StatusCode(500, new AuthCallbackResponse
                {
                    Success = false,
                    Error = "Error interno del servidor",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Prueba simple del token de acceso
        /// </summary>
        /// <param name="accessToken">Token de acceso</param>
        /// <returns>Información básica del token</returns>
        [HttpGet("test-token")]
        public async Task<IActionResult> TestToken([FromQuery] string accessToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    return BadRequest("El token de acceso es requerido");
                }

                _logger.LogInformation("Probando token de acceso");

                // Hacer una llamada simple a la API de MercadoLibre
                var httpClient = HttpContext.RequestServices.GetRequiredService<HttpClient>();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.mercadolibre.com/users/me");
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
                
                var response = await httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Respuesta de prueba. Status: {Status}, Response: {Response}", 
                    response.StatusCode, responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(500, new { 
                        error = "Error probando token", 
                        status = response.StatusCode,
                        response = responseContent 
                    });
                }

                // Parsear la respuesta JSON para devolver un objeto limpio
                var userData = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(responseContent);
                
                // Extraer solo la información relevante
                var cleanUserData = new
                {
                    id = userData.TryGetProperty("id", out var id) ? id.GetInt32() : 0,
                    nickname = userData.TryGetProperty("nickname", out var nickname) ? nickname.GetString() : "",
                    first_name = userData.TryGetProperty("first_name", out var firstName) ? firstName.GetString() : "",
                    last_name = userData.TryGetProperty("last_name", out var lastName) ? lastName.GetString() : "",
                    email = userData.TryGetProperty("email", out var email) ? email.GetString() : "",
                    country_id = userData.TryGetProperty("country_id", out var countryId) ? countryId.GetString() : "",
                    site_id = userData.TryGetProperty("site_id", out var siteId) ? siteId.GetString() : ""
                };

                return Ok(new { 
                    success = true,
                    message = "Token válido",
                    status = response.StatusCode,
                    user = cleanUserData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error probando token");
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        /// <summary>
        /// Prueba simple para obtener productos básicos
        /// </summary>
        /// <param name="accessToken">Token de acceso</param>
        /// <returns>Información básica de productos</returns>
        [HttpGet("test-products")]
        public async Task<IActionResult> TestProducts([FromQuery] string accessToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accessToken))
                {
                    return BadRequest("El token de acceso es requerido");
                }

                _logger.LogInformation("Probando obtención de productos básicos");

                // Obtener información del usuario primero
                var user = await _mercadoLibreService.GetUserWithTokenAsync(accessToken);
                
                // Hacer una llamada simple a la API de productos
                var httpClient = HttpContext.RequestServices.GetRequiredService<HttpClient>();
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.mercadolibre.com/users/{user.Id}/items/search");
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
                
                var response = await httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Respuesta de productos. Status: {Status}, Response: {Response}", 
                    response.StatusCode, responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(500, new { 
                        error = "Error obteniendo productos", 
                        status = response.StatusCode,
                        response = responseContent 
                    });
                }

                // Parsear la respuesta JSON para devolver un objeto limpio
                var productsData = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(responseContent);
                
                // Extraer solo la información relevante
                var cleanProductsData = new
                {
                    seller_id = productsData.TryGetProperty("seller_id", out var sellerId) ? sellerId.GetString() : "",
                    total = productsData.TryGetProperty("paging", out var paging) && paging.TryGetProperty("total", out var total) ? total.GetInt32() : 0,
                    results = productsData.TryGetProperty("results", out var results) ? results.EnumerateArray().Select(r => r.GetString()).ToArray() : new string[0]
                };

                return Ok(new { 
                    success = true,
                    message = "Productos obtenidos correctamente",
                    userId = user.Id,
                    status = response.StatusCode,
                    products = cleanProductsData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error probando productos");
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }
    }
} 