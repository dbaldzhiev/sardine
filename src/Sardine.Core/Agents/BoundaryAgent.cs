using Rhino.Geometry;
using Sardine.Core.Data;
using System.Collections.Generic;

namespace Sardine.Core.Agents
{
    public class BoundaryAgent
    {
        public bool Validate(Curve boundary, out string message)
        {
            if (boundary == null)
            {
                message = "Boundary is null.";
                return false;
            }

            if (!boundary.IsClosed)
            {
                message = "Boundary must be a closed curve.";
                return false;
            }

            if (!boundary.IsPlanar())
            {
                message = "Boundary must be planar.";
                return false;
            }

            message = "Valid.";
            return true;
        }

        public Curve OffsetBoundary(Curve boundary, double distance)
        {
            // Negative distance typically offsets inwards for counter-clockwise closed curves in Rhino
            // But we need to be careful about orientation. 
            // For now assuming the input is properly oriented or we handle it.
            
            // RhinoCommon Offset
            Curve[] offsets = boundary.Offset(Plane.WorldXY, -distance, 0.01, CurveOffsetCornerStyle.Sharp);
            
            if (offsets != null && offsets.Length > 0)
            {
                // Return the largest closed curve if multiple result (e.g. self intersection resolution)
                // Simplified logic:
                return offsets[0];
            }
            return null;
        }

        public List<Curve> GetUsableEdges(Curve boundary)
        {
            var edges = new List<Curve>();
            if (boundary == null) return edges;

            // Explode polyline
            if (boundary.TryGetPolyline(out Polyline pl))
            {
                foreach (var segment in pl.GetSegments())
                {
                    edges.Add(segment.ToNurbsCurve());
                }
            }
            else
            {
                // Fallback for generic curves
                var segments = boundary.DuplicateSegments();
                if (segments != null) edges.AddRange(segments);
            }
            return edges;
        }
    }
}
