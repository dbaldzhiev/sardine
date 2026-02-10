using Grasshopper.Kernel;
using Sardine.Core.Data;
using System;
using System.Drawing;

namespace Sardine.Grasshopper.Components
{
    public class ParkingParametersComponent : GH_Component
    {
        public ParkingParametersComponent()
          : base("Parking Parameters", "SardineParams",
              "Define settings for parking solver",
              "Sardine", "Settings")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Spot Width", "W", "Width of parking spot (cm)", GH_ParamAccess.item, 250.0);
            pManager.AddNumberParameter("Spot Length", "L", "Length of parking spot (cm)", GH_ParamAccess.item, 500.0);
            pManager.AddNumberParameter("Angle", "A", "Parking angle (degrees)", GH_ParamAccess.item, 90.0);
            pManager.AddNumberParameter("Skirt", "S", "Boundary skirt offset (cm)", GH_ParamAccess.item, 50.0);
            pManager.AddNumberParameter("Road Width", "RW", "Width of perimeter road (cm)", GH_ParamAccess.item, 600.0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Settings", "Set", "Parking Settings Object", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double w = 250.0;
            double l = 500.0;
            double a = 90.0;
            double s = 50.0;
            double rw = 600.0;

            if (!DA.GetData(0, ref w)) return;
            if (!DA.GetData(1, ref l)) return;
            if (!DA.GetData(2, ref a)) return;
            if (!DA.GetData(3, ref s)) return;
            if (!DA.GetData(4, ref rw)) return;

            var settings = new Settings
            {
                SpotWidth = w,
                SpotLength = l,
                SpotAngle = a,
                SkirtOffset = s,
                PeripheralRoadWidth = rw,
                AisleWidth = rw, // reuse for now
                AxialRoadWidth = rw
            };

            DA.SetData(0, settings);
        }

        protected override Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("b7e6f8c1-1d2a-3c4b-9e0f-2a5d7c8b9e0f");
    }
}
