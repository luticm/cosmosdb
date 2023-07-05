using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace PartitionDesign
{
    internal class SimpleEntityExpanded
    {
        private string id;
        private string pk;
        private string name;
        private string description;
        private string? timestamp;

        public SimpleEntityExpanded() {
            id = Guid.NewGuid().ToString();
            pk = id;
            name = id;
            description = id + "_" + pk;
            timestamp = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);

            Readings = new List<Reading>();
            for (int i = 0; i < 10; i++)
            {
                Readings.Add(new Reading());
            }
        }

        public SimpleEntityExpanded(string id, string pk, string name, string description, string? timestamp)
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

        public List<Reading> Readings;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }

    public class Reading {

        private string id;
        private DateTime businessTimestamp;
        public int metric1;
        public int metric2;
        public int metric3;
        public int metric4;
        public int metric5;

        public Reading()
        {
            Random rnd = new Random();
            id = Guid.NewGuid().ToString();
            businessTimestamp = DateTime.UtcNow;
            metric1 = rnd.Next();
            metric2 = rnd.Next();
            metric3 = rnd.Next();
            metric4 = rnd.Next();
            metric5 = rnd.Next();
        }

        [JsonProperty("id")]
        public string Id { get => id; set => id = value; }
        public DateTime BusinessTimestamp { get => businessTimestamp; set => businessTimestamp = value; }
    }
}
