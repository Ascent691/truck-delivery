using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckDelivery.Infrastructure
{
    public class ScenarioAnswerParser
    {
        public ScenarioAnswerParser() { }

        public ScenarioAnswer[] Parse(string[] lines)
        {
            var result = new List<ScenarioAnswer>();

            foreach (string line in lines)
            {
                int lineIndex = line.IndexOf(":") + 1;
                var answers = line.Substring(lineIndex).Trim().Split(" ");
                result.Add(new ScenarioAnswer(answers.Select(answer => int.Parse(answer)).ToArray()));
            }

            return result.ToArray();
        }
    }
}
