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

using libtextsecure.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.util;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace Signal.Util
{
    public class Utils
    {
        public static String getSecret(uint size)
        {
            byte[] secret = getSecretBytes(size);
            return Base64.encodeBytes(secret);
        }

        public static byte[] getSecretBytes(uint size)
        {
            byte[] secret = new byte[size];
            IBuffer crypt = CryptographicBuffer.GenerateRandom(size);
            CryptographicBuffer.CopyToByteArray(crypt, out secret);

            return secret;

        }

        public static String canonicalizeNumber(String number)
        {
            String localNumber = TextSecurePreferences.getLocalNumber();
            return PhoneNumberFormatter.formatNumber(number, localNumber);
        }
        
    }


}
