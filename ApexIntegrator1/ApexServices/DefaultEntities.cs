using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexServices.Apex;

namespace ApexServices
{
    public class DefaultEntities
    {
        public long TimeWindowTypeEntityKey { get; private set; }
        public long ServiceTimeTypeEntityKey { get; private set; }

        private static DefaultEntities _sharedDefaults;

        private DefaultEntities()
        {

        }

        public static async Task<DefaultEntities> GetDefaultsAsync()
        {
            if (_sharedDefaults == null)
            {
                _sharedDefaults = new DefaultEntities();

                var connection = await Connection.GetConnectionAsync();

                // service time type

                var svcTimeType = new ServiceTimeType
                {
                    Action = ActionType.Add,
                    Identifier = "APEX_INTEGRATOR_DEF"
                };

                var queryResult = await connection.Query.RetrieveAsync(
                    connection.Session,
                    connection.RegionContext,
                    new RetrievalOptions
                    {
                        Expression = new EqualToExpression
                        {
                            Left = new PropertyExpression { Name = "Identifier" },
                            Right = new ValueExpression { Value = svcTimeType.Identifier }
                        },
                        PropertyInclusionMode = PropertyInclusionMode.All,
                        Type = typeof(ServiceTimeType).Name,
                    });

                if (queryResult.RetrieveResult.Items.Length > 0)
                {
                    _sharedDefaults.ServiceTimeTypeEntityKey = 
                        queryResult.RetrieveResult.Items.OfType<ServiceTimeType>().First().EntityKey;
                }
                else
                {
                    var saveResult =
                        await connection.Routing.SaveAsync(
                            connection.Session,
                            connection.RegionContext,
                            new[] { svcTimeType },
                            new SaveOptions
                            {
                                InclusionMode = PropertyInclusionMode.All,
                                ReturnInclusionMode = PropertyInclusionMode.None,
                                ReturnSavedItems = false
                            });
                    _sharedDefaults.ServiceTimeTypeEntityKey = saveResult.SaveResult.First().EntityKey;
                }

                // time window type

                var twType = new TimeWindowType
                {
                    Action = ActionType.Add,
                    Identifier = "APEX_INTEGRATOR_DEF"
                };

                queryResult = await connection.Query.RetrieveAsync(
                    connection.Session,
                    connection.RegionContext,
                    new RetrievalOptions
                    {
                        Expression = new EqualToExpression
                        {
                            Left = new PropertyExpression { Name = "Identifier" },
                            Right = new ValueExpression { Value = twType.Identifier }
                        },
                        PropertyInclusionMode = PropertyInclusionMode.All,
                        Type = typeof(TimeWindowType).Name,
                    });

                if (queryResult.RetrieveResult.Items.Length > 0)
                {
                    _sharedDefaults.TimeWindowTypeEntityKey =
                        queryResult.RetrieveResult.Items.OfType<TimeWindowType>().First().EntityKey;
                }
                else
                {
                    var saveResult =
                        await connection.Routing.SaveAsync(
                            connection.Session,
                            connection.RegionContext,
                            new[] { twType },
                            new SaveOptions
                            {
                                InclusionMode = PropertyInclusionMode.All,
                                ReturnInclusionMode = PropertyInclusionMode.None,
                                ReturnSavedItems = false
                            });
                    _sharedDefaults.TimeWindowTypeEntityKey = saveResult.SaveResult.First().EntityKey;
                }


            }

            return _sharedDefaults;
        }
    }
}
