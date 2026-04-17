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
            groupBox3 = new GroupBox();
            label20 = new Label();
            label19 = new Label();
            label18 = new Label();
            label17 = new Label();
            z0Slider = new TETMViewer.UI.DecimalSlider();
            label16 = new Label();
            y0Slider = new TETMViewer.UI.DecimalSlider();
            x0Slider = new TETMViewer.UI.DecimalSlider();
            label15 = new Label();
            qualitySlider = new TETMViewer.UI.DecimalSlider();
            label10 = new Label();
            timeGroupBox = new GroupBox();
            stopBtn = new Button();
            startBtn = new Button();
            stepTimeTextBox = new TETMViewer.UI.NumericTextBox();
            label9 = new Label();
            useTimeSimulCheckBox = new CheckBox();
            groupBox2 = new GroupBox();
            phaseTextBox = new TETMViewer.UI.NumericTextBox();
            label8 = new Label();
            crWaveFreqTextBox = new TextBox();
            label13 = new Label();
            label14 = new Label();
            label12 = new Label();
            waveFreqTextBox = new TETMViewer.UI.NumericTextBox();
            label11 = new Label();
            ampETextBox = new TETMViewer.UI.NumericTextBox();
            ampHTextBox = new TETMViewer.UI.NumericTextBox();
            label6 = new Label();
            label7 = new Label();
            label1 = new Label();
            waveTypeCb = new ComboBox();
            numNTextBox = new TETMViewer.UI.NumericTextBox();
            label2 = new Label();
            label3 = new Label();
            numMTextBox = new TETMViewer.UI.NumericTextBox();
            groupBox1 = new GroupBox();
            label4 = new Label();
            numBTextBox = new TETMViewer.UI.NumericTextBox();
            numATextBox = new TETMViewer.UI.NumericTextBox();
            label5 = new Label();
            pictureBox = new PictureBox();
            calcBtn = new Button();
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            YXspc = new StreamPlotControl();
            YZspc = new StreamPlotControl();
            ZXspc = new StreamPlotControl();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            groupBox3.SuspendLayout();
            timeGroupBox.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
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
            splitContainer.Panel1.AutoScroll = true;
            splitContainer.Panel1.Controls.Add(groupBox3);
            splitContainer.Panel1.Controls.Add(timeGroupBox);
            splitContainer.Panel1.Controls.Add(useTimeSimulCheckBox);
            splitContainer.Panel1.Controls.Add(groupBox2);
            splitContainer.Panel1.Controls.Add(groupBox1);
            splitContainer.Panel1.Controls.Add(pictureBox);
            splitContainer.Panel1.Controls.Add(calcBtn);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(splitContainer1);
            splitContainer.Size = new Size(1467, 875);
            splitContainer.SplitterDistance = 314;
            splitContainer.TabIndex = 1;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(label20);
            groupBox3.Controls.Add(label19);
            groupBox3.Controls.Add(label18);
            groupBox3.Controls.Add(label17);
            groupBox3.Controls.Add(z0Slider);
            groupBox3.Controls.Add(label16);
            groupBox3.Controls.Add(y0Slider);
            groupBox3.Controls.Add(x0Slider);
            groupBox3.Controls.Add(label15);
            groupBox3.Controls.Add(qualitySlider);
            groupBox3.Controls.Add(label10);
            groupBox3.Location = new Point(3, 626);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(311, 208);
            groupBox3.TabIndex = 18;
            groupBox3.TabStop = false;
            groupBox3.Text = "Построение";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(281, 171);
            label20.Name = "label20";
            label20.Size = new Size(24, 18);
            label20.TabIndex = 30;
            label20.Text = "см";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(281, 73);
            label19.Name = "label19";
            label19.Size = new Size(24, 18);
            label19.TabIndex = 29;
            label19.Text = "см";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(281, 124);
            label18.Name = "label18";
            label18.Size = new Size(24, 18);
            label18.TabIndex = 28;
            label18.Text = "см";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(38, 171);
            label17.Name = "label17";
            label17.Size = new Size(24, 18);
            label17.TabIndex = 27;
            label17.Text = "z0";
            // 
            // z0Slider
            // 
            z0Slider.BackColor = Color.Transparent;
            z0Slider.FillColor = Color.DodgerBlue;
            z0Slider.Location = new Point(68, 159);
            z0Slider.Maximum = 2D;
            z0Slider.Minimum = 1D;
            z0Slider.Name = "z0Slider";
            z0Slider.Size = new Size(208, 43);
            z0Slider.Step = 0.01D;
            z0Slider.TabIndex = 26;
            z0Slider.ThumbBorderColor = Color.Gray;
            z0Slider.ThumbColor = Color.White;
            z0Slider.TickColor = Color.Gray;
            z0Slider.TrackColor = Color.Silver;
            z0Slider.Value = 1.5D;
            z0Slider.ValueTextColor = Color.Black;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(38, 124);
            label16.Name = "label16";
            label16.Size = new Size(24, 18);
            label16.TabIndex = 25;
            label16.Text = "y0";
            // 
            // y0Slider
            // 
            y0Slider.BackColor = Color.Transparent;
            y0Slider.FillColor = Color.DodgerBlue;
            y0Slider.Location = new Point(68, 109);
            y0Slider.Maximum = 2D;
            y0Slider.Minimum = 1D;
            y0Slider.Name = "y0Slider";
            y0Slider.Size = new Size(208, 43);
            y0Slider.Step = 0.01D;
            y0Slider.TabIndex = 24;
            y0Slider.ThumbBorderColor = Color.Gray;
            y0Slider.ThumbColor = Color.White;
            y0Slider.TickColor = Color.Gray;
            y0Slider.TrackColor = Color.Silver;
            y0Slider.Value = 1.5D;
            y0Slider.ValueTextColor = Color.Black;
            // 
            // x0Slider
            // 
            x0Slider.BackColor = Color.Transparent;
            x0Slider.FillColor = Color.DodgerBlue;
            x0Slider.Location = new Point(68, 60);
            x0Slider.Maximum = 2D;
            x0Slider.Minimum = 1D;
            x0Slider.Name = "x0Slider";
            x0Slider.Size = new Size(208, 43);
            x0Slider.Step = 0.01D;
            x0Slider.TabIndex = 23;
            x0Slider.ThumbBorderColor = Color.Gray;
            x0Slider.ThumbColor = Color.White;
            x0Slider.TickColor = Color.Gray;
            x0Slider.TrackColor = Color.Silver;
            x0Slider.Value = 1.5D;
            x0Slider.ValueTextColor = Color.Black;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(38, 73);
            label15.Name = "label15";
            label15.Size = new Size(24, 18);
            label15.TabIndex = 22;
            label15.Text = "x0";
            // 
            // qualitySlider
            // 
            qualitySlider.BackColor = Color.Transparent;
            qualitySlider.DecimalPlaces = 1;
            qualitySlider.FillColor = Color.DodgerBlue;
            qualitySlider.Location = new Point(68, 21);
            qualitySlider.Maximum = 2D;
            qualitySlider.Minimum = 1D;
            qualitySlider.Name = "qualitySlider";
            qualitySlider.Size = new Size(243, 43);
            qualitySlider.TabIndex = 21;
            qualitySlider.ThumbBorderColor = Color.Gray;
            qualitySlider.ThumbColor = Color.White;
            qualitySlider.TickColor = Color.Gray;
            qualitySlider.TrackColor = Color.Silver;
            qualitySlider.Value = 1.5D;
            qualitySlider.ValueTextColor = Color.Black;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(6, 33);
            label10.Name = "label10";
            label10.Size = new Size(56, 18);
            label10.TabIndex = 20;
            label10.Text = "Кач-во";
            // 
            // timeGroupBox
            // 
            timeGroupBox.Controls.Add(stopBtn);
            timeGroupBox.Controls.Add(startBtn);
            timeGroupBox.Controls.Add(stepTimeTextBox);
            timeGroupBox.Controls.Add(label9);
            timeGroupBox.Enabled = false;
            timeGroupBox.Location = new Point(3, 540);
            timeGroupBox.Name = "timeGroupBox";
            timeGroupBox.Size = new Size(311, 85);
            timeGroupBox.TabIndex = 14;
            timeGroupBox.TabStop = false;
            timeGroupBox.Text = "Время";
            // 
            // stopBtn
            // 
            stopBtn.Enabled = false;
            stopBtn.Location = new Point(139, 51);
            stopBtn.Name = "stopBtn";
            stopBtn.Size = new Size(115, 29);
            stopBtn.TabIndex = 19;
            stopBtn.Text = "Stop";
            stopBtn.UseVisualStyleBackColor = true;
            stopBtn.Click += stopBtn_Click;
            // 
            // startBtn
            // 
            startBtn.Location = new Point(6, 51);
            startBtn.Name = "startBtn";
            startBtn.Size = new Size(118, 29);
            startBtn.TabIndex = 18;
            startBtn.Text = "Start";
            startBtn.UseVisualStyleBackColor = true;
            startBtn.Click += startBtn_Click;
            // 
            // stepTimeTextBox
            // 
            stepTimeTextBox.CheckIfEmpty = false;
            stepTimeTextBox.Location = new Point(55, 20);
            stepTimeTextBox.MaxLength = 5;
            stepTimeTextBox.Name = "stepTimeTextBox";
            stepTimeTextBox.Size = new Size(69, 25);
            stepTimeTextBox.TabIndex = 15;
            stepTimeTextBox.Text = "0,01";
            stepTimeTextBox.TextAlign = HorizontalAlignment.Center;
            stepTimeTextBox.ValueType = UI.NumericTextBox.NumericType.Double;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(9, 21);
            label9.Name = "label9";
            label9.Size = new Size(40, 18);
            label9.TabIndex = 14;
            label9.Text = "Шаг:";
            // 
            // useTimeSimulCheckBox
            // 
            useTimeSimulCheckBox.AutoSize = true;
            useTimeSimulCheckBox.Location = new Point(9, 520);
            useTimeSimulCheckBox.Name = "useTimeSimulCheckBox";
            useTimeSimulCheckBox.Size = new Size(174, 22);
            useTimeSimulCheckBox.TabIndex = 17;
            useTimeSimulCheckBox.Text = "Использовать время";
            useTimeSimulCheckBox.UseVisualStyleBackColor = true;
            useTimeSimulCheckBox.CheckedChanged += useTimeSimulCheckBox_CheckedChanged;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(phaseTextBox);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(crWaveFreqTextBox);
            groupBox2.Controls.Add(label13);
            groupBox2.Controls.Add(label14);
            groupBox2.Controls.Add(label12);
            groupBox2.Controls.Add(waveFreqTextBox);
            groupBox2.Controls.Add(label11);
            groupBox2.Controls.Add(ampETextBox);
            groupBox2.Controls.Add(ampHTextBox);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(waveTypeCb);
            groupBox2.Controls.Add(numNTextBox);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(numMTextBox);
            groupBox2.Location = new Point(3, 297);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(311, 217);
            groupBox2.TabIndex = 13;
            groupBox2.TabStop = false;
            groupBox2.Text = "Параметры волны";
            // 
            // phaseTextBox
            // 
            phaseTextBox.CheckIfEmpty = false;
            phaseTextBox.Location = new Point(102, 184);
            phaseTextBox.MaxLength = 8;
            phaseTextBox.Name = "phaseTextBox";
            phaseTextBox.Size = new Size(151, 25);
            phaseTextBox.TabIndex = 19;
            phaseTextBox.Text = "0";
            phaseTextBox.TextAlign = HorizontalAlignment.Center;
            phaseTextBox.ValueType = UI.NumericTextBox.NumericType.Double;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(8, 187);
            label8.Name = "label8";
            label8.Size = new Size(40, 18);
            label8.TabIndex = 18;
            label8.Text = "Фаза";
            // 
            // crWaveFreqTextBox
            // 
            crWaveFreqTextBox.Location = new Point(102, 150);
            crWaveFreqTextBox.Name = "crWaveFreqTextBox";
            crWaveFreqTextBox.ReadOnly = true;
            crWaveFreqTextBox.Size = new Size(151, 25);
            crWaveFreqTextBox.TabIndex = 15;
            crWaveFreqTextBox.TextAlign = HorizontalAlignment.Center;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(259, 153);
            label13.Name = "label13";
            label13.Size = new Size(32, 18);
            label13.TabIndex = 17;
            label13.Text = "МГц";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(6, 153);
            label14.Name = "label14";
            label14.Size = new Size(96, 18);
            label14.TabIndex = 15;
            label14.Text = "Кр. частота";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(259, 124);
            label12.Name = "label12";
            label12.Size = new Size(32, 18);
            label12.TabIndex = 14;
            label12.Text = "МГц";
            // 
            // waveFreqTextBox
            // 
            waveFreqTextBox.CheckIfEmpty = false;
            waveFreqTextBox.Location = new Point(102, 121);
            waveFreqTextBox.MaxLength = 8;
            waveFreqTextBox.Name = "waveFreqTextBox";
            waveFreqTextBox.Size = new Size(151, 25);
            waveFreqTextBox.TabIndex = 13;
            waveFreqTextBox.Text = "10000";
            waveFreqTextBox.TextAlign = HorizontalAlignment.Center;
            waveFreqTextBox.ValueType = UI.NumericTextBox.NumericType.Double;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(6, 124);
            label11.Name = "label11";
            label11.Size = new Size(64, 18);
            label11.TabIndex = 12;
            label11.Text = "Частота";
            // 
            // ampETextBox
            // 
            ampETextBox.CheckIfEmpty = false;
            ampETextBox.Location = new Point(55, 86);
            ampETextBox.MaxLength = 5;
            ampETextBox.Name = "ampETextBox";
            ampETextBox.Size = new Size(69, 25);
            ampETextBox.TabIndex = 9;
            ampETextBox.Text = "1";
            ampETextBox.TextAlign = HorizontalAlignment.Center;
            ampETextBox.ValueType = UI.NumericTextBox.NumericType.Double;
            // 
            // ampHTextBox
            // 
            ampHTextBox.CheckIfEmpty = false;
            ampHTextBox.Location = new Point(185, 86);
            ampHTextBox.MaxLength = 5;
            ampHTextBox.Name = "ampHTextBox";
            ampHTextBox.Size = new Size(69, 25);
            ampHTextBox.TabIndex = 11;
            ampHTextBox.Text = "1";
            ampHTextBox.TextAlign = HorizontalAlignment.Center;
            ampHTextBox.ValueType = UI.NumericTextBox.NumericType.Double;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(6, 89);
            label6.Name = "label6";
            label6.Size = new Size(56, 18);
            label6.TabIndex = 8;
            label6.Text = "E_0 = ";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(139, 89);
            label7.Name = "label7";
            label7.Size = new Size(56, 18);
            label7.TabIndex = 10;
            label7.Text = "H_0 = ";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 21);
            label1.Name = "label1";
            label1.Size = new Size(88, 18);
            label1.TabIndex = 3;
            label1.Text = "Тип волны:";
            // 
            // waveTypeCb
            // 
            waveTypeCb.DropDownStyle = ComboBoxStyle.DropDownList;
            waveTypeCb.FormattingEnabled = true;
            waveTypeCb.Location = new Point(102, 18);
            waveTypeCb.Name = "waveTypeCb";
            waveTypeCb.Size = new Size(151, 26);
            waveTypeCb.TabIndex = 2;
            // 
            // numNTextBox
            // 
            numNTextBox.CheckIfEmpty = false;
            numNTextBox.Location = new Point(184, 50);
            numNTextBox.MaxLength = 2;
            numNTextBox.Name = "numNTextBox";
            numNTextBox.Size = new Size(69, 25);
            numNTextBox.TabIndex = 7;
            numNTextBox.Text = "0";
            numNTextBox.TextAlign = HorizontalAlignment.Center;
            numNTextBox.ValueType = UI.NumericTextBox.NumericType.Int;
            numNTextBox.TextChanged += numABMNTextBox_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 53);
            label2.Name = "label2";
            label2.Size = new Size(40, 18);
            label2.TabIndex = 4;
            label2.Text = "m = ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(138, 53);
            label3.Name = "label3";
            label3.Size = new Size(40, 18);
            label3.TabIndex = 6;
            label3.Text = "n = ";
            // 
            // numMTextBox
            // 
            numMTextBox.CheckIfEmpty = false;
            numMTextBox.Location = new Point(54, 50);
            numMTextBox.MaxLength = 2;
            numMTextBox.Name = "numMTextBox";
            numMTextBox.Size = new Size(69, 25);
            numMTextBox.TabIndex = 5;
            numMTextBox.Text = "1";
            numMTextBox.TextAlign = HorizontalAlignment.Center;
            numMTextBox.ValueType = UI.NumericTextBox.NumericType.Int;
            numMTextBox.TextChanged += numABMNTextBox_TextChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(numBTextBox);
            groupBox1.Controls.Add(numATextBox);
            groupBox1.Controls.Add(label5);
            groupBox1.Location = new Point(3, 240);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(311, 51);
            groupBox1.TabIndex = 12;
            groupBox1.TabStop = false;
            groupBox1.Text = "Параметры волновода";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 21);
            label4.Name = "label4";
            label4.Size = new Size(40, 18);
            label4.TabIndex = 8;
            label4.Text = "a = ";
            // 
            // numBTextBox
            // 
            numBTextBox.CheckIfEmpty = false;
            numBTextBox.Location = new Point(182, 18);
            numBTextBox.MaxLength = 5;
            numBTextBox.Name = "numBTextBox";
            numBTextBox.Size = new Size(69, 25);
            numBTextBox.TabIndex = 11;
            numBTextBox.Text = "1";
            numBTextBox.TextAlign = HorizontalAlignment.Center;
            numBTextBox.ValueType = UI.NumericTextBox.NumericType.Double;
            numBTextBox.TextChanged += numABMNTextBox_TextChanged;
            // 
            // numATextBox
            // 
            numATextBox.CheckIfEmpty = false;
            numATextBox.Location = new Point(52, 18);
            numATextBox.MaxLength = 5;
            numATextBox.Name = "numATextBox";
            numATextBox.Size = new Size(69, 25);
            numATextBox.TabIndex = 9;
            numATextBox.Text = "2,3";
            numATextBox.TextAlign = HorizontalAlignment.Center;
            numATextBox.ValueType = UI.NumericTextBox.NumericType.Double;
            numATextBox.TextChanged += numABMNTextBox_TextChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(136, 21);
            label5.Name = "label5";
            label5.Size = new Size(40, 18);
            label5.TabIndex = 10;
            label5.Text = "b = ";
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
            calcBtn.Location = new Point(3, 840);
            calcBtn.Name = "calcBtn";
            calcBtn.Size = new Size(308, 32);
            calcBtn.TabIndex = 0;
            calcBtn.Text = "Построить";
            calcBtn.UseVisualStyleBackColor = true;
            calcBtn.Click += calcBtn_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(ZXspc);
            splitContainer1.Size = new Size(1149, 875);
            splitContainer1.SplitterDistance = 573;
            splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(YXspc);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(YZspc);
            splitContainer2.Size = new Size(573, 875);
            splitContainer2.SplitterDistance = 430;
            splitContainer2.TabIndex = 0;
            // 
            // YXspc
            // 
            YXspc.ArrowLengthPx = 8F;
            YXspc.ArrowWidthPx = 8F;
            YXspc.Dock = DockStyle.Fill;
            YXspc.LineWidth = 2F;
            YXspc.Location = new Point(0, 0);
            YXspc.Name = "YXspc";
            YXspc.PlotTitle = "Y(X)";
            YXspc.ShowArrows = true;
            YXspc.Size = new Size(573, 430);
            YXspc.TabIndex = 1;
            YXspc.XLabel = "X";
            YXspc.YLabel = "Y";
            // 
            // YZspc
            // 
            YZspc.ArrowLengthPx = 8F;
            YZspc.ArrowWidthPx = 8F;
            YZspc.Dock = DockStyle.Fill;
            YZspc.LineWidth = 2F;
            YZspc.Location = new Point(0, 0);
            YZspc.Name = "YZspc";
            YZspc.PlotTitle = "Y(Z)";
            YZspc.ShowArrows = true;
            YZspc.Size = new Size(573, 441);
            YZspc.TabIndex = 1;
            YZspc.XLabel = "Z";
            YZspc.YLabel = "Y";
            // 
            // ZXspc
            // 
            ZXspc.ArrowLengthPx = 8F;
            ZXspc.ArrowWidthPx = 8F;
            ZXspc.Dock = DockStyle.Fill;
            ZXspc.LineWidth = 2F;
            ZXspc.Location = new Point(0, 0);
            ZXspc.Name = "ZXspc";
            ZXspc.PlotTitle = "Z(X)";
            ZXspc.ShowArrows = true;
            ZXspc.Size = new Size(572, 875);
            ZXspc.TabIndex = 0;
            ZXspc.XLabel = "X";
            ZXspc.YLabel = "Z";
            // 
            // Form
            // 
            AutoScaleDimensions = new SizeF(8F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1467, 875);
            Controls.Add(splitContainer);
            Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            MinimumSize = new Size(1485, 922);
            Name = "Form";
            Text = "TE-TM Viewer";
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel1.PerformLayout();
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            timeGroupBox.ResumeLayout(false);
            timeGroupBox.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer;
        private Button calcBtn;
        private StreamPlotControl streamPlotControl1;
        private StreamPlotControl streamPlotControl3;
        private PictureBox pictureBox;
        private Label label1;
        private ComboBox waveTypeCb;
        private Label label2;
        private UI.NumericTextBox numNTextBox;
        private Label label3;
        private UI.NumericTextBox numMTextBox;
        private GroupBox groupBox1;
        private UI.NumericTextBox numBTextBox;
        private Label label5;
        private UI.NumericTextBox numATextBox;
        private Label label4;
        private GroupBox timeGroupBox;
        private UI.NumericTextBox stepTimeTextBox;
        private Label label9;
        private GroupBox groupBox2;
        private UI.NumericTextBox ampETextBox;
        private UI.NumericTextBox ampHTextBox;
        private Label label6;
        private Label label7;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private StreamPlotControl streamPlotControl4;
        private StreamPlotControl ZXspc;
        private StreamPlotControl YXspc;
        private StreamPlotControl YZspc;
        private Label label12;
        private UI.NumericTextBox waveFreqTextBox;
        private Label label11;
        private Label label13;
        private Label label14;
        private TextBox crWaveFreqTextBox;
        private CheckBox useTimeSimulCheckBox;
        private Button stopBtn;
        private Button startBtn;
        private Label label8;
        private UI.NumericTextBox phaseTextBox;
        private GroupBox groupBox3;
        private UI.DecimalSlider qualitySlider;
        private Label label10;
        private Label label17;
        private UI.DecimalSlider z0Slider;
        private Label label16;
        private UI.DecimalSlider y0Slider;
        private UI.DecimalSlider x0Slider;
        private Label label15;
        private Label label20;
        private Label label19;
        private Label label18;
    }
}
