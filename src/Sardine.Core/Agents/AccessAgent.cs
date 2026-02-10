using Rhino.Geometry;
using Sardine.Core.Data;
using System.Collections.Generic;
using System.Linq;

namespace Sardine.Core.Agents
{
    public class AccessAgent
    {
        public void ValidateAndSnap(List<AccessPoint> points, Curve boundary)
        {
            if (points == null || boundary == null) return;

            foreach (var ap in points)
            {
                // Snap to closest point on boundary
                double t;
                if (boundary.ClosestPoint(ap.Location, out t))
                {
                    ap.Location = boundary.PointAt(t);
                    
                    // Determine direction (inward normal)
                    // For counter-clockwise closed curve, tangent X Z-up = inward normal
                    Vector3d tangent = boundary.TangentAt(t);
                    Vector3d normal = Vector3d.CrossProduct(tangent, Vector3d.ZAxis);
                    ap.Direction = normal;
                }
            }
        }
    }
}
