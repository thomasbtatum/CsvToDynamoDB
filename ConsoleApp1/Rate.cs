using System;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    public class Rate
    {

        public int Id;
        public int Age;
        public decimal TenYear;
        public decimal TwentyYear;
        public decimal AtAgeSixtyFive;
        public bool Male;
        public bool Smoker;

        public static Rate FromCSV(string csvLine)
        {
            var values = Regex.Split(csvLine, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            
            var rate = new Rate();
            rate.Id = Convert.ToInt32(values[0]);
            rate.Age = Convert.ToInt32(values[1]);
            rate.TenYear = TreatDecimals(values[2]);
            rate.TwentyYear = TreatDecimals(values[3]);
            rate.AtAgeSixtyFive = TreatDecimals(values[4]);
            rate.Male = 'F' == Convert.ToChar(values[5]);
            rate.Smoker = 'Y' == Convert.ToChar(values[6]);
            return rate;

        }

        public static Decimal TreatDecimals(string decimalString)
        {
            if (decimalString == "N/A")
                return Decimal.Zero;
            else
                return Convert.ToDecimal(decimalString.Replace("\\", "").Replace("\"", ""));
        }

    }
}
