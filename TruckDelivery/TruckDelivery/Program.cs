using System.Collections;
using System.Diagnostics;
using System.Text;
using TruckDelivery.Infrastructure;
using static TruckDelivery.Extensions;

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
            // STEP 1: PATHFIND FROM START TO 1
            // hasRoadBeenTraversed boolean

            // STEP 2: Store each paid toll

            // STEP 3: FIND gcd for toll charges

            var cities = scenario.Roads
                .SelectMany((r) => new long[] { r.FirstCityId, r.SecondCityId })
                .Distinct()
                .Select((id) => new City(id))
                .ToDictionary((city) => city.Id);

            foreach (var road in scenario.Roads)
            {
                var firstCity = cities[road.FirstCityId];
                var secondCity = cities[road.SecondCityId];

                var connection = new Connection(firstCity, secondCity, road);
                firstCity.Connections.Add(secondCity.Id, connection);
                secondCity.Connections.Add(firstCity.Id, connection );
            }
            var capitalCity = cities[1];

            foreach(var kvp in capitalCity.Connections)
            {
                var connection = kvp.Value;
                var destinationCity = kvp.Key;

                var connectedCity = connection.SecondCity != capitalCity ?
                    connection.SecondCity :
                    connection.FirstCity;

                foreach(var kvp2 in connectedCity.Connections)
                {
                    // Looping over cities connected to cities connected that directly connect to the capital
                    var connection2 = kvp2.Value;
                    var nextConnectedCity = connection2.SecondCity != capitalCity ?
                    connection2.SecondCity :
                    connection2.FirstCity;

                    nextConnectedCity.Connections.Add(capitalCity.Id, connection2 );
                }
            }

            foreach (var delivery in scenario.Deliveries)
            {
                var vectors = new List<List<long>>();

                var start = delivery.FromCityId;
                var end = delivery.ToCityId; // always 1 for this scenario
            }

            throw new NotImplementedException("Please implement me, remember to convert the toll charges to greatest common divisors (Use MathHelper unless your brave) :)");
        }


        //static List<int> DFS(bool[] visited, int start, int end)
        //{
        //    var stack = new List<int>();
        //    stack.Add(x);

        //    if (start == end)
        //    {
        //        return stack;
        //    }

        //    visited[start] = true;

        //}
    }

    class City(long id)
    {
        public long Id { get; init; } = id;
        public Dictionary<long, Connection> Connections { get; set; } = [];
    }

    class Connection(City firstCity, City secondCity, Road road)
    {
        public City FirstCity { get; init; } = firstCity;
        public City SecondCity { get; init; } = secondCity;
        public Road Road { get; init; } = road;
    }
}
