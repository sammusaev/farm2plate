using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon; //for linking your AWS account
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace farm2plate.AWServices
{
    public class S3Service
    {
        AmazonS3Client S3Client;
        RegionEndpoint RERegion;
        S3Region S3Region;
        String BucketName;

        public S3Service(){
            // References the .env file
            string ACCESS_KEY = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");
            string AWS_SECRET_ACCESS_KEY = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            string AWS_SESSION_TOKEN = Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN");
            BucketName = Environment.GetEnvironmentVariable("AWS_IMAGE_BUCKET_NAME");
            RERegion = RegionEndpoint.USEast1;
            S3Region = S3Region.USEast1;
            // Init client
            S3Client = new AmazonS3Client(ACCESS_KEY, AWS_SECRET_ACCESS_KEY, AWS_SESSION_TOKEN, RERegion);
        }

        public async Task<List<S3Bucket>> GetBuckets() {
            ListBucketsResponse resp = await S3Client.ListBucketsAsync();
            return resp.Buckets;
        }

        public async Task<bool> DoesBucketExist() {
            var buckets = await GetBuckets();
            foreach (S3Bucket bucket in buckets) {
                if (bucket.BucketName == BucketName) {
                    return true;
                } 
            }
            return false;
        }

        [HttpPost("create")]
        public async Task<(bool,string)> CreateBucketAsync() {
            PutBucketRequest request = new PutBucketRequest
            {
                BucketName = BucketName,
                BucketRegion = S3Region,         // set region to EU
                CannedACL = S3CannedACL.PublicRead  // make bucket publicly readable
            };
            PutBucketResponse resp = await S3Client.PutBucketAsync(request);
            if (resp.HttpStatusCode != System.Net.HttpStatusCode.OK) {
                return (false, $"{resp.HttpStatusCode.ToString()}");
            }
            return (true, $"Bucket {BucketName} created");
        }

        [HttpPost("create")]
        public async Task<(bool, string)> UploadToBucket(IFormFile file) {
            string key = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
            PutObjectRequest request = new PutObjectRequest { 
                BucketName = BucketName,
                Key =  key,
                InputStream = file.OpenReadStream(),
                CannedACL = S3CannedACL.PublicRead,
            };
            PutObjectResponse resp = await S3Client.PutObjectAsync(request);
            if (resp.HttpStatusCode != System.Net.HttpStatusCode.OK){
                return (false, $"{resp.HttpStatusCode.ToString()}");
            }
            return (true, key);
        }
    }
}
