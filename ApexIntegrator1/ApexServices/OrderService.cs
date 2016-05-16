using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexServices.Apex;
using Task = System.Threading.Tasks.Task;

namespace ApexServices
{
    public class OrderService
    {
        public async Task<Order> CreateOrder(string address)
        {
            var locationSvc = new LocationService();
            var locationTask = locationSvc.CreateNewLocation(address);

            var sessionSvc = new SessionService();
            var sessionTask = sessionSvc.GetOrCreateSessionAsync();

            var connectionTask = Connection.GetConnectionAsync();

            await Task.WhenAll(
                locationTask, 
                sessionTask,
                connectionTask);



            var connection = connectionTask.Result;

            var order = new OrderSpec
            {
                SessionEntityKey = sessionTask.Result.EntityKey,
                BeginDate = sessionTask.Result.StartDate,
                Identifier = locationTask.Result.Identifier,
                TaskSpec = new DeliveryTaskSpec
                {
                    ServiceLocationEntityKey = locationTask.Result.EntityKey,
                    Quantities = new Quantities()
                },
                ManagedByUserEntityKey = connection.UserEntityKey,
                RegionEntityKey = connection.RegionContext.RegionEntityKey.Value,
                OrderClassEntityKey = connection.OrderClassEntityKey,
            };

            var result = await connection.Routing.SaveOrdersAsync(
                connection.Session,
                connection.RegionContext,
                new[] { order },
                new SaveOptions
                {
                    InclusionMode = PropertyInclusionMode.All,
                    ReturnSavedItems = true,
                    ReturnInclusionMode = PropertyInclusionMode.None
                });

            var savedOrder = result.SaveOrdersResult[0].Object as Order;

            return savedOrder;
        }
    }
}
