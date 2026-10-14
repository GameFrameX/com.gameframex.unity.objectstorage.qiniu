using System;
using GameFrameX.ObjectStorage.QiNiu.Editor;
using GameFrameX.ObjectStorage.Editor;
using NUnit.Framework;

namespace GameFrameX.ObjectStorage.QiNiu.Tests
{
    internal class UnitTests
    {
        private DateTime dateTime, dateTime1;
        IObjectStorageUploadManager _manager;

        [SetUp]
        public void Setup()
        {
            _manager = ObjectStorageUploadFactory.Create<QiNiuYunObjectStorageUploadManager>("cy59rOpZxYeexzxzsasa2MiBheVXIEGzI", "o1kPquVb1JGdascxzdsadpxofJhh8SziAz6oCN", "aliaxzzlank-de");
            _manager.SetSavePath("test/aa/zz");
        }

        [Test]
        public void Create_ReturnsQiniuUploadManager()
        {
            // 工厂按类型参数创建七牛上传管理器：本地凭证构造，无网络 I/O。
            Assert.IsNotNull(_manager);
            Assert.IsInstanceOf<QiNiuYunObjectStorageUploadManager>(_manager);
        }
    }
}