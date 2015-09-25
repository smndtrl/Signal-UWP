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
using curve25519;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using System.Diagnostics;

namespace libaxolotl.ecc
{

    class Curve25519
    {
        private static Curve25519 instance;
        private Curve25519Native provider = new Curve25519Native();

        private Curve25519() { }

        public static Curve25519 getInstance(int test)
        {
            if (instance == null)
            {
                instance = new Curve25519();
            }
            return instance;
        }


        /**
         * {@link Curve25519} is backed by either a native (via JNI)
         * or pure-Java provider.  By default it prefers the native provider, and falls back to the
         * pure-Java provider if the native library fails to load.
         *
         * @return true if backed by a native provider, false otherwise.
         */
        public bool isNative()
        {
            return provider.isNative();
        }

        /**
         * Generates a Curve25519 keypair.
         *
         * @return A randomly generated Curve25519 keypair.
         */
        public Curve25519KeyPair generateKeyPair()
        {
            byte[] privateKey = provider.generatePrivateKey();
            byte[] publicKey = provider.generatePublicKey(privateKey);

            return new Curve25519KeyPair(publicKey, privateKey);
        }

        /**
         * Calculates an ECDH agreement.
         *
         * @param publicKey The Curve25519 (typically remote party's) public key.
         * @param privateKey The Curve25519 (typically yours) private key.
         * @return A 32-byte shared secret.
         */
        public byte[] calculateAgreement(byte[] publicKey, byte[] privateKey)
        {
            return provider.calculateAgreement(privateKey, publicKey);
        }

        /**
         * Calculates a Curve25519 signature.
         *
         * @param privateKey The private Curve25519 key to create the signature with.
         * @param message The message to sign.
         * @return A 64-byte signature.
         */
        public byte[] calculateSignature(byte[] privateKey, byte[] message)
        {

            byte[] random;
            IBuffer rnd = CryptographicBuffer.GenerateRandom(64);
            CryptographicBuffer.CopyToByteArray(rnd, out random);
            return provider.calculateSignature(random, privateKey, message);
        }

        /**
         * Verify a Curve25519 signature.
         *
         * @param publicKey The Curve25519 public key the signature belongs to.
         * @param message The message that was signed.
         * @param signature The signature to verify.
         * @return true if valid, false if not.
         */
        public bool verifySignature(byte[] publicKey, byte[] message, byte[] signature)
        {
            return provider.verifySignature(publicKey, message, signature);
        }
    }

    /**
     * A tuple that contains a Curve25519 public and private key.
     *
     * @author
     */
    public class Curve25519KeyPair
    {

        private readonly byte[] publicKey;
        private readonly byte[] privateKey;

        public Curve25519KeyPair(byte[] publicKey, byte[] privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = privateKey;
        }

        /**
         * @return The Curve25519 public key.
         */
        public byte[] getPublicKey()
        {
            return publicKey;
        }

        /**
         * @return The Curve25519 private key.
         */
        public byte[] getPrivateKey()
        {
            return privateKey;
        }
    }
}
