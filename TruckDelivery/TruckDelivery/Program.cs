using TruckDelivery.Infrastructure;

namespace TruckDelivery
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var scenarios = new ScenarioParser().Parse(File.ReadAllLines("simple.in"));
            var expectedAnswers = new ScenarioAnswerParser().Parse(File.ReadAllLines("simple.ans"));
            
            if (scenarios.Length != expectedAnswers.Length)
            {
                Console.WriteLine("We have a different number of answers compared to questions, are you using the right combination of scenarios and answers files?");
                return;
            }

            var failedScenarios = 0;

            for (int i = 0; i < scenarios.Length; i++)
            {
                var scenario = scenarios[i];
                var expectedAnswer = expectedAnswers[i];
                var computedAnswer = DetermineAnswer(scenario);

                if (!expectedAnswer.IsMatch(computedAnswer))
                {
                    failedScenarios++;
                    Console.WriteLine($"Case #{i + 1}: No Match, expected {string.Join(" ", expectedAnswer.Values)} but computed {string.Join(" ", computedAnswer.Values)}");
                }
                else
                {
                    Console.WriteLine($"Case #{i + 1}: {string.Join(" ", computedAnswer.Values)}");
                }
            }

            Console.WriteLine($"Total Failed Scenarios: {failedScenarios}");
        }

        private static ScenarioAnswer DetermineAnswer(Scenario scenario)
        {
            throw new NotImplementedException("Please implement me, remember to convert the toll charges to greatest common divisors (Use MathHelper unless your brave) :)");
        }
    }
}
