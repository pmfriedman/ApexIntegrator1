using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApexServices
{
    public static class Utilities
    {
        public static string ToApexDate(this DateTime date)
        {
            return date.ToString(_dateFormat);
        }

        private static string _dateFormat = "yyyy-MM-dd";
    }
}
