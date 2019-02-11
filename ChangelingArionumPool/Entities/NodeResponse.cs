using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChangelingArionumPool.Entities
{
    public class NodeResponse
    {
        public string status { get; set; }
        public string coin { get; set; }
    }

    public class NodeErrorResponse : NodeResponse
    {
        public string data { get; set; }
    }

    public class BlockResponse : NodeResponse
    {
        public BlockResponseData data { get; set; }

        public class BlockResponseData
        {
            public string id { get; set; }
            public string generator { get; set; }
            public int height { get; set; }
            [JsonConverter(typeof(DateTimeEpochConverter))]
            public DateTime date { get; set; }
            public string nonce { get; set; }
            public string signature { get; set; }
            public string difficulty { get; set; }
            public string argon { get; set; }
            public int transactions { get; set; }
        }
    }

    public class SubmitResponse : NodeResponse
    {
        public string data { get; set; }
    }

    public class TransactionsResponse : NodeResponse
    {
        public List<TransactionResponseData> data { get; set; }

        public class TransactionResponseData
        {
            public string block { get; set; }
            public int confirmations { get; set; }
            [JsonConverter(typeof(DateTimeEpochConverter))]
            public DateTime date { get; set; }
            public string dst { get; set; }
            public decimal fee { get; set; }
            public int height { get; set; }
            public string id { get; set; }
            public string message { get; set; }
            public string public_key { get; set; }
            public string signature { get; set; }
            public string src { get; set; }
            public string type { get; set; }
            public decimal val { get; set; }
            public int version { get; set; }
        }
    }

    public class DateTimeEpochConverter : DateTimeConverterBase
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(((DateTime)value - _epoch).TotalSeconds.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return _epoch.AddSeconds((long)reader.Value);
        }
    }
}
