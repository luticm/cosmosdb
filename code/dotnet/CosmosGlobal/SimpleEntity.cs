using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Newtonsoft.Json;

namespace CosmosGlobal
{
    internal class SimpleEntity
    {

            private string id;
            private string pk;
            private string name;
            private string description;
            private string? timestamp;

            public SimpleEntity()
            {
                id = Guid.NewGuid().ToString();
                pk = Guid.NewGuid().ToString();
                name = id;
                description = id + "_" + pk;
                timestamp = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
            }

            public SimpleEntity(string id, string pk, string name, string description, string? timestamp)
            {
                Id = id;
                Pk = pk;
                Name = name;
                Description = description;
                Timestamp = timestamp;
            }

            [JsonProperty("id")]
            public string Id { get => id; set => id = value; }
            public string Pk { get => pk; set => pk = value; }
            public string Name { get => name; set => name = value; }
            public string Description { get => description; set => description = value; }
            public string? Timestamp { get => timestamp; set => timestamp = value; }

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }
}
