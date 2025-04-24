using hotel_api_.RequestDto;
using hotel_api.util;
using hotel_business;
using hotel_data.dto;
using Minio;
using Minio.DataModel.Args;

namespace hotel_api.Services
{
    public class MinIoServices
    {
        public enum enBucketName
        {
            USER,
            RoomType,
            ROOM
        }

        private static IMinioClient? _client(IConfigurationServices _config)
        {
            try
            {
                return new MinioClient()
                    .WithEndpoint(_config.getKey("minio_end_point"))
                    .WithCredentials(_config.getKey("accessKy"), _config.getKey("secretKey"))
                    .WithSSL(false)
                    .Build();
            }
            catch (Exception error)
            {
                Console.WriteLine("Error creating MinIO client: {0}", error.Message);
                return null;
            }
        }

        private static async Task<bool> _isExistBucket(IMinioClient client, string bucketName)
        {
            try
            {
                var buckets = await client.ListBucketsAsync().ConfigureAwait(false);
                var result = buckets.Buckets.Any(b => b.Name == bucketName);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking bucket existence: {0}", ex.Message);
                return false;
            }
        }

        private static async Task _createNewBucket(IMinioClient client, string bucketName)
        {
            try
            {
                var mbArgs = new MakeBucketArgs().WithBucket(bucketName);
                await client.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                Console.WriteLine("Bucket '{0}' created successfully.", bucketName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating bucket: {0}", ex.Message);
            }
        }

        public static async Task<string?> uploadFile(
            IConfigurationServices _config,
            IFormFile file,
            enBucketName bucketName,
            string? previuseFileName = null
        )
        {
            try
            {
                var bucketNameStr = bucketName.ToString().ToLower();
                var minioClient = _client(_config);
                string fullName = clsUtil.generateGuid() + ".png";

                if (minioClient == null)
                {
                    Console.WriteLine("Failed to initialize MinIO client.");
                    return null;
                }

                // Check if bucket exists and create if necessary
                var isExistBucket = await _isExistBucket(minioClient, bucketNameStr);
                if (!isExistBucket)
                {
                    await _createNewBucket(minioClient, bucketNameStr);
                }

                if (previuseFileName != null)
                {
                    bool isHasPrevImage = await isFileExist(minioClient, previuseFileName, bucketNameStr);
                    if (!isHasPrevImage)
                        await deletExistFileAndReteurnUnDeletedFile(minioClient, previuseFileName, bucketNameStr);
                }

                // Upload the file
                using (var fileStream = file.OpenReadStream())
                {
                    var putObject = new PutObjectArgs()
                        .WithBucket(bucketNameStr)
                        .WithObject(fullName)
                        .WithStreamData(fileStream)
                        .WithObjectSize(file.Length)
                        .WithContentType(file.ContentType);

                    await minioClient.PutObjectAsync(putObject).ConfigureAwait(false);
                    Console.WriteLine("File '{0}' uploaded successfully to bucket '{1}'.", file.FileName,
                        bucketNameStr);
                    return fullName;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Error uploading file: {0}", error.Message);
                return null;
            }
        }

        public static async Task<List<ImageRequestDto>?> uploadFile(
            IConfigurationServices _config,
            List<ImageRequestDto> file,
            enBucketName bucketName,
            string? filePath = null
        )
        {
            List<ImageRequestDto>? unDeletedImages = null;

            try
            {
                var bucketNameStr = bucketName.ToString().ToLower();
                var minioClient = _client(_config);
                if (minioClient == null)
                {
                    Console.WriteLine("Failed to initialize MinIO client.");
                    return null;
                }

                //unDeletedImages = await deleteFile(minioClient, file, bucketNameStr, filePath);
                unDeletedImages =
                    await deletExistFileAndReteurnUnDeletedFile(minioClient, file, bucketNameStr);


                foreach (var formFile in unDeletedImages)
                {
                    string fullName = clsUtil.generateGuid() + ".png";
                    string fileFullPath = filePath != null ? $"{filePath}/{fullName}" : fullName;


                    var isExistBucket = await _isExistBucket(minioClient, bucketNameStr);

                    if (!isExistBucket)
                    {
                        await _createNewBucket(minioClient, bucketNameStr);
                    }

                    /*   if (previuseFileName != null)
                       {
                           foreach (var se in previuseFileName)
                           {
                               bool isExist = await isFileExist(minioClient, se.path, bucketNameStr);
                               if (!isExist)
                               {
                                   await deleteFile(minioClient, se.path, bucketNameStr);
                               }
                           }
                       }
   */
                    // Upload the file
                    if (formFile.data != null)
                    {
                        using (var fileStream = formFile.data.OpenReadStream())
                        {
                            var putObject = new PutObjectArgs()
                                .WithBucket(bucketNameStr)
                                .WithObject(fileFullPath)
                                .WithStreamData(fileStream)
                                .WithObjectSize(formFile.data.Length)
                                .WithContentType(formFile.data.ContentType);

                            await minioClient.PutObjectAsync(putObject).ConfigureAwait(false);
                            formFile.fileName = fileFullPath;
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Error uploading files: {0}", error.Message);
            }

            return unDeletedImages;
        }

        private static async Task<List<ImageRequestDto>> deletExistFileAndReteurnUnDeletedFile(
            IMinioClient client,
            List<ImageRequestDto> files,
            string bucketName
        )
        {
            List<ImageRequestDto> newFiles = new List<ImageRequestDto>();

            foreach (var file in files)
            {
                if ((file.isDeleted == true || file.data == null))
                {
                    try
                    {
                        if (file.isDeleted == true)
                        {
                            await client.RemoveObjectAsync(
                                new RemoveObjectArgs()
                                    .WithBucket(bucketName)
                                    .WithObject(file.fileName)
                            ).ConfigureAwait(false);
                            if (file.id != null)
                                ImageBuissness.deleteImage((Guid)file.id);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error deleting file: {0}", ex.Message);
                    }
                }
                else if ((file.data != null))
                {
                    newFiles.Add(file);
                }
            }

            return newFiles;
        }


        private static async Task<bool> deletExistFileAndReteurnUnDeletedFile(
            IMinioClient client,
            string fileName,
            string bucketName)
        {
            try
            {
                await client.RemoveObjectAsync(
                    new RemoveObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(fileName)
                ).ConfigureAwait(false);

                Console.WriteLine("File '{0}' deleted from bucket '{1}'.", fileName, bucketName);
                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine("Error deleting file: {0}", error.Message);
                return false;
            }
        }


        private static async Task<bool> isFileExist(
            IMinioClient client,
            string fileName,
            string bucketName
        )
        {
            try
            {
                var args = new StatObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(fileName)
                    ;
                await client.StatObjectAsync(args).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking file: {0}", ex.Message);
                return false;
            }
        }
    }
}