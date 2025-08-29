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
            var results = new List<long>();

            foreach (var delivery in scenario.Deliveries)
            {
                var pathRoads = FindPathRoads(scenario.Roads, delivery.FromCityId, delivery.ToCityId);

                var tollCharges = new List<long>();

                foreach (var road in pathRoads)
                {
                    if (delivery.LoadWeight >= road.LoadLimit)
                    {
                        tollCharges.Add(road.TollCharge);
                    }
                }

                long gcd = tollCharges.Count == 0 ? 0 : MathHelper.GreatestCommonDivisor(tollCharges.ToArray());
                results.Add(gcd);
            }

            return new ScenarioAnswer(results.ToArray());
        }

        private static List<Road> FindPathRoads(Road[] roads, long start, long end)
        {
            if (start == end) return new List<Road>();

            var queue = new Queue<(long city, List<Road> pathRoads)>();
            var visited = new HashSet<long>();

            queue.Enqueue((start, new List<Road>()));
            visited.Add(start);

            while (queue.Count > 0)
            {
                var (currentCity, currentPath) = queue.Dequeue();

                foreach (var road in roads)
                {
                    long nextCity = -1;

                    if (road.FirstCityId == currentCity && !visited.Contains(road.SecondCityId))
                    {
                        nextCity = road.SecondCityId;
                    }
                    else if (road.SecondCityId == currentCity && !visited.Contains(road.FirstCityId))
                    {
                        nextCity = road.FirstCityId;
                    }

                    if (nextCity != -1)
                    {
                        var newPath = new List<Road>(currentPath) { road };

                        if (nextCity == end)
                        {
                            return newPath;
                        }

                        visited.Add(nextCity);
                        queue.Enqueue((nextCity, newPath));
                    }
                }
            }
            return new List<Road>(); 
        }

    }
}
