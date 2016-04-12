using ApexServices;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexServicesTest
{
    [TestFixture]
    public class ConnectionTest
    {
        [Test]
        public async Task Connect()
        {
            var connection = await Connection.GetConnection();
            Assert.That(connection, Is.Not.Null);
        }
    }
}
