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
            foreach (var delivery in scenario.Deliveries)
            {
                var path = new List<(long, long)>();
                var visited = new HashSet<long>();
                var currentCity = delivery.FromCityId;
                visited.Add(currentCity);

                while (currentCity != delivery.ToCityId)
                {
                    var nextRoad = scenario.Roads.FirstOrDefault(r =>
                        delivery.LoadWeight <= r.LoadLimit &&
                        ((r.FirstCityId == currentCity && !visited.Contains(r.SecondCityId)) ||
                         (r.SecondCityId == currentCity && !visited.Contains(r.FirstCityId)))
                    );

                    if (nextRoad == null)
                    {
                        Console.WriteLine($"No path from {delivery.FromCityId} to {delivery.ToCityId}");
                        break;
                    }

                    var nextCity = nextRoad.FirstCityId == currentCity ? nextRoad.SecondCityId : nextRoad.FirstCityId;
                    path.Add((currentCity, nextCity));
                    visited.Add(nextCity);
                    currentCity = nextCity;
                }
                Console.WriteLine(string.Join(", ", path.Select(p => $"({p.Item1},{p.Item2})")));
            }

            return new ScenarioAnswer(new long[scenario.Deliveries.Length]);
        }


    }
}
