using System;
using System.Collections.Generic;
using System.Linq;

namespace TETMViewer;

public readonly record struct Vec2(double X, double Y)
{
    public double Length => Math.Sqrt(X * X + Y * Y);

    public Vec2 Normalized(double eps = 1e-12)
    {
        double len = Length;
        return len < eps ? new Vec2(0, 0) : new Vec2(X / len, Y / len);
    }

    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);
    public static Vec2 operator *(Vec2 a, double k) => new(a.X * k, a.Y * k);
    public static Vec2 operator /(Vec2 a, double k) => new(a.X / k, a.Y / k);
}

public readonly record struct RectD(double XMin, double XMax, double YMin, double YMax)
{
    public double Width => XMax - XMin;
    public double Height => YMax - YMin;

    public bool Contains(Vec2 p) =>
        p.X >= XMin && p.X <= XMax && p.Y >= YMin && p.Y <= YMax;
}

public sealed record StreamArrow(Vec2 Tail, Vec2 Head);

public sealed class Streamline
{
    public required List<Vec2> Points { get; init; }
    public required List<StreamArrow> Arrows { get; init; }
}

public enum StreamIntegrationDirection
{
    Forward,
    Backward,
    Both
}

public sealed class StreamplotOptions
{
    public double DensityX { get; init; } = 1.0;
    public double DensityY { get; init; } = 1.0;
    public double MinLengthAxes { get; init; } = 0.10;
    public double MaxLengthAxes { get; init; } = 4.0;
    public StreamIntegrationDirection IntegrationDirection { get; init; } = StreamIntegrationDirection.Both;
    public bool BrokenStreamlines { get; init; } = true;
    public double IntegrationMaxStepScale { get; init; } = 1.0;
    public double IntegrationMaxErrorScale { get; init; } = 1.0;
    public int NumArrows { get; init; } = 1;

    public int SampleColumns { get; init; } = 128;
    public int SampleRows { get; init; } = 128;
}

public sealed class StreamplotGrid
{
    public required double[] X { get; init; }
    public required double[] Y { get; init; }
    public required double[,] U { get; init; }
    public required double[,] V { get; init; }

    public static StreamplotGrid FromField(
        RectD bounds,
        int nx,
        int ny,
        Func<double, double, Vec2> field)
    {
        if (nx < 2 || ny < 2)
            throw new ArgumentOutOfRangeException(nameof(nx), "Grid must be at least 2x2.");

        double[] x = Linspace(bounds.XMin, bounds.XMax, nx);
        double[] y = Linspace(bounds.YMin, bounds.YMax, ny);
        double[,] u = new double[ny, nx];
        double[,] v = new double[ny, nx];

        for (int iy = 0; iy < ny; iy++)
        {
            for (int ix = 0; ix < nx; ix++)
            {
                Vec2 value = field(x[ix], y[iy]);
                u[iy, ix] = value.X;
                v[iy, ix] = value.Y;
            }
        }

        return new StreamplotGrid
        {
            X = x,
            Y = y,
            U = u,
            V = v,
        };
    }

    private static double[] Linspace(double a, double b, int n)
    {
        double[] values = new double[n];
        if (n == 1)
        {
            values[0] = a;
            return values;
        }

        double step = (b - a) / (n - 1);
        for (int i = 0; i < n; i++)
            values[i] = a + i * step;

        return values;
    }
}

