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
using libaxolotl;
using libaxolotl.util;
using libtextsecure.push;
using libtextsecure.util;
using Strilanc.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static libtextsecure.push.TextSecureProtos;

namespace libtextsecure.messages
{
    /**
     * This class represents an encrypted TextSecure envelope.
     *
     * The envelope contains the wrapping information, such as the sender, the
     * message timestamp, the encrypted message type, etc.
     *
      * @author 
     */
    public class TextSecureEnvelope
    {
        private static readonly String TAG = "TextSecureEnvelope";

        private static readonly int SUPPORTED_VERSION = 1;
        private static readonly int CIPHER_KEY_SIZE = 32;
        private static readonly int MAC_KEY_SIZE = 20;
        private static readonly int MAC_SIZE = 10;

        private static readonly int VERSION_OFFSET = 0;
        private static readonly int VERSION_LENGTH = 1;
        private static readonly int IV_OFFSET = VERSION_OFFSET + VERSION_LENGTH;
        private static readonly int IV_LENGTH = 16;
        private static readonly int CIPHERTEXT_OFFSET = IV_OFFSET + IV_LENGTH;

        private readonly Envelope envelope;

        /**
         * Construct an envelope from a serialized, Base64 encoded TextSecureEnvelope, encrypted
         * with a signaling key.
         *
         * @param message The serialized TextSecureEnvelope, base64 encoded and encrypted.
         * @param signalingKey The signaling key.
         * @throws IOException
         * @throws InvalidVersionException
         */
        public TextSecureEnvelope(String message, String signalingKey)
      //throws IOException, InvalidVersionException
      : this(Base64.decode(message), signalingKey)
        {
        }

        /**
         * Construct an envelope from a serialized TextSecureEnvelope, encrypted with a signaling key.
         *
         * @param ciphertext The serialized and encrypted TextSecureEnvelope.
         * @param signalingKey The signaling key.
         * @throws InvalidVersionException
         * @throws IOException
         */
        public TextSecureEnvelope(byte[] ciphertext, String signalingKey)
        //throws InvalidVersionException, IOException
        {
            if (ciphertext.Length < VERSION_LENGTH || ciphertext[VERSION_OFFSET] != SUPPORTED_VERSION)
                throw new InvalidVersionException("Unsupported version!");

            byte[] cipherKey = getCipherKey(signalingKey);
            byte[] macKey = getMacKey(signalingKey);

            verifyMac(ciphertext, macKey);

            this.envelope = Envelope.ParseFrom(getPlaintext(ciphertext, cipherKey));
        }

        public TextSecureEnvelope(uint type, String source, uint sourceDevice,
                                  String relay, ulong timestamp, byte[] legacyMessage, byte[] content)
        {
            Envelope.Builder builder = Envelope.CreateBuilder()
                                       .SetType((Envelope.Types.Type)type)
                                       .SetSource(source)
                                       .SetSourceDevice(sourceDevice)
                                       .SetRelay(relay)
                                       .SetTimestamp(timestamp);

            if (legacyMessage != null) builder.SetLegacyMessage(ByteString.CopyFrom(legacyMessage));
            if (content != null) builder.SetContent(ByteString.CopyFrom(content));

            this.envelope = builder.Build();
        }

        /**
         * @return The envelope's sender.
         */
        public String getSource()
        {
            return envelope.Source;
        }

        /**
         * @return The envelope's sender device ID.
         */
        public uint getSourceDevice()
        {
            return envelope.SourceDevice;
        }

        /**
         * @return The envelope's sender as a TextSecureAddress.
         */
        public TextSecureAddress getSourceAddress()
        {
            return new TextSecureAddress(envelope.Source,
                                         envelope.HasRelay ? new May<String>(envelope.Relay) :
                                                             May<String>.NoValue);
        }

        /**
         * @return The envelope content type.
         */
        public uint getType()
        {
            return (uint)envelope.Type;
        }

        /**
         * @return The federated server this envelope came from.
         */
        public String getRelay()
        {
            return envelope.Relay;
        }

        /**
         * @return The timestamp this envelope was sent.
         */
        public ulong getTimestamp()
        {
            return envelope.Timestamp;
        }

