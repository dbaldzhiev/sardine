using System;

namespace Sardine.Core.Data
{
    public class Settings
    {
        // Spot Dimensions (cm)
        public double SpotWidth { get; set; } = 250.0;
        public double SpotLength { get; set; } = 500.0;
        public double SpotAngle { get; set; } = 90.0; // Degrees

        // Road Dimensions (cm)
        public double PeripheralRoadWidth { get; set; } = 600.0;
        public double AxialRoadWidth { get; set; } = 600.0;
        public double AisleWidth { get; set; } = 600.0;

        // General
        public double SkirtOffset { get; set; } = 50.0;
        public double IslandRadius { get; set; } = 50.0;

        // Modes
        public int PerimeterMode { get; set; } = 0; // 0=None, 1=OneSide, 2=Double ..

        public Settings() { }
    }
}
