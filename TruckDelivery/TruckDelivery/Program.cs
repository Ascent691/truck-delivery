using System.Collections;
using System.ComponentModel;
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
            // STEP 1: PATHFIND FROM START TO 
            // STEP 2: Store each paid 
            // STEP 3: FIND gcd for toll charges
            var dfs = new DFS();
            var cityPairs = GetCityPairs(scenario);
            var roads = scenario.Roads;
            const int CapitalCityId = 1;
            List<long> gcds = [];

            // TODO: The order of the cities in the road should not matter. 
            // When mapping, road cannot be found because it is looking for an exact match) => i.e., 7 1 is not 1 7 with the current logic.
            // See mappedRoads

            foreach(var delivery in scenario.Deliveries)
            {
                List<long> tollsPaid = [];
                var weight = delivery.LoadWeight;
                var route = dfs.FindPath(delivery.FromCityId, CapitalCityId, cityPairs);

                if (route is null) continue;

                var roadsInRoute = route.Zip(route.Skip(1), (city1, city2) => (city1, city2));
                var roadLookup = roads.ToDictionary(r => (r.FirstCityId, r.SecondCityId));
                var mappedRoads = roadsInRoute
                                    .Select(r => (roadsInRoute: r, Road: roadLookup.TryGetValue(r, out var road) ? road : null))
                                    .ToList();

                foreach (var mappedRoad in mappedRoads)
                {
                    var road = mappedRoad.Road;
                    if (road is null) continue;

                    if (weight >= road.LoadLimit)
                    {
                        tollsPaid.Add(road.TollCharge);
                    }
                }

                gcds.Add(MathHelper.GreatestCommonDivisor([.. tollsPaid]));
            }

            return new ScenarioAnswer([.. gcds]);
        }

        private static List<(long, long)> GetCityPairs(Scenario scenario)
        {
            return scenario.Roads.Select((r) => (r.FirstCityId, r.SecondCityId)).ToList();
        }
    }
}