public static class Streamplot
{
    public static List<Streamline> Generate(StreamplotGrid input, StreamplotOptions? options = null)
    {
        options ??= new StreamplotOptions();

        Grid grid = new(input.X, input.Y);
        StreamMask mask = new(options.DensityX, options.DensityY);
        DomainMap dmap = new(grid, mask);

        if (options.IntegrationMaxStepScale <= 0)
            throw new ArgumentOutOfRangeException(nameof(options.IntegrationMaxStepScale));
        if (options.IntegrationMaxErrorScale <= 0)
            throw new ArgumentOutOfRangeException(nameof(options.IntegrationMaxErrorScale));
        if (options.NumArrows < 0)
            throw new ArgumentOutOfRangeException(nameof(options.NumArrows));

        if (input.U.GetLength(0) != grid.Ny || input.U.GetLength(1) != grid.Nx)
            throw new ArgumentException("U shape must match Y/X lengths.");
        if (input.V.GetLength(0) != grid.Ny || input.V.GetLength(1) != grid.Nx)
            throw new ArgumentException("V shape must match Y/X lengths.");

        double[,] u = (double[,])input.U.Clone();
        double[,] v = (double[,])input.V.Clone();

        Func<double, double, bool, double, double, List<(double X, double Y)>?> integrate =
            GetIntegrator(u, v, dmap, options.MinLengthAxes, options.MaxLengthAxes, options.IntegrationDirection);

        List<List<(double X, double Y)>> trajectories = new();
        foreach ((int xm, int ym) in GenerateStartingPoints(mask.Nx, mask.Ny))
        {
            if (mask[xm, ym] != 0)
                continue;

            (double xg, double yg) = dmap.MaskToGrid(xm, ym);
            List<(double X, double Y)>? trajectory = integrate(
                xg,
                yg,
                options.BrokenStreamlines,
                options.IntegrationMaxStepScale,
                options.IntegrationMaxErrorScale);

            if (trajectory is not null)
                trajectories.Add(trajectory);
        }

        List<Streamline> lines = new(trajectories.Count);
        foreach (List<(double X, double Y)> trajectory in trajectories)
        {
            List<Vec2> points = new(trajectory.Count);
            foreach ((double gx, double gy) in trajectory)
            {
                (double dx, double dy) = dmap.GridToData(gx, gy);
                points.Add(new Vec2(dx + grid.XOrigin, dy + grid.YOrigin));
            }

            List<StreamArrow> arrows = BuildArrows(points, options.NumArrows);
            lines.Add(new Streamline
            {
                Points = points,
                Arrows = arrows,
            });
        }

        return lines;
    }

    private static List<StreamArrow> BuildArrows(List<Vec2> points, int numArrows)
    {
        List<StreamArrow> arrows = new();
        if (numArrows <= 0 || points.Count < 3)
            return arrows;

        double[] cumulative = new double[points.Count];
        for (int i = 1; i < points.Count; i++)
            cumulative[i] = cumulative[i - 1] + (points[i] - points[i - 1]).Length;

        double total = cumulative[^1];
        if (total <= 0)
            return arrows;

        for (int arrowIndex = 1; arrowIndex <= numArrows; arrowIndex++)
        {
            double target = total * arrowIndex / (numArrows + 1.0);
            int idx = Array.BinarySearch(cumulative, target);
            if (idx < 0)
                idx = ~idx;
            idx = Math.Clamp(idx, 1, points.Count - 2);

            Vec2 tail = points[idx];
            Vec2 head = new(
                0.5 * (points[idx].X + points[idx + 1].X),
                0.5 * (points[idx].Y + points[idx + 1].Y));

            arrows.Add(new StreamArrow(tail, head));
        }

        return arrows;
    }

