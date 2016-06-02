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
        public async Task<LocationInfo> CreateNewLocation(string addressText = null)
        {
            var connection = await Connection.GetConnectionAsync();
            var defaults = await DefaultEntities.GetDefaultsAsync();

            var coordinate = new Coordinate();

            if (addressText != null)
            {
                var geocode = (await new GeocodeService().GeocodeAsync(addressText)).FirstOrDefault();
                coordinate = geocode?.Coordinate;
                addressText = geocode.Description;
            }
            else
            {
                addressText = string.Empty;
            }
            

            var location = new ServiceLocation()
            {
                Action = ActionType.Add,
                Identifier = DateTime.Now.Ticks.ToString(),
                Description = addressText,
                StandingDeliveryQuantities = new Quantities(),
                StandingPickupQuantities = new Quantities(),
                TimeWindowTypeEntityKey = defaults.TimeWindowTypeEntityKey,
                ServiceTimeTypeEntityKey = defaults.ServiceTimeTypeEntityKey,
                Address = new Address
                {
                    Locality = new Locality
                    {
                        CountryISO3Abbr = "USA"
                    }
                },
                Coordinate = coordinate,
                WorldTimeZone_TimeZone = WorldTimeZone.EasternTimeUSCanada.ToString()
            };

            var returnPropertyOptions = new ServiceLocationPropertyOptions
            {
                Identifier = true
            };

            var result = await connection.Routing.SaveAsync(
                connection.Session,
                connection.RegionContext,
                new[] { location },
                new SaveOptions
                {
                    InclusionMode = PropertyInclusionMode.All,
                    ReturnInclusionMode = PropertyInclusionMode.AccordingToPropertyOptions,
                    ReturnSavedItems = true,
                    ReturnPropertyOptions = returnPropertyOptions
                });

            var retVal = result.SaveResult[0].Object as ServiceLocation;

            return new LocationInfo
            {
                EntityKey = retVal.EntityKey,
                Identifier = retVal.Identifier,
                AddressDescription = addressText
            };
        }

        public async Task<long> UpdateLocationAddress(long locationEntityKey, long version, string addressText)
        {
            var geocode = (await new GeocodeService().GeocodeAsync(addressText)).FirstOrDefault();

            var location = new ServiceLocation()
            {
                Action = ActionType.Update,
                EntityKey = locationEntityKey,
                Version = version,
                Description = geocode.Description,
                Coordinate = new Coordinate
                {
                    Latitude = geocode.Coordinate.Latitude,
                    Longitude = geocode.Coordinate.Longitude
                }
            };

            var propOptions = new ServiceLocationPropertyOptions
            {
                Coordinate = true,
                Description = true
            };

            var connection = await Connection.GetConnectionAsync();
            var result = await connection.Routing.SaveAsync(
                connection.Session,
                connection.RegionContext,
                new[] { location },                
                new SaveOptions
                {
                    InclusionMode = PropertyInclusionMode.AccordingToPropertyOptions,
                    ReturnInclusionMode = PropertyInclusionMode.None,
                    ReturnSavedItems = false,
                    PropertyOptions = propOptions
                });

            var key = result.SaveResult[0].EntityKey;

            return key;
        }

        public async Task<ServiceLocation> GetFullServiceLocation(long entityKey)
        {
            var connection = await Connection.GetConnectionAsync();
            var locationResult = await connection.Query.RetrieveAsync(
                connection.Session,
                connection.RegionContext,
                new RetrievalOptions
                {
                    Type = typeof(ServiceLocation).Name,
                    PropertyInclusionMode = PropertyInclusionMode.All,
                    Expression = new EqualToExpression
                    {
                        Left = new PropertyExpression { Name = "EntityKey" },
                        Right = new ValueExpression { Value = entityKey }
                    }
                });

            return locationResult.RetrieveResult.Items[0] as ServiceLocation;
        }
    }
}
