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
        public async Task<OrderInfo> CreateOrderAsync(string address)
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

            var routeResult = await connection.Query.RetrieveAsync(
                connection.Session,
                connection.RegionContext,
                new RetrievalOptions
                {
                    Type = typeof(Route).Name,
                    Expression = new AndExpression
                    {
                        Expressions = new[]
                        {
                            new EqualToExpression
                            {
                                Left = new PropertyExpression { Name = "Identifier" },
                                Right = new ValueExpression { Value = Connection.UNASSIGNED_ROUTE_IDENTIFIER }
                            },
                            new EqualToExpression
                            {
                                Left = new PropertyExpression { Name = "RoutingSessionEntityKey" },
                                Right = new ValueExpression { Value = sessionTask.Result.EntityKey }
                            }
                        }
                    },
                    PropertyInclusionMode = PropertyInclusionMode.None
                });

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

            var moveResult = await connection.Routing.MoveOrdersToBestPositionAsync(
                connection.Session,
                connection.RegionContext,
                new DomainInstance[] { new DomainInstance { EntityKey = savedOrder.EntityKey, Version = savedOrder.Version } },
                new OnRouteAutomaticPlacement {
                    AutomaticPlacementGoal_Goal = AutomaticPlacement.AutomaticPlacementGoal.Cost.ToString(),
                    ShouldForcePlacement = true,                    
                    RouteInstance = new DomainInstance { EntityKey = routeResult.RetrieveResult.Items[0].EntityKey, Version = (routeResult.RetrieveResult.Items[0] as Route).Version }
                },
                null
                //new RouteRetrievalOptions[] {
                //    //new RouteRetrievalOptions { EntityKey = routeResult.RetrieveResult.Items[0].EntityKey, InclusionMode = PropertyInclusionMode.None }
                //}
                );
                

            return new OrderInfo
            {
                EntityKey = savedOrder.EntityKey,
                Location = locationTask.Result
            };
        }

        public async Task<List<OrderInfo>> GetOrders()
        {
            var session = await new SessionService().GetOrCreateSessionAsync();

            var connection = await Connection.GetConnectionAsync();

            var ordersResult = await connection.Query.RetrieveAsync(
                connection.Session,
                connection.RegionContext,
                new RetrievalOptions
                {
                    Expression = new EqualToExpression
                    {
                        Left = new PropertyExpression { Name = "SessionEntityKey" },
                        Right = new ValueExpression { Value = session.EntityKey }
                    },
                    Type = typeof(Order).Name,
                    PropertyInclusionMode = PropertyInclusionMode.AllWithoutChildren
                });

            var orders = ordersResult.RetrieveResult.Items.OfType<Order>();

            var orderInfos = orders.Select(o =>
                new OrderInfo
                {
                    EntityKey = o.EntityKey,                    
                });

            return orderInfos.ToList();

        }
    }
}
