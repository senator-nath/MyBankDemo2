using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Persistence.Helper
{
    public class RandomNumberGenerator
    {
        public string Generate11DigitRandomNumber()
        {
            Random random = new Random();
            string result = string.Empty;

            result += random.Next(100000, 1000000).ToString("D6");
            result += random.Next(10000, 100000).ToString("D5");

            return result;
        }
    }
}
