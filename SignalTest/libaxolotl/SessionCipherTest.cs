using libaxolotl;
using libaxolotl.ecc;
using libaxolotl.protocol;
using libaxolotl.ratchet;
using libaxolotl.state;
using libaxolotl.util;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Strilanc.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libaxolotl_test
{
    [TestClass]
    public class SessionCipherTest
    {
        
        [TestMethod]
        public void testBasicSessionV2() //throws InvalidKeyException, DuplicateMessageException, LegacyMessageException, InvalidMessageException, NoSuchAlgorithmException, NoSessionException
        {
            SessionRecord aliceSessionRecord = new SessionRecord();
            SessionRecord bobSessionRecord = new SessionRecord();

            initializeSessionsV2(aliceSessionRecord.getSessionState(), bobSessionRecord.getSessionState());
            runInteraction(aliceSessionRecord, bobSessionRecord);
        }

        [TestMethod]
        public void testBasicSessionV3() //throws InvalidKeyException, DuplicateMessageException, LegacyMessageException, InvalidMessageException, NoSuchAlgorithmException, NoSessionException
        {
            SessionRecord aliceSessionRecord = new SessionRecord();
            SessionRecord bobSessionRecord = new SessionRecord();

            initializeSessionsV3(aliceSessionRecord.getSessionState(), bobSessionRecord.getSessionState());
            runInteraction(aliceSessionRecord, bobSessionRecord);
        }

        private void runInteraction(SessionRecord aliceSessionRecord, SessionRecord bobSessionRecord) //throws DuplicateMessageException, LegacyMessageException, InvalidMessageException, NoSuchAlgorithmException, NoSessionException 
        {
            AxolotlStore aliceStore = new TestInMemoryAxolotlStore();
            AxolotlStore bobStore = new TestInMemoryAxolotlStore();

            aliceStore.storeSession(new AxolotlAddress("+14159999999", 1), aliceSessionRecord);
            bobStore.storeSession(new AxolotlAddress("+14158888888", 1), bobSessionRecord);

            SessionCipher aliceCipher = new SessionCipher(aliceStore, new AxolotlAddress("+14159999999", 1));
            SessionCipher bobCipher = new SessionCipher(bobStore, new AxolotlAddress("+14158888888", 1));

            byte[] alicePlaintext = Encoding.UTF8.GetBytes("This is a plaintext message.");
            CiphertextMessage message = aliceCipher.encrypt(alicePlaintext);
            byte[] bobPlaintext = bobCipher.decrypt(new WhisperMessage(message.serialize()));

            CollectionAssert.AreEqual(alicePlaintext, bobPlaintext);

            byte[] bobReply = Encoding.UTF8.GetBytes("This is a message from Bob.");
            CiphertextMessage reply = bobCipher.encrypt(bobReply);
            byte[] receivedReply = aliceCipher.decrypt(new WhisperMessage(reply.serialize()));

            CollectionAssert.AreEqual(bobReply, receivedReply);

            List<CiphertextMessage> aliceCiphertextMessages = new List<CiphertextMessage>();
            List<byte[]> alicePlaintextMessages = new List<byte[]>();

            for (int i = 0; i < 50; i++)
            {
                alicePlaintextMessages.Add(Encoding.UTF8.GetBytes(("смерть за смерть " + i)));
                aliceCiphertextMessages.Add(aliceCipher.encrypt(Encoding.UTF8.GetBytes("смерть за смерть " + i)));
            }

            long seed = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            CryptoHelper.Shuffle(aliceCiphertextMessages);
            CryptoHelper.Shuffle(alicePlaintextMessages);

            for (int i = 0; i < aliceCiphertextMessages.Count / 2; i++)
            {
                byte[] receivedPlaintext = bobCipher.decrypt(new WhisperMessage(aliceCiphertextMessages[i].serialize()));
                CollectionAssert.AreEqual(receivedPlaintext, alicePlaintextMessages[i]);
            }

            List<CiphertextMessage> bobCiphertextMessages = new List<CiphertextMessage>();
            List<byte[]> bobPlaintextMessages = new List<byte[]>();

            for (int i = 0; i < 20; i++)
            {
                bobPlaintextMessages.Add(Encoding.UTF8.GetBytes(("смерть за смерть " + i)));
                bobCiphertextMessages.Add(bobCipher.encrypt(Encoding.UTF8.GetBytes("смерть за смерть " + i)));
            }

            seed = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            CryptoHelper.Shuffle(bobCiphertextMessages);
            CryptoHelper.Shuffle(bobPlaintextMessages);

            for (int i = 0; i < bobCiphertextMessages.Count() / 2; i++)
            {
                byte[] receivedPlaintext = aliceCipher.decrypt(new WhisperMessage(bobCiphertextMessages[i].serialize()));
                CollectionAssert.AreEqual(receivedPlaintext, bobPlaintextMessages[i]);
            }

            for (int i = aliceCiphertextMessages.Count / 2; i < aliceCiphertextMessages.Count(); i++)
            {
                byte[] receivedPlaintext = bobCipher.decrypt(new WhisperMessage(aliceCiphertextMessages[i].serialize()));
                CollectionAssert.AreEqual(receivedPlaintext, alicePlaintextMessages[i]);
            }

            for (int i = bobCiphertextMessages.Count() / 2; i < bobCiphertextMessages.Count(); i++)
            {
                byte[] receivedPlaintext = aliceCipher.decrypt(new WhisperMessage(bobCiphertextMessages[i].serialize()));
                CollectionAssert.AreEqual(receivedPlaintext, bobPlaintextMessages[i]);
            }
        }

        private void initializeSessionsV2(SessionState aliceSessionState, SessionState bobSessionState) //throws InvalidKeyException
        {
            ECKeyPair aliceIdentityKeyPair = Curve.generateKeyPair();
            IdentityKeyPair aliceIdentityKey = new IdentityKeyPair(new IdentityKey(aliceIdentityKeyPair.getPublicKey()),
                                                               aliceIdentityKeyPair.getPrivateKey());
            ECKeyPair aliceBaseKey = Curve.generateKeyPair();
            ECKeyPair aliceEphemeralKey = Curve.generateKeyPair();

            ECKeyPair bobIdentityKeyPair = Curve.generateKeyPair();
            IdentityKeyPair bobIdentityKey = new IdentityKeyPair(new IdentityKey(bobIdentityKeyPair.getPublicKey()),
                                                               bobIdentityKeyPair.getPrivateKey());
            ECKeyPair bobBaseKey = Curve.generateKeyPair();
            ECKeyPair bobEphemeralKey = bobBaseKey;

            AliceAxolotlParameters aliceParameters = AliceAxolotlParameters.newBuilder()
        .setOurIdentityKey(aliceIdentityKey)
        .setOurBaseKey(aliceBaseKey)
        .setTheirIdentityKey(bobIdentityKey.getPublicKey())
        .setTheirSignedPreKey(bobEphemeralKey.getPublicKey())
        .setTheirRatchetKey(bobEphemeralKey.getPublicKey())
        .setTheirOneTimePreKey(May<ECPublicKey>.NoValue)
        .create();

            BobAxolotlParameters bobParameters = BobAxolotlParameters.newBuilder()
        .setOurIdentityKey(bobIdentityKey)
        .setOurOneTimePreKey(May<ECKeyPair>.NoValue)
        .setOurRatchetKey(bobEphemeralKey)
        .setOurSignedPreKey(bobBaseKey)
        .setTheirBaseKey(aliceBaseKey.getPublicKey())
        .setTheirIdentityKey(aliceIdentityKey.getPublicKey())
        .create();

            RatchetingSession.initializeSession(aliceSessionState, 2, aliceParameters);
            RatchetingSession.initializeSession(bobSessionState, 2, bobParameters);
        }

        private void initializeSessionsV3(SessionState aliceSessionState, SessionState bobSessionState) //throws InvalidKeyException
        {
            ECKeyPair aliceIdentityKeyPair = Curve.generateKeyPair();
            IdentityKeyPair aliceIdentityKey = new IdentityKeyPair(new IdentityKey(aliceIdentityKeyPair.getPublicKey()),
                                                               aliceIdentityKeyPair.getPrivateKey());
            ECKeyPair aliceBaseKey = Curve.generateKeyPair();
            ECKeyPair aliceEphemeralKey = Curve.generateKeyPair();

            ECKeyPair alicePreKey = aliceBaseKey;

            ECKeyPair bobIdentityKeyPair = Curve.generateKeyPair();
            IdentityKeyPair bobIdentityKey = new IdentityKeyPair(new IdentityKey(bobIdentityKeyPair.getPublicKey()),
                                                               bobIdentityKeyPair.getPrivateKey());
            ECKeyPair bobBaseKey = Curve.generateKeyPair();
            ECKeyPair bobEphemeralKey = bobBaseKey;

            ECKeyPair bobPreKey = Curve.generateKeyPair();

            AliceAxolotlParameters aliceParameters = AliceAxolotlParameters.newBuilder()
        .setOurBaseKey(aliceBaseKey)
        .setOurIdentityKey(aliceIdentityKey)
        .setTheirOneTimePreKey(May<ECPublicKey>.NoValue)
        .setTheirRatchetKey(bobEphemeralKey.getPublicKey())
        .setTheirSignedPreKey(bobBaseKey.getPublicKey())
        .setTheirIdentityKey(bobIdentityKey.getPublicKey())
        .create();

            BobAxolotlParameters bobParameters = BobAxolotlParameters.newBuilder()
        .setOurRatchetKey(bobEphemeralKey)
        .setOurSignedPreKey(bobBaseKey)
        .setOurOneTimePreKey(May<ECKeyPair>.NoValue)
        .setOurIdentityKey(bobIdentityKey)
        .setTheirIdentityKey(aliceIdentityKey.getPublicKey())
        .setTheirBaseKey(aliceBaseKey.getPublicKey())
        .create();

            RatchetingSession.initializeSession(aliceSessionState, 3, aliceParameters);
            RatchetingSession.initializeSession(bobSessionState, 3, bobParameters);
        }
    }
}
