using ScottPlot;
using ScottPlot.WinForms;
using System.ComponentModel;

namespace TETMViewer
{
    public sealed class StreamPlotControl : UserControl
    {
        private readonly FormsPlot formsPlot = new();

        private string plotTitle = string.Empty;
        private string xLabel = string.Empty;
        private string yLabel = string.Empty;
        private bool showArrows = true;
        private float lineWidth = 1.5f;
        private float arrowLengthPx = 15;
        private float arrowWidthPx = 8;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<double, double, Vec2>? Field { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RectD Bounds { get; set; } = new(0, 1, 0, 1);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StreamplotOptions Options { get; set; } = new();

        public string PlotTitle
        {
            get => plotTitle;
            set
            {
                plotTitle = value;
                RefreshPlot();
            }
        }

        public string XLabel
        {
            get => xLabel;
            set
            {
                xLabel = value;
                RefreshPlot();
            }
        }

        public string YLabel
        {
            get => yLabel;
            set
            {
                yLabel = value;
                RefreshPlot();
            }
        }

        public bool ShowArrows
        {
            get => showArrows;
            set
            {
                showArrows = value;
                RefreshPlot();
            }
        }

        public float LineWidth
        {
            get => lineWidth;
            set
            {
                lineWidth = value;
                RefreshPlot();
            }
        }

        public float ArrowLengthPx
        {
            get => arrowLengthPx;
            set
            {
                arrowLengthPx = value;
                RefreshPlot();
            }
        }

        public float ArrowWidthPx
        {
            get => arrowWidthPx;
            set
            {
                arrowWidthPx = value;
                RefreshPlot();
            }
        }

        public StreamPlotControl()
        {
            formsPlot.Dock = DockStyle.Fill;
            Controls.Add(formsPlot);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            RefreshPlot();
        }

        public void RefreshPlot()
        {
            Plot plot = formsPlot.Plot;
            plot.Clear();

            plot.Title(PlotTitle);
            plot.XLabel(XLabel);
            plot.YLabel(YLabel);

            plot.Axes.Bottom.Label.FontSize = 14;
            plot.Axes.Left.Label.FontSize = 14;
            plot.Axes.Bottom.TickLabelStyle.FontSize = 10;
            plot.Axes.Left.TickLabelStyle.FontSize = 10;

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

            var cmap = new ScottPlot.Colormaps.Plasma();
            for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++)
            {
                Streamline line = lines[lineIndex];

                double[] xs = line.Points.Select(p => p.X).ToArray();
                double[] ys = line.Points.Select(p => p.Y).ToArray();

                double intensity = lineIntensities[lineIndex];
                double t = (intensity - minIntensity) / intensityRange;

                ScottPlot.Color lineColor = cmap.GetColor(t);

                var sp = plot.Add.ScatterLine(xs, ys);
                sp.LineWidth = LineWidth;
                sp.Color = lineColor;

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
            formsPlot.Refresh();
        }

        public void Clear()
        {
            formsPlot.Plot.Clear();
        }

        private static void AddTriangleMarker(
            Plot plot,
            Vec2 center,
            Vec2 direction,
            float lengthPx,
            float widthPx,
            ScottPlot.Color fillColor,
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
            poly.FillColor = fillColor;
            poly.LineColor = fillColor;
            poly.LineWidth = 1;
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