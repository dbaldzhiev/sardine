using Rhino.Geometry;

namespace Sardine.Core.Data
{
    public enum RoadType
    {
        Perimeter,
        Axial,
        Aisle,
        Access
    }

    public class Road
    {
        public Curve Centerline { get; set; }
        public double Width { get; set; }
        public RoadType Type { get; set; }
        public Curve LeftEdge { get; set; }
        public Curve RightEdge { get; set; }
        
        // Optional: Reference to spots served by this road
        
        public Road(Curve centerline, double width, RoadType type)
        {
            Centerline = centerline;
            Width = width;
            Type = type;
        }
    }
}
