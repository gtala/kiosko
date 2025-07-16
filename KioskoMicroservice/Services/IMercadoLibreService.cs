using KioskoMicroservice.Models;

namespace KioskoMicroservice.Services
{
    public interface IMercadoLibreService
    {
        /// <summary>
        /// Obtiene los productos de un usuario de MercadoLibre usando token de acceso
        /// </summary>
        /// <param name="accessToken">Token de acceso</param>
        /// <returns>Lista de productos del usuario</returns>
        Task<UserProductsResponse> GetUserProductsWithTokenAsync(string accessToken);

        /// <summary>
        /// Obtiene los productos de un usuario de MercadoLibre (simulado para desarrollo)
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Lista de productos del usuario</returns>
        Task<UserProductsResponse> GetUserProductsAsync(string userId);

        /// <summary>
        /// Obtiene información de un usuario de MercadoLibre usando token de acceso
        /// </summary>
        /// <param name="accessToken">Token de acceso</param>
        /// <returns>Información del usuario</returns>
        Task<MercadoLibreUser> GetUserWithTokenAsync(string accessToken);

        /// <summary>
        /// Obtiene información de un usuario de MercadoLibre (simulado para desarrollo)
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Información del usuario</returns>
        Task<MercadoLibreUser> GetUserAsync(string userId);

        /// <summary>
        /// Intercambia el código de autorización por un token de acceso
        /// </summary>
        /// <param name="code">Código de autorización</param>
        /// <param name="codeVerifier">Code verifier para PKCE</param>
        /// <returns>Token de acceso</returns>
        Task<TokenResponse> ExchangeCodeForTokenAsync(string code, string codeVerifier);
    }
} 