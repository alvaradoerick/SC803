using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseIsthmus.Tests
{
    public  class TestSetupFixture: IDisposable
    {
        public IConfiguration Configuration { get; private set; }
        public AseItshmusContext DbContext { get; private set; }

        public TestSetupFixture()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = Configuration.GetConnectionString("AseIsthmusConnDev");

            var options = new DbContextOptionsBuilder<AseItshmusContext>()
                .UseSqlServer(connectionString)
                .Options;


            DbContext = new AseItshmusContext(options);
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }

    }
}
