﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexServices.Apex;

namespace ApexServices
{
    public class GeocodeService
    {
        public async Task<MapLocalSearchResult> GeocodeAsync(string text)
        {
            var connection = await Connection.GetConnectionAsync();
            var result = await connection.Mapping.PerformSearchAsync(
                connection.Session,
                connection.RegionContext,
                new MapLocalSearchCriteria
                {
                    PlaceToSearch = text,
                    PointToBeginSearch = new Coordinate
                    {
                        Latitude = 39376637,
                        Longitude = -76692316
                    }
                },
                new MapLocalSearchOptions
                {
                    PropertyInclusionMode = PropertyInclusionMode.AllWithoutChildren,
                    
                });

            return result.PerformSearchResult;
        }
    }
}
