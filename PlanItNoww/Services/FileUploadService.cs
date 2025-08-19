using Microsoft.Extensions.Options;
using PlanItNoww.Utils;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.IO;

namespace PlanItNoww.Services
{
    public interface IFileUploadService
    {
        Task<FileUploadResponse> UploadFileAsync(FileUploadRequest request);
        Task<bool> DeleteFileAsync(string fileName, string bucketName = null);
        Task<string> GetFileUrlAsync(string fileName, string bucketName = null, int expiryHours = 1);
        Task<bool> FileExistsAsync(string fileName, string bucketName = null);
        Task<FileInfo> GetFileInfoAsync(string fileName, string bucketName = null);
        Task<List<string>> ListFilesAsync(string prefix = "", string bucketName = null);
    }

    public class FileUploadRequest
    {
        public Stream fileStream { get; set; }
        public string fileName { get; set; }
        public string contentType { get; set; }
        public string folder { get; set; } = "uploads";
        public Dictionary<string, string> metadata { get; set; } = new();
        public bool isPublic { get; set; } = false;
    }

    public class FileUploadResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string fileUrl { get; set; }
        public string fileName { get; set; }
        public string bucketName { get; set; }
        public long fileSize { get; set; }
        public string contentType { get; set; }
        public DateTime uploadDate { get; set; }
        public Dictionary<string, string> metadata { get; set; } = new();
    }

    public class FileInfo
    {
        public string fileName { get; set; }
        public long fileSize { get; set; }
        public string contentType { get; set; }
        public DateTime lastModified { get; set; }
        public string etag { get; set; }
        public Dictionary<string, string> metadata { get; set; } = new();
    }

    public class FileUploadService : IFileUploadService
    {
        private readonly ApplicationEnvironment _config;
        private readonly ILogger<FileUploadService> _logger;
        private readonly IAmazonS3 _s3Client;
        private readonly string _defaultBucket;

        public FileUploadService(IOptions<ApplicationEnvironment> config, ILogger<FileUploadService> logger)
        {
            _config = config.Value;
            _logger = logger;
            
            try
            {
                var s3Config = _config.awss3config;
                _defaultBucket = s3Config.bucketname;
                
                var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(s3Config.accesskey, s3Config.secretaccesskey);
                var s3ConfigObj = new AmazonS3Config
                {
                    ServiceURL = s3Config.endpoint,
                    ForcePathStyle = true
                };
                
                _s3Client = new AmazonS3Client(awsCredentials, s3ConfigObj);
                _logger.LogInformation("AWS S3 client initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize AWS S3 client");
                throw;
            }
        }

        public async Task<FileUploadResponse> UploadFileAsync(FileUploadRequest request)
        {
            try
            {
                if (request.fileStream == null || string.IsNullOrEmpty(request.fileName))
                {
                    return new FileUploadResponse
                    {
                        success = false,
                        message = "File stream and file name are required"
                    };
                }

                var bucketName = _defaultBucket;
                var key = $"{request.folder}/{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid()}_{request.fileName}";
                
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    InputStream = request.fileStream,
                    ContentType = request.contentType ?? "application/octet-stream"
                };

                // Add metadata
                foreach (var meta in request.metadata)
                {
                    putRequest.Metadata.Add(meta.Key, meta.Value);
                }

                // Set ACL based on public flag
                if (request.isPublic)
                {
                    putRequest.CannedACL = S3CannedACL.PublicRead;
                }

                var response = await _s3Client.PutObjectAsync(putRequest);
                
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    var fileUrl = request.isPublic 
                        ? $"https://{bucketName}.s3.amazonaws.com/{key}"
                        : await GetFileUrlAsync(key, bucketName);

                    var uploadResponse = new FileUploadResponse
                    {
                        success = true,
                        message = "File uploaded successfully",
                        fileUrl = fileUrl,
                        fileName = key,
                        bucketName = bucketName,
                        fileSize = request.fileStream.Length,
                        contentType = request.contentType ?? "application/octet-stream",
                        uploadDate = DateTime.UtcNow,
                        metadata = request.metadata
                    };

                    _logger.LogInformation("File uploaded successfully: {FileName}, Size: {Size}, Bucket: {Bucket}", 
                        key, request.fileStream.Length, bucketName);

                    return uploadResponse;
                }
                else
                {
                    _logger.LogError("Failed to upload file: {FileName}, Status: {Status}", 
                        request.fileName, response.HttpStatusCode);
                    
                    return new FileUploadResponse
                    {
                        success = false,
                        message = $"Upload failed with status: {response.HttpStatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload file: {FileName}", request.fileName);
                
                return new FileUploadResponse
                {
                    success = false,
                    message = "Upload failed: " + ex.Message
                };
            }
        }

        public async Task<bool> DeleteFileAsync(string fileName, string bucketName = null)
        {
            try
            {
                var bucket = bucketName ?? _defaultBucket;
                
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = bucket,
                    Key = fileName
                };

                var response = await _s3Client.DeleteObjectAsync(deleteRequest);
                
                if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    _logger.LogInformation("File deleted successfully: {FileName}, Bucket: {Bucket}", fileName, bucket);
                    return true;
                }
                else
                {
                    _logger.LogWarning("File deletion returned unexpected status: {FileName}, Status: {Status}", 
                        fileName, response.HttpStatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file: {FileName}", fileName);
                return false;
            }
        }

        public async Task<string> GetFileUrlAsync(string fileName, string bucketName = null, int expiryHours = 1)
        {
            try
            {
                var bucket = bucketName ?? _defaultBucket;
                
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = bucket,
                    Key = fileName,
                    Expires = DateTime.UtcNow.AddHours(expiryHours)
                };

                var url = _s3Client.GetPreSignedURL(request);
                _logger.LogDebug("Generated pre-signed URL for file: {FileName}, Expiry: {ExpiryHours} hours", 
                    fileName, expiryHours);
                
                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate pre-signed URL for file: {FileName}", fileName);
                return null;
            }
        }

        public async Task<bool> FileExistsAsync(string fileName, string bucketName = null)
        {
            try
            {
                var bucket = bucketName ?? _defaultBucket;
                
                var request = new GetObjectMetadataRequest
                {
                    BucketName = bucket,
                    Key = fileName
                };

                await _s3Client.GetObjectMetadataAsync(request);
                _logger.LogDebug("File exists: {FileName}, Bucket: {Bucket}", fileName, bucket);
                return true;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogDebug("File not found: {FileName}, Bucket: {Bucket}", fileName, bucketName ?? _defaultBucket);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check file existence: {FileName}", fileName);
                return false;
            }
        }

        public async Task<FileInfo> GetFileInfoAsync(string fileName, string bucketName = null)
        {
            try
            {
                var bucket = bucketName ?? _defaultBucket;
                
                var request = new GetObjectMetadataRequest
                {
                    BucketName = bucket,
                    Key = fileName
                };

                var response = await _s3Client.GetObjectMetadataAsync(request);
                
                var fileInfo = new FileInfo
                {
                    fileName = fileName,
                    fileSize = response.ContentLength,
                    contentType = response.Headers.ContentType,
                    lastModified = response.LastModified,
                    etag = response.ETag?.Trim('"'),
                    metadata = new Dictionary<string, string>()
                };
                
                // Convert metadata collection to dictionary
                if (response.Metadata != null)
                {
                    foreach (var key in response.Metadata.Keys)
                    {
                        fileInfo.metadata[key] = response.Metadata[key];
                    }
                }

                _logger.LogDebug("Retrieved file info: {FileName}, Size: {Size}, Type: {Type}", 
                    fileName, response.ContentLength, response.Headers.ContentType);
                
                return fileInfo;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogDebug("File not found for info retrieval: {FileName}", fileName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get file info: {FileName}", fileName);
                return null;
            }
        }

        public async Task<List<string>> ListFilesAsync(string prefix = "", string bucketName = null)
        {
            try
            {
                var bucket = bucketName ?? _defaultBucket;
                var fileList = new List<string>();
                
                var request = new ListObjectsV2Request
                {
                    BucketName = bucket,
                    Prefix = prefix,
                    MaxKeys = 1000 // Limit to 1000 files per request
                };

                ListObjectsV2Response response;
                do
                {
                    response = await _s3Client.ListObjectsV2Async(request);
                    
                    foreach (var obj in response.S3Objects)
                    {
                        fileList.Add(obj.Key);
                    }
                    
                    request.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated);

                _logger.LogDebug("Listed {Count} files with prefix: {Prefix}, Bucket: {Bucket}", 
                    fileList.Count, prefix, bucket);
                
                return fileList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list files with prefix: {Prefix}", prefix);
                return new List<string>();
            }
        }

        public void Dispose()
        {
            _s3Client?.Dispose();
        }
    }
}
