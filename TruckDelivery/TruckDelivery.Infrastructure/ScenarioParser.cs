namespace TruckDelivery.Infrastructure
{
    public class ScenarioParser
    {
        public ScenarioParser() { }

        public Scenario[] Parse(string[] lines)
        {
            int lineIndex = 0;
            var totalScenarios = int.Parse(lines[lineIndex++].Trim());
            var results = new Scenario[totalScenarios];

            for (int i = 0; i < results.Length; i++)
            {
                string[] parts = lines[lineIndex++].Split(' ');
                var numRoads = int.Parse(parts[0]) - 1; // No idea why they made the schema N - 1
                var numDeliveries = int.Parse(parts[1]);
                var roads = new Road[numRoads];

                for (int r = 0; r < roads.Length; r++)
                {
                    parts = lines[lineIndex++].Split(' ');
                    var firstCityId = long.Parse(parts[0]);
                    var secondCityId = long.Parse(parts[1]);
                    var loadLimit = long.Parse(parts[2]);
                    var tollCharge = long.Parse(parts[3]);
                    roads[r] = new Road(firstCityId, secondCityId, tollCharge, loadLimit);
                }

                var deliveries = new Delivery[numDeliveries];

                for (int d = 0; d < deliveries.Length; d++)
                {
                    parts = lines[lineIndex++].Split(' ');
                    var fromCityId = long.Parse(parts[0]);
                    var loadWeight = long.Parse(parts[1]);
                    deliveries[d] = new Delivery(fromCityId, 1, loadWeight);
                }

                results[i] = new Scenario(roads, deliveries);
            }

            return results;
        }
    }
}
