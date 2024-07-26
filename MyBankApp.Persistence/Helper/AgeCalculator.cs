using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Persistence.Helper
{
    public class AgeCalculator
    {
        public string CalculateAgeFromDateOfBirth(DateTime Dob)
        {
            var today = DateTime.Today;
            var age = today.Year - Dob.Year;

            if (Dob.Date > today.AddYears(-age)) age--;

            var Age = age.ToString();
            return Age;
        }
    }
}