        /**
        * @return Whether the envelope contains a TextSecureDataMessage
        */
        public bool hasLegacyMessage()
        {
            return envelope.HasLegacyMessage;
        }

        /**
         * @return The envelope's containing TextSecure message.
         */
        public byte[] getLegacyMessage()
        {
            return envelope.LegacyMessage.ToByteArray();
        }

        /**
         * @return Whether the envelope contains an encrypted TextSecureContent
         */
        public bool hasContent()
        {
            return envelope.HasContent;
        }

        /**
         * @return The envelope's containing message.
         */
        public byte[] getContent()
        {
            return envelope.Content.ToByteArray();
        }

        /**
         * @return true if the containing message is a {@link org.whispersystems.libaxolotl.protocol.WhisperMessage}
         */
        public bool isWhisperMessage()
        {
            return envelope.Type == Envelope.Types.Type.CIPHERTEXT;
        }

        /**
         * @return true if the containing message is a {@link org.whispersystems.libaxolotl.protocol.PreKeyWhisperMessage}
         */
        public bool isPreKeyWhisperMessage()
        {
            return envelope.Type == Envelope.Types.Type.PREKEY_BUNDLE;
        }

        /**
         * @return true if the containing message is a delivery receipt.
         */
        public bool isReceipt()
        {
            return envelope.Type == Envelope.Types.Type.RECEIPT;
        }

        private byte[] getPlaintext(byte[] ciphertext, byte[] cipherKey) //throws IOException
        {
            byte[] ivBytes = new byte[IV_LENGTH];
            System.Buffer.BlockCopy(ciphertext, IV_OFFSET, ivBytes, 0, ivBytes.Length);

            byte[] message = new byte[ciphertext.Length - VERSION_LENGTH - IV_LENGTH - MAC_SIZE];
            System.Buffer.BlockCopy(ciphertext, CIPHERTEXT_OFFSET, message, 0, message.Length);

            return Decrypt.aesCbcPkcs5(message, cipherKey, ivBytes);
        }

        private void verifyMac(byte[] ciphertext, byte[] macKey)// throws IOException
        {
            try
            {

                /*Mac mac = Mac.getInstance("HmacSHA256");
                mac.init(macKey);*/

                if (ciphertext.Length < MAC_SIZE + 1)
                    throw new Exception("Invalid MAC!");

                byte[] sign = new byte[ciphertext.Length - MAC_SIZE];
                Buffer.BlockCopy(ciphertext, 0, sign, 0, ciphertext.Length - MAC_SIZE);

                //mac.update(ciphertext, 0, ciphertext.Length - MAC_SIZE);

                byte[] ourMacFull = Sign.sha256sum(macKey, sign);
                byte[] ourMacBytes = new byte[MAC_SIZE];
                System.Buffer.BlockCopy(ourMacFull, 0, ourMacBytes, 0, ourMacBytes.Length);

                byte[] theirMacBytes = new byte[MAC_SIZE];
                System.Buffer.BlockCopy(ciphertext, ciphertext.Length - MAC_SIZE, theirMacBytes, 0, theirMacBytes.Length);

                /*Log.w(TAG, "Our MAC: " + Hex.toString(ourMacBytes));
                Log.w(TAG, "Thr MAC: " + Hex.toString(theirMacBytes));
                */
                if (!(ourMacBytes.SequenceEqual(theirMacBytes)))
                {
                    throw new Exception("Invalid MAC compare!");
                }
            }
            catch (InvalidKeyException e) { }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        private byte[] getCipherKey(String signalingKey)// throws IOException
        {
            byte[] signalingKeyBytes = Base64.decode(signalingKey);
            byte[] cipherKey = new byte[CIPHER_KEY_SIZE];
            System.Buffer.BlockCopy(signalingKeyBytes, 0, cipherKey, 0, cipherKey.Length);

            return cipherKey;
        }


        private byte[] getMacKey(String signalingKey)// throws IOException
        {
            byte[] signalingKeyBytes = Base64.decode(signalingKey);
            byte[] macKey = new byte[MAC_KEY_SIZE];
            System.Buffer.BlockCopy(signalingKeyBytes, CIPHER_KEY_SIZE, macKey, 0, macKey.Length);

            return macKey;
        }

    }
}
