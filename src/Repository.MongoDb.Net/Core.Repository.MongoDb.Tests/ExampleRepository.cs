using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repository.MongoDb.Tests
{
    public class ExampleRepository : MongoRepository<ExampleEntity>, IExampleRepository
    {

        public ExampleRepository(string connectionString): base(connectionString)
        {

        }
    }

    public class ExampleEntity : Entity
    {
        public string Name { get; set; }
    }
}
