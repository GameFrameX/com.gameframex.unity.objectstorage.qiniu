using GameFrameX.ObjectStorage.QiNiu.Runtime;
using GameFrameX.ObjectStorage.Runtime;
using NUnit.Framework;

namespace GameFrameX.ObjectStorage.QiNiu.Tests
{
    internal class UnitTests
    {
        IObjectStorageManager _manager;

        [SetUp]
        public void Setup()
        {
            _manager = ObjectStorageManagerFactory.Create<QiNiuYunObjectStorageManager>(new ObjectStorageOptions
            {
                AccessKey = "cy59rOpZxYeexzxzsasa2MiBheVXIEGzI",
                SecretKey = "o1kPquVb1JGdascxzdsadpxofJhh8SziAz6oCN",
                BucketName = "aliaxzzlank-de",
                SavePath = "test/aa/zz",
            });
        }

        [Test]
        public void Create_ReturnsQiniuManager()
        {
            // 工厂按类型参数创建七牛云管理器：本地凭证构造，无网络 I/O。
            Assert.IsNotNull(_manager);
            Assert.IsInstanceOf<QiNiuYunObjectStorageManager>(_manager);
        }
    }
}
