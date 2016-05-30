using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexServices.Apex;
using System.Text.RegularExpressions;

namespace ApexServices
{
    public class GeocodeService
    {
        public async Task<List<GeocodeInfo>> GeocodeAsync(string text)
        {
            var zipCode = Regex.Match(text, @"\s*\d+$").Value;
            if (zipCode == string.Empty)
                text += ", 21209";
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
                    MaxNumberResultsPerSearch = 5
                });

            var list =
                result.PerformSearchResult.ResultsData
                .Select(r => new GeocodeInfo
                {
                    Coordinate = r.Location,
                    Description = r.Description
                })
                .ToList();


            return list;
        }
    }
}
