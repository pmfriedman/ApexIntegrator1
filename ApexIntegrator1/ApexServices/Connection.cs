using ApexServices.Apex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ApexServices
{
    public class Connection
    {
        private Connection()
        {

        }

        private static Connection _sharedConnection;

        internal LoginServiceClient Login
        {
            get;
            private set;
        }

        internal QueryServiceClient Query
        {
            get;
            private set;
        }

        internal MappingServiceClient Mapping
        {
            get;
            private set;
        }

        internal RoutingServiceClient Routing
        {
            get;
            private set;
        }

        internal SessionHeader Session
        {
            get;
            private set;               
        }

        internal SingleRegionContext RegionContext
        {
            get;
            private set;
        }

        public static async Task<Connection> GetConnection()
        {
            if (_sharedConnection == null)
            {
                _sharedConnection = new Connection();

                _sharedConnection.Login = new LoginServiceClient(
                    new BasicHttpsBinding()
                    {
                        MaxReceivedMessageSize = 2147483647
                    },
                    new EndpointAddress("https://apex-cs-login.aws.roadnet.com/Login/LoginService.svc")
                );

                var loginResult = await _sharedConnection.Login.LoginAsync(
                    "pfriedman@roadnet.com",
                    "Roadnet3",
                    new CultureOptions(),
                    new ClientApplicationInfo
                    {
                        ClientApplicationIdentifier = new Guid("94e962d3-370a-439d-adc3-d461d48bb05f")
                    });

                _sharedConnection.Session = new SessionHeader { SessionGuid = loginResult.UserSession.Guid };

                _sharedConnection.Query = new QueryServiceClient(
                    new BasicHttpsBinding()
                    {
                        MaxReceivedMessageSize = 2147483647
                    },
                    new EndpointAddress(loginResult.QueryServiceUrl));

                var retrievalResults = await _sharedConnection.Query.RetrieveRegionsGrantingPermissionsAsync(
                    _sharedConnection.Session,
                    new RolePermission[] { },
                    false);

                var region = retrievalResults.RetrieveRegionsGrantingPermissionsResult.Items.OfType<Region>().First();
                _sharedConnection.RegionContext = new SingleRegionContext
                {
                    BusinessUnitEntityKey = region.BusinessUnitEntityKey,
                    RegionEntityKey = region.EntityKey
                };

                var urls = await _sharedConnection.Query.RetrieveUrlsForContextAsync(
                    _sharedConnection.Session,
                    _sharedConnection.RegionContext);

                _sharedConnection.Mapping = new MappingServiceClient(
                    new BasicHttpsBinding()
                    {
                        MaxReceivedMessageSize = 2147483647
                    },
                    new EndpointAddress(urls.RetrieveUrlsForContextResult.MappingService));

                _sharedConnection.Routing = new RoutingServiceClient(
                    new BasicHttpsBinding()
                    {
                        MaxReceivedMessageSize = 2147483647
                    },
                    new EndpointAddress(urls.RetrieveUrlsForContextResult.RoutingService));
            }

            return _sharedConnection;
        }        

    }
}
