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
    public class LoginTest
    {
        [Test]
        public void Login()
        {
            var loginSvc = new LoginService();
            loginSvc.Login();
        }
    }
}
