using Goober.Tests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Goober.Config.WebApi.Tests
{
    public static class TestUtils
    {
        public static SystemUnderTest GenerateSystemUnderTest()
        {
            var ret = new SystemUnderTest(
                    new List<string> { }
                );

            ret.Init<Goober.Config.WebApi.Startup>(services => {
                ret.RegisterInMemoryDatabase<Goober.Config.DAL.PostgreSql.DbContext.Implementation.ConfigDbContext>(services);
            });

            return ret;
        }
    }
}
