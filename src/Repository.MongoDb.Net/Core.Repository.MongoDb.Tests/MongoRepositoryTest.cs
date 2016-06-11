using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Core.Repository.MongoDb.Tests
{
    [TestClass]
    public class MongoRepositoryTest
    {
        private IRepository<ExampleEntity> _prodRepository;

        public MongoRepositoryTest()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MongoDb.ConnectionString"].ToString();
            this._prodRepository = new MongoRepository<ExampleEntity>(connectionString);
        }

        #region Get Method Tests
        [TestMethod]
        [TestCategory("Integration")]
        public async Task Get_Valid_Entity_Success()
        {
            var ExampleEntity = new ExampleEntity()
            {
                Name = "ExampleEntity" + DateTime.UtcNow.ToString()
            };

            var insertedExampleEntity = await this._prodRepository.Insert(ExampleEntity);
            var ExampleEntityResult = await this._prodRepository.Get(insertedExampleEntity.Id);

            Assert.AreEqual(ExampleEntityResult.Id, insertedExampleEntity.Id);
            Assert.AreEqual(ExampleEntityResult.Name, ExampleEntity.Name);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task Get_Invalid_Entity_Success()
        {
            var ExampleEntityResult = await this._prodRepository.Get("SomeInvalidId");
            Assert.IsNull(ExampleEntityResult);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task Get_Null_Entity_Success()
        {
            var ExampleEntityResult = await this._prodRepository.Get(null);
            Assert.IsNull(ExampleEntityResult);
        }

        #endregion

        #region Delete Method Tests
        [TestMethod]
        [TestCategory("Integration")]
        public async Task Delete_Valid_Entity_Success()
        {
            var ExampleEntity = new ExampleEntity()
            {
                Name = "ExampleEntity" + DateTime.UtcNow.ToString()
            };

            var insertedExampleEntity = await this._prodRepository.Insert(ExampleEntity);
            var getExampleEntity = await this._prodRepository.Get(insertedExampleEntity.Id);

            Assert.IsNotNull(getExampleEntity);
            Assert.AreEqual(getExampleEntity.Id, insertedExampleEntity.Id);

            var deletedExampleEntity = await this._prodRepository.Delete(getExampleEntity.Id);

            Assert.IsNotNull(deletedExampleEntity);
            Assert.AreEqual(deletedExampleEntity.Id, getExampleEntity.Id);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task Delete_Invalid_Entity_Success()
        {
            var deletedExampleEntity = await this._prodRepository.Delete("InvalidEntityId");
            Assert.IsNull(deletedExampleEntity);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task Delete_Null_Entity_Success()
        {
            var deletedExampleEntity = await this._prodRepository.Delete(null);
            Assert.IsNull(deletedExampleEntity);
        }

        #endregion

        #region Insert Method Tests
        [TestMethod]
        [TestCategory("Integration")]
        public async Task Insert_Empty_Entity_Success()
        {
            var ExampleEntityResult = await this._prodRepository.Insert(new ExampleEntity());
            Assert.IsNotNull(ExampleEntityResult);
            Assert.IsNotNull(ExampleEntityResult.Id);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Insert_Null_Entity_Exception()
        {
            await this._prodRepository.Insert(null);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [ExpectedException(typeof(EntityDuplicateException))]
        public async Task Insert_Duplicated_Entity_Exception()
        {
            var prodId = "DuplicatedId" + DateTime.UtcNow.ToString();
            var ExampleEntity = new ExampleEntity()
            {
                Id = prodId
            };

            await this._prodRepository.Insert(ExampleEntity);
            await this._prodRepository.Insert(ExampleEntity);
        }

        #endregion

        #region Update Method Tests
        [TestMethod]
        [TestCategory("Integration")]
        public async Task Update_Valid_Entity_Success()
        {
            var ExampleEntityName = "ExampleEntity" + DateTime.UtcNow.ToString();
            var updatedExampleEntityName = "Updated Name";

            var ExampleEntity = new ExampleEntity()
            {
                Name = ExampleEntityName
            };

            var insertedExampleEntity = await this._prodRepository.Insert(ExampleEntity);
            var ExampleEntityResult = await this._prodRepository.Get(insertedExampleEntity.Id);
            ExampleEntityResult.Name = updatedExampleEntityName;

            var updatedExampleEntity = await this._prodRepository.Update(ExampleEntityResult);

            Assert.AreEqual(updatedExampleEntity.Name, updatedExampleEntityName);
            Assert.IsTrue(updatedExampleEntity.Version > insertedExampleEntity.Version);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [ExpectedException(typeof(EntityConflictException))]
        public async Task Update_Version_Conflit_Entity_Exception()
        {
            var ExampleEntityName = "ExampleEntity" + DateTime.UtcNow.ToString();

            var ExampleEntity = new ExampleEntity()
            {
                Name = ExampleEntityName
            };

            var insertedExampleEntity = await this._prodRepository.Insert(ExampleEntity);
            var ExampleEntityResult1 = await this._prodRepository.Get(insertedExampleEntity.Id);
            var ExampleEntityResult2 = await this._prodRepository.Get(insertedExampleEntity.Id);

            var updatedExampleEntity1 = await this._prodRepository.Update(ExampleEntityResult1);
            var updatedExampleEntity2 = await this._prodRepository.Update(ExampleEntityResult2);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task Update_Null_Entity_Exception()
        {
            await this._prodRepository.Update(null);
        }

        #endregion
    }
}
