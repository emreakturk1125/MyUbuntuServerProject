using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersonelApi.Models;

namespace PersonelApi.Services
{
    public class MinioStorageService : IStorageService, IDisposable
    {
        private readonly IAmazonS3 _s3Client;
        private readonly MinioSettings _minioSettings;
        private readonly ILogger<MinioStorageService> _logger;
        private bool _disposed;

        public MinioStorageService(
            IOptions<MinioSettings> minioOptions,
            IConfiguration configuration,
            ILogger<MinioStorageService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _minioSettings = minioOptions?.Value ?? new MinioSettings();

            // IConfiguration üzerinden doğrudan okuma ve Docker Environment Variable (örn: Minio__Endpoint) override desteği
            if (configuration != null)
            {
                var minioSection = configuration.GetSection(MinioSettings.SectionName);
                if (!string.IsNullOrWhiteSpace(minioSection["Endpoint"]))
                {
                    _minioSettings.Endpoint = minioSection["Endpoint"];
                }
                if (!string.IsNullOrWhiteSpace(minioSection["AccessKey"]))
                {
                    _minioSettings.AccessKey = minioSection["AccessKey"];
                }
                if (!string.IsNullOrWhiteSpace(minioSection["SecretKey"]))
                {
                    _minioSettings.SecretKey = minioSection["SecretKey"];
                }
                if (!string.IsNullOrWhiteSpace(minioSection["BucketName"]))
                {
                    _minioSettings.BucketName = minioSection["BucketName"];
                }
                if (bool.TryParse(minioSection["WithSSL"], out bool withSsl))
                {
                    _minioSettings.WithSSL = withSsl;
                }
            }

            var serviceUrl = _minioSettings.GetServiceUrl();

            var s3Config = new AmazonS3Config
            {
                ServiceURL = serviceUrl,
                ForcePathStyle = true,
                UseHttp = !_minioSettings.WithSSL
            };

            var credentials = new BasicAWSCredentials(_minioSettings.AccessKey, _minioSettings.SecretKey);
            _s3Client = new AmazonS3Client(credentials, s3Config);

            _logger.LogInformation(
                "MinioStorageService başlatıldı. ServiceURL: {ServiceUrl}, BucketName: {BucketName}, WithSSL: {WithSSL}",
                serviceUrl, _minioSettings.BucketName, _minioSettings.WithSSL);
        }

        public async Task EnsureBucketExistsAsync()
        {
            try
            {
                var bucketName = _minioSettings.BucketName;
                if (string.IsNullOrWhiteSpace(bucketName))
                {
                    throw new InvalidOperationException("MinIO BucketName ayarı (appsettings.json / Environment) boş olamaz.");
                }

                bool bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
                if (!bucketExists)
                {
                    _logger.LogInformation("MinIO üzerinde '{BucketName}' bucket bulunamadı, otomatik oluşturuluyor...", bucketName);
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };
                    await _s3Client.PutBucketAsync(putBucketRequest);
                    _logger.LogInformation("MinIO '{BucketName}' bucket başarıyla oluşturuldu.", bucketName);
                }
                else
                {
                    _logger.LogInformation("MinIO '{BucketName}' bucket halihazırda mevcut.", bucketName);
                }
            }
            catch (AmazonS3Exception s3Ex)
            {
                _logger.LogError(s3Ex, "MinIO bucket kontrolü/oluşturulması sırasında S3 hatası oluştu. Hata Kodu: {ErrorCode}", s3Ex.ErrorCode);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MinIO bucket kontrolü/oluşturulması sırasında beklenmeyen bir hata oluştu.");
                throw;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder = "")
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Yüklenecek dosya boş olamaz.", nameof(file));
            }

            try
            {
                await EnsureBucketExistsAsync();

                var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? string.Empty;
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var objectKey = string.IsNullOrWhiteSpace(folder)
                    ? uniqueFileName
                    : $"{folder.Trim('/')}/{uniqueFileName}";

                using var inputStream = file.OpenReadStream();
                var putRequest = new PutObjectRequest
                {
                    BucketName = _minioSettings.BucketName,
                    Key = objectKey,
                    InputStream = inputStream,
                    ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                        ? "application/octet-stream"
                        : file.ContentType
                };

                await _s3Client.PutObjectAsync(putRequest);

                _logger.LogInformation(
                    "Dosya MinIO'ya başarıyla yüklendi. Orijinal Ad: {OriginalFileName}, MinIO Key: {ObjectKey}, Boyut: {Length} byte",
                    file.FileName, objectKey, file.Length);

                return objectKey;
            }
            catch (AmazonS3Exception s3Ex)
            {
                _logger.LogError(s3Ex, "Dosya MinIO'ya yüklenirken S3 hatası oluştu: {FileName}", file.FileName);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya MinIO'ya yüklenirken beklenmeyen bir hata oluştu: {FileName}", file.FileName);
                throw;
            }
        }

        public async Task<Stream> GetFileAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Dosya adı boş olamaz.", nameof(fileName));
            }

            try
            {
                var getRequest = new GetObjectRequest
                {
                    BucketName = _minioSettings.BucketName,
                    Key = fileName
                };

                using var response = await _s3Client.GetObjectAsync(getRequest);
                var memoryStream = new MemoryStream();
                await response.ResponseStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                _logger.LogInformation("Dosya MinIO'dan başarıyla okundu: {FileName}", fileName);
                return memoryStream;
            }
            catch (AmazonS3Exception s3Ex) when (s3Ex.StatusCode == HttpStatusCode.NotFound || s3Ex.ErrorCode == "NoSuchKey")
            {
                _logger.LogWarning(s3Ex, "İstenen dosya MinIO üzerinde bulunamadı: {FileName}", fileName);
                throw new FileNotFoundException($"MinIO üzerinde '{fileName}' dosyası bulunamadı.", fileName, s3Ex);
            }
            catch (AmazonS3Exception s3Ex)
            {
                _logger.LogError(s3Ex, "MinIO'dan dosya indirilirken S3 hatası oluştu: {FileName}", fileName);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MinIO'dan dosya indirilirken beklenmeyen bir hata oluştu: {FileName}", fileName);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                _logger.LogWarning("Silinecek dosya adı boş olduğu için MinIO silme işlemi atlandı.");
                return false;
            }

            try
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _minioSettings.BucketName,
                    Key = fileName
                };

                await _s3Client.DeleteObjectAsync(deleteRequest);
                _logger.LogInformation("Dosya MinIO'dan başarıyla silindi: {FileName}", fileName);
                return true;
            }
            catch (AmazonS3Exception s3Ex)
            {
                _logger.LogError(s3Ex, "MinIO'dan dosya silinirken S3 hatası oluştu: {FileName}", fileName);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MinIO'dan dosya silinirken beklenmeyen bir hata oluştu: {FileName}", fileName);
                return false;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _s3Client?.Dispose();
                _disposed = true;
            }
        }
    }
}
