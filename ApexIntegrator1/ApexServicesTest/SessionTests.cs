using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using ApexServices;
using ApexServices.Apex;

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

        [Test]
        public async Task GetSessionsByDate()
        {
            var svc = new SessionService();
            await svc.GetOrCreateSessionAsync();

            var sessions = await svc.GetSessionsForDate(DateTime.Today);

            Assert.That(sessions.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task CreateModelingSession()
        {
            var svc = new SessionService();
            await svc.GetOrCreateSessionAsync();
            var sessions = await svc.GetSessionsForDate(DateTime.Today);
            int numSessions = sessions.Count;

            long jobKey = await svc.CreateModelingSessionForSession(sessions.First().EntityKey);

            await new JobService().WaitForJob(jobKey, typeof(SaveSessionAsJobInfo), TimeSpan.FromSeconds(10));

            sessions = await svc.GetSessionsForDate(DateTime.Today);
            Assert.That(sessions.Count, Is.EqualTo(numSessions + 1));

        }
    }
}
