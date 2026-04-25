using ScottPlot;
using ScottPlot.WinForms;
using System.ComponentModel;

namespace TETMViewer;

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
    public StreamplotGrid? GridData { get; set; }

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

        if (GridData is null)
        {
            formsPlot.Refresh();
            return;
        }

        List<Streamline> lines = Streamplot.Generate(GridData, Options);
        RectD bounds = new(GridData.X[0], GridData.X[^1], GridData.Y[0], GridData.Y[^1]);

        double[] lineIntensities = lines
            .Select(line => ComputeLineIntensity(line, GridData))
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
            double t = (lineIntensities[lineIndex] - minIntensity) / intensityRange;
            ScottPlot.Color lineColor = cmap.GetColor(t);

            var sp = plot.Add.ScatterLine(xs, ys);
            sp.LineWidth = LineWidth;
            sp.Color = lineColor;

            if (!ShowArrows)
                continue;

            foreach (StreamArrow arrow in line.Arrows)
            {
                Vec2 dir = (arrow.Head - arrow.Tail).Normalized();
                AddTriangleMarker(plot, arrow.Head, dir, ArrowLengthPx, ArrowWidthPx, lineColor, bounds, formsPlot.ClientSize);
            }
        }

        plot.Axes.Rules.Clear();

        var limits = new AxisLimits(
            bounds.XMin,
            bounds.XMax,
            bounds.YMin,
            bounds.YMax);

        plot.Axes.Rules.Add(
            new ScottPlot.AxisRules.MaximumBoundary(
                plot.Axes.Bottom,
                plot.Axes.Left,
                limits));

        plot.Axes.Rules.Add(
            new ScottPlot.AxisRules.MaximumSpan(
                plot.Axes.Bottom,
                plot.Axes.Left,
                bounds.Width,
                bounds.Height));

        plot.Axes.SetLimits(bounds.XMin, bounds.XMax, bounds.YMin, bounds.YMax);
        formsPlot.Refresh();
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

        Vec2 dirPx = new(dir.X / dataPerPixelX, -dir.Y / dataPerPixelY);
        dirPx = dirPx.Normalized();
        Vec2 normalPx = new(-dirPx.Y, dirPx.X);

        Vec2 tipOffsetPx = dirPx * lengthPx;
        Vec2 baseCenterOffsetPx = dirPx * (-0.5 * lengthPx);
        Vec2 sideOffsetPx = normalPx * (0.5 * widthPx);

        Vec2 tip = new(center.X + tipOffsetPx.X * dataPerPixelX, center.Y - tipOffsetPx.Y * dataPerPixelY);
        Vec2 left = new(
            center.X + (baseCenterOffsetPx.X + sideOffsetPx.X) * dataPerPixelX,
            center.Y - (baseCenterOffsetPx.Y + sideOffsetPx.Y) * dataPerPixelY);
        Vec2 right = new(
            center.X + (baseCenterOffsetPx.X - sideOffsetPx.X) * dataPerPixelX,
            center.Y - (baseCenterOffsetPx.Y - sideOffsetPx.Y) * dataPerPixelY);

        var poly = plot.Add.Polygon(new[] { tip.X, left.X, right.X }, new[] { tip.Y, left.Y, right.Y });
        poly.FillColor = fillColor;
        poly.LineColor = fillColor;
        poly.LineWidth = 1;
    }

    private static double ComputeLineIntensity(Streamline line, StreamplotGrid grid)
    {
        if (line.Points.Count == 0)
            return 0;

        double sum = 0;
        foreach (Vec2 p in line.Points)
        {
            Vec2 v = SampleField(grid, p.X, p.Y);
            sum += v.Length;
        }

        return sum / line.Points.Count;
    }

    private static Vec2 SampleField(StreamplotGrid grid, double x, double y)
    {
        double tx = (x - grid.X[0]) / (grid.X[^1] - grid.X[0]) * (grid.X.Length - 1);
        double ty = (y - grid.Y[0]) / (grid.Y[^1] - grid.Y[0]) * (grid.Y.Length - 1);
        tx = Math.Clamp(tx, 0, grid.X.Length - 1);
        ty = Math.Clamp(ty, 0, grid.Y.Length - 1);

        int ix = (int)tx;
        int iy = (int)ty;
        int ix1 = ix == grid.X.Length - 1 ? ix : ix + 1;
        int iy1 = iy == grid.Y.Length - 1 ? iy : iy + 1;

        double fx = tx - ix;
        double fy = ty - iy;

        double u00 = grid.U[iy, ix];
        double u01 = grid.U[iy, ix1];
        double u10 = grid.U[iy1, ix];
        double u11 = grid.U[iy1, ix1];
        double v00 = grid.V[iy, ix];
        double v01 = grid.V[iy, ix1];
        double v10 = grid.V[iy1, ix];
        double v11 = grid.V[iy1, ix1];

        double u0 = u00 * (1 - fx) + u01 * fx;
        double u1 = u10 * (1 - fx) + u11 * fx;
        double vv0 = v00 * (1 - fx) + v01 * fx;
        double vv1 = v10 * (1 - fx) + v11 * fx;

        return new Vec2(u0 * (1 - fy) + u1 * fy, vv0 * (1 - fy) + vv1 * fy);
    }
}
