using libaxolotl;
using libaxolotl.ecc;
using libaxolotl.protocol;
using libaxolotl.state;
using libaxolotl.util;
using libaxolotl.exceptions;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libaxolotl_test
{
    public class DecryptionCallbackTest : DecryptionCallback
    {
        public void handlePlaintext(byte[] plaintext)
        {
            throw new NotImplementedException();
        }

        public void handlePlaintext(byte[] plaintext, String originalMessage, AxolotlStore bobStore)
        {
            //assertTrue(originalMessage.equals(new String(plaintext)));
            //assertFalse(bobStore.containsSession(ALICE_ADDRESS));
        }
    }
    [TestClass]
    public class SessionBuilderTest
    {
        private static readonly AxolotlAddress ALICE_ADDRESS = new AxolotlAddress("+14151111111", 1);
        private static readonly AxolotlAddress BOB_ADDRESS = new AxolotlAddress("+14152222222", 1);



        [TestMethod]
        public void testBasicPreKeyV2()
        {
            AxolotlStore aliceStore = new TestInMemoryAxolotlStore();
            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);

            AxolotlStore bobStore = new TestInMemoryAxolotlStore();
            ECKeyPair bobPreKeyPair = Curve.generateKeyPair();
            PreKeyBundle bobPreKey = new PreKeyBundle(bobStore.getLocalRegistrationId(), 1,
                                                          31337, bobPreKeyPair.getPublicKey(),
                                                          0, null, null,
                                                          bobStore.getIdentityKeyPair().getPublicKey());

            aliceSessionBuilder.process(bobPreKey);

            Assert.IsTrue(aliceStore.containsSession(BOB_ADDRESS));
            Assert.IsTrue(aliceStore.loadSession(BOB_ADDRESS).getSessionState().getSessionVersion() == 2);

            String originalMessage = "L'homme est condamné à être libre";
            SessionCipher aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);
            CiphertextMessage outgoingMessage = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));

            Assert.IsTrue(outgoingMessage.getType() == CiphertextMessage.PREKEY_TYPE);

            PreKeyWhisperMessage incomingMessage = new PreKeyWhisperMessage(outgoingMessage.serialize());
            bobStore.storePreKey(31337, new PreKeyRecord(bobPreKey.getPreKeyId(), bobPreKeyPair));

            SessionCipher bobSessionCipher = new SessionCipher(bobStore, ALICE_ADDRESS);
            byte[] plaintext = bobSessionCipher.decrypt(incomingMessage);

            Assert.IsTrue(bobStore.containsSession(ALICE_ADDRESS));
            Assert.IsTrue(bobStore.loadSession(ALICE_ADDRESS).getSessionState().getSessionVersion() == 2);
            //Assert.IsTrue(originalMessage.Equals(plaintext));
            CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(originalMessage), plaintext);

            CiphertextMessage bobOutgoingMessage = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));
            Assert.IsTrue(bobOutgoingMessage.getType() == CiphertextMessage.WHISPER_TYPE);

            byte[] alicePlaintext = aliceSessionCipher.decrypt((WhisperMessage)bobOutgoingMessage);
            //Assert.IsTrue(alicePlaintext.Equals(originalMessage));
            CollectionAssert.AreEqual(alicePlaintext, Encoding.UTF8.GetBytes(originalMessage));

            runInteraction(aliceStore, bobStore);

            aliceStore = new TestInMemoryAxolotlStore();
            aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);
            aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);

            bobPreKeyPair = Curve.generateKeyPair();
            bobPreKey = new PreKeyBundle(bobStore.getLocalRegistrationId(),
                                         1, 31338, bobPreKeyPair.getPublicKey(),
                                         0, null, null, bobStore.getIdentityKeyPair().getPublicKey());

            bobStore.storePreKey(31338, new PreKeyRecord(bobPreKey.getPreKeyId(), bobPreKeyPair));
            aliceSessionBuilder.process(bobPreKey);

            outgoingMessage = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));

            try
            {
                bobSessionCipher.decrypt(new PreKeyWhisperMessage(outgoingMessage.serialize()));
                throw new Exception("shouldn't be trusted!");
            }
            catch (UntrustedIdentityException uie)
            {
                bobStore.saveIdentity(ALICE_ADDRESS.getName(), new PreKeyWhisperMessage(outgoingMessage.serialize()).getIdentityKey());
            }

            plaintext = bobSessionCipher.decrypt(new PreKeyWhisperMessage(outgoingMessage.serialize()));

            //Assert.IsTrue(plaintext.Equals(originalMessage));
            CollectionAssert.AreEqual(plaintext, Encoding.UTF8.GetBytes(originalMessage));


            bobPreKey = new PreKeyBundle(bobStore.getLocalRegistrationId(), 1,
                                         31337, Curve.generateKeyPair().getPublicKey(),
                                         0, null, null,
                                         aliceStore.getIdentityKeyPair().getPublicKey());

            try
            {
                aliceSessionBuilder.process(bobPreKey);
                throw new Exception("shoulnd't be trusted!");
            }
            catch (UntrustedIdentityException uie)
            {
                // good
            }
        }

        [TestMethod]
        public void testBasicPreKeyV3()
        {
            AxolotlStore aliceStore = new TestInMemoryAxolotlStore();
            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);

            AxolotlStore bobStore = new TestInMemoryAxolotlStore();
            ECKeyPair bobPreKeyPair = Curve.generateKeyPair();
            ECKeyPair bobSignedPreKeyPair = Curve.generateKeyPair();
            byte[] bobSignedPreKeySignature = Curve.calculateSignature(bobStore.getIdentityKeyPair().getPrivateKey(),
                                                                             bobSignedPreKeyPair.getPublicKey().serialize());

            PreKeyBundle bobPreKey = new PreKeyBundle(bobStore.getLocalRegistrationId(), 1,
                                                      31337, bobPreKeyPair.getPublicKey(),
                                                      22, bobSignedPreKeyPair.getPublicKey(),
                                                      bobSignedPreKeySignature,
                                                      bobStore.getIdentityKeyPair().getPublicKey());

            aliceSessionBuilder.process(bobPreKey);

            Assert.IsTrue(aliceStore.containsSession(BOB_ADDRESS));
            Assert.IsTrue(aliceStore.loadSession(BOB_ADDRESS).getSessionState().getSessionVersion() == 3);

            String originalMessage = "L'homme est condamné à être libre";
            SessionCipher aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);
            CiphertextMessage outgoingMessage = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));

            Assert.IsTrue(outgoingMessage.getType() == CiphertextMessage.PREKEY_TYPE);

            PreKeyWhisperMessage incomingMessage = new PreKeyWhisperMessage(outgoingMessage.serialize());
            bobStore.storePreKey(31337, new PreKeyRecord(bobPreKey.getPreKeyId(), bobPreKeyPair));
            bobStore.storeSignedPreKey(22, new SignedPreKeyRecord(22, (ulong)DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond, bobSignedPreKeyPair, bobSignedPreKeySignature));

            SessionCipher bobSessionCipher = new SessionCipher(bobStore, ALICE_ADDRESS);
            byte[] plaintext = bobSessionCipher.decrypt(incomingMessage/*, new DecryptionCallbackTest().handlePlaintext({ 0x55}, originalMessage, bobStore)*/);

            Assert.IsTrue(bobStore.containsSession(ALICE_ADDRESS));
            Assert.IsTrue(bobStore.loadSession(ALICE_ADDRESS).getSessionState().getSessionVersion() == 3);
            Assert.IsTrue(bobStore.loadSession(ALICE_ADDRESS).getSessionState().getAliceBaseKey() != null);
			String plaintextString = Encoding.UTF8.GetString(plaintext, 0, plaintext.Length);
			Assert.IsTrue(originalMessage.Equals(plaintextString));

            CiphertextMessage bobOutgoingMessage = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));
            Assert.IsTrue(bobOutgoingMessage.getType() == CiphertextMessage.WHISPER_TYPE);

            byte[] alicePlaintext = aliceSessionCipher.decrypt(new WhisperMessage(bobOutgoingMessage.serialize()));
			String alicePlaintextString = Encoding.UTF8.GetString(alicePlaintext, 0, alicePlaintext.Length);
            Assert.IsTrue(alicePlaintextString.Equals(originalMessage));

            runInteraction(aliceStore, bobStore);

            aliceStore = new TestInMemoryAxolotlStore();
            aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);
            aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);

            bobPreKeyPair = Curve.generateKeyPair();
            bobSignedPreKeyPair = Curve.generateKeyPair();
            bobSignedPreKeySignature = Curve.calculateSignature(bobStore.getIdentityKeyPair().getPrivateKey(), bobSignedPreKeyPair.getPublicKey().serialize());
            bobPreKey = new PreKeyBundle(bobStore.getLocalRegistrationId(),
                                         1, 31338, bobPreKeyPair.getPublicKey(),
                                         23, bobSignedPreKeyPair.getPublicKey(), bobSignedPreKeySignature,
                                         bobStore.getIdentityKeyPair().getPublicKey());

            bobStore.storePreKey(31338, new PreKeyRecord(bobPreKey.getPreKeyId(), bobPreKeyPair));
            bobStore.storeSignedPreKey(23, new SignedPreKeyRecord(23, (ulong)DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond, bobSignedPreKeyPair, bobSignedPreKeySignature));
            aliceSessionBuilder.process(bobPreKey);

            outgoingMessage = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));

            try
            {
                plaintext = bobSessionCipher.decrypt(new PreKeyWhisperMessage(outgoingMessage.serialize()));
                throw new Exception("shouldn't be trusted!");
            }
            catch (UntrustedIdentityException uie)
            {
                bobStore.saveIdentity(ALICE_ADDRESS.getName(), new PreKeyWhisperMessage(outgoingMessage.serialize()).getIdentityKey());
            }

            plaintext = bobSessionCipher.decrypt(new PreKeyWhisperMessage(outgoingMessage.serialize()));
			plaintextString = Encoding.UTF8.GetString(plaintext, 0, plaintext.Length);
            Assert.IsTrue(plaintextString.Equals(originalMessage));

            bobPreKey = new PreKeyBundle(bobStore.getLocalRegistrationId(), 1,
                                         31337, Curve.generateKeyPair().getPublicKey(),
                                         23, bobSignedPreKeyPair.getPublicKey(), bobSignedPreKeySignature,
                                         aliceStore.getIdentityKeyPair().getPublicKey());

            try
            {
                aliceSessionBuilder.process(bobPreKey);
                throw new Exception("shoulnd't be trusted!");
            }
            catch (UntrustedIdentityException uie)
            {
                // good
            }
        }


        [TestMethod]
        public void testBadSignedPreKeySignature()// throws InvalidKeyException, UntrustedIdentityException 
        {
            AxolotlStore aliceStore = new TestInMemoryAxolotlStore();
            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);

            IdentityKeyStore bobIdentityKeyStore = new TestInMemoryIdentityKeyStore();

            ECKeyPair bobPreKeyPair = Curve.generateKeyPair();
            ECKeyPair bobSignedPreKeyPair = Curve.generateKeyPair();
            byte[] bobSignedPreKeySignature = Curve.calculateSignature(bobIdentityKeyStore.getIdentityKeyPair().getPrivateKey(),
                                                                          bobSignedPreKeyPair.getPublicKey().serialize());


            for (int i = 0; i < bobSignedPreKeySignature.Length * 8; i++)
            {
                byte[] modifiedSignature = new byte[bobSignedPreKeySignature.Length];
                System.Buffer.BlockCopy(bobSignedPreKeySignature, 0, modifiedSignature, 0, modifiedSignature.Length);

                //modifiedSignature[i / 8] ^= (0x01 << (i % 8)); // TODO: yep

                PreKeyBundle bobPreKey1 = new PreKeyBundle(bobIdentityKeyStore.getLocalRegistrationId(), 1,
                                                          31337, bobPreKeyPair.getPublicKey(),
                                                          22, bobSignedPreKeyPair.getPublicKey(), modifiedSignature,
                                                          bobIdentityKeyStore.getIdentityKeyPair().getPublicKey());

                try
                {
                    aliceSessionBuilder.process(bobPreKey1);
                    throw new Exception("Accepted modified device key signature!");
                }
                catch (Exception ike)
                {

                }
            }

            PreKeyBundle bobPreKey = new PreKeyBundle(bobIdentityKeyStore.getLocalRegistrationId(), 1,
                                                      31337, bobPreKeyPair.getPublicKey(),
                                                      22, bobSignedPreKeyPair.getPublicKey(), bobSignedPreKeySignature,
                                                      bobIdentityKeyStore.getIdentityKeyPair().getPublicKey());

            aliceSessionBuilder.process(bobPreKey);
        }

        [TestMethod]
        public void testRepeatBundleMessageV2()// throws InvalidKeyException, UntrustedIdentityException, InvalidVersionException, InvalidMessageException, InvalidKeyIdException, DuplicateMessageException, LegacyMessageException, NoSessionException 
        {
            AxolotlStore aliceStore = new TestInMemoryAxolotlStore();
            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);

            AxolotlStore bobStore = new TestInMemoryAxolotlStore();

            ECKeyPair bobPreKeyPair = Curve.generateKeyPair();
            ECKeyPair bobSignedPreKeyPair = Curve.generateKeyPair();
            byte[] bobSignedPreKeySignature = Curve.calculateSignature(bobStore.getIdentityKeyPair().getPrivateKey(),
                                                                          bobSignedPreKeyPair.getPublicKey().serialize());

            PreKeyBundle bobPreKey = new PreKeyBundle(bobStore.getLocalRegistrationId(), 1,
                                                      31337, bobPreKeyPair.getPublicKey(),
                                                      0, null, null,
                                                      bobStore.getIdentityKeyPair().getPublicKey());

            bobStore.storePreKey(31337, new PreKeyRecord(bobPreKey.getPreKeyId(), bobPreKeyPair));
            bobStore.storeSignedPreKey(22, new SignedPreKeyRecord(22, (ulong)DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond, bobSignedPreKeyPair, bobSignedPreKeySignature));

            aliceSessionBuilder.process(bobPreKey);

            String originalMessage = "L'homme est condamné à être libre";
            SessionCipher aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);
            CiphertextMessage outgoingMessageOne = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));
            CiphertextMessage outgoingMessageTwo = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));

            Assert.IsTrue(outgoingMessageOne.getType() == CiphertextMessage.PREKEY_TYPE);

            PreKeyWhisperMessage incomingMessage = new PreKeyWhisperMessage(outgoingMessageOne.serialize());

            SessionCipher bobSessionCipher = new SessionCipher(bobStore, ALICE_ADDRESS);

            byte[] plaintext = bobSessionCipher.decrypt(incomingMessage);
            CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(originalMessage), plaintext);

            CiphertextMessage bobOutgoingMessage = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));

            byte[] alicePlaintext = aliceSessionCipher.decrypt(new WhisperMessage(bobOutgoingMessage.serialize()));
            CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(originalMessage), alicePlaintext);

            // The test

            PreKeyWhisperMessage incomingMessageTwo = new PreKeyWhisperMessage(outgoingMessageTwo.serialize());

            plaintext = bobSessionCipher.decrypt(incomingMessageTwo);
            CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(originalMessage), plaintext);


            bobOutgoingMessage = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));
            alicePlaintext = aliceSessionCipher.decrypt(new WhisperMessage(bobOutgoingMessage.serialize()));
            CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(originalMessage), alicePlaintext);


        }

        [TestMethod]
        public void testRepeatBundleMessageV3()// throws InvalidKeyException, UntrustedIdentityException, InvalidVersionException, InvalidMessageException, InvalidKeyIdException, DuplicateMessageException, LegacyMessageException, NoSessionException 
        {
            AxolotlStore aliceStore = new TestInMemoryAxolotlStore();
            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);

            AxolotlStore bobStore = new TestInMemoryAxolotlStore();

            ECKeyPair bobPreKeyPair = Curve.generateKeyPair();
            ECKeyPair bobSignedPreKeyPair = Curve.generateKeyPair();
            byte[] bobSignedPreKeySignature = Curve.calculateSignature(bobStore.getIdentityKeyPair().getPrivateKey(),
                                                                          bobSignedPreKeyPair.getPublicKey().serialize());

            PreKeyBundle bobPreKey = new PreKeyBundle(bobStore.getLocalRegistrationId(), 1,
                                                      31337, bobPreKeyPair.getPublicKey(),
                                                      22, bobSignedPreKeyPair.getPublicKey(), bobSignedPreKeySignature,
                                                      bobStore.getIdentityKeyPair().getPublicKey());

            bobStore.storePreKey(31337, new PreKeyRecord(bobPreKey.getPreKeyId(), bobPreKeyPair));
            bobStore.storeSignedPreKey(22, new SignedPreKeyRecord(22, (ulong)DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond, bobSignedPreKeyPair, bobSignedPreKeySignature));

            aliceSessionBuilder.process(bobPreKey);

            String originalMessage = "L'homme est condamné à être libre";
            SessionCipher aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);
            CiphertextMessage outgoingMessageOne = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));
            CiphertextMessage outgoingMessageTwo = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));

            Assert.IsTrue(outgoingMessageOne.getType() == CiphertextMessage.PREKEY_TYPE);
            Assert.IsTrue(outgoingMessageTwo.getType() == CiphertextMessage.PREKEY_TYPE);

            PreKeyWhisperMessage incomingMessage = new PreKeyWhisperMessage(outgoingMessageOne.serialize());

            SessionCipher bobSessionCipher = new SessionCipher(bobStore, ALICE_ADDRESS);

            byte[] plaintext = bobSessionCipher.decrypt(incomingMessage);
            //Assert.IsTrue(originalMessage.Equals((plaintext)));
            CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(originalMessage), plaintext);


            CiphertextMessage bobOutgoingMessage = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));

            byte[] alicePlaintext = aliceSessionCipher.decrypt(new WhisperMessage(bobOutgoingMessage.serialize()));
            CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(originalMessage), alicePlaintext);

            // The test

            PreKeyWhisperMessage incomingMessageTwo = new PreKeyWhisperMessage(outgoingMessageTwo.serialize());

            plaintext = bobSessionCipher.decrypt(new PreKeyWhisperMessage(incomingMessageTwo.serialize()));
            //Assert.IsTrue(originalMessage.Equals(plaintext));
            CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(originalMessage), plaintext);


            bobOutgoingMessage = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));
            alicePlaintext = aliceSessionCipher.decrypt(new WhisperMessage(bobOutgoingMessage.serialize()));
            //Assert.IsTrue(originalMessage.Equals(alicePlaintext));
            CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(originalMessage), alicePlaintext);


        }

        [TestMethod]
        public void testBadMessageBundle()
        {
            AxolotlStore aliceStore = new TestInMemoryAxolotlStore();
            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);

            AxolotlStore bobStore = new TestInMemoryAxolotlStore();

            ECKeyPair bobPreKeyPair = Curve.generateKeyPair();
            ECKeyPair bobSignedPreKeyPair = Curve.generateKeyPair();
            byte[] bobSignedPreKeySignature = Curve.calculateSignature(bobStore.getIdentityKeyPair().getPrivateKey(),
                                                                          bobSignedPreKeyPair.getPublicKey().serialize());

            PreKeyBundle bobPreKey = new PreKeyBundle(bobStore.getLocalRegistrationId(), 1,
                                                      31337, bobPreKeyPair.getPublicKey(),
                                                      22, bobSignedPreKeyPair.getPublicKey(), bobSignedPreKeySignature,
                                                      bobStore.getIdentityKeyPair().getPublicKey());

            bobStore.storePreKey(31337, new PreKeyRecord(bobPreKey.getPreKeyId(), bobPreKeyPair));
            bobStore.storeSignedPreKey(22, new SignedPreKeyRecord(22, (ulong)DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond, bobSignedPreKeyPair, bobSignedPreKeySignature));

            aliceSessionBuilder.process(bobPreKey);

            String originalMessage = "L'homme est condamné à être libre";
            SessionCipher aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);
            CiphertextMessage outgoingMessageOne = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));

            Assert.IsTrue(outgoingMessageOne.getType() == CiphertextMessage.PREKEY_TYPE);

            byte[] goodMessage = outgoingMessageOne.serialize();
            byte[] badMessage = new byte[goodMessage.Length];
            System.Buffer.BlockCopy(goodMessage, 0, badMessage, 0, badMessage.Length);

            badMessage[badMessage.Length - 10] ^= 0x01;

            PreKeyWhisperMessage incomingMessage = new PreKeyWhisperMessage(badMessage);
            SessionCipher bobSessionCipher = new SessionCipher(bobStore, ALICE_ADDRESS);

            byte[] plaintext = new byte[0];

            try
            {
                plaintext = bobSessionCipher.decrypt(incomingMessage);
                throw new Exception("Decrypt should have failed!");
            }
            catch (InvalidMessageException e)
            {
                // good.
            }

            Assert.IsTrue(bobStore.containsPreKey(31337));

            plaintext = bobSessionCipher.decrypt(new PreKeyWhisperMessage(goodMessage));

            //Assert.IsTrue(originalMessage.Equals(plaintext));
            CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(originalMessage), plaintext);

            Assert.IsTrue(!bobStore.containsPreKey(31337));
        }

        [TestMethod]
        public void testBasicKeyExchange()
        {
            AxolotlStore aliceStore = new TestInMemoryAxolotlStore();
            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);

            AxolotlStore bobStore = new TestInMemoryAxolotlStore();
            SessionBuilder bobSessionBuilder = new SessionBuilder(bobStore, ALICE_ADDRESS);

            KeyExchangeMessage aliceKeyExchangeMessage = aliceSessionBuilder.process();
            Assert.IsTrue(aliceKeyExchangeMessage != null);

            byte[] aliceKeyExchangeMessageBytes = aliceKeyExchangeMessage.serialize();
            KeyExchangeMessage bobKeyExchangeMessage = bobSessionBuilder.process(new KeyExchangeMessage(aliceKeyExchangeMessageBytes));

            Assert.IsTrue(bobKeyExchangeMessage != null);

            byte[] bobKeyExchangeMessageBytes = bobKeyExchangeMessage.serialize();
            KeyExchangeMessage response = aliceSessionBuilder.process(new KeyExchangeMessage(bobKeyExchangeMessageBytes));

            Assert.IsTrue(response == null);
            Assert.IsTrue(aliceStore.containsSession(BOB_ADDRESS));
            Assert.IsTrue(bobStore.containsSession(ALICE_ADDRESS));

            runInteraction(aliceStore, bobStore);

            aliceStore = new TestInMemoryAxolotlStore();
            aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);
            aliceKeyExchangeMessage = aliceSessionBuilder.process();

            try
            {
                bobKeyExchangeMessage = bobSessionBuilder.process(aliceKeyExchangeMessage);
                throw new Exception("This identity shouldn't be trusted!");
            }
            catch (UntrustedIdentityException e)
            {
                bobStore.saveIdentity(ALICE_ADDRESS.getName(), aliceKeyExchangeMessage.getIdentityKey());
                bobKeyExchangeMessage = bobSessionBuilder.process(aliceKeyExchangeMessage);
            }

            Assert.IsTrue(aliceSessionBuilder.process(bobKeyExchangeMessage) == null);

            runInteraction(aliceStore, bobStore);
        }

        [TestMethod]
        public void testSimultaneousKeyExchange()
        {
            AxolotlStore aliceStore = new TestInMemoryAxolotlStore();
            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);

            AxolotlStore bobStore = new TestInMemoryAxolotlStore();
            SessionBuilder bobSessionBuilder = new SessionBuilder(bobStore, ALICE_ADDRESS);

            KeyExchangeMessage aliceKeyExchange = aliceSessionBuilder.process();
            KeyExchangeMessage bobKeyExchange = bobSessionBuilder.process();

            Assert.IsTrue(aliceKeyExchange != null);
            Assert.IsTrue(bobKeyExchange != null);

            KeyExchangeMessage aliceResponse = aliceSessionBuilder.process(bobKeyExchange);
            KeyExchangeMessage bobResponse = bobSessionBuilder.process(aliceKeyExchange);

            Assert.IsTrue(aliceResponse != null);
            Assert.IsTrue(bobResponse != null);

            KeyExchangeMessage aliceAck = aliceSessionBuilder.process(bobResponse);
            KeyExchangeMessage bobAck = bobSessionBuilder.process(aliceResponse);

            Assert.IsTrue(aliceAck == null);
            Assert.IsTrue(bobAck == null);

            runInteraction(aliceStore, bobStore);
        }

        [TestMethod]
        public void testOptionalOneTimePreKey()
        {
            AxolotlStore aliceStore = new TestInMemoryAxolotlStore();
            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);

            AxolotlStore bobStore = new TestInMemoryAxolotlStore();

            ECKeyPair bobPreKeyPair = Curve.generateKeyPair();
            ECKeyPair bobSignedPreKeyPair = Curve.generateKeyPair();
            byte[] bobSignedPreKeySignature = Curve.calculateSignature(bobStore.getIdentityKeyPair().getPrivateKey(),
                                                                          bobSignedPreKeyPair.getPublicKey().serialize());

            PreKeyBundle bobPreKey = new PreKeyBundle(bobStore.getLocalRegistrationId(), 1,
                                                      0, null,
                                                      22, bobSignedPreKeyPair.getPublicKey(),
                                                      bobSignedPreKeySignature,
                                                      bobStore.getIdentityKeyPair().getPublicKey());

            aliceSessionBuilder.process(bobPreKey);

            Assert.IsTrue(aliceStore.containsSession(BOB_ADDRESS));
            Assert.IsTrue(aliceStore.loadSession(BOB_ADDRESS).getSessionState().getSessionVersion() == 3);

            String originalMessage = "L'homme est condamné à être libre";
            SessionCipher aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);
            CiphertextMessage outgoingMessage = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));

            Assert.IsTrue(outgoingMessage.getType() == CiphertextMessage.PREKEY_TYPE);

            PreKeyWhisperMessage incomingMessage = new PreKeyWhisperMessage(outgoingMessage.serialize());
            Assert.IsTrue(!incomingMessage.getPreKeyId().HasValue);

            bobStore.storePreKey(31337, new PreKeyRecord(bobPreKey.getPreKeyId(), bobPreKeyPair));
            bobStore.storeSignedPreKey(22, new SignedPreKeyRecord(22, (ulong)DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond, bobSignedPreKeyPair, bobSignedPreKeySignature));

            SessionCipher bobSessionCipher = new SessionCipher(bobStore, ALICE_ADDRESS);
            byte[] plaintext = bobSessionCipher.decrypt(incomingMessage);

            Assert.IsTrue(bobStore.containsSession(ALICE_ADDRESS));
            Assert.IsTrue(bobStore.loadSession(ALICE_ADDRESS).getSessionState().getSessionVersion() == 3);
            Assert.IsTrue(bobStore.loadSession(ALICE_ADDRESS).getSessionState().getAliceBaseKey() != null);
            //Assert.IsTrue(originalMessage.Equals(plaintext));
            CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(originalMessage), plaintext);
        }


        private void runInteraction(AxolotlStore aliceStore, AxolotlStore bobStore)
        {
            SessionCipher aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);
            SessionCipher bobSessionCipher = new SessionCipher(bobStore, ALICE_ADDRESS);

            String originalMessage = "smert ze smert";
            CiphertextMessage aliceMessage = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));

            Assert.IsTrue(aliceMessage.getType() == CiphertextMessage.WHISPER_TYPE);

            byte[] plaintext = bobSessionCipher.decrypt(new WhisperMessage(aliceMessage.serialize()));
            //Assert.IsTrue(plaintext.Equals(originalMessage));
            CollectionAssert.AreEqual(plaintext, Encoding.UTF8.GetBytes(originalMessage));

            CiphertextMessage bobMessage = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes(originalMessage));

            Assert.IsTrue(bobMessage.getType() == CiphertextMessage.WHISPER_TYPE);

            plaintext = aliceSessionCipher.decrypt(new WhisperMessage(bobMessage.serialize()));
            //Assert.IsTrue(plaintext.Equals(originalMessage));
            CollectionAssert.AreEqual(plaintext, Encoding.UTF8.GetBytes(originalMessage));


            for (int i = 0; i < 10; i++)
            {
                String loopingMessage = ("What do we mean by saying that existence precedes essence? " +
                                         "We mean that man first of all exists, encounters himself, " +
                                         "surges up in the world--and defines himself aftward. " + i);
                CiphertextMessage aliceLoopingMessage = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(loopingMessage));

                byte[] loopingPlaintext = bobSessionCipher.decrypt(new WhisperMessage(aliceLoopingMessage.serialize()));
                //Assert.IsTrue(loopingPlaintext.Equals(loopingMessage));
                CollectionAssert.AreEqual(loopingPlaintext, Encoding.UTF8.GetBytes(loopingMessage));

            }

            for (int i = 0; i < 10; i++)
            {
                String loopingMessage = ("What do we mean by saying that existence precedes essence? " +
                                         "We mean that man first of all exists, encounters himself, " +
                                         "surges up in the world--and defines himself aftward. " + i);
                CiphertextMessage bobLoopingMessage = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes(loopingMessage));

                byte[] loopingPlaintext = aliceSessionCipher.decrypt(new WhisperMessage(bobLoopingMessage.serialize()));
                //Assert.IsTrue(loopingPlaintext.Equals(loopingMessage));
                CollectionAssert.AreEqual(loopingPlaintext, Encoding.UTF8.GetBytes(loopingMessage));

            }

            ISet<Pair<String, CiphertextMessage>> aliceOutOfOrderMessages = new HashSet<Pair<String, CiphertextMessage>>();

            for (int i = 0; i < 10; i++)
            {
                String loopingMessage = ("What do we mean by saying that existence precedes essence? " +
                                         "We mean that man first of all exists, encounters himself, " +
                                         "surges up in the world--and defines himself aftward. " + i);
                CiphertextMessage aliceLoopingMessage = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(loopingMessage));

                aliceOutOfOrderMessages.Add(new Pair<String, CiphertextMessage>(loopingMessage, aliceLoopingMessage));
            }

            for (int i = 0; i < 10; i++)
            {
                String loopingMessage = ("What do we mean by saying that existence precedes essence? " +
                                         "We mean that man first of all exists, encounters himself, " +
                                         "surges up in the world--and defines himself aftward. " + i);
                CiphertextMessage aliceLoopingMessage = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes(loopingMessage));

                byte[] loopingPlaintext = bobSessionCipher.decrypt(new WhisperMessage(aliceLoopingMessage.serialize()));
                CollectionAssert.AreEqual(loopingPlaintext, Encoding.UTF8.GetBytes(loopingMessage));
            }

            for (int i = 0; i < 10; i++)
            {
                String loopingMessage = ("You can only desire based on what you know: " + i);
                CiphertextMessage bobLoopingMessage = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes(loopingMessage));

                byte[] loopingPlaintext = aliceSessionCipher.decrypt(new WhisperMessage(bobLoopingMessage.serialize()));
                CollectionAssert.AreEqual(loopingPlaintext, Encoding.UTF8.GetBytes(loopingMessage));
            }

            foreach (Pair<String, CiphertextMessage> aliceOutOfOrderMessage in aliceOutOfOrderMessages)
            {
                byte[] outOfOrderPlaintext = bobSessionCipher.decrypt(new WhisperMessage(aliceOutOfOrderMessage.second().serialize()));
                //Assert.IsTrue(outOfOrderPlaintext.Equals(aliceOutOfOrderMessage.first()));
                CollectionAssert.AreEqual(outOfOrderPlaintext, Encoding.UTF8.GetBytes(aliceOutOfOrderMessage.first()));

            }
        }
    }
}

