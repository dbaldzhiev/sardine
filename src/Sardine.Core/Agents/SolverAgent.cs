using Rhino.Geometry;
using Sardine.Core.Data;
using System.Collections.Generic;
using System.Linq;

namespace Sardine.Core.Agents
{
    public class SolverAgent
    {
        private BoundaryAgent _boundaryAgent;
        private RoadAgent _roadAgent;
        private SpotPlacementAgent _spotAgent;
        private AccessAgent _accessAgent;

        public SolverAgent()
        {
            _boundaryAgent = new BoundaryAgent();
            _roadAgent = new RoadAgent();
            _spotAgent = new SpotPlacementAgent();
            _accessAgent = new AccessAgent();
        }

        public ParkingLot Solve(Curve boundary, List<AccessPoint> accessPoints, List<Curve> axialLines, Settings settings)
        {
            var lot = new ParkingLot();
            
            // 1. Validate
            if (!_boundaryAgent.Validate(boundary, out string msg))
            {
                // Return empty lot with error logic handled by caller or separate status
                return lot;
            }
            lot.Boundary = boundary;
            
            // 2. Process Access
            if (accessPoints != null)
            {
                _accessAgent.ValidateAndSnap(accessPoints, boundary);
                lot.AccessPoints.AddRange(accessPoints);
            }

            // 3. Pre-process Boundary (Skirt)
            Curve skirtCurve = _boundaryAgent.OffsetBoundary(boundary, settings.SkirtOffset);
            if (skirtCurve == null) skirtCurve = boundary; // Fallback

            // 4. Generate Roads (Simplified)
            // Create perimeter road behind the skirt
            // The skirt is effectively the edge of driveable area if we simply offset.
            // Let's assume input boundary -> parking -> aisle -> inner parking
            // If SkirtOffset is small (curb), spots start there.
            
            // For this version 1.0, let's just create a perimeter road representation for visualization
            var roads = _roadAgent.CreatePerimeterRoads(skirtCurve, settings);
            lot.Roads.AddRange(roads);

            // 5. Place Outer Spots
            // Use the skirt curve as the guide for outer spots.
            // We need to exclude areas near access points.
            
            int spotIndex = 0;
            
            // Naive implementations: place all around
            // Real implementation: cull near access points
            
            // Determine usable segments (e.g. split by access points)
            // For now, just generate on the whole closed loop
            var spots = _spotAgent.GenerateSpotsAlongCurve(skirtCurve, settings, true, spotIndex, out spotIndex);
            
            lot.Spots.AddRange(spots);

            return lot;
        }
    }
}
