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
            var sessionKey = await svc.GetOrCreateSessionAsync();

            Assert.That(sessionKey, Is.Not.EqualTo(0));
        }

        [Test]
        public async Task DeleteCurrentSession()
        {
            var svc = new SessionService();
            await svc.DeleteCurrentSession();
        }
    }
}
