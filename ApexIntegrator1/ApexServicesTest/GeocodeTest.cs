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
    public class GeocodeTest
    {
        [Test]
        public async Task Geocode()
        {
            var service = new GeocodeService();

            var result = await service.GeocodeAsync("2807 Laurelwood Ct, Baltimore, MD 21209");

            Assert.That(
                result.ResultsData.Single().Description, 
                Is.EqualTo("2807 LAURELWOOD CT, BALTIMORE, BALTIMORE CITY, MD, USA 21209"));
        }
    }
}
