using System.Text.Json.Serialization;

namespace KioskoMicroservice.Models
{
    /// <summary>
    /// Respuesta de productos de un usuario
    /// </summary>
    public class UserProductsResponse
    {
        public string UserId { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = new List<Product>();
        public int TotalProducts { get; set; }
    }

    /// <summary>
    /// Producto de MercadoLibre
    /// </summary>
    public class Product
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Pictures { get; set; } = new List<string>();
        public string Permalink { get; set; } = string.Empty;
    }

    /// <summary>
    /// Usuario de MercadoLibre
    /// </summary>
    public class MercadoLibreUser
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; } = string.Empty;

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("country_id")]
        public string CountryId { get; set; } = string.Empty;

        [JsonPropertyName("site_id")]
        public string SiteId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Respuesta de token de acceso OAuth 2.0
    /// </summary>
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; } = string.Empty;

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }
    }

    /// <summary>
    /// Error de token OAuth 2.0
    /// </summary>
    public class TokenErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; } = string.Empty;
    }

    /// <summary>
    /// Respuesta del callback de autenticación
    /// </summary>
    public class AuthCallbackResponse
    {
        public bool Success { get; set; }
        public string? AccessToken { get; set; }
        public string? Error { get; set; }
        public string? Message { get; set; }
        public int? UserId { get; set; }
    }
} 