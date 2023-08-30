using Itinero.Profiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Itinero.Safety
{
    /// <summary>
    /// 
    /// </summary>
    public class SafeScoreHandler
    {

        public static SafeScoreHandler Instance { get; set; }
        private IDictionary<uint, float> values = new SortedDictionary<uint, float>();
        private IList<SafetyInfo> rawValues = new List<SafetyInfo>();


        public SafeScoreHandler()
        {
        }

        public async Task LoadProcessedValuesFromFile(string filename)
        {
            using var stream = File.OpenRead(filename);
            values = await JsonSerializer.DeserializeAsync<Dictionary<uint, float>>(stream, (JsonSerializerOptions)null);
        }

        public void LoadRawValuesFromFile(string filename)
        {
            using (var stream = File.OpenRead(filename))
                rawValues = JsonSerializer.Deserialize<List<SafetyInfo>>(stream, (JsonSerializerOptions)null);
        }

        public void SaveRawValuesToFile(string filename)
        {
            string content = System.Text.Json.JsonSerializer.Serialize(rawValues);
            File.WriteAllText(filename, content);
        }


        public void SaveProcessedValuesToFile(string filename)
        {
            string content = System.Text.Json.JsonSerializer.Serialize(values);
            File.WriteAllText(filename, content);
        }

        public void Log(float latitude, float longitude, float score)
        {
            rawValues.Add(new SafetyInfo { Latitude = latitude, Longitude = longitude, Score = score });

        }

        public void ProcessRaw(RouterDb db, IProfileInstance[] profiles)
        {
            var router = new Router(db);
            foreach (var rawValue in rawValues)
            {
                var point = router.TryResolve(profiles, rawValue.Latitude, rawValue.Longitude, 50f, CancellationToken.None);
                if (!point.IsError)
                {
                    if (!values.ContainsKey(point.Value.EdgeId))
                        values.Add(point.Value.EdgeId, rawValue.Score);
                }
            }

        }

        public float? GetScore(uint edgeId)
        {
            if (values.TryGetValue(edgeId, out var val))
            {
                Console.WriteLine($"Hit {edgeId} | {val}");
                return val;
            }
            return null;
        }

        private class SafetyInfo
        {
            public float Score { get; set; }

            public float Latitude { get; set; }

            public float Longitude { get; set; }
        }

    }
}
