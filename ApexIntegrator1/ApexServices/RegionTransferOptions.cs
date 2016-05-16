using ApexServices.Apex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexServices
{
    public class RegionTransferOptions
    {
        public Region Source { get; set; }
        public Region Target { get; set; }
        public bool ShouldRemoveFromSource { get; set; }
    }
}
