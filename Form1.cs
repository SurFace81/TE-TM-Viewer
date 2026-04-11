using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TETMViewer
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
            waveTypeCb.DataSource = new List<string> { "TE", "TM" };
        }

        private void calcBtn_Click(object sender, EventArgs e)
        {
            BuildTe10Plots();
        }

        private void BuildTe10Plots()
        {
            double a = 1.0;
            double b = 0.5;
            double beta = 8.0;
            double phase = 0.0;

            double kx = Math.PI / a;
            double zMax = 2.0 * Math.PI / beta;
            double x0 = a / 2.0;

            BuildXY_E(a, b, kx, phase);
            BuildXZ_H(a, kx, beta, phase, zMax);
            BuildYZ_E(b, kx, beta, phase, zMax, x0);
        }

        private void BuildXY_E(double a, double b, double kx, double phase)
        {
            YXspc.Field = (x, y) =>
            {
                double ex = 0.0;
                double ey = Math.Sin(kx * x) * Math.Cos(phase);
                return new Vec2(ex, ey);
            };

            YXspc.Bounds = new RectD(0, a, 0, b);
            YXspc.Options = new StreamplotOptions
            {
                SeedColumns = 32,
                SeedRows = 18,
                OccupancyColumns = 90,
                OccupancyRows = 45,
                StepSize = 0.008,
                MaxStepsPerDirection = 1200,
                MinLength = 0.03,
                NormalizeField = false
            };

            YXspc.RefreshPlot();
        }

        private void BuildXZ_H(double a, double kx, double beta, double phase, double zMax)
        {
            ZXspc.Field = (x, z) =>
            {
                double oldZ = z;
                double oldX = a - x;

                double hz = Math.Cos(kx * oldX) * Math.Cos(beta * oldZ - phase);
                double hx = Math.Sin(kx * oldX) * Math.Sin(beta * oldZ - phase);

                return new Vec2(-hx, hz);
            };

            ZXspc.Bounds = new RectD(0, a, 0, zMax);
            ZXspc.Options = new StreamplotOptions
            {
                SeedColumns = 40,
                SeedRows = 24,
                OccupancyColumns = 100,
                OccupancyRows = 50,
                StepSize = 0.01,
                MaxStepsPerDirection = 1400,
                MinLength = 0.03,
                NormalizeField = false
            };

            ZXspc.RefreshPlot();
        }

        private void BuildYZ_E(double b, double kx, double beta, double phase, double zMax, double x0)
        {
            YZspc.Field = (z, y) =>
            {
                double ez = 0.0;
                double ey = Math.Sin(kx * x0) * Math.Cos(beta * z - phase);

                double u = ez;
                double v = ey;

                return new Vec2(u, v);
            };

            YZspc.Bounds = new RectD(0, zMax, 0, b);
            YZspc.Options = new StreamplotOptions
            {
                SeedColumns = 40,
                SeedRows = 20,
                OccupancyColumns = 100,
                OccupancyRows = 45,
                StepSize = 0.01,
                MaxStepsPerDirection = 1400,
                MinLength = 0.03,
                NormalizeField = false
            };

            YZspc.RefreshPlot();
        }
    }
}