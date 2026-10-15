using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using GameFrameX.ObjectStorage.Runtime;
using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;
using UnityEngine;

namespace GameFrameX.ObjectStorage.QiNiu.Runtime
{
    /// <summary>
    /// 七牛云的对象存储实现。支持上传、下载与列举对象。下载需要配置 <see cref="ObjectStorageOptions.DownloadDomain"/>。
    /// 示例文档：https://developer.qiniu.com/kodo/4056/c-sdk-v7-2-15
    /// SDK：https://github.com/qiniu/csharp-sdk
    /// </summary>
    public sealed class QiNiuYunObjectStorageManager : BaseObjectStorageManager
    {
        private const int DownloadUrlExpireSeconds = 3600;
        private const int ListPageSize = 100;

        private Mac _mac;
        private string _uploadToken;
        private UploadManager _uploadManager;
        private BucketManager _bucketManager;

        /// <summary>
        /// 使用七牛云凭证初始化。Endpoint 当前忽略（存储区域固定为华南），重复调用会被忽略。
        /// </summary>
        /// <param name="options">初始化参数</param>
        public override void Init(ObjectStorageOptions options)
        {
            if (_uploadManager != null)
            {
                return;
            }

            base.Init(options);

            _mac = new Mac(options.AccessKey, options.SecretKey);
            PutPolicy putPolicy = new PutPolicy
            {
                Scope = options.BucketName,
                isPrefixalScope = 1
            };
            _uploadToken = Auth.CreateUploadToken(_mac, putPolicy.ToJsonString());

            Config config = new Config
            {
                Zone = Zone.ZONE_CN_South,
                MaxRetryTimes = 3
            };
            _uploadManager = new UploadManager(config);
            _bucketManager = new BucketManager(_mac, config);
        }

        protected override bool UploadFileInternal(string objectKey, string localFilePath)
        {
            HttpResult result = _uploadManager.UploadFile(localFilePath, objectKey, _uploadToken, null);
            if (result.Code != (int)HttpStatusCode.OK)
            {
                Debug.LogError("上传文件失败，本地文件路径：" + localFilePath + " 对象键：" + objectKey + "\n" + result.Text);
                return false;
            }

            return true;
        }

        protected override bool DownloadFileInternal(string objectKey, string localFilePath)
        {
            if (string.IsNullOrEmpty(Options.DownloadDomain))
            {
                Debug.LogError("下载文件失败，七牛云下载需要在 ObjectStorageOptions.DownloadDomain 配置外链域名");
                return false;
            }

            string downloadUrl = DownloadManager.CreatePrivateUrl(_mac, Options.DownloadDomain.TrimEnd('/'), objectKey, DownloadUrlExpireSeconds);
            HttpResult result = DownloadManager.Download(downloadUrl, localFilePath);
            if (result.Code != (int)HttpStatusCode.OK)
            {
                Debug.LogError("下载文件失败，对象键：" + objectKey + " 本地文件路径：" + localFilePath + "\n" + result.Text);
                return false;
            }

            return true;
        }

        protected override IEnumerable<string> ListObjectKeys(string prefix)
        {
            List<string> objectKeys = new List<string>();
            string marker = null;
            do
            {
                ListResult listResult = _bucketManager.ListFiles(Options.BucketName, prefix, marker, ListPageSize, null);
                if (listResult.Code != (int)HttpStatusCode.OK)
                {
                    throw new Exception("列举对象失败：" + listResult.Text);
                }

                ListInfo listInfo = listResult.Result;
                if (listInfo != null && listInfo.Items != null)
                {
                    foreach (ListItem listItem in listInfo.Items)
                    {
                        if (!string.IsNullOrEmpty(listItem.Key) && !listItem.Key.EndsWith("/"))
                        {
                            objectKeys.Add(listItem.Key);
                        }
                    }

                    marker = string.IsNullOrEmpty(listInfo.Marker) ? null : listInfo.Marker;
                }
                else
                {
                    marker = null;
                }
            } while (marker != null);

            return objectKeys;
        }
    }
}
