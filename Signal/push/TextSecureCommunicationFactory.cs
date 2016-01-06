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

using libtextsecure;
using libtextsecure.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.util;

namespace Signal.Push
{
    public class TextSecureCommunicationFactory
    {
        public readonly static String PUSH_URL = "https://textsecure-service-staging.whispersystems.org";

        private readonly static string USER_AGENT = Signal.App.CurrentVersion;

        public static TextSecureAccountManager createManager()
        {
            return new TextSecureAccountManager(PUSH_URL,
                                                new TextSecurePushTrustStore(),
                                                TextSecurePreferences.getLocalNumber(),
                                                TextSecurePreferences.getPushServerPassword(), USER_AGENT);
        }

        public static TextSecureAccountManager createManager(String number, String password)
        {
            return new TextSecureAccountManager(PUSH_URL, new TextSecurePushTrustStore(),
                                                number, password, USER_AGENT);
        }

        public static TextSecureMessageReceiver createReceiver()
        {
            return new TextSecureMessageReceiver(PUSH_URL,
                                             new TextSecurePushTrustStore(),
                                             new DynamicCredentialsProvider(), USER_AGENT);
        }

        public class DynamicCredentialsProvider : CredentialsProvider
        {
            public DynamicCredentialsProvider() { }


            public String GetUser()
            {
                return TextSecurePreferences.getLocalNumber();
            }


            public String GetPassword()
            {
                return TextSecurePreferences.getPushServerPassword();
            }

            public String GetSignalingKey()
            {
                return TextSecurePreferences.getSignalingKey();
            }


        }

    }
}
