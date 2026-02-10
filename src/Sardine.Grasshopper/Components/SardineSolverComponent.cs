using Grasshopper.Kernel;
using Rhino.Geometry;
using Sardine.Core.Agents;
using Sardine.Core.Data;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Sardine.Grasshopper.Components
{
    public class SardineSolverComponent : GH_Component
    {
        private SolverAgent _solver;

        public SardineSolverComponent()
          : base("Sardine Solver", "SardineSolve",
              "Generates parking layout valid for single closed boundary",
              "Sardine", "Solver")
        {
            _solver = new SolverAgent();
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Boundary", "B", "Closed planar boundary curve", GH_ParamAccess.item);
            pManager.AddGenericParameter("Settings", "S", "Parking Settings", GH_ParamAccess.item);
            pManager.AddPointParameter("AccessPoints", "AP", "Access points", GH_ParamAccess.list);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ParkingLot", "Lot", "Resulting Parking Lot Data", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve boundary = null;
            Settings settings = null;
            List<Point3d> rawPoints = new List<Point3d>();

            if (!DA.GetData(0, ref boundary)) return;
            if (!DA.GetData(1, ref settings)) return;
            DA.GetDataList(2, rawPoints);

            // Convert raw points to AccessPoint objects
            var aps = new List<AccessPoint>();
            foreach (var p in rawPoints)
            {
                aps.Add(new AccessPoint(p, settings.PeripheralRoadWidth));
            }

            // Run Solver
            var result = _solver.Solve(boundary, aps, new List<Curve>(), settings);

            DA.SetData(0, result);
        }

        protected override Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("c8f7e9d2-2e3b-4c5d-0f1a-3b6c8d9e0f1a");
    }
}
