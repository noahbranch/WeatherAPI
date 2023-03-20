using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

public class Program {
    public static async Task Main() {
        var apiKey = "347a9a3a94cba1f28b40a02f9da0c6c3";
        var cityList = new List<string>() {
            "chattanooga,tn,us",
            "dayton,oh,us",
            "chicago,il,us"
        };

        var highs = await GetForecastedHighAsync(cityList, apiKey);

        

        foreach(var high in highs) {
            Console.WriteLine($"The high in {high.CityName} this week will be {high.WeeklyHigh} Farenheit");
        }
    }

    [JsonObject]
    public class CityLatLong {
        [JsonProperty("name")]
        public string? name { get; set; }
        [JsonIgnore]
        public string[]? local_names { get; set; }
        [JsonProperty("lat")]
        public double lat { get; set; }
        [JsonProperty("lon")]
        public double lon { get; set; }
        [JsonProperty("country")]
        public string? country { get; set; }
        [JsonProperty("state")]
        public string? state { get; set; }
    }

    public class CityWeeklyHigh {
        public string? CityName { get; set; }
        public double? WeeklyHigh { get; set; }
    }

    public static async Task<List<CityWeeklyHigh>> GetForecastedHighAsync(List<string> cities, string apiKey) {
        if (cities == null) {
            return new List<CityWeeklyHigh>();
        }
        var highs = new List<CityWeeklyHigh>();

        foreach (var city in cities) {
            var url = $"http://api.openweathermap.org/data/2.5/forecast?q={city}&units=imperial&appid={apiKey}";
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) {
                throw new Exception($"Data not found for {city}");
            }

            var content = await response.Content.ReadAsStringAsync();

            var data = JObject.Parse(content);
            double tempMax = 0;
            foreach(var id in data["list"]) {
                string temp_max = (string)id.SelectToken("main.temp_max");
                if (Convert.ToDouble(temp_max) > tempMax) {
                    tempMax = Convert.ToDouble(temp_max);
                }
            }
            var cityHigh = new CityWeeklyHigh() {
                CityName = (string)data.SelectToken("city.name"),
                WeeklyHigh = tempMax
            };

            highs.Add(cityHigh);
        }

        return highs;
    }
}