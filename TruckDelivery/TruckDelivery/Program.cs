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
            // Figure out how many cities there are.
            // We look at all roads and take the biggest city number we see.
            int biggestCityId = scenario.Roads.Max(
                road => (int)Math.Max(road.FirstCityId, road.SecondCityId)
            );

            // Make a map of cities.
            // Each city will have a list of roads leaving it.
            var map = new List<(int toCity, long weightLimit, long toll)>[biggestCityId + 1];
            for (int i = 0; i <= biggestCityId; i++)
                map[i] = new List<(int, long, long)>();

            // Fill the map with all roads (both directions)
            foreach (var road in scenario.Roads)
            {
                map[(int)road.FirstCityId].Add(((int)road.SecondCityId, road.LoadLimit, road.TollCharge));
                map[(int)road.SecondCityId].Add(((int)road.FirstCityId, road.LoadLimit, road.TollCharge));
            }

            // Prepare to explore the map
            int[] parent = new int[biggestCityId + 1];   // Who is the parent city
            long[] roadLimit = new long[biggestCityId + 1]; // Weight limit of road to parent
            long[] roadToll = new long[biggestCityId + 1];  // Toll of road to parent
            bool[] visited = new bool[biggestCityId + 1];   // Have we visited this city yet?

            // Explore the cities using DFS (Depth-First Search)
            // We start at city 1 and mark all roads leading to children
            void Explore(int city, int parentCity)
            {
                visited[city] = true;    // Mark city as visited
                parent[city] = parentCity; // Remember who the parent is

                // Look at all roads leaving this city
                foreach (var (nextCity, limit, toll) in map[city])
                {
                    if (!visited[nextCity])
                    {
                        // Record info about the road to the next city
                        roadLimit[nextCity] = limit;
                        roadToll[nextCity] = toll;

                        // Keep exploring from the next city
                        Explore(nextCity, city);
                    }
                }
            }

            // Start DFS from city 1 (the root city)
            Explore(1, 0);

            // Answer each delivery
            var answers = new long[scenario.Deliveries.Length];

            for (int i = 0; i < scenario.Deliveries.Length; i++)
            {
                var delivery = scenario.Deliveries[i];
                int from = (int)delivery.FromCityId;
                long load = delivery.LoadWeight;

                // Collect all tolls along the path to city 1
                var tollsOnPath = new List<long>();

                while (from != 1) // Keep moving toward city 1
                {
                    if (roadLimit[from] <= load)   // Can the truck travel this road?
                        tollsOnPath.Add(roadToll[from]); // If yes, pay the toll

                    from = parent[from]; // Move one step closer to city 1
                }

                // Find the biggest number that divides all tolls (GCD)
                answers[i] = tollsOnPath.Count > 0
                    ? MathHelper.GreatestCommonDivisor(tollsOnPath.ToArray())
                    : 0; // If no tolls, answer is 0
            }

            // Return all answers for this scenario
            return new ScenarioAnswer(answers);
        }
    }
}

//Diagram of the sample input

//         1
//        / \
//  (2,4)/   \(5,7)
//      2       7
//     /|\
// (1,5)  (7,8)
//   6     3
//         |\
//      (6,2) (9,9)
//         4   5