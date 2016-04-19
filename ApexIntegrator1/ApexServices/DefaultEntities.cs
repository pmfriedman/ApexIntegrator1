using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApexServices.Apex;

namespace ApexServices
{
    public class DefaultEntities
    {
        public long TimeWindowTypeEntityKey { get; private set; }
        public long ServiceTimeTypeEntityKey { get; private set; }

        private static DefaultEntities _sharedDefaults;

        private DefaultEntities()
        {

        }

        public static async Task<DefaultEntities> GetDefaultsAsync()
        {
            if (_sharedDefaults == null)
            {
                var connection = await Connection.GetConnectionAsync();



            }

            return _sharedDefaults;
        }
    }
}
