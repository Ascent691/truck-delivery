using System.Diagnostics;
using System.Text;
using TruckDelivery.Infrastructure;

namespace TruckDelivery
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();
            var scenarios = new ScenarioParser().Parse(File.ReadAllLines("1.in"));
            var expectedAnswers = new ScenarioAnswerParser().Parse(File.ReadAllLines("1.ans"));

            if (scenarios.Length != expectedAnswers.Length)
            {
                Console.WriteLine("We have a different number of answers compared to questions, are you using the right combination of scenarios and answers files?");
                return;
            }


            var failedScenarios = 0;
            var builder = new StringBuilder();

            for (int i = 0; i < scenarios.Length; i++)
            {
                var scenario = scenarios[i];
                var expectedAnswer = expectedAnswers[i];
                var computedAnswer = DetermineAnswer(scenario);

                if (!expectedAnswer.IsMatch(computedAnswer))
                {
                    failedScenarios++;
                    builder.AppendLine($"Case #{i + 1}: No Match, expected {string.Join(" ", expectedAnswer.Values)} but computed {string.Join(" ", computedAnswer.Values)}");
                }
                else
                {
                    builder.AppendLine($"Case #{i + 1}: {string.Join(" ", computedAnswer.Values)}");
                }
            }

            Console.Write(builder.ToString());

            Console.WriteLine($"Total Failed Scenarios: {failedScenarios}");
            Console.WriteLine($"Total Passed Scenarios: {scenarios.Length - failedScenarios}");
            timer.Stop();
            Console.WriteLine($"Execution Time: {timer.Elapsed}");
            Console.ReadLine();
        }

        private static ScenarioAnswer DetermineAnswer(Scenario scenario)
        {
            Dictionary<long, City> cities = CreateCities(scenario);
            var tollsForDeliveries = new List<long>();

            for (int d = 0; d < scenario.Deliveries.Length; d++)
            {
                var delivery = scenario.Deliveries[d];
                var originCity = cities[delivery.FromCityId];
                var destinationCity = cities[delivery.ToCityId];
                var tolls = new List<long>();

                var currentCity = originCity;

                while (currentCity != destinationCity)
                {
                    var connection = currentCity.GetConnectionTowards(destinationCity);

                    if (connection.Item2.LoadLimit <= delivery.LoadWeight)
                    {
                        tolls.Add(connection.Item2.TollCharge);
                    }

                    currentCity = connection.Item1;
                }

                var gcd = tolls.Count == 0 ? 0 : MathHelper.GreatestCommonDivisor(tolls.ToArray());
                tollsForDeliveries.Add(gcd);
            }

            return new ScenarioAnswer(tollsForDeliveries.ToArray());
        }

        private static Dictionary<long, City> CreateCities(Scenario scenario)
        {
            var allCityIds = new HashSet<long>(scenario.Roads.SelectMany(road => new long[] { road.FirstCityId, road.SecondCityId }));


            var cities = allCityIds.Select(cityId => new City(cityId)).ToDictionary(city => city.Id);

            foreach (var road in scenario.Roads)
            {
                cities[road.FirstCityId].AddRoute(cities[road.SecondCityId], cities[road.SecondCityId], road);
                cities[road.SecondCityId].AddRoute(cities[road.FirstCityId], cities[road.FirstCityId], road);
            }

            allCityIds.Clear();
            var citiesToPropogate = new List<Tuple<City, City>>();

            var capitalCity = cities[1];
            citiesToPropogate.Add(new Tuple<City, City>(capitalCity, capitalCity));
            allCityIds.Add(capitalCity.Id);

            for (int p = 0; p < citiesToPropogate.Count; p++)
            {
                var cityToPropogate = citiesToPropogate[p];

                foreach (var connection in cityToPropogate.Item1.GetConnectedCities())
                {
                    if (connection.Item1 == cityToPropogate.Item2) continue;

                    if (allCityIds.Add(connection.Item1.Id))
                    {
                        citiesToPropogate.Add(new Tuple<City, City>(connection.Item1, cityToPropogate.Item1));
                    }

                    connection.Item1.AddRoute(capitalCity, cityToPropogate.Item1, connection.Item2);
                }
            }

            return cities;
        }
    }

    public class City
    {
        private Dictionary<long, Tuple<City, Road>> _routes = new Dictionary<long, Tuple<City, Road>>();
        public long Id { get; init; }

        public City(long id)
        {
            Id = id;
        }

        public void AddRoute(City destinationCity, City nextCity, Road road)
        {
            this._routes[destinationCity.Id] = new Tuple<City, Road>(nextCity, road);
        }

        public Tuple<City, Road> GetConnectionTowards(City destination)
        {
            return _routes[destination.Id];
        }

        public IEnumerable<Tuple<City, Road>> GetConnectedCities()
        {
            foreach (var item in _routes)
            {
                yield return item.Value;
            }
        }
    }
}
