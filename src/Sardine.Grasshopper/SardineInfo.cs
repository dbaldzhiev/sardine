using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace Sardine.Grasshopper
{
    public class SardineInfo : GH_AssemblyInfo
    {
        public override string Name => "Sardine";
        public override Bitmap Icon => null;
        public override string Description => "Parking Solver for Rhino 8";
        public override Guid Id => new Guid("a6d7f9c2-0e1b-4b1a-8c3d-2f5e7a9b0c1d");
        public override string AuthorName => "Antigravity";
        public override string AuthorContact => "deepmind";
    }
}
