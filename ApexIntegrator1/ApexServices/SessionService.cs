using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexServices.Apex;

namespace ApexServices
{
    public class SessionService
    {
        public async Task<bool> DeleteCurrentSession()
        {
            long? currrentSessionKey = await GetCurrentSessionIfExistsAsync();

            if (currrentSessionKey.HasValue)
            {
                var connection = await Connection.GetConnectionAsync();
                var result = await connection.Routing.SaveAsync(
                    connection.Session,
                    connection.RegionContext,
                    new[]
                    {
                    new DailyRoutingSession
                    {
                        Action = ActionType.Delete,
                        EntityKey = currrentSessionKey.Value,
                        StartDate = DateTime.Today.ToApexDate(),
                        Description = SESSION_DESCRIPTION
                    }
                    },
                    new SaveOptions
                    {
                        InclusionMode = PropertyInclusionMode.All
                    });
            }
            return true;
        }

        public async Task<RoutingSession> GetCurrentSessionIfExistsAsync()
        {
            var connection = await Connection.GetConnectionAsync();
            var response = await connection.Query.RetrieveAsync(
                connection.Session,
                connection.RegionContext,
                new RetrievalOptions
                {
                    Expression = new EqualToExpression
                    {
                        Left = new PropertyExpression { Name = "StartDate" },
                        Right = new ValueExpression { Value = DateTime.Today }
                    },
                    Type = typeof(DailyRoutingSession).Name,
                    Paged = true,
                    PageSize = 1,
                    PropertyInclusionMode = PropertyInclusionMode.AllWithoutChildren
                });

            long? sessionKey = null;
            if (response.RetrieveResult.Items.Length != 0)
                sessionKey = response.RetrieveResult.Items[0].EntityKey;

            return sessionKey;
        }

        public async Task<long> GetOrCreateSessionAsync()
        {
            var connection = await Connection.GetConnectionAsync();

            var sessionKey = await GetCurrentSessionIfExistsAsync();

            if (sessionKey != null)
            {
                var session = new DailyRoutingSession
                {
                    Action = ActionType.Add,
                    Description = SESSION_DESCRIPTION,
                    NumberOfTimeUnits = 1,
                    SessionMode_Mode = SessionMode.Modeling.ToString(),
                    StartDate = DateTime.Today.ToApexDate(),
                    TimeUnit_TimeUnitType = TimeUnit.Day.ToString()
                };

                var saveResponse = await connection.Routing.SaveAsync(
                    connection.Session,
                    connection.RegionContext,
                    new[] { session },
                    new SaveOptions
                    {
                        InclusionMode = PropertyInclusionMode.All
                    });

                sessionKey = saveResponse.SaveResult[0].EntityKey;
            }

            return sessionKey.Value;
        }

        private static string SESSION_DESCRIPTION = "This and That";
    }
}
