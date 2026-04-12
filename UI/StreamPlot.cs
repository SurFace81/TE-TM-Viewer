namespace TETMViewer
{
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

    public sealed class Streamline
    {
        public required List<Vec2> Points { get; init; }
    }

    public sealed class StreamplotOptions
    {
        public int SeedColumns { get; init; } = 40;
        public int SeedRows { get; init; } = 40;

        public int OccupancyColumns { get; init; } = 60;
        public int OccupancyRows { get; init; } = 60;

        public double StepSize { get; init; } = 0.0003;
        public int MaxStepsPerDirection { get; init; } = 4000;
        public double MinSpeed { get; init; } = 1e-4;
        public double MinLength { get; init; } = 0.0015;

        public bool NormalizeField { get; init; } = true;
        public bool Bidirectional { get; init; } = true;
        public int SeedJitterDivisor { get; init; } = 6;
    }

    public static class Streamplot
    {
        public static List<Streamline> Generate(
            Func<double, double, Vec2> field,
            RectD bounds,
            StreamplotOptions? options = null)
        {
            options ??= new StreamplotOptions();

            bool[,] occupied = new bool[options.OccupancyColumns, options.OccupancyRows];
            List<Vec2> seeds = BuildSeeds(bounds, options);
            List<Streamline> lines = new();

            foreach (Vec2 seed in seeds)
            {
                if (!bounds.Contains(seed))
                    continue;

                if (TryGetCell(seed, bounds, options, out int sx, out int sy) && occupied[sx, sy])
                    continue;

                Vec2 v0 = field(seed.X, seed.Y);
                if (v0.Length < options.MinSpeed)
                    continue;

                List<Vec2> backward = options.Bidirectional
                    ? Trace(field, bounds, options, occupied, seed, -1)
                    : new List<Vec2>();

                List<Vec2> forward = Trace(field, bounds, options, occupied, seed, +1);

                backward.Reverse();

                List<Vec2> merged = new(backward.Count + 1 + forward.Count);
                merged.AddRange(backward);
                merged.Add(seed);
                merged.AddRange(forward);

                double length = PolylineLength(merged);
                if (length < options.MinLength || merged.Count < 2)
                    continue;

                MarkOccupied(merged, occupied, bounds, options);
                lines.Add(new Streamline { Points = merged });
            }

            return lines;
        }

        private static List<Vec2> BuildSeeds(RectD bounds, StreamplotOptions options)
        {
            List<Vec2> seeds = new(options.SeedColumns * options.SeedRows);
            double dx = bounds.Width / Math.Max(1, options.SeedColumns - 1);
            double dy = bounds.Height / Math.Max(1, options.SeedRows - 1);
            double jitterX = dx / options.SeedJitterDivisor;
            double jitterY = dy / options.SeedJitterDivisor;

            for (int iy = 0; iy < options.SeedRows; iy++)
            {
                for (int ix = 0; ix < options.SeedColumns; ix++)
                {
                    double x = bounds.XMin + ix * dx;
                    double y = bounds.YMin + iy * dy;

                    double jx = ((ix * 73856093) ^ (iy * 19349663)) % 1000 / 999.0 - 0.5;
                    double jy = ((ix * 83492791) ^ (iy * 2971215073L % int.MaxValue)) % 1000 / 999.0 - 0.5;

                    seeds.Add(new Vec2(x + jx * jitterX, y + jy * jitterY));
                }
            }

            return seeds;
        }

        private static List<Vec2> Trace(
            Func<double, double, Vec2> field,
            RectD bounds,
            StreamplotOptions options,
            bool[,] occupied,
            Vec2 start,
            int sign)
        {
            List<Vec2> pts = new();
            Vec2 p = start;

            for (int i = 0; i < options.MaxStepsPerDirection; i++)
            {
                Vec2 next = Rk4(field, p, sign * options.StepSize, options.NormalizeField);

                if (!bounds.Contains(next))
                    break;

                Vec2 v = field(next.X, next.Y);
                if (v.Length < options.MinSpeed)
                    break;

                if (TryGetCell(next, bounds, options, out int cx, out int cy) && occupied[cx, cy])
                    break;

                pts.Add(next);
                p = next;
            }

            return pts;
        }

        private static Vec2 Rk4(
            Func<double, double, Vec2> field,
            Vec2 p,
            double h,
            bool normalize)
        {
            Vec2 F(Vec2 q)
            {
                Vec2 v = field(q.X, q.Y);
                return normalize ? v.Normalized() : v;
            }

            Vec2 k1 = F(p);
            Vec2 k2 = F(p + k1 * (h * 0.5));
            Vec2 k3 = F(p + k2 * (h * 0.5));
            Vec2 k4 = F(p + k3 * h);

            return p + (k1 + k2 * 2.0 + k3 * 2.0 + k4) * (h / 6.0);
        }

        private static void MarkOccupied(
            List<Vec2> pts,
            bool[,] occupied,
            RectD bounds,
            StreamplotOptions options)
        {
            foreach (Vec2 p in pts)
            {
                if (TryGetCell(p, bounds, options, out int cx, out int cy))
                    occupied[cx, cy] = true;
            }
        }

        private static bool TryGetCell(
            Vec2 p,
            RectD bounds,
            StreamplotOptions options,
            out int cx,
            out int cy)
        {
            double tx = (p.X - bounds.XMin) / bounds.Width;
            double ty = (p.Y - bounds.YMin) / bounds.Height;

            cx = (int)Math.Floor(tx * options.OccupancyColumns);
            cy = (int)Math.Floor(ty * options.OccupancyRows);

            if (cx < 0 || cy < 0 || cx >= options.OccupancyColumns || cy >= options.OccupancyRows)
                return false;

            return true;
        }

        private static double PolylineLength(List<Vec2> pts)
        {
            double sum = 0;
            for (int i = 1; i < pts.Count; i++)
            {
                Vec2 d = pts[i] - pts[i - 1];
                sum += d.Length;
            }

            return sum;
        }
    }
}
