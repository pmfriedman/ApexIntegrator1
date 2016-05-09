using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexServices;

namespace ApexServicesTest
{
    [TestFixture]
    public class SessionTests
    {
        [Test]
        public async Task GetOrCreateSession()
        {
            var svc = new SessionService();
            await svc.DeleteCurrentSession();
            var session = await svc.GetOrCreateSessionAsync();

            Assert.That(session, Is.Not.Null);
        }

        [Test]
        public async Task DeleteCurrentSession()
        {
            var svc = new SessionService();
            await svc.DeleteCurrentSession();

            var session = await svc.GetCurrentSessionIfExistsAsync();
            Assert.That(session, Is.Null);
        }
    }
}
