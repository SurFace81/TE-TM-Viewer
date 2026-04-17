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
            Calculate();
        }

        private void Calculate()
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

            if (useTimeSimulCheckBox.Checked)
                phase += double.Parse(stepTimeTextBox.Text);

            t = phase / omega;

            zMax = 0.05;
            x0 = a / 1.8;
            y0 = b / 1.8;
            z0 = 0.05;

            if (waveTypeCb.SelectedIndex == (int)WAVE_TYPES.TE)
                BuildTePlots();
            else
                BuildTmPlots();
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

        private StreamplotOptions CreatePlanarOptions(bool dense = false)
        {
            return new StreamplotOptions
            {
                DensityX = dense ? 1.6 : 1.3,
                DensityY = dense ? 1.6 : 1.3,
                MinLengthAxes = 0.06,
                MaxLengthAxes = 4.0,
                IntegrationDirection = StreamIntegrationDirection.Both,
                BrokenStreamlines = true,
                IntegrationMaxStepScale = 1.0,
                IntegrationMaxErrorScale = 1.0,
                NumArrows = 1
            };
        }

        private static StreamplotGrid SampleGrid(
            RectD bounds,
            int nx,
            int ny,
            Func<double, double, Vec2> field)
        {
            return StreamplotGrid.FromField(bounds, nx, ny, field);
        }

        private void SetPlot(
            StreamPlotControl control,
            string title,
            StreamplotGrid grid,
            StreamplotOptions options)
        {
            control.GridData = grid;
            control.PlotTitle = title;
            control.Options = options;
            control.RefreshPlot();
        }

        private void BuildXY_H_TM()
        {
            RectD bounds = new(0, a, 0, b);
            StreamplotGrid grid = SampleGrid(
                bounds,
                nx: 160,
                ny: 160,
                field: (x, y) =>
                {
                    double hx = omega * eps0 * ky / kc2 * E0 * Math.Sin(kx * x) * Math.Cos(ky * y) * Math.Sin(beta * z0 - omega * t); 
                    double hy = -omega * eps0 * kx / kc2 * E0 * Math.Cos(kx * x) * Math.Sin(ky * y) * Math.Sin(beta * z0 - omega * t);
                    return new Vec2(hx, hy);
                });

            SetPlot(YXspc, $"Y(X), H, z0 = {z0:F4}", grid, CreatePlanarOptions(dense: true));
        }

        private void BuildXZ_E_TM()
        {
            RectD bounds = new(0, a, 0, zMax);
            StreamplotGrid grid = SampleGrid(
                bounds,
                nx: 180,
                ny: 180,
                field: (x, z) =>
                {
                    double ez = E0 * Math.Sin(kx * x) * Math.Sin(ky * y0) * Math.Cos(beta * z - omega * t);
                    double ex = -beta * kx / kc2 * E0 * Math.Cos(kx * x) * Math.Sin(ky * y0) * Math.Sin(beta * z - omega * t);
                    return new Vec2(-ex, ez);
                });

            SetPlot(ZXspc, $"Z(X), E, y0 = {y0:F4}", grid, CreatePlanarOptions());
        }

        private void BuildYZ_E_TM()
        {
            RectD bounds = new(0, zMax, 0, b);
            StreamplotGrid grid = SampleGrid(
                bounds,
                nx: 180,
                ny: 180,
                field: (z, y) =>
                {
                    double ez = E0 * Math.Sin(kx * x0) * Math.Sin(ky * y) * Math.Cos(beta * z - omega * t);
                    double ey = -beta * ky / kc2 * E0 * Math.Sin(kx * x0) * Math.Cos(ky * y) * Math.Sin(beta * z - omega * t);
                    return new Vec2(ez, ey);
                });

            SetPlot(YZspc, $"Y(Z), E, x0 = {x0:F4}", grid, CreatePlanarOptions());
        }

        private void BuildXY_E_TE()
        {
            RectD bounds = new(0, a, 0, b);
            StreamplotGrid grid = SampleGrid(
                bounds,
                nx: 160,
                ny: 160,
                field: (x, y) =>
                {
                    double ex = -omega * mu0 * ky / kc2 * H0 * Math.Cos(kx * x) * Math.Sin(ky * y) * Math.Sin(beta * z0 - omega * t);
                    double ey = omega * mu0 * kx / kc2 * H0 * Math.Sin(kx * x) * Math.Cos(ky * y) * Math.Sin(beta * z0 - omega * t);
                    return new Vec2(ex, ey);
                });

            SetPlot(YXspc, $"Y(X), E, z0 = {z0:F4}", grid, CreatePlanarOptions(dense: true));
        }

        private void BuildXZ_H_TE()
        {
            RectD bounds = new(0, a, 0, zMax);
            StreamplotGrid grid = SampleGrid(
                bounds,
                nx: 180,
                ny: 180,
                field: (x, z) =>
                {
                    double hz = H0 * Math.Cos(kx * x) * Math.Cos(ky * y0) * Math.Cos(beta * z - omega * t);
                    double hx = -beta * kx / kc2 * H0 * Math.Sin(kx * x) * Math.Cos(ky * y0) * Math.Sin(beta * z - omega * t);
                    return new Vec2(-hx, hz);
                });

            SetPlot(ZXspc, $"Z(X), H, y0 = {y0:F4}", grid, CreatePlanarOptions());
        }

        private void BuildYZ_E_TE()
        {
            RectD bounds = new(0, zMax, 0, b);
            StreamplotGrid grid = SampleGrid(
                bounds,
                nx: 180,
                ny: 180,
                field: (z, y) =>
                {
                    double ez = 0.0;
                    double ey = omega * mu0 * kx / kc2 * H0 * Math.Sin(kx * x0) * Math.Cos(ky * y) * Math.Sin(beta * z - omega * t);
                    return new Vec2(ez, ey);
                });

            SetPlot(YZspc, $"Y(Z), E, x0 = {x0:F4}", grid, CreatePlanarOptions());
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

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        private void startBtn_Click(object sender, EventArgs e)
        {
            startBtn.Enabled = false;
            stopBtn.Enabled = true;
            timer.Interval = 1;
            timer.Tick += Timer_Elapsed;
            timer.Start();
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            startBtn.Enabled = true;
            stopBtn.Enabled = false;
            timer.Tick -= Timer_Elapsed;
            timer.Stop();
        }

        private void Timer_Elapsed(object? sender, EventArgs e)
        {
            Calculate();
        }

        private void useTimeSimulCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            startBtn.Enabled = true;
            stopBtn.Enabled = false;
            timer.Tick -= Timer_Elapsed;
            timer.Stop();
            timeGroupBox.Visible = useTimeSimulCheckBox.Checked;
        }
    }
}
