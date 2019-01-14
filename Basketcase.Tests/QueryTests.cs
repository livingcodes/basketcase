using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Basketcase.Tests
{
    [TestClass] public class QueryTests : BaseTests
    {
        [ClassInitialize]
        public static void InitializeClass(TestContext context) =>
            initialize();

        [TestInitialize]
        public void RunBeforeEachTest() =>
            createPostTable();

        [TestMethod] public void QueryMultiple() {
            var posts = db.Select<Post>("select * from post");
            assert(posts.Count == 4);
        }

        [TestMethod] public void QueryMultipleWithParameter() {
            var posts = db
                .Parameter("@min", 2)
                .Select<Post>("select * from post where id > @min");
            assert(posts.Count == 2);
        }

        [TestMethod] public void QueryMultipleWithParameters() {
            var posts = db.Select<Post>("select * from post where id > @min and id < @max", 1, 4);
            assert(posts.Count == 2);
            assert(posts.Exists(p => p.Id == 2));
            assert(posts.Exists(p => p.Id == 3));
        }

        [TestMethod] public void QuerySql() {
            var posts = db
                .Sql("select * from post where id > @min")
                .Parameter("@min", 1)
                .Select<Post>();
            assert(posts.Count > 0);
        }

        [TestMethod] public void QueryPage() {
            var posts = db.Sql("select * from post order by id")
                .Paging(2, 2)
                .Select<Post>();
            assert(posts.Count == 2);

            posts = db
                .Paging(1,2)
                .Select<Post>("select * from post where id > @id order by id", 1);
            assert(posts.Count == 2);
            assert(!posts.Exists(p => p.Id == 1));
        }

        [TestMethod] public void QueryOne() {
            var post = db.SelectOne<Post>("select top 1 * from post");
            assert(post != null);
        }

        [TestMethod] public void QueryOneWithParameter() {
            var post = db
                .Parameter("@min", 2)
                .SelectOne<Post>("select top 1 * from post where id > @min");
            assert(post.Id > 2);
        }

        [TestMethod] public void QueryOneWithParameters() {
            var post = db.SelectOne<Post>("select top 1 * from post where id > @min and id < @max", 2, 4);
            assert(post.Id == 3);
        }
    }
}
