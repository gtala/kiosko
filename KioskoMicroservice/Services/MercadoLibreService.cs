using Microsoft.Extensions.Options;
using KioskoMicroservice.Models;
using System.Text;
using System.Text.Json;

namespace KioskoMicroservice.Services
{
    public class MercadoLibreService : IMercadoLibreService
    {
        private readonly MercadoLibreConfig _config;
        private readonly HttpClient _httpClient;
        private readonly ILogger<MercadoLibreService> _logger;

        public MercadoLibreService(IOptions<MercadoLibreConfig> config, HttpClient httpClient, ILogger<MercadoLibreService> logger)
        {
            _config = config.Value;
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene los productos de un usuario de MercadoLibre usando token de acceso
        /// </summary>
        /// <param name="accessToken">Token de acceso</param>
        /// <returns>Lista de productos del usuario</returns>
        public async Task<UserProductsResponse> GetUserProductsWithTokenAsync(string accessToken)
        {
            try
            {
                _logger.LogInformation("Obteniendo productos reales usando token de acceso");

                // Primero obtener información del usuario
                var user = await GetUserWithTokenAsync(accessToken);
                
                // Obtener productos del usuario usando el user ID
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.mercadolibre.com/users/{user.Id}/items/search");
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
                
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error obteniendo productos. Status: {Status}, Response: {Response}", 
                        response.StatusCode, responseContent);
                    throw new Exception($"Error obteniendo productos: {response.StatusCode}");
                }

                var searchResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                
                // Obtener los IDs de los productos
                var itemIds = new List<string>();
                if (searchResponse.TryGetProperty("results", out var results))
                {
                    foreach (var item in results.EnumerateArray())
                    {
                        // Los resultados son strings (IDs), no objetos
                        itemIds.Add(item.GetString() ?? "");
                    }
                }

                _logger.LogInformation("Encontrados {Count} productos", itemIds.Count);

                // Obtener detalles de cada producto
                var products = new List<Product>();
                foreach (var itemId in itemIds.Take(10)) // Limitar a 10 productos para evitar rate limiting
                {
                    try
                    {
                        var product = await GetProductDetailsAsync(itemId, accessToken);
                        if (product != null)
                        {
                            products.Add(product);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error obteniendo detalles del producto {ItemId}", itemId);
                    }
                }

                return new UserProductsResponse
                {
                    UserId = user.Id.ToString(),
                    Products = products,
                    TotalProducts = products.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo productos con token");
                throw;
            }
        }

        /// <summary>
        /// Obtiene los productos de un usuario de MercadoLibre (simulado para desarrollo)
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Lista de productos del usuario</returns>
        public async Task<UserProductsResponse> GetUserProductsAsync(string userId)
        {
            try
            {
                _logger.LogInformation("Obteniendo productos del usuario {UserId}", userId);

                // Simulación de datos para desarrollo
                var products = new List<Product>
                {
                    new Product
                    {
                        Id = "MLA123456789",
                        Title = "iPhone 14 Pro Max 256GB",
                        Price = 1500000,
                        Currency = "ARS",
                        Condition = "new",
                        Category = "Celulares y Teléfonos",
                        Pictures = new List<string> { "https://http2.mlstatic.com/D_123456-MLA123456789_123456-O.jpg" },
                        Permalink = "https://articulo.mercadolibre.com.ar/MLA123456789"
                    },
                    new Product
                    {
                        Id = "MLA987654321",
                        Title = "MacBook Air M2 13 pulgadas",
                        Price = 2500000,
                        Currency = "ARS",
                        Condition = "new",
                        Category = "Computación",
                        Pictures = new List<string> { "https://http2.mlstatic.com/D_987654-MLA987654321_987654-O.jpg" },
                        Permalink = "https://articulo.mercadolibre.com.ar/MLA987654321"
                    }
                };

                return new UserProductsResponse
                {
                    UserId = userId,
                    Products = products,
                    TotalProducts = products.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo productos del usuario {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene información de un usuario de MercadoLibre usando token de acceso
        /// </summary>
        /// <param name="accessToken">Token de acceso</param>
        /// <returns>Información del usuario</returns>
        public async Task<MercadoLibreUser> GetUserWithTokenAsync(string accessToken)
        {
            try
            {
                _logger.LogInformation("Obteniendo información del usuario usando token");

                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.mercadolibre.com/users/me");
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
                
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Respuesta del usuario. Status: {Status}, Response: {Response}", 
                    response.StatusCode, responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error obteniendo usuario. Status: {Status}, Response: {Response}", 
                        response.StatusCode, responseContent);
                    throw new Exception($"Error obteniendo usuario: {response.StatusCode}");
                }

                // Deserializar directamente usando el modelo con JsonPropertyName
                var user = JsonSerializer.Deserialize<MercadoLibreUser>(responseContent);
                
                if (user == null)
                {
                    throw new Exception("No se pudo deserializar la información del usuario");
                }

                _logger.LogInformation("Usuario obtenido exitosamente: {UserId}", user.Id);
                
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuario con token");
                throw;
            }
        }

        /// <summary>
        /// Obtiene información de un usuario de MercadoLibre (simulado para desarrollo)
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Información del usuario</returns>
        public async Task<MercadoLibreUser> GetUserAsync(string userId)
        {
            try
            {
                _logger.LogInformation("Obteniendo información del usuario {UserId}", userId);

                // Simulación de datos para desarrollo
                return new MercadoLibreUser
                {
                    Id = int.TryParse(userId, out var id) ? id : 0,
                    Nickname = "usuario_ejemplo",
                    FirstName = "Juan",
                    LastName = "Pérez",
                    Email = "juan.perez@example.com",
                    CountryId = "AR",
                    SiteId = "MLA"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuario {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene detalles de un producto específico
        /// </summary>
        /// <param name="itemId">ID del producto</param>
        /// <param name="accessToken">Token de acceso</param>
        /// <returns>Detalles del producto</returns>
        private async Task<Product?> GetProductDetailsAsync(string itemId, string accessToken)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.mercadolibre.com/items/{itemId}");
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
                
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Error obteniendo detalles del producto {ItemId}. Status: {Status}", 
                        itemId, response.StatusCode);
                    return null;
                }

                var itemData = JsonSerializer.Deserialize<JsonElement>(responseContent);
                
                var pictures = new List<string>();
                if (itemData.TryGetProperty("pictures", out var picturesArray))
                {
                    foreach (var picture in picturesArray.EnumerateArray())
                    {
                        if (picture.TryGetProperty("url", out var url))
                        {
                            pictures.Add(url.GetString() ?? "");
                        }
                    }
                }

                return new Product
                {
                    Id = itemData.TryGetProperty("id", out var id) ? id.GetString() ?? "" : "",
                    Title = itemData.TryGetProperty("title", out var title) ? title.GetString() ?? "" : "",
                    Price = itemData.TryGetProperty("price", out var price) ? price.GetDecimal() : 0,
                    Currency = itemData.TryGetProperty("currency_id", out var currency) ? currency.GetString() ?? "" : "",
                    Condition = itemData.TryGetProperty("condition", out var condition) ? condition.GetString() ?? "" : "",
                    Category = itemData.TryGetProperty("category_id", out var category) ? category.GetString() ?? "" : "",
                    Pictures = pictures,
                    Permalink = itemData.TryGetProperty("permalink", out var permalink) ? permalink.GetString() ?? "" : ""
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error obteniendo detalles del producto {ItemId}", itemId);
                return null;
            }
        }

        /// <summary>
        /// Intercambia el código de autorización por un token de acceso
        /// </summary>
        /// <param name="code">Código de autorización</param>
        /// <param name="codeVerifier">Code verifier para PKCE</param>
        /// <returns>Token de acceso</returns>
        public async Task<TokenResponse> ExchangeCodeForTokenAsync(string code, string codeVerifier)
        {
            try
            {
                _logger.LogInformation("Intercambiando código por token de acceso. Código: {Code}", code);

                // MercadoLibre espera form-urlencoded, no JSON
                var formData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_id", _config.ClientId),
                    new KeyValuePair<string, string>("client_secret", _config.ClientSecret),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", _config.RedirectUri),
                    new KeyValuePair<string, string>("code_verifier", codeVerifier)
                };

                var content = new FormUrlEncodedContent(formData);
                
                // Asegurar que el Content-Type sea application/x-www-form-urlencoded
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

                _logger.LogInformation("Enviando solicitud de token a: {TokenUrl}", _config.TokenUrl);
                _logger.LogInformation("Client ID: {ClientId}, Redirect URI: {RedirectUri}", _config.ClientId, _config.RedirectUri);

                var response = await _httpClient.PostAsync(_config.TokenUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Respuesta del token. Status: {Status}, Response: {Response}", 
                    response.StatusCode, responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error intercambiando token. Status: {Status}, Response: {Response}", 
                        response.StatusCode, responseContent);
                    
                    var errorResponse = JsonSerializer.Deserialize<TokenErrorResponse>(responseContent);
                    throw new Exception($"Error de token: {errorResponse?.Error} - {errorResponse?.ErrorDescription}");
                }

                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
                
                if (tokenResponse == null)
                {
                    throw new Exception("No se pudo deserializar la respuesta del token");
                }

                _logger.LogInformation("Token de acceso obtenido exitosamente para usuario {UserId}", tokenResponse.UserId);
                
                return tokenResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error intercambiando código por token");
                throw;
            }
        }
    }
} 