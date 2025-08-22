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
                int numRoads = int.Parse(parts[0]);
                int numDeliveries = int.Parse(parts[1]);
                var roads = new Road[numRoads];

                for (int r = 0; r < roads.Length; r++)
                {
                    parts = lines[lineIndex++].Split(' ');
                    int firstCityId = int.Parse(parts[0]);
                    int secondCityId = int.Parse(parts[1]);
                    int loadLimit = int.Parse(parts[2]);
                    int tollCharge = int.Parse(parts[3]);
                    roads[r] = new Road(firstCityId, secondCityId, tollCharge, loadLimit);
                }

                var deliveries = new Delivery[numDeliveries];

                for (int d = 0; d < deliveries.Length; d++)
                {
                    parts = lines[lineIndex++].Split(' ');
                    int fromCityId = int.Parse(parts[0]);
                    int loadWeight = int.Parse(parts[1]);
                    deliveries[d] = new Delivery(fromCityId, 1, loadWeight);
                }

                int[,] cells = new int[numRoads, numDeliveries];

                for (int k = 0; k < numRoads; k++)
                {
                    parts = lines[lineIndex++].Split(' ');
                    for (int j = 0; j < numDeliveries; j++)
                    {
                        cells[k, j] = int.Parse(parts[j]);
                    }
                }

                results[i] = new Scenario(roads, deliveries);
            }

            return results;
        }
    }
}
