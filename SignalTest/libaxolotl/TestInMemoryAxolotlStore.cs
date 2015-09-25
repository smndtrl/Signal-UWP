using libaxolotl;
using libaxolotl.ecc;
using libaxolotl.util;
using libaxolotl.state.impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace libaxolotl_test
{
    [TestClass]
    public class TestInMemoryAxolotlStore : InMemoryAxolotlStore
    {
        public TestInMemoryAxolotlStore()
            : base(generateIdentityKeyPair(), generateRegistrationId())
        {
        }

        private static IdentityKeyPair generateIdentityKeyPair()
        {
            ECKeyPair identityKeyPairKeys = Curve.generateKeyPair();

            return new IdentityKeyPair(new IdentityKey(identityKeyPairKeys.getPublicKey()),
                                                       identityKeyPairKeys.getPrivateKey());
        }

        private static uint generateRegistrationId()
        {
            return KeyHelper.generateRegistrationId(false);
        }
    }
}
