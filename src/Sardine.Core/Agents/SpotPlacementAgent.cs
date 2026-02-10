using Rhino.Geometry;
using Rhino;
using Sardine.Core.Data;
using System.Collections.Generic;
using System;

namespace Sardine.Core.Agents
{
    public class SpotPlacementAgent
    {
        public List<Spot> GenerateSpotsAlongCurve(Curve guideCurve, Settings settings, bool isOuter, int startIndex, out int nextIndex)
        {
            var spots = new List<Spot>();
            nextIndex = startIndex;

            if (guideCurve == null) return spots;

            // Simplified logic: Divide curve by spot width
            // In reality, this needs to account for angle, orientation (nose-in/out), etc.

            double spacing = settings.SpotWidth; // Assuming 90 degree for now
            if (settings.SpotAngle != 90.0)
            {
                // effective width changes if angled
                double rad = RhinoMath.ToRadians(settings.SpotAngle);
                spacing = settings.SpotWidth / Math.Sin(rad); 
            }

            // Divide by length
            // We should use DivideByLength to get parameters
            double[] tVals = guideCurve.DivideByLength(spacing, true);
            
            if (tVals == null) return spots;

            foreach (double t in tVals)
            {
                var pt = guideCurve.PointAt(t);
                var tangent = guideCurve.TangentAt(t);
                
                // Construct spot geometry
                // Plane
                // If nose-in to curve, X axis is tangent, Y axis is perp (inward/outward)
                // We need to determine "inward" side. 
                // For a closed clockwise loop, "in" is to the right. 
                
                // Let's create a placeholder rectangle for now
                var plane = new Plane(pt, tangent, Vector3d.ZAxis);
                
                // If 90 deg, spot extends along Y. 
                // We need to shift it so 'pt' is the corner or center of the entry.
                
                // Simplified visualization geometry:
                var rect = new Rectangle3d(plane, spacing, settings.SpotLength);
                var spotCurve = rect.ToNurbsCurve();

                spots.Add(new Spot(nextIndex++, SpotType.Standard, spotCurve));
            }

            return spots;
        }

        public void FillRegion(Curve boundary, Settings settings, ParkingLot lot)
        {
            // This is the complex interior filling logic.
            // 1. Define rows based on boundary bounds or axial alignment
            // 2. Intersect rows with boundary
            // 3. Place spots along row lines
            // 4. Cull spots that clash with boundary
        }
    }
}
