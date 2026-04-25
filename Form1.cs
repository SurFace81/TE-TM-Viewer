using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TETMViewer
{
    public partial class Form : System.Windows.Forms.Form
    {
        const double mu0 = 4 * Math.PI * 1e-7;
        const double eps0 = 8.854e-12;
        const double zMax = 0.05;
        enum WAVE_TYPES { TE, TM };

        double a, b;
        double kx, ky, kc2;
        double beta, phase = Math.PI / 2;
        double x0, y0, z0;
        double t = 0;
        int m, n;

        double f, fc, omega;

        public Form()
        {
            InitializeComponent();
            timer.Tick += Timer_Elapsed;
            waveTypeCb.DataSource = new List<string> { "TE", "TM" };
            waveTypeCb.SelectedIndex = (int)WAVE_TYPES.TE;

            a = double.Parse(numATextBox.Text) * 1e-2;
            b = double.Parse(numBTextBox.Text) * 1e-2;

            x0Slider.Minimum = 0;
            x0Slider.Maximum = a / 1e-2;
            x0Slider.Value = a / 1e-2 / 2;
            
            y0Slider.Minimum = 0;
            y0Slider.Maximum = b / 1e-2;
            y0Slider.Value = b / 1e-2 / 2;

            z0Slider.Minimum = 0;
            z0Slider.Maximum = zMax * 100;
            z0Slider.Value = zMax * 100 / 2;
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
            f = double.Parse(waveFreqTextBox.Text) * 1e6;
            phase = double.Parse(phaseTextBox.Text);

            kx = m * Math.PI / a;
            ky = n * Math.PI / b;
            kc2 = kx * kx + ky * ky;

            fc = 1 / (2 * Math.PI * Math.Sqrt(mu0 * eps0)) * Math.Sqrt(kc2);
            crWaveFreqTextBox.Text = (fc / 1e6).ToString("F2");

            omega = 2 * Math.PI * f;
            double beta2 = omega * omega * mu0 * eps0 - kc2;
            if (beta2 < 0)
            {
                StopRefreshTimer();
                MessageBox.Show("Мода не распространяется!");
                return;
            }
            beta = Math.Sqrt(beta2);

            if (waveTypeCb.SelectedIndex == (int)WAVE_TYPES.TE && (m == 0 && n == 0))
            {
                StopRefreshTimer();
                MessageBox.Show("Для TЕ-моды нельзя, чтобы m = n = 0.");
                return;
            }

            if (waveTypeCb.SelectedIndex == (int)WAVE_TYPES.TM && (m == 0 || n == 0))
            {
                StopRefreshTimer();
                MessageBox.Show("Для TM-моды нужно m >= 1 и n >= 1.");
                return;
            }

            if (useTimeSimulCheckBox.Checked)
            {
                phase += double.Parse(stepTimeTextBox.Text);
                phaseTextBox.Text = phase.ToString("F2");
            }

            t = phase / omega;

            x0 = x0Slider.Value * 1e-2;
            y0 = y0Slider.Value * 1e-2;
            z0 = z0Slider.Value * 1e-2;

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

        private StreamplotOptions CreatePlanarOptions()
        {
            return new StreamplotOptions
            {
                DensityX = qualitySlider.Value,
                DensityY = qualitySlider.Value,
                MinLengthAxes = 0.06,
                MaxLengthAxes = 4.0,
                IntegrationDirection = StreamIntegrationDirection.Both,
                BrokenStreamlines = false,
                IntegrationMaxStepScale = 1.0,
                IntegrationMaxErrorScale = 1.0,
                NumArrows = 1
            };
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
            StreamplotGrid grid = StreamplotGrid.FromField(
                bounds,
                nx: 160,
                ny: 160,
                field: (x, y) =>
                {
                    double hx = omega * eps0 * ky / kc2 * Math.Sin(kx * x) * Math.Cos(ky * y) * Math.Sin(beta * z0 - omega * t);
                    double hy = -omega * eps0 * kx / kc2 * Math.Cos(kx * x) * Math.Sin(ky * y) * Math.Sin(beta * z0 - omega * t);
                    return new Vec2(hx, hy);
                });

            SetPlot(YXspc, $"Y(X), H, z0 = {z0:F4}", grid, CreatePlanarOptions());
        }

        private void BuildXZ_E_TM()
        {
            RectD bounds = new(0, a, 0, zMax);
            StreamplotGrid grid = StreamplotGrid.FromField(
                bounds,
                nx: 180,
                ny: 180,
                field: (x, z) =>
                {
                    double ez = Math.Sin(kx * x) * Math.Sin(ky * y0) * Math.Cos(beta * z - omega * t);
                    double ex = -beta * kx / kc2 * Math.Cos(kx * x) * Math.Sin(ky * y0) * Math.Sin(beta * z - omega * t);
                    return new Vec2(-ex, ez);
                });

            SetPlot(ZXspc, $"Z(X), E, y0 = {y0:F4}", grid, CreatePlanarOptions());
        }

        private void BuildYZ_E_TM()
        {
            RectD bounds = new(0, zMax, 0, b);
            StreamplotGrid grid = StreamplotGrid.FromField(
                bounds,
                nx: 180,
                ny: 180,
                field: (z, y) =>
                {
                    double ez = Math.Sin(kx * x0) * Math.Sin(ky * y) * Math.Cos(beta * z - omega * t);
                    double ey = -beta * ky / kc2 * Math.Sin(kx * x0) * Math.Cos(ky * y) * Math.Sin(beta * z - omega * t);
                    return new Vec2(ez, ey);
                });

            SetPlot(YZspc, $"Y(Z), E, x0 = {x0:F4}", grid, CreatePlanarOptions());
        }

        private void BuildXY_E_TE()
        {
            RectD bounds = new(0, a, 0, b);
            StreamplotGrid grid = StreamplotGrid.FromField(
                bounds,
                nx: 160,
                ny: 160,
                field: (x, y) =>
                {
                    double ex = -omega * mu0 * ky / kc2 * Math.Cos(kx * x) * Math.Sin(ky * y) * Math.Sin(beta * z0 - omega * t);
                    double ey = omega * mu0 * kx / kc2 * Math.Sin(kx * x) * Math.Cos(ky * y) * Math.Sin(beta * z0 - omega * t);
                    return new Vec2(ex, ey);
                });

            SetPlot(YXspc, $"Y(X), E, z0 = {z0:F4}", grid, CreatePlanarOptions());
        }

        private void BuildXZ_H_TE()
        {
            RectD bounds = new(0, a, 0, zMax);
            StreamplotGrid grid = StreamplotGrid.FromField(
                bounds,
                nx: 180,
                ny: 180,
                field: (x, z) =>
                {
                    double hz = Math.Cos(kx * x) * Math.Cos(ky * y0) * Math.Cos(beta * z - omega * t);
                    double hx = -beta * kx / kc2 * Math.Sin(kx * x) * Math.Cos(ky * y0) * Math.Sin(beta * z - omega * t);
                    return new Vec2(-hx, hz);
                });

            SetPlot(ZXspc, $"Z(X), H, y0 = {y0:F4}", grid, CreatePlanarOptions());
        }

        private void BuildYZ_E_TE()
        {
            RectD bounds = new(0, zMax, 0, b);
            StreamplotGrid grid = StreamplotGrid.FromField(
                bounds,
                nx: 180,
                ny: 180,
                field: (z, y) =>
                {
                    double ez = 0.0;
                    double ey = omega * mu0 * kx / kc2 * Math.Sin(kx * x0) * Math.Cos(ky * y) * Math.Sin(beta * z - omega * t);
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

            x0Slider.Minimum = 0;
            x0Slider.Maximum = a / 1e-2;
            x0Slider.Value = a / 1e-2 / 2;

            y0Slider.Minimum = 0;
            y0Slider.Maximum = b / 1e-2;
            y0Slider.Value = b / 1e-2 / 2;

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
            timer.Start();
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            StopRefreshTimer();
        }

        private void Timer_Elapsed(object? sender, EventArgs e)
        {
            Calculate();
        }

        private void useTimeSimulCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            StopRefreshTimer();
            timeGroupBox.Enabled = useTimeSimulCheckBox.Checked;
        }

        private void StopRefreshTimer()
        {
            startBtn.Enabled = true;
            stopBtn.Enabled = false;
            timer.Stop();
        }
    }
}