    private static Func<double, double, bool, double, double, List<(double X, double Y)>?> GetIntegrator(
        double[,] uData,
        double[,] vData,
        DomainMap dmap,
        double minlength,
        double maxlength,
        StreamIntegrationDirection integrationDirection)
    {
        int ny = uData.GetLength(0);
        int nx = uData.GetLength(1);
        double[,] u = new double[ny, nx];
        double[,] v = new double[ny, nx];
        double[,] speed = new double[ny, nx];

        for (int iy = 0; iy < ny; iy++)
        {
            for (int ix = 0; ix < nx; ix++)
            {
                u[iy, ix] = uData[iy, ix] * dmap.XDataToGrid;
                v[iy, ix] = vData[iy, ix] * dmap.YDataToGrid;

                double uax = u[iy, ix] / (dmap.Grid.Nx - 1);
                double vax = v[iy, ix] / (dmap.Grid.Ny - 1);
                speed[iy, ix] = Math.Sqrt(uax * uax + vax * vax);
            }
        }

        (double Dx, double Dy) ForwardTime(double xi, double yi)
        {
            if (!dmap.Grid.WithinGrid(xi, yi))
                throw new OutOfBoundsException();

            double dsdt = InterpGrid(speed, xi, yi);
            if (dsdt == 0)
                throw new TerminateTrajectoryException();

            double dtds = 1.0 / dsdt;
            double ui = InterpGrid(u, xi, yi);
            double vi = InterpGrid(v, xi, yi);
            return (ui * dtds, vi * dtds);
        }

        (double Dx, double Dy) BackwardTime(double xi, double yi)
        {
            (double dx, double dy) = ForwardTime(xi, yi);
            return (-dx, -dy);
        }

        return Integrate;

        List<(double X, double Y)>? Integrate(
            double x0,
            double y0,
            bool brokenStreamlines,
            double integrationMaxStepScale,
            double integrationMaxErrorScale)
        {
            double localMaxLength = integrationDirection == StreamIntegrationDirection.Both
                ? maxlength / 2.0
                : maxlength;

            double totalLength = 0;
            List<(double X, double Y)> trajectory = new();

            try
            {
                dmap.StartTrajectory(x0, y0, brokenStreamlines);
            }
            catch (InvalidIndexException)
            {
                return null;
            }

            if (integrationDirection is StreamIntegrationDirection.Both or StreamIntegrationDirection.Backward)
            {
                (double s, List<(double X, double Y)> backward) = IntegrateRk12(
                    x0,
                    y0,
                    dmap,
                    BackwardTime,
                    localMaxLength,
                    brokenStreamlines,
                    integrationMaxStepScale,
                    integrationMaxErrorScale);

                totalLength += s;
                backward.Reverse();
                trajectory.AddRange(backward);
            }

            if (integrationDirection is StreamIntegrationDirection.Both or StreamIntegrationDirection.Forward)
            {
                dmap.ResetStartPoint(x0, y0);
                (double s, List<(double X, double Y)> forward) = IntegrateRk12(
                    x0,
                    y0,
                    dmap,
                    ForwardTime,
                    localMaxLength,
                    brokenStreamlines,
                    integrationMaxStepScale,
                    integrationMaxErrorScale);

                totalLength += s;
                for (int i = 1; i < forward.Count; i++)
                    trajectory.Add(forward[i]);
            }

            if (totalLength > minlength)
                return trajectory;

            dmap.UndoTrajectory();
            return null;
        }
    }

    private static (double Length, List<(double X, double Y)> Trajectory) IntegrateRk12(
        double x0,
        double y0,
        DomainMap dmap,
        Func<double, double, (double Dx, double Dy)> velocity,
        double maxlength,
        bool brokenStreamlines,
        double integrationMaxStepScale,
        double integrationMaxErrorScale)
    {
        double maxerror = 0.003 * integrationMaxErrorScale;
        double maxds = Math.Min(Math.Min(1.0 / dmap.Mask.Nx, 1.0 / dmap.Mask.Ny), 0.1) * integrationMaxStepScale;
        double ds = maxds;
        double stotal = 0;
        double xi = x0;
        double yi = y0;
        List<(double X, double Y)> traj = new();

        while (true)
        {
            try
            {
                if (dmap.Grid.WithinGrid(xi, yi))
                    traj.Add((xi, yi));
                else
                    throw new OutOfBoundsException();

                (double k1x, double k1y) = velocity(xi, yi);
                (double k2x, double k2y) = velocity(xi + ds * k1x, yi + ds * k1y);

                double dx1 = ds * k1x;
                double dy1 = ds * k1y;
                double dx2 = ds * 0.5 * (k1x + k2x);
                double dy2 = ds * 0.5 * (k1y + k2y);

                double error = Math.Sqrt(
                    Math.Pow(dx2 - dx1, 2) / Math.Pow(dmap.Grid.Nx - 1, 2) +
                    Math.Pow(dy2 - dy1, 2) / Math.Pow(dmap.Grid.Ny - 1, 2));

                if (error < maxerror)
                {
                    xi += dx2;
                    yi += dy2;

                    try
                    {
                        dmap.UpdateTrajectory(xi, yi, brokenStreamlines);
                    }
                    catch (InvalidIndexException)
                    {
                        break;
                    }

                    if (stotal + ds > maxlength)
                        break;

                    stotal += ds;
                }

                ds = error == 0
                    ? maxds
                    : Math.Min(maxds, 0.85 * ds * Math.Sqrt(maxerror / error));
            }
            catch (OutOfBoundsException)
            {
                if (traj.Count > 0)
                {
                    (double step, List<(double X, double Y)> updated) = EulerStep(traj, dmap, velocity);
                    traj = updated;
                    stotal += step;
                }

                break;
            }
            catch (TerminateTrajectoryException)
            {
                break;
            }
        }

        return (stotal, traj);
    }

