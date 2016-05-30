using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexServices
{
    public class OrderInfo
    {
        public long EntityKey { get; set; }
        public LocationInfo Location { get; set; } = new LocationInfo();
    }
}
