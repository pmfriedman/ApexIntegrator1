using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ApexServices;

namespace ApexServicesTest
{
    [TestFixture]
    public class LocationTests
    {
        [Test]
        public async Task CreateNewLocation()
        {
            var service = new LocationService();
            var key = await service.CreateNewLocation();

            Assert.That(key, Is.GreaterThan(0));
        }
    }
}