    private static (double Length, List<(double X, double Y)> Trajectory) EulerStep(
        List<(double X, double Y)> traj,
        DomainMap dmap,
        Func<double, double, (double Dx, double Dy)> velocity)
    {
        (double xi, double yi) = traj[^1];
        (double cx, double cy) = velocity(xi, yi);
        int nx = dmap.Grid.Nx;
        int ny = dmap.Grid.Ny;

        double dsx = cx switch
        {
            0 => double.PositiveInfinity,
            < 0 => xi / -cx,
            _ => (nx - 1 - xi) / cx,
        };

        double dsy = cy switch
        {
            0 => double.PositiveInfinity,
            < 0 => yi / -cy,
            _ => (ny - 1 - yi) / cy,
        };

        double ds = Math.Min(dsx, dsy);
        traj.Add((xi + cx * ds, yi + cy * ds));
        return (ds, traj);
    }

    private static double InterpGrid(double[,] values, double xi, double yi)
    {
        int ny = values.GetLength(0);
        int nx = values.GetLength(1);

        int x = (int)xi;
        int y = (int)yi;
        int xn = x == nx - 1 ? x : x + 1;
        int yn = y == ny - 1 ? y : y + 1;

        double a00 = values[y, x];
        double a01 = values[y, xn];
        double a10 = values[yn, x];
        double a11 = values[yn, xn];

        double xt = xi - x;
        double yt = yi - y;
        double a0 = a00 * (1 - xt) + a01 * xt;
        double a1 = a10 * (1 - xt) + a11 * xt;
        return a0 * (1 - yt) + a1 * yt;
    }

    private static IEnumerable<(int X, int Y)> GenerateStartingPoints(int nx, int ny)
    {
        int xfirst = 0;
        int yfirst = 1;
        int xlast = nx - 1;
        int ylast = ny - 1;
        int x = 0;
        int y = 0;
        SpiralDirection direction = SpiralDirection.Right;

        for (int i = 0; i < nx * ny; i++)
        {
            yield return (x, y);

            switch (direction)
            {
                case SpiralDirection.Right:
                    x += 1;
                    if (x >= xlast)
                    {
                        xlast -= 1;
                        direction = SpiralDirection.Up;
                    }
                    break;
                case SpiralDirection.Up:
                    y += 1;
                    if (y >= ylast)
                    {
                        ylast -= 1;
                        direction = SpiralDirection.Left;
                    }
                    break;
                case SpiralDirection.Left:
                    x -= 1;
                    if (x <= xfirst)
                    {
                        xfirst += 1;
                        direction = SpiralDirection.Down;
                    }
                    break;
                case SpiralDirection.Down:
                    y -= 1;
                    if (y <= yfirst)
                    {
                        yfirst += 1;
                        direction = SpiralDirection.Right;
                    }
                    break;
            }
        }
    }

    private enum SpiralDirection
    {
        Right,
        Up,
        Left,
        Down
    }

    private sealed class Grid
    {
        public Grid(double[] x, double[] y)
        {
            if (x.Length < 2 || y.Length < 2)
                throw new ArgumentException("Grid axes must have at least 2 points.");
            if (!StrictlyIncreasing(x) || !StrictlyIncreasing(y))
                throw new ArgumentException("Grid axes must be strictly increasing.");

            Nx = x.Length;
            Ny = y.Length;
            Dx = x[1] - x[0];
            Dy = y[1] - y[0];
            XOrigin = x[0];
            YOrigin = y[0];
            Width = x[^1] - x[0];
            Height = y[^1] - y[0];

            if (!EquallySpaced(x, Width / (Nx - 1)) || !EquallySpaced(y, Height / (Ny - 1)))
                throw new ArgumentException("Grid axes must be equally spaced.");
        }

        public int Nx { get; }
        public int Ny { get; }
        public double Dx { get; }
        public double Dy { get; }
        public double XOrigin { get; }
        public double YOrigin { get; }
        public double Width { get; }
        public double Height { get; }

        public bool WithinGrid(double xi, double yi) =>
            0 <= xi && xi <= Nx - 1 && 0 <= yi && yi <= Ny - 1;

