using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Basketcase.Tests
{
    [TestClass] public class CacheTests : BaseTests
    {
        [ClassInitialize]
        public static void InitializeClass(TestContext context) {
            initialize();
            createPostTable();
            for (var i = 0; i < 4; i++)
                db.Insert(new Post { Html = $"Post {i}" });
        }

        [TestMethod] public void CacheBySeconds() {
            var posts = db
                .Cache(key:"two", seconds:120)
                .Select<Post>("select * from post where id > 1 and id < 3");
            assert(posts.Count == 1);
            assert(posts[0].Id == 2);

            // doesn't require sql
            posts = db
                .Cache("two", 120)
                .Select<Post>();
            assert(posts[0].Id == 2);

            // sql can be anything
            posts = db
                .Cache("two", 120)
                .Select<Post>("anything");
            assert(posts[0].Id == 2);
        }

        [TestMethod] public void CacheByDate() {
            var posts = db
                .Cache("two", DateTime.Now.AddSeconds(120))
                .Select<Post>("select * from post where id > 1 and id < 3");
            assert(posts[0].Id == 2);

            posts = db
                .Cache("two", 120)
                .Select<Post>();
            assert(posts[0].Id == 2);
        }
    }
}
