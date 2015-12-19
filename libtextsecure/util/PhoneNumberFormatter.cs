/** 
 * Copyright (C) 2015 smndtrl
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace CustomExtensions {
    public static class StringExtension
    {
        public static string ReplaceAll(this String str, string regex, string replacement)
        {

            return Regex.Replace(str, regex, replacement) ;
        }
    }
}
namespace libtextsecure.util
{
    /**
    * Phone number formats are a pain.
    *
    * @author
    *
    */

    using CustomExtensions;
    public class PhoneNumberFormatter
    {

        public static bool isValidNumber(String number)
        {
            return (new Regex("^\\+[0-9]{10,}").Match(number)).Success;
        }

        private static String impreciseFormatNumber(String number, String localNumber)
        //throws InvalidNumberException
        {
            number = number.ReplaceAll("[^0-9+]", "");

            if (number[0] == '+')
                return number;

            if (localNumber[0] == '+')
                localNumber = localNumber.Substring(1);

            if (localNumber.Length == number.Length || number.Length > localNumber.Length)
                return "+" + number;

            int difference = localNumber.Length - number.Length;

            return "+" + localNumber.Substring(0, difference) + number;
        }

        public static String formatNumberInternational(String number)
        {
            try
            {
                PhoneNumberUtil util = PhoneNumberUtil.GetInstance();
                PhoneNumber parsedNumber = util.Parse(number, null);
                return util.Format(parsedNumber, PhoneNumberFormat.INTERNATIONAL);
            }
            catch (NumberParseException e)
            {
                //Log.w(TAG, e);
                return number;
            }
        }

        public static String formatNumber(String number, String localNumber) //throws InvalidNumberException
        {
            if (number.Contains("@"))
            {
                throw new InvalidNumberException("Possible attempt to use email address.");
            }

            number = number.ReplaceAll("[^0-9+]", "");

            if (number.Length == 0)
            {
                throw new InvalidNumberException("No valid characters found.");
            }

            if (number[0] == '+')
                return number;

            try
            {
                PhoneNumberUtil util = PhoneNumberUtil.GetInstance();
                PhoneNumber localNumberObject = util.Parse(localNumber, null);

                String localCountryCode = util.GetRegionCodeForNumber(localNumberObject);
                //Log.w(TAG, "Got local CC: " + localCountryCode);

                PhoneNumber numberObject = util.Parse(number, localCountryCode);
                return util.Format(numberObject, PhoneNumberFormat.E164);
            }
            catch (NumberParseException e)
            {
                //Log.w(TAG, e);
                return impreciseFormatNumber(number, localNumber);
            }
        }

        public static String getRegionDisplayName(String regionCode)
        {
            return (regionCode == null || regionCode.Equals("ZZ") || regionCode.Equals(PhoneNumberUtil.REGION_CODE_FOR_NON_GEO_ENTITY))
                ? "Unknown country" : "TODO COUNTRY NAM";
        }

        public static String formatE164(String countryCode, String number)
        {
            if (countryCode == string.Empty || number == string.Empty) return string.Empty;
            try
            {
                PhoneNumberUtil util = PhoneNumberUtil.GetInstance();
                int parsedCountryCode = Convert.ToInt32(countryCode);
                PhoneNumber parsedNumber = util.Parse(number,
                                                      util.GetRegionCodeForCountryCode(parsedCountryCode));

                return util.Format(parsedNumber, PhoneNumberFormat.E164);
            }
            catch (NumberParseException npe) {
                return string.Empty;
            } catch (Exception npe)
            {
                return string.Empty;
            }

            return "+" +
                countryCode.ReplaceAll("[^0-9]", "").ReplaceAll("^0*", "") +
                number.ReplaceAll("[^0-9]", "");
            }

  public static String getInternationalFormatFromE164(String e164number)
        {
            try
            {
                PhoneNumberUtil util = PhoneNumberUtil.GetInstance();
                PhoneNumber parsedNumber = util.Parse(e164number, null);
                return util.Format(parsedNumber, PhoneNumberFormat.INTERNATIONAL);
            }
            catch (NumberParseException e)
            {
                //Log.w(TAG, e);
                return e164number;
            }
        }

    }
}
