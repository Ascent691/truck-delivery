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
            foreach (var delivery in deliveries)
            {
                var availableRoads = roads.ToList();
                var startingRoad = GetStartingRoad(delivery, availableRoads);
                var routeRoot = new RouteNode<Road>(startingRoad);
                SetNextRoad(routeRoot, availableRoads, delivery.ToCityId);
                var getTotalTollCharges = GetTotalTollCharges(routeRoot, delivery.ToCityId);
            }
            
            
            // var tollTotals = new List<long>();
            //
            // foreach (var delivery in deliveries)
            // {
            //     var startingRoad = GetStartingRoad(delivery, roads);
            //     var secondCityId = startingRoad.FirstCityId != delivery.FromCityId ? startingRoad.FirstCityId : startingRoad.SecondCityId;
            //     var root = new RouteNode<Road>(startingRoad with
            //     {
            //         FirstCityId = delivery.FromCityId, SecondCityId = secondCityId
            //     });
            //     RouteNode<Road> nextRoad = root;
            //     var availableRoads = roads.ToList();
            //     availableRoads.Remove(root.Value!);
            //     for (int i = 0; i < availableRoads.Count - 1; i++)
            //     {
            //         var road = availableRoads[i];
            //         if(road.FirstCityId != nextRoad.Value!.SecondCityId && road.SecondCityId != nextRoad.Value!.SecondCityId) continue;
            //         var nextSecondCityId = road.FirstCityId != delivery.FromCityId ? road.FirstCityId : road.SecondCityId;
            //         nextRoad.AddChild(road with
            //         {
            //             FirstCityId = nextRoad.Value!.SecondCityId,
            //             SecondCityId = nextSecondCityId
            //         });
            //         nextRoad = root.Children.First();
            //         availableRoads.Remove(road);
            //         break;
            //     }
            // }

            throw new NotImplementedException(
                "Please implement me, remember to convert the toll charges to greatest common divisors (Use MathHelper unless your brave) :)");
        }

        private static Road GetStartingRoad(Delivery delivery, List<Road> roads)
        {
            return roads.First(road => road.FirstCityId == delivery.FromCityId || road.SecondCityId == delivery.FromCityId);
        }
        
        private static void SetNextRoad(RouteNode<Road> nextRoad, List<Road> roads, long endingCityId)
        {
            foreach (var road in roads)
            {
                if(road.RoadVisited.Visited) continue;
                if (road.FirstCityId == nextRoad.Value.SecondCityId)
                {
                    road.RoadVisited.Visited = true;
                    nextRoad.AddChild(road with
                    {
                        FirstCityId = road.FirstCityId,
                        SecondCityId = road.SecondCityId
                    });
                } 
                else if (road.SecondCityId == nextRoad.Value.SecondCityId)
                {
                    road.RoadVisited.Visited = true;
                    nextRoad.AddChild(road with
                    {
                        FirstCityId = nextRoad.Value.SecondCityId,
                        SecondCityId = road.FirstCityId
                    });
                }
            }
            
            if(nextRoad.Children.Count == 0) return;
            foreach (var child in nextRoad.Children)
            {
                SetNextRoad(child, roads, endingCityId);
            }
        }

        private static long GetTotalTollCharges(RouteNode<Road> root, long endingCityId)
        {
            var path = root.Children;
            return 0L;
        }
    }
}

public class RouteNode<T>
{
    public T Value { get; set; }
    public RouteNode<T> Parent { get; set; }
    public List<RouteNode<T>> Children { get; set; }

    public RouteNode(T value)
    {
        Value = value;
        Children = new List<RouteNode<T>>();
    }

    public RouteNode<T> AddChild(T child)
    {
        var childNode = new RouteNode<T>(child)
        {
            Parent = this
        };
        Children.Add(childNode);
        return childNode;
    }
}
