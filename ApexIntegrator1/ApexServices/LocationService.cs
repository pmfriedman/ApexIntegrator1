using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexServices.Apex;

namespace ApexServices
{
    public class LocationService
    {
        public async Task<long> CreateNewLocation()
        {
            var connection = await Connection.GetConnectionAsync();
            var defaults = await DefaultEntities.GetDefaultsAsync();

            var location = new ServiceLocation()
            {
                Action = ActionType.Add,
                Identifier = DateTime.Now.Ticks.ToString(),
                StandingDeliveryQuantities = new Quantities(),
                StandingPickupQuantities = new Quantities(),
                TimeWindowTypeEntityKey = defaults.TimeWindowTypeEntityKey,
                ServiceTimeTypeEntityKey = defaults.ServiceTimeTypeEntityKey,
                Address = new Address()
            };

            var result = await connection.Routing.SaveAsync(
                connection.Session,
                connection.RegionContext,
                new[] { location },
                new SaveOptions
                {
                    InclusionMode = PropertyInclusionMode.All,
                    ReturnInclusionMode = PropertyInclusionMode.None,
                    ReturnSavedItems = false
                });

            var key = result.SaveResult[0].EntityKey;

            return key;
        }
    }
}
