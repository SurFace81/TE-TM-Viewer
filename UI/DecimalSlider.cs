using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TETMViewer.UI
{
    [DefaultEvent(nameof(ValueChanged))]
    public class DecimalSlider : Control
    {
        private double _minimum = 0.0;
        private double _maximum = 10.0;
        private double _step = 0.1;
        private double _value = 0.0;
        private int _decimalPlaces = 2;
        private bool _showValueText = true;
        private bool _showTicks = true;
        private int _tickCount = 10;
        private bool _dragging;

        public DecimalSlider()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.Selectable |
                ControlStyles.SupportsTransparentBackColor,
                true);

            Size = new Size(220, 60);
            BackColor = Color.Transparent;
            TabStop = true;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(0.0)]
        public double Minimum
        {
            get => _minimum;
            set
            {
                if (value >= _maximum)
                    throw new ArgumentOutOfRangeException(nameof(value), "Minimum must be less than Maximum.");

                _minimum = value;

                if (_value < _minimum)
                    _value = _minimum;

                _value = CoerceToStep(_value);
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(10.0)]
        public double Maximum
        {
            get => _maximum;
            set
            {
                if (value <= _minimum)
                    throw new ArgumentOutOfRangeException(nameof(value), "Maximum must be greater than Minimum.");

                _maximum = value;

                if (_value > _maximum)
                    _value = _maximum;

                _value = CoerceToStep(_value);
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(0.1)]
        public double Step
        {
            get => _step;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Step must be greater than zero.");

                if (value > (_maximum - _minimum))
                    throw new ArgumentOutOfRangeException(nameof(value), "Step must not exceed the range size.");

                _step = value;
                _value = CoerceToStep(_value);
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(0.0)]
        public double Value
        {
            get => _value;
            set
            {
                double coerced = CoerceToStep(value);

                if (Math.Abs(_value - coerced) < 1e-12)
                    return;

                _value = coerced;
                Invalidate();
                OnValueChanged(EventArgs.Empty);
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(2)]
        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "DecimalPlaces must be greater than or equal to zero.");

                if (_decimalPlaces == value)
                    return;

                _decimalPlaces = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowValueText
        {
            get => _showValueText;
            set
            {
                if (_showValueText == value)
                    return;

                _showValueText = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowTicks
        {
            get => _showTicks;
            set
            {
                if (_showTicks == value)
                    return;

                _showTicks = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(10)]
        public int TickCount
        {
            get => _tickCount;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "TickCount must be greater than or equal to zero.");

                if (_tickCount == value)
                    return;

                _tickCount = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public Color TrackColor { get; set; } = Color.Silver;

        [Browsable(true)]
        [Category("Appearance")]
        public Color FillColor { get; set; } = Color.DodgerBlue;

        [Browsable(true)]
        [Category("Appearance")]
        public Color ThumbColor { get; set; } = Color.White;

        [Browsable(true)]
        [Category("Appearance")]
        public Color ThumbBorderColor { get; set; } = Color.Gray;

        [Browsable(true)]
        [Category("Appearance")]
        public Color TickColor { get; set; } = Color.Gray;

        [Browsable(true)]
        [Category("Appearance")]
        public Color ValueTextColor { get; set; } = Color.Black;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(12)]
        public int ThumbWidth { get; set; } = 12;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(16)]
        public int ThumbHeight { get; set; } = 16;

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(3)]
        public int TrackHeight { get; set; } = 3;

        public event EventHandler? ValueChanged;

        protected virtual void OnValueChanged(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle trackRect = GetTrackRectangle();
            Rectangle thumbRect = GetThumbRectangle();
            int thumbCenterX = thumbRect.Left + thumbRect.Width / 2;
            int trackCenterY = trackRect.Top + trackRect.Height / 2;

            using var trackBrush = new SolidBrush(TrackColor);
            using var fillBrush = new SolidBrush(FillColor);
            using var thumbBrush = new SolidBrush(ThumbColor);
            using var thumbBorderPen = new Pen(ThumbBorderColor);
            using var tickPen = new Pen(TickColor);
            using var textBrush = new SolidBrush(ValueTextColor);

            e.Graphics.FillRoundedRectangle(trackBrush, trackRect, TrackHeight / 2);

            Rectangle fillRect = new Rectangle(
                trackRect.Left,
                trackRect.Top,
                Math.Max(0, thumbCenterX - trackRect.Left),
                trackRect.Height);

            if (fillRect.Width > 0)
                e.Graphics.FillRoundedRectangle(fillBrush, fillRect, TrackHeight / 2);

            if (_showTicks && _tickCount > 0)
            {
                for (int i = 0; i <= _tickCount; i++)
                {
                    float ratio = _tickCount == 0 ? 0f : (float)i / _tickCount;
                    int x = trackRect.Left + (int)Math.Round(trackRect.Width * ratio, MidpointRounding.AwayFromZero);
                    e.Graphics.DrawLine(tickPen, x, trackCenterY - 12, x, trackCenterY - 7);
                }
            }

            e.Graphics.FillEllipse(thumbBrush, thumbRect);
            e.Graphics.DrawEllipse(thumbBorderPen, thumbRect);

            if (_showValueText)
            {
                string text = GetValueText();
                SizeF textSize = e.Graphics.MeasureString(text, Font);

                float textX = thumbCenterX - textSize.Width / 2f;
                float textY = -2;

                if (textX < 0)
                    textX = 0;

                if (textX + textSize.Width > Width)
                    textX = Width - textSize.Width;

                e.Graphics.DrawString(text, Font, textBrush, textX, textY);
            }

            if (Focused && ShowFocusCues)
                ControlPaint.DrawFocusRectangle(e.Graphics, ClientRectangle);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Left)
                return;

            Focus();
            _dragging = true;
            Capture = true;
            UpdateValueFromMouse(e.X);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!_dragging)
                return;

            UpdateValueFromMouse(e.X);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button != MouseButtons.Left)
                return;

            _dragging = false;
            Capture = false;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            //if (e.Delta > 0)
            //    Value += _step;
            //else if (e.Delta < 0)
            //    Value -= _step;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;

            return key == Keys.Left ||
                   key == Keys.Right ||
                   key == Keys.Home ||
                   key == Keys.End ||
                   base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Left:
                    Value -= _step;
                    e.Handled = true;
                    break;

                case Keys.Right:
                    Value += _step;
                    e.Handled = true;
                    break;

                case Keys.Home:
                    Value = _minimum;
                    e.Handled = true;
                    break;

                case Keys.End:
                    Value = _maximum;
                    e.Handled = true;
                    break;
            }
        }

        private void UpdateValueFromMouse(int mouseX)
        {
            Rectangle trackRect = GetTrackRectangle();

            if (trackRect.Width <= 0)
                return;

            int x = mouseX;

            if (x < trackRect.Left)
                x = trackRect.Left;

            if (x > trackRect.Right)
                x = trackRect.Right;

            double ratio = (double)(x - trackRect.Left) / trackRect.Width;
            double rawValue = _minimum + ((_maximum - _minimum) * ratio);

            Value = rawValue;
        }

        private Rectangle GetTrackRectangle()
        {
            int leftPadding = ThumbWidth / 2 + 4;
            int rightPadding = ThumbWidth / 2 + 4;
            int topOffset = _showValueText ? 14 : 4;
            int tickOffset = _showTicks ? 6 : 0;

            int y = topOffset + tickOffset + Math.Max(ThumbHeight / 2 - TrackHeight / 2, 0);
            int width = Math.Max(10, Width - leftPadding - rightPadding);

            return new Rectangle(leftPadding, y, width, TrackHeight);
        }

        private Rectangle GetThumbRectangle()
        {
            Rectangle trackRect = GetTrackRectangle();
            double ratio = GetRatio();

            int centerX = trackRect.Left + (int)Math.Round(trackRect.Width * ratio, MidpointRounding.AwayFromZero);
            int centerY = trackRect.Top + trackRect.Height / 2;

            return new Rectangle(
                centerX - ThumbWidth / 2,
                centerY - ThumbHeight / 2,
                ThumbWidth,
                ThumbHeight);
        }

        private double GetRatio()
        {
            if (_maximum <= _minimum)
                return 0;

            return (_value - _minimum) / (_maximum - _minimum);
        }

        private double CoerceToStep(double value)
        {
            if (value < _minimum)
                value = _minimum;

            if (value > _maximum)
                value = _maximum;

            int stepIndex = (int)Math.Round((value - _minimum) / _step, MidpointRounding.AwayFromZero);
            double snapped = _minimum + stepIndex * _step;

            if (snapped < _minimum)
                snapped = _minimum;

            if (snapped > _maximum)
                snapped = _maximum;

            return snapped;
        }

        private string GetValueText()
        {
            return _value.ToString("F" + _decimalPlaces);
        }
    }

    internal static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int radius)
        {
            if (radius <= 0)
            {
                graphics.FillRectangle(brush, bounds);
                return;
            }

            using var path = new System.Drawing.Drawing2D.GraphicsPath();

            int diameter = radius * 2;

            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            graphics.FillPath(brush, path);
        }
    }
}