        private static bool StrictlyIncreasing(double[] values)
        {
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] <= values[i - 1])
                    return false;
            }

            return true;
        }

        private static bool EquallySpaced(double[] values, double expectedStep)
        {
            for (int i = 1; i < values.Length; i++)
            {
                if (Math.Abs((values[i] - values[i - 1]) - expectedStep) > 1e-12 * Math.Max(1.0, Math.Abs(expectedStep)))
                    return false;
            }

            return true;
        }
    }

    private sealed class StreamMask
    {
        private readonly byte[,] mask;
        private readonly List<(int Y, int X)> trajectory = new();
        private (int X, int Y)? current;

        public StreamMask(double densityX, double densityY)
        {
            Nx = Math.Max(1, (int)(30 * densityX));
            Ny = Math.Max(1, (int)(30 * densityY));
            mask = new byte[Ny, Nx];
        }

        public int Nx { get; }
        public int Ny { get; }

        public byte this[int xm, int ym] => mask[ym, xm];

        public void StartTrajectory(int xm, int ym, bool brokenStreamlines)
        {
            trajectory.Clear();
            current = null;
            UpdateTrajectory(xm, ym, brokenStreamlines);
        }

        public void UndoTrajectory()
        {
            foreach ((int y, int x) in trajectory)
                mask[y, x] = 0;
        }

        public void ResetCurrent(int xm, int ym)
        {
            current = (xm, ym);
        }

        public void UpdateTrajectory(int xm, int ym, bool brokenStreamlines)
        {
            if (current == (xm, ym))
                return;

            if (mask[ym, xm] == 0)
            {
                trajectory.Add((ym, xm));
                mask[ym, xm] = 1;
                current = (xm, ym);
                return;
            }

            if (brokenStreamlines)
                throw new InvalidIndexException();
        }
    }

    private sealed class DomainMap
    {
        public DomainMap(Grid grid, StreamMask mask)
        {
            Grid = grid;
            Mask = mask;
            XGridToMask = (mask.Nx - 1.0) / (grid.Nx - 1.0);
            YGridToMask = (mask.Ny - 1.0) / (grid.Ny - 1.0);
            XMaskToGrid = 1.0 / XGridToMask;
            YMaskToGrid = 1.0 / YGridToMask;
            XDataToGrid = 1.0 / grid.Dx;
            YDataToGrid = 1.0 / grid.Dy;
        }

        public Grid Grid { get; }
        public StreamMask Mask { get; }
        public double XGridToMask { get; }
        public double YGridToMask { get; }
        public double XMaskToGrid { get; }
        public double YMaskToGrid { get; }
        public double XDataToGrid { get; }
        public double YDataToGrid { get; }

        public (int Xm, int Ym) GridToMask(double xi, double yi) =>
            ((int)Math.Round(xi * XGridToMask), (int)Math.Round(yi * YGridToMask));

        public (double Xg, double Yg) MaskToGrid(int xm, int ym) =>
            (xm * XMaskToGrid, ym * YMaskToGrid);

        public (double Xg, double Yg) DataToGrid(double xd, double yd) =>
            (xd * XDataToGrid, yd * YDataToGrid);

        public (double Xd, double Yd) GridToData(double xg, double yg) =>
            (xg / XDataToGrid, yg / YDataToGrid);

        public void StartTrajectory(double xg, double yg, bool brokenStreamlines)
        {
            (int xm, int ym) = GridToMask(xg, yg);
            Mask.StartTrajectory(xm, ym, brokenStreamlines);
        }

        public void ResetStartPoint(double xg, double yg)
        {
            (int xm, int ym) = GridToMask(xg, yg);
            Mask.ResetCurrent(xm, ym);
        }

        public void UpdateTrajectory(double xg, double yg, bool brokenStreamlines)
        {
            if (!Grid.WithinGrid(xg, yg))
                throw new InvalidIndexException();

            (int xm, int ym) = GridToMask(xg, yg);
            Mask.UpdateTrajectory(xm, ym, brokenStreamlines);
        }

        public void UndoTrajectory() => Mask.UndoTrajectory();
    }

    private sealed class InvalidIndexException : Exception;
    private sealed class OutOfBoundsException : Exception;
    private sealed class TerminateTrajectoryException : Exception;
}
