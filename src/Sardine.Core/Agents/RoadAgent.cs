using Rhino.Geometry;
using Sardine.Core.Data;
using System.Collections.Generic;

namespace Sardine.Core.Agents
{
    public class RoadAgent
    {
        public List<Road> CreatePerimeterRoads(Curve offsetBoundary, Settings settings)
        {
            // Create a loop road inside the offset boundary?
            // Or just define the logical road entity.
            
            var roads = new List<Road>();
            if (offsetBoundary == null) return roads;

            // The "Perimeter Road" might be the aisle serving the outer spots.
            // If outer spots are at the boundary, the aisle is behind them.
            
            // For now, let's just create a simplistic road representation
            // In a real implementation this would join segments, handle corners.
            
            var r = new Road(offsetBoundary, settings.PeripheralRoadWidth, RoadType.Perimeter);
            roads.Add(r);
            return roads;
        }
    }
}
