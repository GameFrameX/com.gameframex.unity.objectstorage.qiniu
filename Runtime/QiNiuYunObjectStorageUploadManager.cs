using System.IO;
using GameFrameX.ObjectStorage.Runtime;
using Qiniu.Storage;
using Qiniu.Util;
using UnityEngine;
using FileInfo = System.IO.FileInfo;

namespace GameFrameX.ObjectStorage.QiNiu.Runtime
{
    /// <summary>
    /// 七牛云的对象存储
    /// 示例文档：  https://developer.qiniu.com/kodo/4056/c-sdk-v7-2-15#3
    /// SDK：https://github.com/qiniu/csharp-sdk
    /// </summary>
    public sealed class QiNiuYunObjectStorageUploadManager : BaseObjectStorageUploadManager
    {
        private bool _isInit = false;
        private UploadManager _uploadManager = null;
        private string _token;
        private Mac _mac;
        private string _bucketName;

        public override void Init(string accessKey, string secretKey, string bucketName, string endpoint = null)
        {
            if (_isInit)
            {
                return;
            }

            _mac = new Mac(accessKey, secretKey);
            _bucketName = bucketName;
            var putPolicy = new PutPolicy
            {
                Scope = _bucketName,
                isPrefixalScope = 1
            };
            string jsonBody = putPolicy.ToJsonString();

            _token = Auth.CreateUploadToken(_mac, jsonBody);
            _isInit = true;

            _uploadManager = new UploadManager(new Config() { Zone = Zone.ZONE_CN_South, MaxRetryTimes = 3, });
        }


        protected override void UploadDirectoryInternal(string localDirectory)
        {
            var directoryInfo = new DirectoryInfo(localDirectory);
            var files = directoryInfo.GetFiles();
            foreach (var fileInfo in files)
            {
                var savePath = BucketSavePath + fileInfo.FullName.Substring(UploadRootPath.Length);
                var result = _uploadManager.UploadFile(fileInfo.FullName, savePath, _token, null);
                if (result.Code != 200)
                {
                    Debug.LogError($"上传文件失败,本地文件路径：{fileInfo.FullName}\n 目标存储路径:{savePath}");
                    Debug.LogError(result.Text);
                }
            }

            var directories = directoryInfo.GetDirectories();
            foreach (var directory in directories)
            {
                UploadDirectoryInternal(directory.FullName);
            }
        }
    }
}