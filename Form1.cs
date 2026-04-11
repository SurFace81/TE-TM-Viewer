using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TETMViewer
{
    public partial class Form : System.Windows.Forms.Form
    {
        const double mu0 = 4 * Math.PI * 1e-7;
        const double eps0 = 8.854e-12;
        enum WAVE_TYPES { TE, TM };

        double a, b;
        double kx, ky, kc2;
        double H0, E0;
        double beta, phase = Math.PI / 2;
        double zMax, x0, y0, z0;
        double t = 0;
        int m, n;

        double f, fc, omega;

        public Form()
        {
            InitializeComponent();
            waveTypeCb.DataSource = new List<string> { "TE", "TM" };
            waveTypeCb.SelectedIndex = (int)WAVE_TYPES.TE;
        }

        private void calcBtn_Click(object sender, EventArgs e)
        {
            a = double.Parse(numATextBox.Text) * 1e-2;
            b = double.Parse(numBTextBox.Text) * 1e-2;
            m = int.Parse(numMTextBox.Text);
            n = int.Parse(numNTextBox.Text);
            H0 = double.Parse(ampHTextBox.Text);
            E0 = double.Parse(ampETextBox.Text);
            f = double.Parse(waveFreqTextBox.Text) * 1e6;

            kx = m * Math.PI / a;
            ky = n * Math.PI / b;
            kc2 = kx * kx + ky * ky;

            fc = 1 / (2 * Math.PI * Math.Sqrt(mu0 * eps0)) * Math.Sqrt(kc2);
            crWaveFreqTextBox.Text = (fc / 1e6).ToString("F2");

            omega = 2 * Math.PI * f;
            double beta2 = omega * omega * mu0 * eps0 - kc2;
            if (beta2 < 0)
            {
                MessageBox.Show("Мода не распространяется!");
                return;
            }
            beta = Math.Sqrt(beta2);

            if (waveTypeCb.SelectedIndex == (int)WAVE_TYPES.TE && (m == 0 && n == 0))
            {
                MessageBox.Show("Для TЕ-моды нельзя, чтобы m = n = 0.");
                return;
            }
            if (waveTypeCb.SelectedIndex == (int)WAVE_TYPES.TM && (m == 0 || n == 0))
            {
                MessageBox.Show("Для TM-моды нужно m >= 1 и n >= 1.");
                return;
            }

            t = phase / omega;

            zMax = 2 * Math.PI / beta;
            x0 = a / 1.8;
            y0 = b / 1.8;
            z0 = (phase + Math.PI / 2) / beta;

            if (waveTypeCb.SelectedIndex == (int)WAVE_TYPES.TE)
            {
                BuildTePlots();
            }
            else
            {
                BuildTmPlots();
            }
        }

        private void BuildTePlots()
        {
            BuildXY_E_TE();
            BuildXZ_H_TE();
            BuildYZ_E_TE();
        }

        private void BuildTmPlots()
        {
            BuildXY_H_TM();
            BuildXZ_E_TM();
            BuildYZ_E_TM();
        }

        private void BuildXY_H_TM()
        {
            YXspc.Field = (x, y) =>
            {
                double hx = omega * eps0 * ky / kc2 * E0 * Math.Sin(kx * x) * Math.Cos(ky * y) * Math.Sin(beta * z0 - omega * t);
                double hy = -omega * eps0 * kx / kc2 * E0 * Math.Cos(kx * x) * Math.Sin(ky * y) * Math.Sin(beta * z0 - omega * t);
                return new Vec2(hx, hy);
            };

            YXspc.Bounds = new RectD(0, a, 0, b);
            YXspc.PlotTitle = $"Y(X), H, z0 = {z0:F4}";
            YXspc.Options = new StreamplotOptions
            {
                SeedColumns = 40,
                SeedRows = 20,
                OccupancyColumns = 100,
                OccupancyRows = 50,
                StepSize = 0.0002,
                MaxStepsPerDirection = 3000,
                MinLength = 0.001,
                NormalizeField = true
            };

            YXspc.RefreshPlot();
        }

        private void BuildXZ_E_TM()
        {
            ZXspc.Field = (x, z) =>
            {
                double ez = E0 * Math.Sin(kx * x) * Math.Sin(ky * y0) * Math.Cos(beta * z - omega * t);
                double ex = -beta * kx / kc2 * E0 * Math.Cos(kx * x) * Math.Sin(ky * y0) * Math.Sin(beta * z - omega * t);

                return new Vec2(-ex, ez);
            };

            ZXspc.Bounds = new RectD(0, a, 0, zMax);
            ZXspc.PlotTitle = $"Z(X), E, y0 = {y0:F4}";
            ZXspc.Options = new StreamplotOptions
            {
                SeedColumns = 50,
                SeedRows = 28,
                OccupancyColumns = 120,
                OccupancyRows = 60,
                StepSize = 0.0003,
                MaxStepsPerDirection = 4000,
                MinLength = 0.0015,
                NormalizeField = false
            };

            ZXspc.RefreshPlot();
        }

        private void BuildYZ_E_TM()
        {
            YZspc.Field = (z, y) =>
            {
                double ez = E0 * Math.Sin(kx * x0) * Math.Sin(ky * y) * Math.Cos(beta * z - omega * t);
                double ey = -beta * ky / kc2 * E0 * Math.Sin(kx * x0) * Math.Cos(ky * y) * Math.Sin(beta * z - omega * t);

                return new Vec2(ez, ey);
            };

            YZspc.Bounds = new RectD(0, zMax, 0, b);
            YZspc.PlotTitle = $"Y(Z), E, x0 = {x0:F4}";
            YZspc.Options = new StreamplotOptions
            {
                SeedColumns = 50,
                SeedRows = 24,
                OccupancyColumns = 120,
                OccupancyRows = 60,
                StepSize = 0.0003,
                MaxStepsPerDirection = 4000,
                MinLength = 0.0015,
                NormalizeField = true
            };

            YZspc.RefreshPlot();
        }

        private void BuildXY_E_TE()
        {
            YXspc.Field = (x, y) =>
            {
                double ex = -omega * mu0 * ky / kc2 * H0 * Math.Cos(kx * x) * Math.Sin(ky * y) * Math.Sin(beta * z0 - omega * t);
                double ey = omega * mu0 * kx / kc2 * H0 * Math.Sin(kx * x) * Math.Cos(ky * y) * Math.Sin(beta * z0 - omega * t);
                return new Vec2(ex, ey);
            };

            YXspc.Bounds = new RectD(0, a, 0, b);
            YXspc.PlotTitle = $"Y(X), E, z0 = {z0:F4}";
            YXspc.Options = new StreamplotOptions
            {
                SeedColumns = 40,
                SeedRows = 20,
                OccupancyColumns = 100,
                OccupancyRows = 50,
                StepSize = 0.0002,
                MaxStepsPerDirection = 3000,
                MinLength = 0.001,
                NormalizeField = true
            };

            YXspc.RefreshPlot();
        }

        private void BuildXZ_H_TE()
        {
            ZXspc.Field = (x, z) =>
            {
                double hz = H0 * Math.Cos(kx * x) * Math.Cos(ky * y0) * Math.Cos(beta * z - omega * t);
                double hx = -beta * kx / kc2 * H0 * Math.Sin(kx * x) * Math.Cos(ky * y0) * Math.Sin(beta * z - omega * t);

                return new Vec2(-hx, hz);
            };

            ZXspc.Bounds = new RectD(0, a, 0, zMax);
            ZXspc.PlotTitle = $"Z(X), H, y0 = {y0:F4}";
            ZXspc.Options = new StreamplotOptions
            {
                SeedColumns = 50,
                SeedRows = 28,
                OccupancyColumns = 120,
                OccupancyRows = 60,
                StepSize = 0.0003,
                MaxStepsPerDirection = 4000,
                MinLength = 0.0015,
                NormalizeField = false
            };

            ZXspc.RefreshPlot();
        }

        private void BuildYZ_E_TE()
        {
            YZspc.Field = (z, y) =>
            {
                double ez = 0.0;
                double ey = omega * mu0 * kx / kc2 * H0 * Math.Sin(kx * x0) * Math.Cos(ky * y) * Math.Sin(beta * z - omega * t);

                return new Vec2(ez, ey);
            };

            YZspc.Bounds = new RectD(0, zMax, 0, b);
            YZspc.PlotTitle = $"Y(Z), E, x0 = {x0:F4}";
            YZspc.Options = new StreamplotOptions
            {
                SeedColumns = 50,
                SeedRows = 24,
                OccupancyColumns = 120,
                OccupancyRows = 60,
                StepSize = 0.0003,
                MaxStepsPerDirection = 4000,
                MinLength = 0.0015,
                NormalizeField = true
            };

            YZspc.RefreshPlot();
        }

        private void numABMNTextBox_TextChanged(object sender, EventArgs e)
        {
            a = double.Parse(numATextBox.Text) * 1e-2;
            b = double.Parse(numBTextBox.Text) * 1e-2;
            m = int.Parse(numMTextBox.Text);
            n = int.Parse(numNTextBox.Text);

            kx = m * Math.PI / a;
            ky = n * Math.PI / b;
            kc2 = kx * kx + ky * ky;

            crWaveFreqTextBox.Text = (1 / (2 * Math.PI * Math.Sqrt(mu0 * eps0)) * Math.Sqrt(kc2) / 1e6).ToString("F2");
        }
    }
}