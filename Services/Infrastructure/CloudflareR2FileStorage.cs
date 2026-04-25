using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Services.Abstractions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Infrastructure
{
    public class CloudflareR2FileStorage : IFileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _publicUrl;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public CloudflareR2FileStorage(IConfiguration configuration)
        {
            var accountId = configuration["CloudflareR2:AccountId"];
            var accessKeyId = configuration["CloudflareR2:AccessKeyId"];
            var secretAccessKey = configuration["CloudflareR2:SecretAccessKey"];
            _bucketName = configuration["CloudflareR2:BucketName"] ?? throw new InvalidOperationException("CloudflareR2:BucketName is not configured");
            var s3Endpoint = configuration["CloudflareR2:S3Endpoint"];
            _publicUrl = configuration["CloudflareR2:PublicUrl"] ?? throw new InvalidOperationException("CloudflareR2:PublicUrl is not configured");

            var config = new AmazonS3Config
            {
                ServiceURL = s3Endpoint,
                ForcePathStyle = true,
                SignatureVersion = "4"
            };

            _s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, config);
        }

        public async Task<string> UploadAsync(IFormFile file, string folder)
        {
            Console.WriteLine($"[R2] Starting upload - File: {file?.FileName}, Folder: {folder}");
            
            // Validate file
            if (file == null || file.Length == 0)
            {
                Console.WriteLine("[R2 ERROR] File is null or empty");
                throw new ArgumentException("File is empty");
            }

            Console.WriteLine($"[R2] File size: {file.Length} bytes, Max: {MaxFileSize} bytes");
            if (file.Length > MaxFileSize)
                throw new ArgumentException($"File size exceeds maximum allowed size of {MaxFileSize / 1024 / 1024}MB");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            Console.WriteLine($"[R2] File extension: {extension}");
            
            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException($"File type {extension} is not allowed. Allowed types: {string.Join(", ", AllowedExtensions)}");

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{extension}";
            var key = $"{folder}/{fileName}";
            Console.WriteLine($"[R2] Generated key: {key}");
            Console.WriteLine($"[R2] Bucket: {_bucketName}");

            try
            {
                Console.WriteLine("[R2] Opening file stream...");
                using var stream = file.OpenReadStream();
                Console.WriteLine($"[R2] Stream opened, length: {stream.Length}");
                
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    Key = key,
                    BucketName = _bucketName,
                    ContentType = file.ContentType,
                    CannedACL = S3CannedACL.PublicRead,
                    DisablePayloadSigning = true  // R2 doesn't support STREAMING-AWS4-HMAC-SHA256-PAYLOAD
                };

                Console.WriteLine("[R2] Creating TransferUtility...");
                var transferUtility = new TransferUtility(_s3Client);
                
                Console.WriteLine("[R2] Starting upload to R2...");
                await transferUtility.UploadAsync(uploadRequest);
                
                Console.WriteLine("[R2] Upload completed successfully!");

                // Return public URL
                var publicUrl = $"{_publicUrl.TrimEnd('/')}/{key}";
                Console.WriteLine($"[R2] Public URL: {publicUrl}");
                return publicUrl;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"[R2 ERROR] AmazonS3Exception: {ex.Message}");
                Console.WriteLine($"[R2 ERROR] Status Code: {ex.StatusCode}");
                Console.WriteLine($"[R2 ERROR] Error Code: {ex.ErrorCode}");
                Console.WriteLine($"[R2 ERROR] Request ID: {ex.RequestId}");
                throw new InvalidOperationException($"Error uploading file to Cloudflare R2: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[R2 ERROR] General Exception: {ex.GetType().Name}");
                Console.WriteLine($"[R2 ERROR] Message: {ex.Message}");
                Console.WriteLine($"[R2 ERROR] Stack: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return false;

            try
            {
                // Extract key from public URL
                var key = fileUrl.Replace(_publicUrl.TrimEnd('/') + "/", "");

                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                await _s3Client.DeleteObjectAsync(deleteRequest);
                return true;
            }
            catch (AmazonS3Exception)
            {
                return false;
            }
        }
    }
}
