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
        private static object _connectionLock = new object();

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

        internal long UserEntityKey
        {
            get;
            private set;
        }

        internal long OrderClassEntityKey
        {
            get;
            private set;
        }
             

        public static async Task<Connection> GetConnectionAsync()
        {
            lock(_connectionLock)
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

                    var loginResult = _sharedConnection.Login.Login(
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

                    var retrievalResults = _sharedConnection.Query.RetrieveRegionsGrantingPermissions(
                        _sharedConnection.Session,
                        new RolePermission[] { },
                        false);

                    var region = retrievalResults.Items.OfType<Region>().First();
                    _sharedConnection.RegionContext = new SingleRegionContext
                    {
                        BusinessUnitEntityKey = region.BusinessUnitEntityKey,
                        RegionEntityKey = region.EntityKey
                    };

                    var urls = _sharedConnection.Query.RetrieveUrlsForContext(
                        _sharedConnection.Session,
                        _sharedConnection.RegionContext);

                    _sharedConnection.Mapping = new MappingServiceClient(
                        new BasicHttpsBinding()
                        {
                            MaxReceivedMessageSize = 2147483647
                        },
                        new EndpointAddress(urls.MappingService));

                    _sharedConnection.Routing = new RoutingServiceClient(
                        new BasicHttpsBinding()
                        {
                            MaxReceivedMessageSize = 2147483647
                        },
                        new EndpointAddress(urls.RoutingService));
                    

                    var userResults = _sharedConnection.Query.Retrieve(
                        _sharedConnection.Session,
                        _sharedConnection.RegionContext,
                        new RetrievalOptions
                        {
                            Expression = new EqualToExpression
                            {
                                Left = new PropertyExpression { Name = "EmailAddress" },
                                Right = new ValueExpression { Value = "pfriedman@roadnet.com" }
                            },
                            PropertyInclusionMode = PropertyInclusionMode.None,
                            Type = typeof(User).Name
                        });
                    _sharedConnection.UserEntityKey = userResults.Items.Single().EntityKey;


                    var orderClassResults = _sharedConnection.Query.Retrieve(
                        _sharedConnection.Session,
                        _sharedConnection.RegionContext,
                        new RetrievalOptions
                        {
                            PropertyInclusionMode = PropertyInclusionMode.None,
                            Type = typeof(OrderClass).Name,
                            Paged = true,
                            PageSize = 1
                        });
                    _sharedConnection.OrderClassEntityKey = orderClassResults.Items.Single().EntityKey;
                }
            }
            return _sharedConnection;
        }   
        
        public static void Reset()
        {
            _sharedConnection = null;
        }

        public static readonly string UNASSIGNED_ROUTE_IDENTIFIER = "UNASSIGNED";     

    }
}
