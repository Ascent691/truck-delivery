namespace TruckDelivery.Infrastructure
{
    public class ScenarioAnswer
    {
        private readonly long[] _values;

        public long[] Values { get { return _values; } } // yeah I know...

        public ScenarioAnswer(long[] values)
        {
            _values = values;
        }

        public bool IsMatch(ScenarioAnswer other)
        {
            if (other == null) return false;
            if (other._values.Length != _values.Length) return false;

            for (int i = 0; i < _values.Length; i++)
            {
                if (_values[i] != other._values[i])
                    return false;
            }

            return true;
        }
    }
}
