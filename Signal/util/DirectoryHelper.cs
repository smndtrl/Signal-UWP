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
using libtextsecure.push;
using libtextsecure.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using Signal.Push;

namespace TextSecure.util
{
    public class DirectoryHelper
    {

        public static async Task<bool> refreshDirectory()
        {
            return await refreshDirectory(TextSecureCommunicationFactory.createManager());
        }

        public static async Task<bool> refreshDirectory(TextSecureAccountManager accountManager)
        {
            return await refreshDirectory(accountManager, TextSecurePreferences.getLocalNumber());
        }

        public static async Task<bool> refreshDirectory(TextSecureAccountManager accountManager, String localNumber)
        {
            TextSecureDirectory directory = DatabaseFactory.getDirectoryDatabase();
            List<string> eligibleContactNumbers = await directory.GetNumbersAsync(localNumber);
            List<ContactTokenDetails> activeTokens = await accountManager.getContacts(eligibleContactNumbers);

            if (activeTokens != null)
            {
                foreach (ContactTokenDetails activeToken in activeTokens)
                {
                    Debug.WriteLine($"Token {activeToken.getNumber()}, {activeToken.getToken()}");
                    eligibleContactNumbers.Remove(activeToken.getNumber());
                    activeToken.setNumber(activeToken.getNumber());
                }

                directory.setNumbers(activeTokens, eligibleContactNumbers);
            }

            return true;
        }

        /*public static bool isPushDestination(Recipients recipients)
        {
            try
            {
                if (recipients == null)
                {
                    return false;
                }

                if (!TextSecurePreferences.isPushRegistered(context))
                {
                    return false;
                }

                if (!recipients.isSingleRecipient())
                {
                    return false;
                }

                if (recipients.isGroupRecipient())
                {
                    return true;
                }

                String number = recipients.getPrimaryRecipient().getNumber();

                if (number == null)
                {
                    return false;
                }

                String e164number = Util.canonicalizeNumber(context, number);

                return DatabaseFactory.getDirectoryDatabase().isActiveNumber(e164number);
            }
            catch (InvalidNumberException e)
            {
                //Log.w(TAG, e);
                return false;
            }
            catch (NotInDirectoryException e)
            {
                return false;
            }
        }

        public static bool isSmsFallbackAllowed(Recipients recipients)
        {
            try
            {
                if (recipients == null || !recipients.isSingleRecipient() || recipients.isGroupRecipient())
                {
                    return false;
                }

                String number = recipients.getPrimaryRecipient().getNumber();

                if (number == null)
                {
                    return false;
                }

                String e164number = Util.canonicalizeNumber(context, number);

                return TextSecureDirectory.getInstance(context).isSmsFallbackSupported(e164number);
            }
            catch (InvalidNumberException e)
            {
                Log.w(TAG, e);
                return false;
            }
        }

        public static interface DirectoryUpdateFinishedListener
        {
            public void onUpdateFinished();
        }*/
    }
}
