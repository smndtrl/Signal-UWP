using libaxolotl;
using libaxolotl.ecc;
using libaxolotl.protocol;
using libaxolotl.state;
using libaxolotl.util;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libaxolotl_test
{
    [TestClass]
    public class SimultaneousInitiateTests
    {
        private static readonly AxolotlAddress BOB_ADDRESS = new AxolotlAddress("+14151231234", 1);
        private static readonly AxolotlAddress ALICE_ADDRESS = new AxolotlAddress("+14159998888", 1);

        private static readonly ECKeyPair aliceSignedPreKey = Curve.generateKeyPair();
        private static readonly ECKeyPair bobSignedPreKey = Curve.generateKeyPair();

        private static readonly uint aliceSignedPreKeyId = KeyHelper.getRandomSequence(Medium.MAX_VALUE);
        private static readonly uint bobSignedPreKeyId = KeyHelper.getRandomSequence(Medium.MAX_VALUE);

        [TestMethod]
        public void testBasicSimultaneousInitiate()
        {
            AxolotlStore aliceStore = new TestInMemoryAxolotlStore();
            AxolotlStore bobStore = new TestInMemoryAxolotlStore();

            PreKeyBundle alicePreKeyBundle = createAlicePreKeyBundle(aliceStore);
            PreKeyBundle bobPreKeyBundle = createBobPreKeyBundle(bobStore);

            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);
            SessionBuilder bobSessionBuilder = new SessionBuilder(bobStore, ALICE_ADDRESS);

            SessionCipher aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);
            SessionCipher bobSessionCipher = new SessionCipher(bobStore, ALICE_ADDRESS);

            aliceSessionBuilder.process(bobPreKeyBundle);
            bobSessionBuilder.process(alicePreKeyBundle);

            CiphertextMessage messageForBob = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("hey there"));
            CiphertextMessage messageForAlice = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("sample message"));

            Assert.IsTrue(messageForBob.getType() == CiphertextMessage.PREKEY_TYPE);
            Assert.IsTrue(messageForAlice.getType() == CiphertextMessage.PREKEY_TYPE);

            Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));

            byte[] alicePlaintext = aliceSessionCipher.decrypt(new PreKeyWhisperMessage(messageForAlice.serialize()));
            byte[] bobPlaintext = bobSessionCipher.decrypt(new PreKeyWhisperMessage(messageForBob.serialize()));

            CollectionAssert.AreEqual(alicePlaintext, Encoding.UTF8.GetBytes("sample message"));
            CollectionAssert.AreEqual(bobPlaintext, Encoding.UTF8.GetBytes("hey there"));

            Assert.IsTrue(aliceStore.LoadSession(BOB_ADDRESS).getSessionState().getSessionVersion() == 3);
            Assert.IsTrue(bobStore.LoadSession(ALICE_ADDRESS).getSessionState().getSessionVersion() == 3);

            Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));

            CiphertextMessage aliceResponse = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("second message"));

            Assert.IsTrue(aliceResponse.getType() == CiphertextMessage.WHISPER_TYPE);

            byte[] responsePlaintext = bobSessionCipher.decrypt(new WhisperMessage(aliceResponse.serialize()));

            CollectionAssert.AreEqual(responsePlaintext, Encoding.UTF8.GetBytes("second message"));
            Assert.IsTrue(isSessionIdEqual(aliceStore, bobStore));

            CiphertextMessage finalMessage = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("third message"));

            Assert.IsTrue(finalMessage.getType() == CiphertextMessage.WHISPER_TYPE);

            byte[] finalPlaintext = aliceSessionCipher.decrypt(new WhisperMessage(finalMessage.serialize()));

            CollectionAssert.AreEqual(finalPlaintext, Encoding.UTF8.GetBytes("third message"));
            Assert.IsTrue(isSessionIdEqual(aliceStore, bobStore));
        }

        [TestMethod]
        public void testLostSimultaneousInitiate() //throws InvalidKeyException, UntrustedIdentityException, InvalidVersionException, InvalidMessageException, DuplicateMessageException, LegacyMessageException, InvalidKeyIdException, NoSessionException 
        {
            AxolotlStore aliceStore = new TestInMemoryAxolotlStore();
            AxolotlStore bobStore = new TestInMemoryAxolotlStore();

            PreKeyBundle alicePreKeyBundle = createAlicePreKeyBundle(aliceStore);
            PreKeyBundle bobPreKeyBundle = createBobPreKeyBundle(bobStore);

            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);
            SessionBuilder bobSessionBuilder = new SessionBuilder(bobStore, ALICE_ADDRESS);

            SessionCipher aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);
            SessionCipher bobSessionCipher = new SessionCipher(bobStore, ALICE_ADDRESS);

            aliceSessionBuilder.process(bobPreKeyBundle);
            bobSessionBuilder.process(alicePreKeyBundle);

            CiphertextMessage messageForBob = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("hey there"));
            CiphertextMessage messageForAlice = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("sample message"));

            Assert.IsTrue(messageForBob.getType() == CiphertextMessage.PREKEY_TYPE);
            Assert.IsTrue(messageForAlice.getType() == CiphertextMessage.PREKEY_TYPE);

            Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));

            byte[] bobPlaintext = bobSessionCipher.decrypt(new PreKeyWhisperMessage(messageForBob.serialize()));

            CollectionAssert.AreEqual(bobPlaintext, Encoding.UTF8.GetBytes("hey there"));
            Assert.IsTrue(bobStore.LoadSession(ALICE_ADDRESS).getSessionState().getSessionVersion() == 3);

            CiphertextMessage aliceResponse = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("second message"));

            Assert.IsTrue(aliceResponse.getType() == CiphertextMessage.PREKEY_TYPE);

            byte[] responsePlaintext = bobSessionCipher.decrypt(new PreKeyWhisperMessage(aliceResponse.serialize()));

            CollectionAssert.AreEqual(responsePlaintext, Encoding.UTF8.GetBytes("second message"));
            Assert.IsTrue(isSessionIdEqual(aliceStore, bobStore));

            CiphertextMessage finalMessage = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("third message"));

            Assert.IsTrue(finalMessage.getType() == CiphertextMessage.WHISPER_TYPE);

            byte[] finalPlaintext = aliceSessionCipher.decrypt(new WhisperMessage(finalMessage.serialize()));

            CollectionAssert.AreEqual(finalPlaintext, Encoding.UTF8.GetBytes("third message"));
            Assert.IsTrue(isSessionIdEqual(aliceStore, bobStore));
        }

        [TestMethod]
        public void testSimultaneousInitiateLostMessage() //throws InvalidKeyException, UntrustedIdentityException, InvalidVersionException,InvalidMessageException, DuplicateMessageException, LegacyMessageException,InvalidKeyIdException, NoSessionException
        {
            AxolotlStore aliceStore = new TestInMemoryAxolotlStore();
            AxolotlStore bobStore = new TestInMemoryAxolotlStore();

            PreKeyBundle alicePreKeyBundle = createAlicePreKeyBundle(aliceStore);
            PreKeyBundle bobPreKeyBundle = createBobPreKeyBundle(bobStore);

            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);
            SessionBuilder bobSessionBuilder = new SessionBuilder(bobStore, ALICE_ADDRESS);

            SessionCipher aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);
            SessionCipher bobSessionCipher = new SessionCipher(bobStore, ALICE_ADDRESS);

            aliceSessionBuilder.process(bobPreKeyBundle);
            bobSessionBuilder.process(alicePreKeyBundle);

            CiphertextMessage messageForBob = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("hey there"));
            CiphertextMessage messageForAlice = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("sample message"));

            Assert.IsTrue(messageForBob.getType() == CiphertextMessage.PREKEY_TYPE);
            Assert.IsTrue(messageForAlice.getType() == CiphertextMessage.PREKEY_TYPE);

            Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));

            byte[] alicePlaintext = aliceSessionCipher.decrypt(new PreKeyWhisperMessage(messageForAlice.serialize()));
            byte[] bobPlaintext = bobSessionCipher.decrypt(new PreKeyWhisperMessage(messageForBob.serialize()));

            CollectionAssert.AreEqual(alicePlaintext, Encoding.UTF8.GetBytes("sample message"));
            CollectionAssert.AreEqual(bobPlaintext, Encoding.UTF8.GetBytes("hey there"));

            Assert.IsTrue(aliceStore.LoadSession(BOB_ADDRESS).getSessionState().getSessionVersion() == 3);
            Assert.IsTrue(bobStore.LoadSession(ALICE_ADDRESS).getSessionState().getSessionVersion() == 3);

            Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));

            CiphertextMessage aliceResponse = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("second message"));

            Assert.IsTrue(aliceResponse.getType() == CiphertextMessage.WHISPER_TYPE);

            //    byte[] responsePlaintext = bobSessionCipher.decrypt(new WhisperMessage(aliceResponse.serialize()));
            //
            //    Assert.IsTrue(responsePlaintext), Encoding.UTF8.GetBytes("second message"));
            //    Assert.IsTrue(isSessionIdEqual(aliceStore, bobStore));
            Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));

            CiphertextMessage finalMessage = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("third message"));

            Assert.IsTrue(finalMessage.getType() == CiphertextMessage.WHISPER_TYPE);

            byte[] finalPlaintext = aliceSessionCipher.decrypt(new WhisperMessage(finalMessage.serialize()));

            CollectionAssert.AreEqual(finalPlaintext, Encoding.UTF8.GetBytes("third message"));
            Assert.IsTrue(isSessionIdEqual(aliceStore, bobStore));
        }

        [TestMethod]
        public void testSimultaneousInitiateRepeatedMessages()
        //throws InvalidKeyException, UntrustedIdentityException, InvalidVersionException,
        //InvalidMessageException, DuplicateMessageException, LegacyMessageException,
        //InvalidKeyIdException, NoSessionException
        {
            AxolotlStore aliceStore = new TestInMemoryAxolotlStore();
            AxolotlStore bobStore = new TestInMemoryAxolotlStore();

            PreKeyBundle alicePreKeyBundle = createAlicePreKeyBundle(aliceStore);
            PreKeyBundle bobPreKeyBundle = createBobPreKeyBundle(bobStore);

            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);
            SessionBuilder bobSessionBuilder = new SessionBuilder(bobStore, ALICE_ADDRESS);

            SessionCipher aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);
            SessionCipher bobSessionCipher = new SessionCipher(bobStore, ALICE_ADDRESS);

            aliceSessionBuilder.process(bobPreKeyBundle);
            bobSessionBuilder.process(alicePreKeyBundle);

            CiphertextMessage messageForBob = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("hey there"));
            CiphertextMessage messageForAlice = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("sample message"));

            Assert.IsTrue(messageForBob.getType() == CiphertextMessage.PREKEY_TYPE);
            Assert.IsTrue(messageForAlice.getType() == CiphertextMessage.PREKEY_TYPE);

            Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));

            byte[] alicePlaintext = aliceSessionCipher.decrypt(new PreKeyWhisperMessage(messageForAlice.serialize()));
            byte[] bobPlaintext = bobSessionCipher.decrypt(new PreKeyWhisperMessage(messageForBob.serialize()));

            CollectionAssert.AreEqual(alicePlaintext, Encoding.UTF8.GetBytes("sample message"));
            CollectionAssert.AreEqual(bobPlaintext, Encoding.UTF8.GetBytes("hey there"));

            Assert.IsTrue(aliceStore.LoadSession(BOB_ADDRESS).getSessionState().getSessionVersion() == 3);
            Assert.IsTrue(bobStore.LoadSession(ALICE_ADDRESS).getSessionState().getSessionVersion() == 3);

            Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));

            for (int i = 0; i < 50; i++)
            {
                CiphertextMessage messageForBobRepeat = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("hey there"));
                CiphertextMessage messageForAliceRepeat = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("sample message"));

                Assert.IsTrue(messageForBobRepeat.getType() == CiphertextMessage.WHISPER_TYPE);
                Assert.IsTrue(messageForAliceRepeat.getType() == CiphertextMessage.WHISPER_TYPE);

                Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));

                byte[] alicePlaintextRepeat = aliceSessionCipher.decrypt(new WhisperMessage(messageForAliceRepeat.serialize()));
                byte[] bobPlaintextRepeat = bobSessionCipher.decrypt(new WhisperMessage(messageForBobRepeat.serialize()));

                CollectionAssert.AreEqual(alicePlaintextRepeat, Encoding.UTF8.GetBytes("sample message"));
                CollectionAssert.AreEqual(bobPlaintextRepeat, Encoding.UTF8.GetBytes("hey there"));

                Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));
            }

            CiphertextMessage aliceResponse = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("second message"));

            Assert.IsTrue(aliceResponse.getType() == CiphertextMessage.WHISPER_TYPE);

            byte[] responsePlaintext = bobSessionCipher.decrypt(new WhisperMessage(aliceResponse.serialize()));

            CollectionAssert.AreEqual(responsePlaintext, Encoding.UTF8.GetBytes("second message"));
            Assert.IsTrue(isSessionIdEqual(aliceStore, bobStore));

            CiphertextMessage finalMessage = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("third message"));

            Assert.IsTrue(finalMessage.getType() == CiphertextMessage.WHISPER_TYPE);

            byte[] finalPlaintext = aliceSessionCipher.decrypt(new WhisperMessage(finalMessage.serialize()));

            CollectionAssert.AreEqual(finalPlaintext, Encoding.UTF8.GetBytes("third message"));
            Assert.IsTrue(isSessionIdEqual(aliceStore, bobStore));
        }

        [TestMethod]
        public void testRepeatedSimultaneousInitiateRepeatedMessages()
        //throws InvalidKeyException, UntrustedIdentityException, InvalidVersionException,
        //InvalidMessageException, DuplicateMessageException, LegacyMessageException,
        //InvalidKeyIdException, NoSessionException
        {
            AxolotlStore aliceStore = new TestInMemoryAxolotlStore();
            AxolotlStore bobStore = new TestInMemoryAxolotlStore();


            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);
            SessionBuilder bobSessionBuilder = new SessionBuilder(bobStore, ALICE_ADDRESS);

            SessionCipher aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);
            SessionCipher bobSessionCipher = new SessionCipher(bobStore, ALICE_ADDRESS);

            for (int i = 0; i < 15; i++)
            {
                PreKeyBundle alicePreKeyBundle = createAlicePreKeyBundle(aliceStore);
                PreKeyBundle bobPreKeyBundle = createBobPreKeyBundle(bobStore);

                aliceSessionBuilder.process(bobPreKeyBundle);
                bobSessionBuilder.process(alicePreKeyBundle);

                CiphertextMessage messageForBob = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("hey there"));
                CiphertextMessage messageForAlice = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("sample message"));

                Assert.IsTrue(messageForBob.getType() == CiphertextMessage.PREKEY_TYPE);
                Assert.IsTrue(messageForAlice.getType() == CiphertextMessage.PREKEY_TYPE);

                Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));

                byte[] alicePlaintext = aliceSessionCipher.decrypt(new PreKeyWhisperMessage(messageForAlice.serialize()));
                byte[] bobPlaintext = bobSessionCipher.decrypt(new PreKeyWhisperMessage(messageForBob.serialize()));

                CollectionAssert.AreEqual(alicePlaintext, Encoding.UTF8.GetBytes("sample message"));
                CollectionAssert.AreEqual(bobPlaintext, Encoding.UTF8.GetBytes("hey there"));

                Assert.IsTrue(aliceStore.LoadSession(BOB_ADDRESS).getSessionState().getSessionVersion() == 3);
                Assert.IsTrue(bobStore.LoadSession(ALICE_ADDRESS).getSessionState().getSessionVersion() == 3);

                Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));
            }

            for (int i = 0; i < 50; i++)
            {
                CiphertextMessage messageForBobRepeat = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("hey there"));
                CiphertextMessage messageForAliceRepeat = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("sample message"));

                Assert.IsTrue(messageForBobRepeat.getType() == CiphertextMessage.WHISPER_TYPE);
                Assert.IsTrue(messageForAliceRepeat.getType() == CiphertextMessage.WHISPER_TYPE);

                Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));

                byte[] alicePlaintextRepeat = aliceSessionCipher.decrypt(new WhisperMessage(messageForAliceRepeat.serialize()));
                byte[] bobPlaintextRepeat = bobSessionCipher.decrypt(new WhisperMessage(messageForBobRepeat.serialize()));

                CollectionAssert.AreEqual(alicePlaintextRepeat, Encoding.UTF8.GetBytes("sample message"));
                CollectionAssert.AreEqual(bobPlaintextRepeat, Encoding.UTF8.GetBytes("hey there"));

                Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));
            }

            CiphertextMessage aliceResponse = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("second message"));

            Assert.IsTrue(aliceResponse.getType() == CiphertextMessage.WHISPER_TYPE);

            byte[] responsePlaintext = bobSessionCipher.decrypt(new WhisperMessage(aliceResponse.serialize()));

            CollectionAssert.AreEqual(responsePlaintext, Encoding.UTF8.GetBytes("second message"));
            Assert.IsTrue(isSessionIdEqual(aliceStore, bobStore));

            CiphertextMessage finalMessage = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("third message"));

            Assert.IsTrue(finalMessage.getType() == CiphertextMessage.WHISPER_TYPE);

            byte[] finalPlaintext = aliceSessionCipher.decrypt(new WhisperMessage(finalMessage.serialize()));

            CollectionAssert.AreEqual(finalPlaintext, Encoding.UTF8.GetBytes("third message"));
            Assert.IsTrue(isSessionIdEqual(aliceStore, bobStore));
        }

        [TestMethod]
        public void testRepeatedSimultaneousInitiateLostMessageRepeatedMessages()
        //throws InvalidKeyException, UntrustedIdentityException, InvalidVersionException,
        //InvalidMessageException, DuplicateMessageException, LegacyMessageException,
        //InvalidKeyIdException, NoSessionException
        {
            AxolotlStore aliceStore = new TestInMemoryAxolotlStore();
            AxolotlStore bobStore = new TestInMemoryAxolotlStore();


            SessionBuilder aliceSessionBuilder = new SessionBuilder(aliceStore, BOB_ADDRESS);
            SessionBuilder bobSessionBuilder = new SessionBuilder(bobStore, ALICE_ADDRESS);

            SessionCipher aliceSessionCipher = new SessionCipher(aliceStore, BOB_ADDRESS);
            SessionCipher bobSessionCipher = new SessionCipher(bobStore, ALICE_ADDRESS);

            //    PreKeyBundle aliceLostPreKeyBundle = createAlicePreKeyBundle(aliceStore);
            PreKeyBundle bobLostPreKeyBundle = createBobPreKeyBundle(bobStore);

            aliceSessionBuilder.process(bobLostPreKeyBundle);
            //    bobSessionBuilder.process(aliceLostPreKeyBundle);

            CiphertextMessage lostMessageForBob = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("hey there"));
            //    CiphertextMessage lostMessageForAlice = bobSessionCipher.encrypt("sample message"));

            for (int i = 0; i < 15; i++)
            {
                PreKeyBundle alicePreKeyBundle = createAlicePreKeyBundle(aliceStore);
                PreKeyBundle bobPreKeyBundle = createBobPreKeyBundle(bobStore);

                aliceSessionBuilder.process(bobPreKeyBundle);
                bobSessionBuilder.process(alicePreKeyBundle);

                CiphertextMessage messageForBob = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("hey there"));
                CiphertextMessage messageForAlice = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("sample message"));

                Assert.IsTrue(messageForBob.getType() == CiphertextMessage.PREKEY_TYPE);
                Assert.IsTrue(messageForAlice.getType() == CiphertextMessage.PREKEY_TYPE);

                Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));

                byte[] alicePlaintext = aliceSessionCipher.decrypt(new PreKeyWhisperMessage(messageForAlice.serialize()));
                byte[] bobPlaintext = bobSessionCipher.decrypt(new PreKeyWhisperMessage(messageForBob.serialize()));

                CollectionAssert.AreEqual(alicePlaintext, Encoding.UTF8.GetBytes("sample message"));
                CollectionAssert.AreEqual(bobPlaintext, Encoding.UTF8.GetBytes("hey there"));

                Assert.IsTrue(aliceStore.LoadSession(BOB_ADDRESS).getSessionState().getSessionVersion() == 3);
                Assert.IsTrue(bobStore.LoadSession(ALICE_ADDRESS).getSessionState().getSessionVersion() == 3);

                Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));
            }

            for (int i = 0; i < 50; i++)
            {
                CiphertextMessage messageForBobRepeat = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("hey there"));
                CiphertextMessage messageForAliceRepeat = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("sample message"));

                Assert.IsTrue(messageForBobRepeat.getType() == CiphertextMessage.WHISPER_TYPE);
                Assert.IsTrue(messageForAliceRepeat.getType() == CiphertextMessage.WHISPER_TYPE);

                Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));

                byte[] alicePlaintextRepeat = aliceSessionCipher.decrypt(new WhisperMessage(messageForAliceRepeat.serialize()));
                byte[] bobPlaintextRepeat = bobSessionCipher.decrypt(new WhisperMessage(messageForBobRepeat.serialize()));

                CollectionAssert.AreEqual(alicePlaintextRepeat, Encoding.UTF8.GetBytes("sample message"));
                CollectionAssert.AreEqual(bobPlaintextRepeat, Encoding.UTF8.GetBytes("hey there"));

                Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));
            }

            CiphertextMessage aliceResponse = aliceSessionCipher.encrypt(Encoding.UTF8.GetBytes("second message"));

            Assert.IsTrue(aliceResponse.getType() == CiphertextMessage.WHISPER_TYPE);

            byte[] responsePlaintext = bobSessionCipher.decrypt(new WhisperMessage(aliceResponse.serialize()));

            CollectionAssert.AreEqual(responsePlaintext, Encoding.UTF8.GetBytes("second message"));
            Assert.IsTrue(isSessionIdEqual(aliceStore, bobStore));

            CiphertextMessage finalMessage = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("third message"));

            Assert.IsTrue(finalMessage.getType() == CiphertextMessage.WHISPER_TYPE);

            byte[] finalPlaintext = aliceSessionCipher.decrypt(new WhisperMessage(finalMessage.serialize()));

            CollectionAssert.AreEqual(finalPlaintext, Encoding.UTF8.GetBytes("third message"));
            Assert.IsTrue(isSessionIdEqual(aliceStore, bobStore));

            byte[] lostMessagePlaintext = bobSessionCipher.decrypt(new PreKeyWhisperMessage(lostMessageForBob.serialize()));
            CollectionAssert.AreEqual(lostMessagePlaintext, Encoding.UTF8.GetBytes("hey there"));

            Assert.IsFalse(isSessionIdEqual(aliceStore, bobStore));

            CiphertextMessage blastFromThePast = bobSessionCipher.encrypt(Encoding.UTF8.GetBytes("unexpected!"));
            byte[] blastFromThePastPlaintext = aliceSessionCipher.decrypt(new WhisperMessage(blastFromThePast.serialize()));

            CollectionAssert.AreEqual(blastFromThePastPlaintext, Encoding.UTF8.GetBytes("unexpected!"));
            Assert.IsTrue(isSessionIdEqual(aliceStore, bobStore));
        }

        private bool isSessionIdEqual(AxolotlStore aliceStore, AxolotlStore bobStore)
        {
            return Enumerable.SequenceEqual(aliceStore.LoadSession(BOB_ADDRESS).getSessionState().getAliceBaseKey(),
                                 bobStore.LoadSession(ALICE_ADDRESS).getSessionState().getAliceBaseKey());
        }

        private PreKeyBundle createAlicePreKeyBundle(AxolotlStore aliceStore)
        {
            ECKeyPair aliceUnsignedPreKey = Curve.generateKeyPair();
            uint aliceUnsignedPreKeyId = KeyHelper.getRandomSequence(Medium.MAX_VALUE);
            byte[] aliceSignature = Curve.calculateSignature(aliceStore.getIdentityKeyPair().getPrivateKey(),
                                                                       aliceSignedPreKey.getPublicKey().serialize());

            PreKeyBundle alicePreKeyBundle = new PreKeyBundle(1, 1,
                                                              aliceUnsignedPreKeyId, aliceUnsignedPreKey.getPublicKey(),
                                                              aliceSignedPreKeyId, aliceSignedPreKey.getPublicKey(),
                                                              aliceSignature, aliceStore.getIdentityKeyPair().getPublicKey());

            aliceStore.storeSignedPreKey(aliceSignedPreKeyId, new SignedPreKeyRecord(aliceSignedPreKeyId, KeyHelper.getTime(), aliceSignedPreKey, aliceSignature));
            aliceStore.storePreKey(aliceUnsignedPreKeyId, new PreKeyRecord(aliceUnsignedPreKeyId, aliceUnsignedPreKey));

            return alicePreKeyBundle;
        }

        private PreKeyBundle createBobPreKeyBundle(AxolotlStore bobStore)
        {
            ECKeyPair bobUnsignedPreKey = Curve.generateKeyPair();
            uint bobUnsignedPreKeyId = KeyHelper.getRandomSequence(Medium.MAX_VALUE);
            byte[] bobSignature = Curve.calculateSignature(bobStore.getIdentityKeyPair().getPrivateKey(),
                                                                     bobSignedPreKey.getPublicKey().serialize());

            PreKeyBundle bobPreKeyBundle = new PreKeyBundle(1, 1,
                                                        bobUnsignedPreKeyId, bobUnsignedPreKey.getPublicKey(),
                                                        bobSignedPreKeyId, bobSignedPreKey.getPublicKey(),
                                                        bobSignature, bobStore.getIdentityKeyPair().getPublicKey());

            bobStore.storeSignedPreKey(bobSignedPreKeyId, new SignedPreKeyRecord(bobSignedPreKeyId, KeyHelper.getTime(), bobSignedPreKey, bobSignature));
            bobStore.storePreKey(bobUnsignedPreKeyId, new PreKeyRecord(bobUnsignedPreKeyId, bobUnsignedPreKey));

            return bobPreKeyBundle;
        }
    }
}
