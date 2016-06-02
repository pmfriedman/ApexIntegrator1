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
        public async System.Threading.Tasks.Task DeleteCurrentSession()
        {
            RoutingSession currrentSession = await GetCurrentSessionIfExistsAsync();

            if (currrentSession != null)
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
                        EntityKey = currrentSession.EntityKey,
                        Version = currrentSession.Version,
                        StartDate = DateTime.Today.ToApexDate(),
                        Description = SESSION_DESCRIPTION
                    }
                    },
                    new SaveOptions
                    {
                        InclusionMode = PropertyInclusionMode.All
                    });
            }
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

            RoutingSession session = null;
            if (response.RetrieveResult.Items.Length != 0)
                session = response.RetrieveResult.Items[0] as DailyRoutingSession;

            return session;
        }

        public async Task<RoutingSession> GetOrCreateSessionAsync()
        {
            var connection = await Connection.GetConnectionAsync();

            var session = await GetCurrentSessionIfExistsAsync();

            if (session == null)
            {
                session = new DailyRoutingSession
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
                        InclusionMode = PropertyInclusionMode.All,
                        ReturnInclusionMode = PropertyInclusionMode.All,
                        ReturnSavedItems = true
                    });

                session = saveResponse.SaveResult[0].Object as DailyRoutingSession;
            }

            return session;
        }

        private static string SESSION_DESCRIPTION = "ApexIntegrator1";
    }
}
