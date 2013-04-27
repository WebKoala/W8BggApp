using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGGApp.Common
{
    public static class TextHelper
    {
        
        public static string UCFirst(string input)
        {
            if(string.IsNullOrWhiteSpace(input))
                return input;
            if (input.Length == 1)
                return input.ToUpper();

            return input.Substring(0, 1).ToUpper() + input.Substring(1);

        }
    }
}
