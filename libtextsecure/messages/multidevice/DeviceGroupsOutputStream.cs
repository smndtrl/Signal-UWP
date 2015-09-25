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

using Google.ProtocolBuffers;
using libtextsecure.push;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace libtextsecure.messages.multidevice
{
    public class DeviceGroupsOutputStream : ChunkedOutputStream
    {

        public DeviceGroupsOutputStream(IOutputStream output)
            : base(output)
        {
        }

        public void write(DeviceGroup group)
        {
            writeGroupDetails(group);
            writeAvatarImage(group);
        }

        public void close()
        {
            //output.close();
        }

        private void writeAvatarImage(DeviceGroup contact)
        {
            if (contact.getAvatar().HasValue)
            {
                //writeStream(contact.getAvatar().get().getInputStream());
            }
        }

        private void writeGroupDetails(DeviceGroup group)// throws IOException
        {
            TextSecureProtos.GroupDetails.Builder groupDetails = TextSecureProtos.GroupDetails.CreateBuilder();
            groupDetails.SetId(ByteString.CopyFrom(group.getId()));

            /*if (group.getName().HasValue)
            {
                groupDetails.SetName(group.getName().get());
            }

            if (group.getAvatar().HasValue)
            {
                TextSecureProtos.GroupDetails.Avatar.Builder avatarBuilder = TextSecureProtos.GroupDetails.Avatar.newBuilder();
                avatarBuilder.setContentType(group.getAvatar().get().getContentType());
                avatarBuilder.setLength((int)group.getAvatar().get().getLength());
                groupDetails.setAvatar(avatarBuilder);
            }

            groupDetails.AddRangeMembers(group.getMembers());

            byte[] serializedContactDetails = groupDetails.Build().ToByteArray();

            writeVarint32(serializedContactDetails.Length);
            output.write(serializedContactDetails);*/
            throw new NotImplementedException();
        }


    }
}
