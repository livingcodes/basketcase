using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Basketcase.Tests
{
    [TestClass]
    public class DbTests : BaseTests
    {
        [ClassInitialize]
        public static void InitializeClass(TestContext context) {
            initialize();
            createTable("Profile");
        }
        public class Profile {
            public int Id { get; set; }
            public string Html { get; set; }
        }

        [TestMethod]
        public void InsertReturns() {
            db.Admin.Truncate("Profile");
            var (id, rows) = db.Insert(new Profile { Html = "A" });
            assert(id == 1);
            assert(rows == 1);

            (id, rows) = db.Insert(new Profile { Html = "B" });
            assert(id == 2);
            assert(rows == 1);
        }

        [TestMethod]
        public void InsertSelectUpdateDelete() {
            db.Admin.Truncate("Profile");
            
            var (id, rows) = db.Insert(new Profile { Html="B" });
            assert(id == 1);
            assert(rows == 1);

            var posts = db.Select<Profile>("select * from profile where id = 1");
            assert(posts.Count == 1);
            var post = posts[0];
            assert(post.Html == "B");
            
            post.Html = "c";
            rows = db.Update(post);
            assert(rows == 1);
            posts = db.Select<Profile>("select * from profile where id = 1");
            post = posts[0];
            assert(post.Html == "c");

            rows = db.Delete<Profile>(1);
            assert(rows == 1);
            posts = db.Select<Profile>("select * from profile where id = 1");
            assert(posts.Count == 0);
        }
    }
}
