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

            for (int i = 0; i < scenarios.Length; i++)
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
            var deliveries = scenario.Deliveries.ToList();
            var roads = scenario.Roads;

            var roadNodes = new List<RouteNode<Road>>();
            foreach (var road in roads)
            {
                roadNodes.Add(new RouteNode<Road>(road));
            }
            
            foreach (var delivery in deliveries)
            {
                var availableRoads = roads.ToList();
                var startingRoad = GetStartingRoad(delivery, availableRoads);
                var routeRoot = new RouteNode<Road>(startingRoad);

                var visited = new HashSet<Road>();
                CreateRouteTree(routeRoot, roadNodes, visited);

                var path = GetPath(routeRoot, delivery.FromCityId, delivery.ToCityId);

            }
            throw new NotImplementedException(
                "Please implement me, remember to convert the toll charges to greatest common divisors (Use MathHelper unless your brave) :)");
        }

        private static void CreateRouteTree(
            RouteNode<Road> currentNode,
            List<RouteNode<Road>> availableRoads,
            HashSet<Road> visited)
        {
            visited.Add(currentNode.Value);

            var currentRoad = currentNode.Value;
            var connectedCities = new[] { currentRoad.FirstCityId, currentRoad.SecondCityId };

            foreach (var city in connectedCities)
            {
                var nextRoads = availableRoads
                    .Where(roadNode =>
                        !visited.Contains(roadNode.Value) &&
                        (roadNode.Value.FirstCityId == city || roadNode.Value.SecondCityId == city))
                    .ToList();
                if(nextRoads.Count <= 0) continue;

                foreach (var nextRoad in nextRoads)
                {
                    var childNode = new RouteNode<Road>(nextRoad.Value)
                    {
                        Parent = currentNode
                    };
                    currentNode.AddChild(childNode);

                    CreateRouteTree(childNode, availableRoads, visited);
                }
            }
        }

        private static Road GetStartingRoad(Delivery delivery, List<Road> roads)
        {
            return roads.First(road => road.FirstCityId == delivery.FromCityId || road.SecondCityId == delivery.FromCityId);
        }
        
        private static long GetTotalTollCharges(RouteNode<Road> root)
        {
            return 0L;
        }
        
        private static List<long> GetPath(RouteNode<Road> root, long startCity, long targetCity)
        {
            var path = new List<long>();
            var visited = new HashSet<RouteNode<Road>>();

            if (FindPathDFS(root, startCity, targetCity, visited, path))
                return path;

            return new List<long>();
        }

        
        private static bool FindPathDFS(
            RouteNode<Road> currentNode,
            long currentCity,
            long targetCity,
            HashSet<RouteNode<Road>> visited,
            List<long> path)
        {
            path.Add(currentCity);

            if (currentCity == targetCity)
                return true;

            visited.Add(currentNode);

            var connectedCities = new[] { currentNode.Value.FirstCityId, currentNode.Value.SecondCityId };

            foreach (var nextCity in connectedCities)
            {
                if (nextCity == currentCity)
                    continue;

                foreach (var child in currentNode.Children)
                {
                    if (!visited.Contains(child) &&
                        (child.Value.FirstCityId == nextCity || child.Value.SecondCityId == nextCity))
                    {
                        if (FindPathDFS(child, nextCity, targetCity, visited, path))
                            return true;
                    }
                }
            }

            path.RemoveAt(path.Count - 1);
            return false;
        }

    }
}


