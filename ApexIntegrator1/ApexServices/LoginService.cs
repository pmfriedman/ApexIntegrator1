using ApexServices.Apex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ApexServices
{
    public class LoginService
    {
        public void Login()
        {
            LoginServiceClient loginServiceClient = new LoginServiceClient(
                new BasicHttpsBinding(),
                new EndpointAddress("https://apex-cs-login.aws.roadnet.com/Login/LoginService.svc")
            );
            
            LoginResult loginResult = loginServiceClient.Login(
                "pfriedman@roadnet.com",
                "Roadnet3",
                new CultureOptions(),
                new ClientApplicationInfo
                {
                    ClientApplicationIdentifier = new Guid("94e962d3-370a-439d-adc3-d461d48bb05f")
                });
        }
    }
}
