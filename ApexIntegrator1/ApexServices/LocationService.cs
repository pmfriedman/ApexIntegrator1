﻿using System;
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
                Address = new Address
                {
                    Locality = new Locality
                    {
                        CountryISO3Abbr = "USA"
                    }
                },
                WorldTimeZone_TimeZone = WorldTimeZone.EasternTimeUSCanada.ToString()
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

        public async Task<long> UpdateLocationAddress(long locationEntityKey, long version, string addressText)
        {
            var geocode = (await new GeocodeService().GeocodeAsync(addressText)).ResultsData[0];

            var location = new ServiceLocation()
            {
                Action = ActionType.Update,
                EntityKey = locationEntityKey,
                Version = version,
                Description = geocode.Description,
                Coordinate = new Coordinate
                {
                    Latitude = geocode.Location.Latitude,
                    Longitude = geocode.Location.Longitude
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
