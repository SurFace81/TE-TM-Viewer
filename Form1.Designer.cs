namespace TETMViewer
{
    partial class Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form));
            splitContainer = new SplitContainer();
            pictureBox = new PictureBox();
            calcBtn = new Button();
            YZspc = new StreamPlotControl();
            YXspc = new StreamPlotControl();
            ZXspc = new StreamPlotControl();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.FixedPanel = FixedPanel.Panel1;
            splitContainer.IsSplitterFixed = true;
            splitContainer.Location = new Point(0, 0);
            splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(pictureBox);
            splitContainer.Panel1.Controls.Add(calcBtn);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(YZspc);
            splitContainer.Panel2.Controls.Add(YXspc);
            splitContainer.Panel2.Controls.Add(ZXspc);
            splitContainer.Size = new Size(1445, 746);
            splitContainer.SplitterDistance = 314;
            splitContainer.TabIndex = 1;
            // 
            // pictureBox
            // 
            pictureBox.BackgroundImage = (Image)resources.GetObject("pictureBox.BackgroundImage");
            pictureBox.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox.Dock = DockStyle.Top;
            pictureBox.Location = new Point(0, 0);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(314, 234);
            pictureBox.TabIndex = 1;
            pictureBox.TabStop = false;
            // 
            // calcBtn
            // 
            calcBtn.Location = new Point(60, 700);
            calcBtn.Name = "calcBtn";
            calcBtn.Size = new Size(182, 32);
            calcBtn.TabIndex = 0;
            calcBtn.Text = "Построить";
            calcBtn.UseVisualStyleBackColor = true;
            calcBtn.Click += calcBtn_Click;
            // 
            // YZspc
            // 
            YZspc.ArrowLengthPx = 8F;
            YZspc.ArrowWidthPx = 8F;
            YZspc.LineWidth = 1.5F;
            YZspc.Location = new Point(3, 377);
            YZspc.Name = "YZspc";
            YZspc.PlotTitle = "";
            YZspc.ShowArrows = true;
            YZspc.Size = new Size(561, 372);
            YZspc.TabIndex = 3;
            YZspc.XLabel = "";
            YZspc.YLabel = "";
            // 
            // YXspc
            // 
            YXspc.ArrowLengthPx = 8F;
            YXspc.ArrowWidthPx = 8F;
            YXspc.LineWidth = 1.5F;
            YXspc.Location = new Point(3, 0);
            YXspc.Name = "YXspc";
            YXspc.PlotTitle = "";
            YXspc.ShowArrows = true;
            YXspc.Size = new Size(561, 371);
            YXspc.TabIndex = 2;
            YXspc.XLabel = "";
            YXspc.YLabel = "";
            // 
            // ZXspc
            // 
            ZXspc.ArrowLengthPx = 8F;
            ZXspc.ArrowWidthPx = 8F;
            ZXspc.LineWidth = 1.5F;
            ZXspc.Location = new Point(570, 0);
            ZXspc.Name = "ZXspc";
            ZXspc.PlotTitle = "";
            ZXspc.ShowArrows = true;
            ZXspc.Size = new Size(561, 749);
            ZXspc.TabIndex = 1;
            ZXspc.XLabel = "";
            ZXspc.YLabel = "";
            // 
            // Form
            // 
            AutoScaleDimensions = new SizeF(8F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1445, 746);
            Controls.Add(splitContainer);
            Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            MaximumSize = new Size(1463, 793);
            MinimumSize = new Size(1463, 793);
            Name = "Form";
            Text = "TE-TM Viewer";
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer;
        private Button calcBtn;
        private StreamPlotControl streamPlotControl1;
        private StreamPlotControl streamPlotControl3;
        private StreamPlotControl ZXspc;
        private PictureBox pictureBox;
        private StreamPlotControl YZspc;
        private StreamPlotControl YXspc;
    }
}
