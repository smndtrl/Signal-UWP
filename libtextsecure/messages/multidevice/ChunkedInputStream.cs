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

namespace libtextsecure.messages.multidevice
{
    public class ChunkedInputStream
    {

        protected readonly IInputStream input;

        public ChunkedInputStream(IInputStream input)
        {
            this.input = input;
        }

        protected int readRawVarint32()// throws IOException
        {
            /*byte tmp = (byte)in.read();
            if (tmp >= 0)
            {
                return tmp;
            }
            int result = tmp & 0x7f;
            if ((tmp = (byte)in.read()) >= 0) {
                result |= tmp << 7;
            } else {
                result |= (tmp & 0x7f) << 7;
                if ((tmp = (byte)in.read()) >= 0) {
                    result |= tmp << 14;
                } else {
                    result |= (tmp & 0x7f) << 14;
                    if ((tmp = (byte)in.read()) >= 0) {
                        result |= tmp << 21;
                    } else {
                        result |= (tmp & 0x7f) << 21;
                        result |= (tmp = (byte)in.read()) << 28;
                        if (tmp < 0)
                        {
                            // Discard upper 32 bits.
                            for (int i = 0; i < 5; i++)
                            {
                                if ((byte)in.read() >= 0) {
                                return result;
                            }
                        }

                        throw new Exception("Malformed varint!");
                    }
                }
            }


        }*/
            throw new NotImplementedException();
            //return result;
        }

        /*protected static class LimitedInputStream : FilterInputStream
        {

            private long left;
            private long mark = -1;

            LimitedInputStream(InputStream in, long limit)
            {
                super(in);
                left = limit;
            }

            @Override public int available() throws IOException
            {
            return (int)Math.min(in.available(), left);
        }

        // it's okay to mark even if mark isn't supported, as reset won't work
        @Override public synchronized void mark(int readLimit)
        {
      in.mark(readLimit);
            mark = left;
        }

        @Override public int read() throws IOException
        {
            if (left == 0)
            {
                return -1;
            }

            int result = in.read();
            if (result != -1)
            {
                --left;
            }
            return result;
        }

@Override public int read(byte[] b, int off, int len) throws IOException
{
            if (left == 0)
            {
        return -1;
    }

    len = (int)Math.min(len, left);
            int result = in.read(b, off, len);
            if (result != -1)
            {
        left -= result;
    }
            return result;
}

@Override public synchronized void reset() throws IOException
{
            if (!in.markSupported()) {
        throw new IOException("Mark not supported");
    }
            if (mark == -1)
            {
        throw new IOException("Mark not set");
    }

      in.reset();
    left = mark;
}

@Override public long skip(long n) throws IOException
{
    n = Math.min(n, left);
            long skipped = in.skip(n);
    left -= skipped;
            return skipped;
}
    }
    */
    }
}
