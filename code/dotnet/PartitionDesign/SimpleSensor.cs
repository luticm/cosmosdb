using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartitionDesign
{
    internal class SimpleSensor
    {
        private string id; // ReadingId
        private string sensorId; // SensorId
        private string partitionKey;

        private string timestamp;
        private string location; 
        private decimal? metric1;
        private decimal? metric2;
        private decimal? metric3;
        private decimal? metric4;
        private string filler;

        public SimpleSensor() : this(Guid.NewGuid().ToString()) { 
        }

        public SimpleSensor(string sensorId): this(sensorId
            , Guid.NewGuid().ToString()
            , DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)) {
        }

        public SimpleSensor(string sensorId, string id, string partitionKey)
        {
            SensorId = sensorId;
            PartitionKey = partitionKey;
            Id = id;
            Timestamp = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);

            Random r = new Random();
            Metric1 = ((decimal)r.NextDouble() * 10);
            Metric2 = ((decimal)r.NextDouble() * 100);
            Metric3 = ((decimal)r.NextDouble() * 100);
            Metric4 = ((decimal)r.NextDouble() * 1000);            
            
        }

        /// <summary>
        /// Id = MeasurementId -> Beachname and timestamp combined
        /// </summary>
        [JsonProperty("id")]
        public string Id { get => id; set => id = value; }
        public string Timestamp { get => timestamp; set => timestamp = value; }
        public string SensorId { get => sensorId; set => sensorId = value; }
        public string Location { get => location; set => location = value; }
        public decimal? Metric1 { get => metric1; set => metric1 = value; }
        public decimal? Metric2 { get => metric2; set => metric2 = value; }
        public decimal? Metric3 { get => metric3; set => metric3 = value; }
        public decimal? Metric4 { get => metric4; set => metric4 = value; }
        public string Filler { get => filler; set => filler = value; }
        public string PartitionKey { get => partitionKey; set => partitionKey = value; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
