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
using Strilanc.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using Signal.Models;

namespace TextSecure.recipient
{
    public class RecipientFactory
    {

        private static readonly RecipientProvider provider = new RecipientProvider();

        public static Recipients getRecipientsForIds(String recipientIds, bool asynchronous)
        {
            if (recipientIds.Equals(""))
                return new Recipients();

            return getRecipientsForIds(recipientIds.Split(' ').ToList(), asynchronous);
        }

        public static Recipients getRecipientsFor(List<Recipient> recipients, bool asynchronous)
        {
            long[] ids = new long[recipients.Count];
            int i = 0;

            foreach (Recipient recipient in recipients)
            {
                ids[i++] = recipient.getRecipientId();
            }

            return provider.getRecipients(ids, asynchronous);

            //return DatabaseFactory.getRecipientDatabase().GetRecipients(recipients);
        }

        public static Recipients getRecipientsFor(Recipient recipient, bool asynchronous)
        {
            long[] ids = new long[1];
            ids[0] = recipient.getRecipientId();

            return provider.getRecipients(ids, asynchronous);

            //return DatabaseFactory.getRecipientDatabase().GetRecipients(new long[] { recipient.RecipientId });
        }

        public static Recipient getRecipientForId(long recipientId, bool asynchronous)
        {
            return provider.getRecipient(recipientId, asynchronous);

            //return DatabaseFactory.getRecipientDatabase().Get(recipientId);
        }

        public static Recipients getRecipientsForIds(long[] recipientIds, bool asynchronous)
        {
            return provider.getRecipients(recipientIds, asynchronous);

            //return DatabaseFactory.getRecipientDatabase().GetRecipients(recipientIds);
        }

        public static Recipients getRecipientsFromString(String rawText, bool asynchronous)
        {
            var tokens = rawText.Split(',');
            List<String> ids = new List<string>();

            foreach (var token in tokens)
            {
                May<long> id = getRecipientIdFromNumber(token);

                if (id.HasValue)
                {
                    ids.Add(Convert.ToString(id.ForceGetValue()));
                }
            }

            return getRecipientsForIds(ids, asynchronous);
        }
        
        private static Recipients getRecipientsForIds(List<String> idStrings, bool asynchronous)
        {
            long[] ids = new long[idStrings.Count];
            int i = 0;

            foreach (String id in idStrings)
            {
                ids[i++] = Convert.ToUInt32(id);
            }


            //return DatabaseFactory.getRecipientDatabase().GetRecipients(ids);
            return provider.getRecipients(ids, asynchronous);
        }

        private static May<long> getRecipientIdFromNumber(String number)
        {
            number = number.Trim();

            if (number.Length == 0) return May<long>.NoValue;

            if (hasBracketedNumber(number))
            {
                number = parseBracketedNumber(number);
            }

            //return new May<long>(DatabaseFactory.getRecipientDatabase().GetRecipientIdForNumber(number));
            var id = provider.getRecipientIdForNumber(number);
            return id == -1 ? May<long>.NoValue : new May<long>(id);

        }

        private static bool hasBracketedNumber(String recipient)
        {
            int openBracketIndex = recipient.IndexOf('<');

            return (openBracketIndex != -1) &&
                   (recipient.IndexOf('>', openBracketIndex) != -1);
        }

        private static String parseBracketedNumber(String recipient)
        {
            int begin = recipient.IndexOf('<');
            int end = recipient.IndexOf('>', begin);
            String value = recipient.Substring(begin + 1, end);

            return value;
        }

        /*
        public static void clearCache()
        {
            ContactPhotoFactory.clearCache();
            provider.clearCache();
        }*/

    }
}
