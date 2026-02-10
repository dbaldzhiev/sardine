using System.Collections.Generic;
using Rhino.Geometry;

namespace Sardine.Core.Data
{
    public class ParkingLot
    {
        public Curve Boundary { get; set; }
        public List<Spot> Spots { get; set; }
        public List<Road> Roads { get; set; }
        public List<Island> Islands { get; set; }
        public List<AccessPoint> AccessPoints { get; set; }

        public ParkingLot()
        {
            Spots = new List<Spot>();
            Roads = new List<Road>();
            Islands = new List<Island>();
            AccessPoints = new List<AccessPoint>();
        }
    }
}
