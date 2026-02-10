using Rhino.Geometry;

namespace Sardine.Core.Data
{
    public enum SpotType
    {
        Standard,
        Handicap,
        Elderly,
        Bike,
        EV // Potential future extensibility
    }

    public class Spot
    {
        public int Index { get; set; }
        public SpotType Type { get; set; }
        public Curve Boundary { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double Angle { get; set; }
        public Plane BasePlane { get; set; }

        public Spot() { }
        
        public Spot(int index, SpotType type, Curve boundary)
        {
            Index = index;
            Type = type;
            Boundary = boundary;
        }
    }
}
