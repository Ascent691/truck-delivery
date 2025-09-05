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
        private static ScenarioAnswer DetermineAnswerPreOld(Scenario scenario)
        {
            var allRoads = scenario.Roads.ToList();
            var finalGcdValues = new List<long>();

            foreach (var delivery in scenario.Deliveries)
            {
                var tollsForThisDelivery = new HashSet<long>();
                var visitedCities = new HashSet<long>();

                long currentCity = delivery.FromCityId;
                long destination = delivery.ToCityId;
                long deliveryWeight = delivery.LoadWeight;

                visitedCities.Add(currentCity);

                while (currentCity != destination)
                {
                    bool foundNextRoad = false;
                    foreach (var road in allRoads)
                    {
                        long nextCity = -1;

                        if (road.FirstCityId == currentCity && !visitedCities.Contains(road.SecondCityId))
                        {
                            nextCity = road.SecondCityId;
                        }
                        else if (road.SecondCityId == currentCity && !visitedCities.Contains(road.FirstCityId))
                        {
                            nextCity = road.FirstCityId;
                        }

                        if (nextCity != -1)
                        {
                            if (deliveryWeight >= road.LoadLimit)
                            {
                                tollsForThisDelivery.Add(road.TollCharge);
                            }

                            currentCity = nextCity;
                            visitedCities.Add(currentCity);
                            foundNextRoad = true;
                            break;
                        }
                    }

                    if (!foundNextRoad)
                    {
                        break;
                    }
                }

                long dailyGcd = 0;
                if (tollsForThisDelivery.Count > 0)
                {
                    dailyGcd = MathHelper.GreatestCommonDivisor(tollsForThisDelivery.ToArray());
                }
                finalGcdValues.Add(dailyGcd);
            }
            return new ScenarioAnswer(finalGcdValues.ToArray());
        }

        private static ScenarioAnswer DetermineAnswerAi(Scenario scenario)
        {
            var adjacency = new Dictionary<long, List<Road>>();
            foreach (var road in scenario.Roads)
            {
                if (!adjacency.ContainsKey(road.FirstCityId))
                    adjacency[road.FirstCityId] = new List<Road>();
                if (!adjacency.ContainsKey(road.SecondCityId))
                    adjacency[road.SecondCityId] = new List<Road>();

                adjacency[road.FirstCityId].Add(road);
                adjacency[road.SecondCityId].Add(road);
            }

            var finalGcdValues = new List<long>();

            foreach (var delivery in scenario.Deliveries)
            {
                var tollsForThisDelivery = new HashSet<long>();
                var visitedCities = new HashSet<long>();

                long currentCity = delivery.FromCityId;
                long destination = delivery.ToCityId;
                long deliveryWeight = delivery.LoadWeight;

                visitedCities.Add(currentCity);

                while (currentCity != destination)
                {
                    bool foundNextRoad = false;

                    foreach (var road in adjacency[currentCity])
                    {
                        long nextCity = -1;

                        if (road.FirstCityId == currentCity && !visitedCities.Contains(road.SecondCityId))
                        {
                            nextCity = road.SecondCityId;
                        }
                        else if (road.SecondCityId == currentCity && !visitedCities.Contains(road.FirstCityId))
                        {
                            nextCity = road.FirstCityId;
                        }

                        if (nextCity != -1)
                        {
                            if (deliveryWeight >= road.LoadLimit)
                                tollsForThisDelivery.Add(road.TollCharge);

                            currentCity = nextCity;
                            visitedCities.Add(currentCity);
                            foundNextRoad = true;
                            break; 
                        }
                    }

                    if (!foundNextRoad)
                        break;
                }

                long dailyGcd = tollsForThisDelivery.Count > 0
                    ? MathHelper.GreatestCommonDivisor(tollsForThisDelivery.ToArray())
                    : 0;

                finalGcdValues.Add(dailyGcd);
            }

            return new ScenarioAnswer(finalGcdValues.ToArray());
        }

        //Using Parallel
        record Neighbor(long CityId, long LoadLimit, long TollCharge);

        private static ScenarioAnswer DetermineAnswer(Scenario scenario)
        {
            var adjacency = new Dictionary<long, List<Neighbor>>();
            long maxCityId = 0;

            foreach (var road in scenario.Roads)
            {
                if (!adjacency.ContainsKey(road.FirstCityId))
                    adjacency[road.FirstCityId] = new List<Neighbor>();
                if (!adjacency.ContainsKey(road.SecondCityId))
                    adjacency[road.SecondCityId] = new List<Neighbor>();

                adjacency[road.FirstCityId].Add(new Neighbor(road.SecondCityId, road.LoadLimit, road.TollCharge));
                adjacency[road.SecondCityId].Add(new Neighbor(road.FirstCityId, road.LoadLimit, road.TollCharge));

                if (road.FirstCityId > maxCityId) maxCityId = road.FirstCityId;
                if (road.SecondCityId > maxCityId) maxCityId = road.SecondCityId;
            }

            var finalGcdValues = new long[scenario.Deliveries.Count()];

            Parallel.ForEach(
                scenario.Deliveries.Select((delivery, index) => (delivery, index)),
                tuple =>
                {
                    var (delivery, index) = tuple;

                    var visited = new bool[maxCityId + 1];
                    var tolls = new HashSet<long>();

                    long currentCity = delivery.FromCityId;
                    long destination = delivery.ToCityId;
                    long deliveryWeight = delivery.LoadWeight;

                    visited[currentCity] = true;

                    while (currentCity != destination)
                    {
                        bool foundNextRoad = false;

                        foreach (var neighbor in adjacency[currentCity])
                        {
                            if (visited[neighbor.CityId]) continue;

                            if (deliveryWeight >= neighbor.LoadLimit)
                                tolls.Add(neighbor.TollCharge);

                            currentCity = neighbor.CityId;
                            visited[currentCity] = true;
                            foundNextRoad = true;
                            break;
                        }

                        if (!foundNextRoad)
                            break;
                    }

                    long dailyGcd = 0;
                    foreach (var toll in tolls)
                        dailyGcd = dailyGcd == 0 ? toll : MathHelper.GreatestCommonDivisor(dailyGcd, toll);

                    finalGcdValues[index] = dailyGcd;
                });

            return new ScenarioAnswer(finalGcdValues);
        }
    }
}
