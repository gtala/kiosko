namespace KioskoMicroservice.Models
{
    public class MercadoLibreConfig
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
        public string AuthUrl { get; set; } = string.Empty;
        public string TokenUrl { get; set; } = string.Empty;
    }
} 