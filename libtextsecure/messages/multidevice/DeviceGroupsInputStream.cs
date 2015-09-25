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
using Strilanc.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using static libtextsecure.push.TextSecureProtos;

namespace libtextsecure.messages.multidevice
{
    public class DeviceGroupsInputStream : ChunkedInputStream
    {

        public DeviceGroupsInputStream(IInputStream input)
        : base(input)
        {
        }

        public DeviceGroup read()// throws IOException
        {
            /*long detailsLength = readRawVarint32();
            byte[] detailsSerialized = new byte[(int)detailsLength];
            Util.readFully(input, detailsSerialized);

            GroupDetails details = GroupDetails.ParseFrom(detailsSerialized);

            if (!details.HasId)
            {
                throw new Exception("ID missing on group record!");
            }

            byte[] id = details.Id.ToByteArray();
            May<String> name = details.Name == null ? May<string>.NoValue : new May<string>(details.Name);
            IList<String> members = details.MembersList;
            May<TextSecureAttachmentStream> avatar = May<TextSecureAttachmentStream>.NoValue;

            if (details.HasAvatar)
            {
                ulong avatarLength = details.Avatar.Length;
                IInputStream avatarStream = new ChunkedInputStream.LimitedInputStream(input, avatarLength);
                String avatarContentType = details.Avatar.ContentType;

                avatar = new May<TextSecureAttachmentStream>(new TextSecureAttachmentStream(avatarStream, avatarContentType, avatarLength));
            }

            return new DeviceGroup(id, name, members, avatar);*/
            throw new NotImplementedException();
        }

    }
}
