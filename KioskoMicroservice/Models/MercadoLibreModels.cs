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
    /// Producto detallado de MercadoLibre con información completa
    /// </summary>
    public class ProductDetail
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("currency_id")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("available_quantity")]
        public int AvailableQuantity { get; set; }

        [JsonPropertyName("sold_quantity")]
        public int SoldQuantity { get; set; }

        [JsonPropertyName("condition")]
        public string Condition { get; set; } = string.Empty;

        [JsonPropertyName("permalink")]
        public string Permalink { get; set; } = string.Empty;

        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; } = string.Empty;

        [JsonPropertyName("pictures")]
        public List<ProductPicture> Pictures { get; set; } = new List<ProductPicture>();

        [JsonPropertyName("attributes")]
        public List<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();

        [JsonPropertyName("shipping")]
        public ProductShipping Shipping { get; set; } = new ProductShipping();

        [JsonPropertyName("seller_address")]
        public ProductSellerAddress SellerAddress { get; set; } = new ProductSellerAddress();

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("listing_type_id")]
        public string ListingType { get; set; } = string.Empty;

        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("end_time")]
        public DateTime EndTime { get; set; }

        [JsonPropertyName("date_created")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }

        [JsonPropertyName("health")]
        public decimal Health { get; set; }

        [JsonPropertyName("warranty")]
        public string Warranty { get; set; } = string.Empty;

        [JsonPropertyName("domain_id")]
        public string DomainId { get; set; } = string.Empty;

        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; } = string.Empty;

        [JsonPropertyName("seller_id")]
        public int SellerId { get; set; }

        [JsonPropertyName("variations")]
        public List<ProductVariation> Variations { get; set; } = new List<ProductVariation>();
    }

    /// <summary>
    /// Imagen de producto
    /// </summary>
    public class ProductPicture
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("secure_url")]
        public string SecureUrl { get; set; } = string.Empty;

        [JsonPropertyName("size")]
        public string Size { get; set; } = string.Empty;

        [JsonPropertyName("max_size")]
        public string MaxSize { get; set; } = string.Empty;

        [JsonPropertyName("quality")]
        public string Quality { get; set; } = string.Empty;
    }

    /// <summary>
    /// Atributo de producto
    /// </summary>
    public class ProductAttribute
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("value_id")]
        public string? ValueId { get; set; }

        [JsonPropertyName("value_name")]
        public string? ValueName { get; set; }

        [JsonPropertyName("value_type")]
        public string ValueType { get; set; } = string.Empty;

        [JsonPropertyName("values")]
        public List<AttributeValue> Values { get; set; } = new List<AttributeValue>();
    }

    /// <summary>
    /// Valor de atributo
    /// </summary>
    public class AttributeValue
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("struct")]
        public AttributeStruct? Struct { get; set; }
    }

    /// <summary>
    /// Estructura de atributo
    /// </summary>
    public class AttributeStruct
    {
        [JsonPropertyName("number")]
        public decimal? Number { get; set; }

        [JsonPropertyName("unit")]
        public string? Unit { get; set; }
    }

    /// <summary>
    /// Información de envío
    /// </summary>
    public class ProductShipping
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; } = string.Empty;

        [JsonPropertyName("free_shipping")]
        public bool FreeShipping { get; set; }

        [JsonPropertyName("local_pick_up")]
        public bool LocalPickUp { get; set; }

        [JsonPropertyName("store_pick_up")]
        public bool StorePickUp { get; set; }

        [JsonPropertyName("logistic_type")]
        public string LogisticType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Dirección del vendedor
    /// </summary>
    public class ProductSellerAddress
    {
        [JsonPropertyName("comment")]
        public string Comment { get; set; } = string.Empty;

        [JsonPropertyName("address_line")]
        public string AddressLine { get; set; } = string.Empty;

        [JsonPropertyName("zip_code")]
        public string ZipCode { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public ProductLocation City { get; set; } = new ProductLocation();

        [JsonPropertyName("state")]
        public ProductLocation State { get; set; } = new ProductLocation();

        [JsonPropertyName("country")]
        public ProductLocation Country { get; set; } = new ProductLocation();

        [JsonPropertyName("latitude")]
        public decimal Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public decimal Longitude { get; set; }
    }

    /// <summary>
    /// Ubicación
    /// </summary>
    public class ProductLocation
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Variación de producto
    /// </summary>
    public class ProductVariation
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("available_quantity")]
        public int AvailableQuantity { get; set; }

        [JsonPropertyName("sold_quantity")]
        public int SoldQuantity { get; set; }

        [JsonPropertyName("attribute_combinations")]
        public List<ProductAttribute> AttributeCombinations { get; set; } = new List<ProductAttribute>();

        [JsonPropertyName("picture_ids")]
        public List<string> PictureIds { get; set; } = new List<string>();
    }

    /// <summary>
    /// Respuesta de productos detallados
    /// </summary>
    public class ProductDetailsResponse
    {
        public string UserId { get; set; } = string.Empty;
        public List<ProductDetail> Products { get; set; } = new List<ProductDetail>();
        public int TotalProducts { get; set; }
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