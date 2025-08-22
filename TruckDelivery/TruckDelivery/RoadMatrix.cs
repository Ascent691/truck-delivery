using TruckDelivery.Infrastructure;

namespace TruckDelivery;

public class RoadMatrix(long vertices)
{
    private readonly Road?[,] _matrix = new Road[vertices, vertices];
    private readonly long _vertices = vertices;

    public Road? At(long v1, long v2)
    {
        return _matrix[v1, v2];
    }

    public void AddRoad(long v1, long v2, Road road)
    {
        _matrix[v1, v2] = road;
        _matrix[v2, v1] = road;
    }

    public IEnumerable<long> GetConnected(long node)
    {
        for (var i = 0L; i < _vertices; i++)
        {
            if (_matrix[node, i] != null)
            {
                yield return i;
            }
        }
    }
}