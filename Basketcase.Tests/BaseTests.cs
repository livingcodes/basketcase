using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using static Basketcase.Table;

namespace Basketcase.Tests
{
    [TestClass] public class BaseTests
    {
        protected static IDb db;
        protected static ICache cache;
        
        protected static void initialize() {
            //this.configuration = new ConfigurationBuilder()
            //    .AddJsonFile(@"c:\code\secrets\Tent\settings.json")
            //    .Build();
            //string connectionString = configuration["connectionString"];
            
            // i wasn't able to figure out how to construct distributed memory cache
            // the IOptions in particular
            // so i used the service provider to build it
            IServiceCollection services = new ServiceCollection();
            services.AddDistributedMemoryCache();
            var serviceProvider = services.BuildServiceProvider();
            var distributedCache = serviceProvider.GetService<IDistributedCache>();
            cache = new SerializedCache(distributedCache);

            var connectionString = "server=(LocalDb)\\MSSQLLocalDB; database=Basketcase; trusted_connection=true;";

            db = new Db(
                new ConnectionFactory(connectionString),
                new Reader(),
                cache,
                new TableName_ClassName()
            );
        }
        
        protected static void createTable(string tableName) {
            var sql = new Table(tableName)
                .AddColumn("Id", SqlType.Int, Syntax.Identity(1, 1))
                .AddColumn("Html", SqlType.VarChar(200))
                .End()
                .Sql;
            db.Execute(sql);
        }

        protected static void createPostTable() {
            createTable("Post");
            for (var i = 0; i < 4; i++)
                db.Insert(new Post { Html = $"Post {i}" });
        }

        protected void assert(bool condition) {
            Assert.IsTrue(condition);
        }

        public class Post {
            public int Id { get; set; }
            public string Html { get; set; }
        }
    }
}