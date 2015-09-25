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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSecure.util
{
    public class GroupUtil
    {

        private static readonly String ENCODED_GROUP_PREFIX = "__textsecure_group__!";

        public static String getEncodedId(byte[] groupId)
        {
            return ENCODED_GROUP_PREFIX + Hex.toStringCondensed(groupId);
        }

        public static byte[] getDecodedId(String groupId)// throws IOException
        {
            if (!isEncodedGroup(groupId))
            {
                throw new Exception("Invalid encoding");
            }

            return Hex.fromStringCondensed(groupId.Split('!')[1]);
        }
        

        public static bool isEncodedGroup(String groupId)
        {
            return groupId.StartsWith(ENCODED_GROUP_PREFIX);
        }

        /*public static String getDescription(Context context, String encodedGroup)
        {
            if (encodedGroup == null)
            {
                return context.getString(R.string.GroupUtil_group_updated);
            }

            try
            {
                StringBuilder description = new StringBuilder();
                GroupContext groupContext = GroupContext.parseFrom(Base64.decode(encodedGroup));
                List<String> members = groupContext.getMembersList();
                String title = groupContext.getName();

                if (!members.isEmpty())
                {
                    description.append(context.getString(R.string.GroupUtil_joined_the_group, Util.join(members, ", ")));
                }

                if (title != null && !title.trim().isEmpty())
                {
                    if (description.length() > 0) description.append(" ");
                    description.append(context.getString(R.string.GroupUtil_title_is_now, title));
                }

                if (description.length() > 0)
                {
                    return description.toString();
                }
                else
                {
                    return context.getString(R.string.GroupUtil_group_updated);
                }
            }
            catch (InvalidProtocolBufferException e)
            {
                Log.w("GroupUtil", e);
                return context.getString(R.string.GroupUtil_group_updated);
            }
            catch (IOException e)
            {
                Log.w("GroupUtil", e);
                return context.getString(R.string.GroupUtil_group_updated);
            }
        }*/
    }
}
