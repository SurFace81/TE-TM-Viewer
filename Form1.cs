namespace TETMViewer
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();

            waveTypeCb.DataSource = new List<string>() { "TE", "TM" };
        }

        private void calcBtn_Click(object sender, EventArgs e)
        {
            BuildStreamPlot();
        }

        private void BuildStreamPlot()
        {
            double a1 = 1.0;
            double b1 = 0.5;
            int m = 1;
            int n = 1;

            YXspc.Field = (x, y) =>
            {
                double ex =
                    -(n * Math.PI / b1) *
                    Math.Sin(m * Math.PI * x / a1) *
                    Math.Cos(n * Math.PI * y / b1);

                double ey =
                    +(m * Math.PI / a1) *
                    Math.Cos(m * Math.PI * x / a1) *
                    Math.Sin(n * Math.PI * y / b1);

                return new Vec2(ex, ey);
            };

            YXspc.Bounds = new RectD(0, a1, 0, b1);
            YXspc.Options = new StreamplotOptions
            {
                SeedColumns = 32,
                SeedRows = 18,
                OccupancyColumns = 90,
                OccupancyRows = 45,
                StepSize = 0.008,
                MaxStepsPerDirection = 1200,
                MinLength = 0.03,
                NormalizeField = true
            };
            YXspc.RefreshPlot();
        }
    }
}
