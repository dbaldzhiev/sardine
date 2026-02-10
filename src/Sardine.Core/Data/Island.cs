using Rhino.Geometry;

namespace Sardine.Core.Data
{
    public enum IslandType
    {
        Standard, // Buffer/Greenery
        Pedestrian, // Sidewalk
        Corner,
        Median
    }

    public class Island
    {
        public Curve Boundary { get; set; }
        public IslandType Type { get; set; }

        public Island(Curve boundary, IslandType type)
        {
            Boundary = boundary;
            Type = type;
        }
    }
}
