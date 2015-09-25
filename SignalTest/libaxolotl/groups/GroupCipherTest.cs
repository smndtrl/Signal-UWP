using libaxolotl.protocol;
using libaxolotl;
using libaxolotl.groups;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace libaxolotl_test.groups
{
    [TestClass]
    public class GroupCipherTest
    {
        private static readonly AxolotlAddress SENDER_ADDRESS = new AxolotlAddress("+14150001111", 1);
        private static readonly SenderKeyName GROUP_SENDER = new SenderKeyName("nihilist history reading group", SENDER_ADDRESS);

        [TestMethod]
        public void testNoSession()// throws InvalidMessageException, LegacyMessageException, NoSessionException, DuplicateMessageException
        {
            InMemorySenderKeyStore aliceStore = new InMemorySenderKeyStore();
            InMemorySenderKeyStore bobStore = new InMemorySenderKeyStore();

            GroupSessionBuilder aliceSessionBuilder = new GroupSessionBuilder(aliceStore);
            GroupSessionBuilder bobSessionBuilder = new GroupSessionBuilder(bobStore);

            GroupCipher aliceGroupCipher = new GroupCipher(aliceStore, GROUP_SENDER);
            GroupCipher bobGroupCipher = new GroupCipher(bobStore, GROUP_SENDER);

            SenderKeyDistributionMessage sentAliceDistributionMessage = aliceSessionBuilder.create(GROUP_SENDER);
            SenderKeyDistributionMessage receivedAliceDistributionMessage = new SenderKeyDistributionMessage(sentAliceDistributionMessage.serialize());

            bobSessionBuilder.process(GROUP_SENDER, receivedAliceDistributionMessage);

            byte[] ciphertextFromAlice = aliceGroupCipher.encrypt(Encoding.UTF8.GetBytes("smert ze smert"));
            try
            {
                byte[] plaintextFromAlice = bobGroupCipher.decrypt(ciphertextFromAlice);
                throw new NoSessionException("Should be no session!");
            }
            catch (NoSessionException e)
            {
                // good
            }
        }


        [TestMethod]
        public void testBasicEncryptDecrypt()
        {
            InMemorySenderKeyStore aliceStore = new InMemorySenderKeyStore();
            InMemorySenderKeyStore bobStore = new InMemorySenderKeyStore();

            GroupSessionBuilder aliceSessionBuilder = new GroupSessionBuilder(aliceStore);
            GroupSessionBuilder bobSessionBuilder = new GroupSessionBuilder(bobStore);

            GroupCipher aliceGroupCipher = new GroupCipher(aliceStore, GROUP_SENDER);
            GroupCipher bobGroupCipher = new GroupCipher(bobStore, GROUP_SENDER);

            SenderKeyDistributionMessage sentAliceDistributionMessage = aliceSessionBuilder.create(GROUP_SENDER);
            SenderKeyDistributionMessage receivedAliceDistributionMessage = new SenderKeyDistributionMessage(sentAliceDistributionMessage.serialize());
            bobSessionBuilder.process(GROUP_SENDER, receivedAliceDistributionMessage);

            byte[] ciphertextFromAlice = aliceGroupCipher.encrypt(Encoding.UTF8.GetBytes("smert ze smert"));
            byte[] plaintextFromAlice = bobGroupCipher.decrypt(ciphertextFromAlice);

            Assert.IsTrue(Encoding.UTF8.GetString(plaintextFromAlice, 0, plaintextFromAlice.Length).Equals("smert ze smert"));
            // CollectionAssert.AreEqual(plaintextFromAlice, plaintextBytes);
        }

        [TestMethod]
        public void testLargeMessages()
        {
            InMemorySenderKeyStore aliceStore = new InMemorySenderKeyStore();
            InMemorySenderKeyStore bobStore = new InMemorySenderKeyStore();

            GroupSessionBuilder aliceSessionBuilder = new GroupSessionBuilder(aliceStore);
            GroupSessionBuilder bobSessionBuilder = new GroupSessionBuilder(bobStore);

            GroupCipher aliceGroupCipher = new GroupCipher(aliceStore, GROUP_SENDER);
            GroupCipher bobGroupCipher = new GroupCipher(bobStore, GROUP_SENDER);

            SenderKeyDistributionMessage sentAliceDistributionMessage = aliceSessionBuilder.create(GROUP_SENDER);
            SenderKeyDistributionMessage receivedAliceDistributionMessage = new SenderKeyDistributionMessage(sentAliceDistributionMessage.serialize());
            bobSessionBuilder.process(GROUP_SENDER, receivedAliceDistributionMessage);

            byte[] plaintext = new byte[1024 * 0124];// 1024*1024
            new Random().NextBytes(plaintext);

            byte[] ciphertextFromAlice = aliceGroupCipher.encrypt(plaintext);
            byte[] plaintextFromAlice = bobGroupCipher.decrypt(ciphertextFromAlice);

            CollectionAssert.AreEqual(plaintext, plaintextFromAlice);
        }
        [TestMethod]
        public void testBasicRatchet()
        {
            InMemorySenderKeyStore aliceStore = new InMemorySenderKeyStore();
            InMemorySenderKeyStore bobStore = new InMemorySenderKeyStore();

            GroupSessionBuilder aliceSessionBuilder = new GroupSessionBuilder(aliceStore);
            GroupSessionBuilder bobSessionBuilder = new GroupSessionBuilder(bobStore);

            SenderKeyName aliceName = GROUP_SENDER;

            GroupCipher aliceGroupCipher = new GroupCipher(aliceStore, aliceName);
            GroupCipher bobGroupCipher = new GroupCipher(bobStore, aliceName);

            SenderKeyDistributionMessage sentAliceDistributionMessage =
                aliceSessionBuilder.create(aliceName);
            SenderKeyDistributionMessage receivedAliceDistributionMessage =
                new SenderKeyDistributionMessage(sentAliceDistributionMessage.serialize());

            bobSessionBuilder.process(aliceName, receivedAliceDistributionMessage);

            byte[] ciphertextFromAlice = aliceGroupCipher.encrypt(Encoding.UTF8.GetBytes("smert ze smert"));
            byte[] ciphertextFromAlice2 = aliceGroupCipher.encrypt(Encoding.UTF8.GetBytes("smert ze smert2"));
            byte[] ciphertextFromAlice3 = aliceGroupCipher.encrypt(Encoding.UTF8.GetBytes("smert ze smert3"));

            byte[] plaintextFromAlice = bobGroupCipher.decrypt(ciphertextFromAlice);

            try
            {
                bobGroupCipher.decrypt(ciphertextFromAlice);
                throw new DuplicateMessageException("Should have ratcheted forward!");
            }
            catch (DuplicateMessageException dme)
            {
                // good
            }

            byte[] plaintextFromAlice2 = bobGroupCipher.decrypt(ciphertextFromAlice2);
            byte[] plaintextFromAlice3 = bobGroupCipher.decrypt(ciphertextFromAlice3);

            Assert.IsTrue(Encoding.UTF8.GetString(plaintextFromAlice, 0, plaintextFromAlice.Length).Equals("smert ze smert"));
            Assert.IsTrue(Encoding.UTF8.GetString(plaintextFromAlice2, 0, plaintextFromAlice2.Length).Equals("smert ze smert2"));
            Assert.IsTrue(Encoding.UTF8.GetString(plaintextFromAlice3, 0, plaintextFromAlice3.Length).Equals("smert ze smert3"));
        }

        [TestMethod]
        public void testLateJoin()
        {
            InMemorySenderKeyStore aliceStore = new InMemorySenderKeyStore();
            InMemorySenderKeyStore bobStore = new InMemorySenderKeyStore();

            GroupSessionBuilder aliceSessionBuilder = new GroupSessionBuilder(aliceStore);


            SenderKeyName aliceName = GROUP_SENDER;

            GroupCipher aliceGroupCipher = new GroupCipher(aliceStore, aliceName);


            SenderKeyDistributionMessage aliceDistributionMessage = aliceSessionBuilder.create(aliceName);
            // Send off to some people.

            for (int i = 0; i < 100; i++)
            {
                aliceGroupCipher.encrypt(Encoding.UTF8.GetBytes("up the punks up the punks up the punks"));
            }

            // Now Bob Joins.
            GroupSessionBuilder bobSessionBuilder = new GroupSessionBuilder(bobStore);
            GroupCipher bobGroupCipher = new GroupCipher(bobStore, aliceName);


            SenderKeyDistributionMessage distributionMessageToBob = aliceSessionBuilder.create(aliceName);
            bobSessionBuilder.process(aliceName, new SenderKeyDistributionMessage(distributionMessageToBob.serialize()));

            byte[] ciphertext = aliceGroupCipher.encrypt(Encoding.UTF8.GetBytes("welcome to the group"));
            byte[] plaintext = bobGroupCipher.decrypt(ciphertext);

            Assert.IsTrue(Encoding.UTF8.GetString(plaintext, 0, plaintext.Length).Equals("welcome to the group"));
        }

        [TestMethod]
        public void testOutOfOrder()
        {
            InMemorySenderKeyStore aliceStore = new InMemorySenderKeyStore();
            InMemorySenderKeyStore bobStore = new InMemorySenderKeyStore();

            GroupSessionBuilder aliceSessionBuilder = new GroupSessionBuilder(aliceStore);
            GroupSessionBuilder bobSessionBuilder = new GroupSessionBuilder(bobStore);

            SenderKeyName aliceName = GROUP_SENDER;

            GroupCipher aliceGroupCipher = new GroupCipher(aliceStore, aliceName);
            GroupCipher bobGroupCipher = new GroupCipher(bobStore, aliceName);

            SenderKeyDistributionMessage aliceDistributionMessage =
                aliceSessionBuilder.create(aliceName);

            bobSessionBuilder.process(aliceName, aliceDistributionMessage);

            List<byte[]> ciphertexts = new List<byte[]>(100);

            for (int i = 0; i < 100; i++)
            {
                ciphertexts.Add(aliceGroupCipher.encrypt(Encoding.UTF8.GetBytes("up the punks")));
            }

            while (ciphertexts.Count > 0)
            {
                int index = (int)(randomInt() % (uint)ciphertexts.Count);
                byte[] ciphertext = ciphertexts[index];
                ciphertexts.RemoveAt(index);
                byte[] plaintext = bobGroupCipher.decrypt(ciphertext);

                Assert.IsTrue(Encoding.UTF8.GetString(plaintext, 0, plaintext.Length).Equals("up the punks"));
            }
        }

        [TestMethod]
        public void testEncryptNoSession()
        {
            InMemorySenderKeyStore aliceStore = new InMemorySenderKeyStore();
            GroupCipher aliceGroupCipher = new GroupCipher(aliceStore, new SenderKeyName("coolio groupio", new AxolotlAddress("+10002223333", 1)));
            try
            {
                aliceGroupCipher.encrypt(Encoding.UTF8.GetBytes("up the punks"));
                throw new NoSessionException("Should have failed!");
            }
            catch (NoSessionException nse)
            {
                // good
            }
        }
        [TestMethod]
        public void testTooFarInFuture()// throws DuplicateMessageException, InvalidMessageException, LegacyMessageException, NoSessionException
        {
            InMemorySenderKeyStore aliceStore = new InMemorySenderKeyStore();
            InMemorySenderKeyStore bobStore = new InMemorySenderKeyStore();

            GroupSessionBuilder aliceSessionBuilder = new GroupSessionBuilder(aliceStore);
            GroupSessionBuilder bobSessionBuilder = new GroupSessionBuilder(bobStore);

            SenderKeyName aliceName = GROUP_SENDER;

            GroupCipher aliceGroupCipher = new GroupCipher(aliceStore, aliceName);
            GroupCipher bobGroupCipher = new GroupCipher(bobStore, aliceName);

            SenderKeyDistributionMessage aliceDistributionMessage = aliceSessionBuilder.create(aliceName);

            bobSessionBuilder.process(aliceName, aliceDistributionMessage);

            for (int i = 0; i < 2001; i++)
            {
                aliceGroupCipher.encrypt(Encoding.UTF8.GetBytes("up the punks"));
            }

            byte[] tooFarCiphertext = aliceGroupCipher.encrypt(Encoding.UTF8.GetBytes("notta gonna worka"));
            try
            {
                bobGroupCipher.decrypt(tooFarCiphertext);
                throw new InvalidMessageException("Should have failed!");
            }
            catch (InvalidMessageException e)
            {
                // good
            }
        }

        private uint randomInt()
        {
            try
            {
                return CryptographicBuffer.GenerateRandomNumber();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
