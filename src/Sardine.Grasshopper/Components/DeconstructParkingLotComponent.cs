using Grasshopper.Kernel;
using Rhino.Geometry;
using Sardine.Core.Data;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Sardine.Grasshopper.Components
{
    public class DeconstructParkingLotComponent : GH_Component
    {
        public DeconstructParkingLotComponent()
          : base("Deconstruct Lot", "DeconLot",
              "Extract geometry from ParkingLot result",
              "Sardine", "Info")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("ParkingLot", "Lot", "Parking Lot Data", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Spots", "S", "Spot boundaries", GH_ParamAccess.list);
            pManager.AddCurveParameter("Roads", "R", "Road centerlines or outlines", GH_ParamAccess.list);
            pManager.AddCurveParameter("Boundary", "B", "Lot boundary", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ParkingLot lot = null;
            if (!DA.GetData(0, ref lot)) return;

            if (lot == null) return;

            var spotCurves = new List<Curve>();
            foreach(var s in lot.Spots)
            {
                if (s.Boundary != null) spotCurves.Add(s.Boundary);
            }

            var roadCurves = new List<Curve>();
            foreach(var r in lot.Roads)
            {
                // Return centerline for now, or maybe create outline
                if (r.Centerline != null) roadCurves.Add(r.Centerline);
            }

            DA.SetDataList(0, spotCurves);
            DA.SetDataList(1, roadCurves);
            DA.SetData(2, lot.Boundary);
        }

        protected override Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("d9e0f1a2-3b4c-5d6e-1f2a-4b5c6d7e8f9a");
    }
}
