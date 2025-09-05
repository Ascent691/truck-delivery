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
            var scenarios = new ScenarioParser().Parse(File.ReadAllLines("simple.in"));
            var expectedAnswers = new ScenarioAnswerParser().Parse(File.ReadAllLines("simple.ans"));

            if (scenarios.Length != expectedAnswers.Length)
            {
                Console.WriteLine(
                    "We have a different number of answers compared to questions, are you using the right combination of scenarios and answers files?");
                return;
            }


            var failedScenarios = 0;
            var builder = new StringBuilder();

            for (var i = 0; i < scenarios.Length; i++)
            {
                var scenario = scenarios[i];
                var expectedAnswer = expectedAnswers[i];
                var computedAnswer = DetermineAnswer(scenario);

                if (!expectedAnswer.IsMatch(computedAnswer))
                {
                    failedScenarios++;
                    builder.AppendLine(
                        $"Case #{i + 1}: No Match, expected {string.Join(" ", expectedAnswer.Values)} but computed {string.Join(" ", computedAnswer.Values)}");
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
            var results = new long[scenario.Deliveries.Length];

            for (var i = 0; i < scenario.Deliveries.Length; i++)
            {
                var delivery = scenario.Deliveries[i];
                var toll = FindMinimumToll(scenario.Roads, delivery.FromCityId, delivery.ToCityId, delivery.LoadWeight);
                results[i] = toll;
            }

            return new ScenarioAnswer(results);

        }
        private static long FindMinimumToll(Road[] roads, long from, long finalCity, long loadWeight)
        {
            var roadMap = new Dictionary<long, List<(long destinationCity, long loadLimit, long toll)>>();

            foreach (var road in roads)
            {
                if (!roadMap.ContainsKey(road.FirstCityId))
                {
                    roadMap[road.FirstCityId] = new List<(long, long, long)>();
                }

                if (!roadMap.ContainsKey(road.SecondCityId))
                {
                    roadMap[road.SecondCityId] = new List<(long, long, long)>();
                }

                roadMap[road.FirstCityId].Add((road.SecondCityId, road.LoadLimit, road.TollCharge));
                roadMap[road.SecondCityId].Add((road.FirstCityId, road.LoadLimit, road.TollCharge));
            }

            var cityQueue = new PriorityQueue<(long city, long dist), long>();
            var cityDistances = new Dictionary<long, long>();
            var cameFrom = new Dictionary<long, long>();

            cityQueue.Enqueue((from, 0), 0);
            cityDistances[from] = 0;

            while (cityQueue.Count > 0)
            {
                cityQueue.TryDequeue(out var current, out var _);

                if (current.city == finalCity)
                {
                    var travelPath = new List<(long from, long to)>();
                    var tollCharges = new List<long>();

                    var currentCityInPath = finalCity;
                    while (cameFrom.ContainsKey(currentCityInPath))
                    {
                        var prev = cameFrom[currentCityInPath];
                        travelPath.Add((prev, currentCityInPath));

                        long toll = 0;
                        foreach (var edge in roadMap[prev])
                        {
                            if (edge.destinationCity == currentCityInPath)
                            {
                                toll = loadWeight >= edge.loadLimit ? edge.toll : 0;
                                break;
                            }
                        }
                        tollCharges.Add(toll);
                        currentCityInPath = prev;
                    }

                    travelPath.Reverse();
                    tollCharges.Reverse();

                    var pathToll = MathHelper.GreatestCommonDivisor(tollCharges.ToArray());

                    Console.WriteLine($"Path from {from} to {finalCity} (load {loadWeight}): " +
                                      $"{string.Join(", ", travelPath.Select(e => $"({e.from},{e.to})"))}, " +
                                      $"Edge Tolls: {string.Join(", ", tollCharges)}, " +
                                      $"Combined Toll (GCD): {pathToll}");

                    return pathToll;
                }

                if (cityDistances.ContainsKey(current.city))
                {
                    if (current.dist > cityDistances[current.city])
                    {
                        continue;
                    }
                }

                foreach (var edge in roadMap[current.city])
                {
                    var neighbor = edge.destinationCity;
                    var toll = loadWeight >= edge.loadLimit ? edge.toll : 0;
                    var newDist = current.dist + toll;

                    if (!cityDistances.ContainsKey(neighbor))
                    {
                        cityDistances[neighbor] = newDist;
                        cameFrom[neighbor] = current.city;
                        cityQueue.Enqueue((neighbor, newDist), newDist);
                    }
                    else if (newDist < cityDistances[neighbor])
                    {
                        cityDistances[neighbor] = newDist;
                        cameFrom[neighbor] = current.city;
                        cityQueue.Enqueue((neighbor, newDist), newDist);
                    }
                }
            }
            Console.WriteLine($"No path from {from} to {finalCity} (load {loadWeight})");
            return 0;
        }
    }
}

