﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            var profiles = db.Select<Profile>("select * from profile where id = 1");
            assert(profiles.Count == 1);
            var profile = profiles[0];
            assert(profile.Html == "B");
            
            profile.Html = "c";
            rows = db.Update(profile);
            assert(rows == 1);
            profiles = db.Select<Profile>("select * from profile where id = 1");
            profile = profiles[0];
            assert(profile.Html == "c");

            rows = db.Delete<Profile>(1);
            assert(rows == 1);
            profiles = db.Select<Profile>("select * from profile where id = 1");
            assert(profiles.Count == 0);
        }

        [TestMethod] public void ExecuteWithParameter() {
            db.Admin.Truncate("Profile");

            db.Execute("insert into profile values (@html)", "<h1>hello</h1>");
            assert(db.Select<Profile>().Count == 1);
        }
    }
}
