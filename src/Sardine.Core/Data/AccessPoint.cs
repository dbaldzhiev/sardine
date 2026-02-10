using Rhino.Geometry;

namespace Sardine.Core.Data
{
    public class AccessPoint
    {
        public Point3d Location { get; set; }
        public double Width { get; set; }
        // Vector pointing into the lot, or tangent to boundary
        public Vector3d Direction { get; set; } 

        public AccessPoint(Point3d location, double width)
        {
            Location = location;
            Width = width;
            Direction = Vector3d.Unset;
        }
    }
}
