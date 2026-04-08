using ScottPlot;
using ScottPlot.WinForms;
using System.ComponentModel;

namespace TETMViewer
{
    public sealed class StreamPlotControl : UserControl
    {
        private readonly FormsPlot formsPlot = new();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<double, double, Vec2>? Field { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RectD Bounds { get; set; } = new(0, 1, 0, 1);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StreamplotOptions Options { get; set; } = new();

        public string PlotTitle { get; set; } = string.Empty;
        public string XLabel { get; set; } = string.Empty;
        public string YLabel { get; set; } = string.Empty;

        public bool ShowArrows { get; set; } = true;
        public float LineWidth { get; set; } = 1.5f;
        public float ArrowLengthPx { get; set; } = 15;
        public float ArrowWidthPx { get; set; } = 8;

        public StreamPlotControl()
        {
            formsPlot.Dock = DockStyle.Fill;
            Controls.Add(formsPlot);
        }

        public void RefreshPlot()
        {
            Plot plot = formsPlot.Plot;
            plot.Clear();

            if (Field is null || Bounds.Width <= 0 || Bounds.Height <= 0)
            {
                formsPlot.Refresh();
                return;
            }

            List<Streamline> lines = Streamplot.Generate(Field, Bounds, Options);

            double[] lineIntensities = lines
                .Select(line => ComputeLineIntensity(line, Field))
                .ToArray();

            double minIntensity = lineIntensities.Length > 0 ? lineIntensities.Min() : 0;
            double maxIntensity = lineIntensities.Length > 0 ? lineIntensities.Max() : 1;

            double intensityRange = maxIntensity - minIntensity;
            if (intensityRange < 1e-12)
                intensityRange = 1;

            for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
            {
                Streamline line = lines[lineIndex];

                double[] xs = line.Points.Select(p => p.X).ToArray();
                double[] ys = line.Points.Select(p => p.Y).ToArray();

                double intensity = lineIntensities[lineIndex];
                double t = (intensity - minIntensity) / intensityRange;
                System.Drawing.Color lineColor = SampleColormap(t);

                var sp = plot.Add.ScatterLine(xs, ys);
                sp.LineWidth = LineWidth;
                sp.Color = ScottPlot.Color.FromColor(lineColor);

                if (ShowArrows && line.Points.Count >= 3)
                {
                    int i = line.Points.Count / 2;
                    Vec2 a = line.Points[Math.Max(0, i - 1)];
                    Vec2 b = line.Points[i];
                    Vec2 c = line.Points[Math.Min(line.Points.Count - 1, i + 1)];

                    Vec2 dir = (c - a).Normalized();

                    AddTriangleMarker(
                        plot,
                        b,
                        dir,
                        ArrowLengthPx,
                        ArrowWidthPx,
                        lineColor,
                        Bounds,
                        formsPlot.ClientSize);
                }
            }

            plot.Axes.SetLimits(Bounds.XMin, Bounds.XMax, Bounds.YMin, Bounds.YMax);
            plot.Title(PlotTitle);
            plot.XLabel(XLabel);
            plot.YLabel(YLabel);

            formsPlot.Refresh();
        }

        private static void AddTriangleMarker(
            Plot plot,
            Vec2 center,
            Vec2 direction,
            float lengthPx,
            float widthPx,
            System.Drawing.Color fillColor,
            RectD bounds,
            Size clientSize)
        {
            if (clientSize.Width <= 1 || clientSize.Height <= 1)
                return;

            Vec2 dir = direction.Normalized();
            if (dir.Length < 1e-12)
                return;

            double dataPerPixelX = bounds.Width / clientSize.Width;
            double dataPerPixelY = bounds.Height / clientSize.Height;

            Vec2 dirPx = new Vec2(
                dir.X / dataPerPixelX,
                -dir.Y / dataPerPixelY).Normalized();

            Vec2 normalPx = new Vec2(-dirPx.Y, dirPx.X);

            Vec2 tipOffsetPx = dirPx * lengthPx;
            Vec2 baseCenterOffsetPx = dirPx * (-0.5 * lengthPx);
            Vec2 sideOffsetPx = normalPx * (0.5 * widthPx);

            Vec2 tip = new(
                center.X + tipOffsetPx.X * dataPerPixelX,
                center.Y - tipOffsetPx.Y * dataPerPixelY);

            Vec2 left = new(
                center.X + (baseCenterOffsetPx.X + sideOffsetPx.X) * dataPerPixelX,
                center.Y - (baseCenterOffsetPx.Y + sideOffsetPx.Y) * dataPerPixelY);

            Vec2 right = new(
                center.X + (baseCenterOffsetPx.X - sideOffsetPx.X) * dataPerPixelX,
                center.Y - (baseCenterOffsetPx.Y - sideOffsetPx.Y) * dataPerPixelY);

            double[] xs = { tip.X, left.X, right.X };
            double[] ys = { tip.Y, left.Y, right.Y };

            var poly = plot.Add.Polygon(xs, ys);
            poly.FillColor = ScottPlot.Color.FromColor(fillColor);
            poly.LineColor = ScottPlot.Color.FromColor(fillColor);
            poly.LineWidth = 1;
        }

        private static double Clamp01(double x)
        {
            if (x < 0) return 0;
            if (x > 1) return 1;
            return x;
        }

        private static System.Drawing.Color LerpColor(System.Drawing.Color a, System.Drawing.Color b, double t)
        {
            t = Clamp01(t);

            int r = (int)Math.Round(a.R + (b.R - a.R) * t);
            int g = (int)Math.Round(a.G + (b.G - a.G) * t);
            int bb = (int)Math.Round(a.B + (b.B - a.B) * t);

            return System.Drawing.Color.FromArgb(r, g, bb);
        }

        private static System.Drawing.Color SampleColormap(double t)
        {
            t = Clamp01(t);

            System.Drawing.Color[] palette =
            {
                System.Drawing.Color.FromArgb(68, 1, 84),
                System.Drawing.Color.FromArgb(59, 82, 139),
                System.Drawing.Color.FromArgb(33, 145, 140),
                System.Drawing.Color.FromArgb(94, 201, 98),
                System.Drawing.Color.FromArgb(253, 231, 37),
            };

            double scaled = t * (palette.Length - 1);
            int i = (int)Math.Floor(scaled);

            if (i >= palette.Length - 1)
                return palette[^1];

            double frac = scaled - i;
            return LerpColor(palette[i], palette[i + 1], frac);
        }

        private static double ComputeLineIntensity(Streamline line, Func<double, double, Vec2> field)
        {
            if (line.Points.Count == 0)
                return 0;

            double sum = 0;
            foreach (Vec2 p in line.Points)
                sum += field(p.X, p.Y).Length;

            return sum / line.Points.Count;
        }
    }
}
