using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libaxolotl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Signal.Util
{
    class JsonSerializers
    {
    }

    class IdentityKeySerializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {

                var token = JToken.Load(reader); // skip devices token

                var str = token.Value<string>();
                byte[] test = libtextsecure.util.Base64.decodeWithoutPadding(str);
                return new IdentityKey(test, 0);

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IdentityKey pubKey = (IdentityKey)value;
            writer.WriteValue(libtextsecure.util.Base64.encodeBytesWithoutPadding(pubKey.serialize()));
        }
    }
}
