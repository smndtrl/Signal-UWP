

using Strilanc.Value;
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
using Windows.Storage.Streams;

namespace libtextsecure.messages
{
    public class TextSecureAttachmentStream : TextSecureAttachment
    {

        private readonly IInputStream inputStream;
        private readonly ulong length;
        private readonly May<byte[]> preview;

        public TextSecureAttachmentStream(IInputStream inputStream, String contentType, ulong length)
           : this(inputStream, contentType, length, May<byte[]>.NoValue)
        {
        }

        public TextSecureAttachmentStream(IInputStream inputStream, String contentType, ulong length, May<byte[]> preview)
                : base(contentType)
        {

            this.inputStream = inputStream;
            this.length = length;
        }


        public override bool isStream()
        {
            return true;
        }


        public override bool isPointer()
        {
            return false;
        }

        public IInputStream getInputStream()
        {
            return inputStream;
        }

        public ulong getLength()
        {
            return length;
        }

        public May<byte[]> getPreview()
        {
            return preview;
        }
    }
}

