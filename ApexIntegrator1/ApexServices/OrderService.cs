using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexServices.Apex;

namespace ApexServices
{
    public class OrderService
    {
        public async Task<Order> CreateOrder(string address)
        {
            var geocoder = new GeocodeService();
            var geocode = await geocoder.GeocodeAsync(address);

            throw new NotImplementedException();
        }
    }
}
