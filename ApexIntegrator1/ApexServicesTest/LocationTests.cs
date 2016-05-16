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

            Assert.That(key.EntityKey, Is.GreaterThan(0));
        }

        [Test]
        public async Task UpdateLocationAddress()
        {
            var service = new LocationService();
            var key = await service.CreateNewLocation();
            
            var location = await service.GetFullServiceLocation(key.EntityKey);
            Assert.That(location.Identifier, Is.Not.Empty);
            Assert.That(location.Coordinate.Latitude, Is.EqualTo(0));

            await service.UpdateLocationAddress(key.EntityKey, location.Version, "2807 LAURELWOOD CT, BALTIMORE, BALTIMORE CITY, MD, USA 21209");
            
            location = await service.GetFullServiceLocation(key.EntityKey);            
            Assert.That(location.Coordinate.Latitude, Is.Not.EqualTo(0));
        }
    }
}
