using System;

namespace PersonelApi.Models
{
    public class MinioSettings
    {
        public const string SectionName = "Minio";

        public string Endpoint { get; set; } = "localhost:9000";
        public string AccessKey { get; set; } = "minioadmin";
        public string SecretKey { get; set; } = "minioadminsecretkey123";
        public string BucketName { get; set; } = "personel-cvler";
        public bool WithSSL { get; set; } = false;

        /// <summary>
        /// AWSSDK.S3 ServiceURL için tam protokol (http:// veya https://) içeren güvenli URL döner.
        /// </summary>
        public string GetServiceUrl()
        {
            var rawEndpoint = string.IsNullOrWhiteSpace(Endpoint)
                ? "localhost:9000"
                : Endpoint.Trim().TrimEnd('/');

            if (rawEndpoint.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                rawEndpoint.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return rawEndpoint;
            }

            var scheme = WithSSL ? "https://" : "http://";
            return $"{scheme}{rawEndpoint}";
        }
    }
}
