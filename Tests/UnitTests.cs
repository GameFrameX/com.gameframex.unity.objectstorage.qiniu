using System;
using GameFrameX.ObjectStorage.QiNiu.Runtime;
using GameFrameX.ObjectStorage.Runtime;
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
        public void TestUpload()
        {
            _manager.UploadDirectory("./");
        }
    }
}