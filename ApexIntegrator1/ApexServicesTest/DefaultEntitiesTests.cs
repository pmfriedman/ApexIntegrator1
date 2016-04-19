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
    public class DefaultEntitiesTests
    {
        [Test]
        public async Task GetDefaults()
        {
            var defaults = await DefaultEntities.GetDefaultsAsync();

            Assert.That(defaults.ServiceTimeTypeEntityKey, Is.GreaterThan(0));
            Assert.That(defaults.TimeWindowTypeEntityKey, Is.GreaterThan(0));
        }
    }
}
