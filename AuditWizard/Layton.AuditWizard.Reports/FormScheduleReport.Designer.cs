namespace Layton.AuditWizard.Reports
{
    partial class FormScheduleReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnRemoveExisting = new System.Windows.Forms.Button();
            this.gbMain = new System.Windows.Forms.GroupBox();
            this.nudStartTime = new System.Windows.Forms.DateTimePicker();
            this.lbAllStartTime = new System.Windows.Forms.Label();
            this.lbAllScheduleReport = new System.Windows.Forms.Label();
            this.cmbScheduleInterval = new System.Windows.Forms.ComboBox();
            this.gbMonthly = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lbMonthlyMonths = new System.Windows.Forms.Label();
            this.nudMonthlyFreq = new System.Windows.Forms.NumericUpDown();
            this.gbWeekly = new System.Windows.Forms.GroupBox();
            this.cbWeeklySat = new System.Windows.Forms.CheckBox();
            this.cbWeeklySun = new System.Windows.Forms.CheckBox();
            this.cbWeeklyFri = new System.Windows.Forms.CheckBox();
            this.cbWeeklyThu = new System.Windows.Forms.CheckBox();
            this.cbWeeklyWed = new System.Windows.Forms.CheckBox();
            this.cbWeeklyTue = new System.Windows.Forms.CheckBox();
            this.cbWeeklyMon = new System.Windows.Forms.CheckBox();
            this.lbWeeklyWeeks = new System.Windows.Forms.Label();
            this.nudWeeklyFreq = new System.Windows.Forms.NumericUpDown();
            this.lbWeeklyEvery = new System.Windows.Forms.Label();
            this.gbDaily = new System.Windows.Forms.GroupBox();
            this.lbDailyDays = new System.Windows.Forms.Label();
            this.lbDailyEvery = new System.Windows.Forms.Label();
            this.nudDailyFreq = new System.Windows.Forms.NumericUpDown();
            this.gbOnce = new System.Windows.Forms.GroupBox();
            this.dtpOnce = new System.Windows.Forms.DateTimePicker();
            this.lbOnceRunOn = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bnCreateSchedule = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.gbMain.SuspendLayout();
            this.gbMonthly.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMonthlyFreq)).BeginInit();
            this.gbWeekly.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWeeklyFreq)).BeginInit();
            this.gbDaily.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDailyFreq)).BeginInit();
            this.gbOnce.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(312, 443);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 12;
            this.bnCancel.Text = "Close";
            this.bnCancel.UseVisualStyleBackColor = true;
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // bnRemoveExisting
            // 
            this.bnRemoveExisting.Location = new System.Drawing.Point(87, 58);
            this.bnRemoveExisting.Name = "bnRemoveExisting";
            this.bnRemoveExisting.Size = new System.Drawing.Size(200, 23);
            this.bnRemoveExisting.TabIndex = 13;
            this.bnRemoveExisting.Text = "Remove All Existing Schedules";
            this.bnRemoveExisting.UseVisualStyleBackColor = true;
            this.bnRemoveExisting.Click += new System.EventHandler(this.bnRemoveExisting_Click);
            // 
            // gbMain
            // 
            this.gbMain.Controls.Add(this.bnCreateSchedule);
            this.gbMain.Controls.Add(this.nudStartTime);
            this.gbMain.Controls.Add(this.lbAllStartTime);
            this.gbMain.Controls.Add(this.lbAllScheduleReport);
            this.gbMain.Controls.Add(this.cmbScheduleInterval);
            this.gbMain.Controls.Add(this.gbWeekly);
            this.gbMain.Controls.Add(this.gbOnce);
            this.gbMain.Controls.Add(this.gbMonthly);
            this.gbMain.Controls.Add(this.gbDaily);
            this.gbMain.Location = new System.Drawing.Point(12, 22);
            this.gbMain.Name = "gbMain";
            this.gbMain.Size = new System.Drawing.Size(375, 282);
            this.gbMain.TabIndex = 14;
            this.gbMain.TabStop = false;
            this.gbMain.Text = "Create schedule for";
            // 
            // nudStartTime
            // 
            this.nudStartTime.CustomFormat = "HH:mm";
            this.nudStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.nudStartTime.Location = new System.Drawing.Point(142, 59);
            this.nudStartTime.Name = "nudStartTime";
            this.nudStartTime.ShowUpDown = true;
            this.nudStartTime.Size = new System.Drawing.Size(64, 21);
            this.nudStartTime.TabIndex = 19;
            // 
            // lbAllStartTime
            // 
            this.lbAllStartTime.AutoSize = true;
            this.lbAllStartTime.Location = new System.Drawing.Point(139, 36);
            this.lbAllStartTime.Name = "lbAllStartTime";
            this.lbAllStartTime.Size = new System.Drawing.Size(67, 13);
            this.lbAllStartTime.TabIndex = 17;
            this.lbAllStartTime.Text = "Start Time";
            // 
            // lbAllScheduleReport
            // 
            this.lbAllScheduleReport.AutoSize = true;
            this.lbAllScheduleReport.Location = new System.Drawing.Point(23, 36);
            this.lbAllScheduleReport.Name = "lbAllScheduleReport";
            this.lbAllScheduleReport.Size = new System.Drawing.Size(101, 13);
            this.lbAllScheduleReport.TabIndex = 14;
            this.lbAllScheduleReport.Text = "Schedule Report";
            // 
            // cmbScheduleInterval
            // 
            this.cmbScheduleInterval.FormattingEnabled = true;
            this.cmbScheduleInterval.Items.AddRange(new object[] {
            "Daily",
            "Weekly",
            "Monthly",
            "Once"});
            this.cmbScheduleInterval.Location = new System.Drawing.Point(26, 59);
            this.cmbScheduleInterval.Name = "cmbScheduleInterval";
            this.cmbScheduleInterval.Size = new System.Drawing.Size(102, 21);
            this.cmbScheduleInterval.TabIndex = 11;
            this.cmbScheduleInterval.SelectedIndexChanged += new System.EventHandler(this.cmbScheduleInterval_SelectedIndexChanged);
            // 
            // gbMonthly
            // 
            this.gbMonthly.Controls.Add(this.label2);
            this.gbMonthly.Controls.Add(this.lbMonthlyMonths);
            this.gbMonthly.Controls.Add(this.nudMonthlyFreq);
            this.gbMonthly.Location = new System.Drawing.Point(26, 106);
            this.gbMonthly.Name = "gbMonthly";
            this.gbMonthly.Size = new System.Drawing.Size(322, 73);
            this.gbMonthly.TabIndex = 16;
            this.gbMonthly.TabStop = false;
            this.gbMonthly.Text = "Schedule Report Monthly";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(70, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Day";
            // 
            // lbMonthlyMonths
            // 
            this.lbMonthlyMonths.AutoSize = true;
            this.lbMonthlyMonths.Location = new System.Drawing.Point(173, 29);
            this.lbMonthlyMonths.Name = "lbMonthlyMonths";
            this.lbMonthlyMonths.Size = new System.Drawing.Size(80, 13);
            this.lbMonthlyMonths.TabIndex = 17;
            this.lbMonthlyMonths.Text = "of the month";
            // 
            // nudMonthlyFreq
            // 
            this.nudMonthlyFreq.Location = new System.Drawing.Point(107, 26);
            this.nudMonthlyFreq.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nudMonthlyFreq.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMonthlyFreq.Name = "nudMonthlyFreq";
            this.nudMonthlyFreq.Size = new System.Drawing.Size(58, 21);
            this.nudMonthlyFreq.TabIndex = 16;
            this.nudMonthlyFreq.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // gbWeekly
            // 
            this.gbWeekly.Controls.Add(this.cbWeeklySat);
            this.gbWeekly.Controls.Add(this.cbWeeklySun);
            this.gbWeekly.Controls.Add(this.cbWeeklyFri);
            this.gbWeekly.Controls.Add(this.cbWeeklyThu);
            this.gbWeekly.Controls.Add(this.cbWeeklyWed);
            this.gbWeekly.Controls.Add(this.cbWeeklyTue);
            this.gbWeekly.Controls.Add(this.cbWeeklyMon);
            this.gbWeekly.Controls.Add(this.lbWeeklyWeeks);
            this.gbWeekly.Controls.Add(this.nudWeeklyFreq);
            this.gbWeekly.Controls.Add(this.lbWeeklyEvery);
            this.gbWeekly.Location = new System.Drawing.Point(26, 106);
            this.gbWeekly.Name = "gbWeekly";
            this.gbWeekly.Size = new System.Drawing.Size(322, 152);
            this.gbWeekly.TabIndex = 15;
            this.gbWeekly.TabStop = false;
            this.gbWeekly.Text = "Schedule Report Weekly";
            // 
            // cbWeeklySat
            // 
            this.cbWeeklySat.AutoSize = true;
            this.cbWeeklySat.Location = new System.Drawing.Point(259, 22);
            this.cbWeeklySat.Name = "cbWeeklySat";
            this.cbWeeklySat.Size = new System.Drawing.Size(45, 17);
            this.cbWeeklySat.TabIndex = 15;
            this.cbWeeklySat.Text = "Sat";
            this.cbWeeklySat.UseVisualStyleBackColor = true;
            // 
            // cbWeeklySun
            // 
            this.cbWeeklySun.AutoSize = true;
            this.cbWeeklySun.Location = new System.Drawing.Point(259, 45);
            this.cbWeeklySun.Name = "cbWeeklySun";
            this.cbWeeklySun.Size = new System.Drawing.Size(48, 17);
            this.cbWeeklySun.TabIndex = 14;
            this.cbWeeklySun.Text = "Sun";
            this.cbWeeklySun.UseVisualStyleBackColor = true;
            // 
            // cbWeeklyFri
            // 
            this.cbWeeklyFri.AutoSize = true;
            this.cbWeeklyFri.Location = new System.Drawing.Point(192, 114);
            this.cbWeeklyFri.Name = "cbWeeklyFri";
            this.cbWeeklyFri.Size = new System.Drawing.Size(40, 17);
            this.cbWeeklyFri.TabIndex = 13;
            this.cbWeeklyFri.Text = "Fri";
            this.cbWeeklyFri.UseVisualStyleBackColor = true;
            // 
            // cbWeeklyThu
            // 
            this.cbWeeklyThu.AutoSize = true;
            this.cbWeeklyThu.Location = new System.Drawing.Point(192, 91);
            this.cbWeeklyThu.Name = "cbWeeklyThu";
            this.cbWeeklyThu.Size = new System.Drawing.Size(47, 17);
            this.cbWeeklyThu.TabIndex = 12;
            this.cbWeeklyThu.Text = "Thu";
            this.cbWeeklyThu.UseVisualStyleBackColor = true;
            // 
            // cbWeeklyWed
            // 
            this.cbWeeklyWed.AutoSize = true;
            this.cbWeeklyWed.Location = new System.Drawing.Point(192, 68);
            this.cbWeeklyWed.Name = "cbWeeklyWed";
            this.cbWeeklyWed.Size = new System.Drawing.Size(51, 17);
            this.cbWeeklyWed.TabIndex = 11;
            this.cbWeeklyWed.Text = "Wed";
            this.cbWeeklyWed.UseVisualStyleBackColor = true;
            // 
            // cbWeeklyTue
            // 
            this.cbWeeklyTue.AutoSize = true;
            this.cbWeeklyTue.Location = new System.Drawing.Point(192, 45);
            this.cbWeeklyTue.Name = "cbWeeklyTue";
            this.cbWeeklyTue.Size = new System.Drawing.Size(47, 17);
            this.cbWeeklyTue.TabIndex = 10;
            this.cbWeeklyTue.Text = "Tue";
            this.cbWeeklyTue.UseVisualStyleBackColor = true;
            // 
            // cbWeeklyMon
            // 
            this.cbWeeklyMon.AutoSize = true;
            this.cbWeeklyMon.Checked = true;
            this.cbWeeklyMon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbWeeklyMon.Location = new System.Drawing.Point(192, 22);
            this.cbWeeklyMon.Name = "cbWeeklyMon";
            this.cbWeeklyMon.Size = new System.Drawing.Size(49, 17);
            this.cbWeeklyMon.TabIndex = 9;
            this.cbWeeklyMon.Text = "Mon";
            this.cbWeeklyMon.UseVisualStyleBackColor = true;
            // 
            // lbWeeklyWeeks
            // 
            this.lbWeeklyWeeks.AutoSize = true;
            this.lbWeeklyWeeks.Location = new System.Drawing.Point(124, 26);
            this.lbWeeklyWeeks.Name = "lbWeeklyWeeks";
            this.lbWeeklyWeeks.Size = new System.Drawing.Size(53, 13);
            this.lbWeeklyWeeks.TabIndex = 8;
            this.lbWeeklyWeeks.Text = "week(s)";
            // 
            // nudWeeklyFreq
            // 
            this.nudWeeklyFreq.Location = new System.Drawing.Point(60, 23);
            this.nudWeeklyFreq.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudWeeklyFreq.Name = "nudWeeklyFreq";
            this.nudWeeklyFreq.Size = new System.Drawing.Size(58, 21);
            this.nudWeeklyFreq.TabIndex = 6;
            this.nudWeeklyFreq.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lbWeeklyEvery
            // 
            this.lbWeeklyEvery.AutoSize = true;
            this.lbWeeklyEvery.Location = new System.Drawing.Point(15, 26);
            this.lbWeeklyEvery.Name = "lbWeeklyEvery";
            this.lbWeeklyEvery.Size = new System.Drawing.Size(40, 13);
            this.lbWeeklyEvery.TabIndex = 7;
            this.lbWeeklyEvery.Text = "Every";
            // 
            // gbDaily
            // 
            this.gbDaily.Controls.Add(this.lbDailyDays);
            this.gbDaily.Controls.Add(this.lbDailyEvery);
            this.gbDaily.Controls.Add(this.nudDailyFreq);
            this.gbDaily.Location = new System.Drawing.Point(26, 106);
            this.gbDaily.Name = "gbDaily";
            this.gbDaily.Size = new System.Drawing.Size(322, 73);
            this.gbDaily.TabIndex = 12;
            this.gbDaily.TabStop = false;
            this.gbDaily.Text = "Schedule Report Daily";
            // 
            // lbDailyDays
            // 
            this.lbDailyDays.AutoSize = true;
            this.lbDailyDays.Location = new System.Drawing.Point(127, 34);
            this.lbDailyDays.Name = "lbDailyDays";
            this.lbDailyDays.Size = new System.Drawing.Size(44, 13);
            this.lbDailyDays.TabIndex = 5;
            this.lbDailyDays.Text = "day(s)";
            // 
            // lbDailyEvery
            // 
            this.lbDailyEvery.AutoSize = true;
            this.lbDailyEvery.Location = new System.Drawing.Point(18, 34);
            this.lbDailyEvery.Name = "lbDailyEvery";
            this.lbDailyEvery.Size = new System.Drawing.Size(40, 13);
            this.lbDailyEvery.TabIndex = 4;
            this.lbDailyEvery.Text = "Every";
            // 
            // nudDailyFreq
            // 
            this.nudDailyFreq.Location = new System.Drawing.Point(63, 31);
            this.nudDailyFreq.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudDailyFreq.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudDailyFreq.Name = "nudDailyFreq";
            this.nudDailyFreq.Size = new System.Drawing.Size(58, 21);
            this.nudDailyFreq.TabIndex = 0;
            this.nudDailyFreq.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // gbOnce
            // 
            this.gbOnce.Controls.Add(this.dtpOnce);
            this.gbOnce.Controls.Add(this.lbOnceRunOn);
            this.gbOnce.ImeMode = System.Windows.Forms.ImeMode.On;
            this.gbOnce.Location = new System.Drawing.Point(26, 106);
            this.gbOnce.Name = "gbOnce";
            this.gbOnce.Size = new System.Drawing.Size(322, 96);
            this.gbOnce.TabIndex = 18;
            this.gbOnce.TabStop = false;
            this.gbOnce.Text = "Schedule Report Once";
            // 
            // dtpOnce
            // 
            this.dtpOnce.Location = new System.Drawing.Point(79, 38);
            this.dtpOnce.Name = "dtpOnce";
            this.dtpOnce.Size = new System.Drawing.Size(228, 21);
            this.dtpOnce.TabIndex = 12;
            // 
            // lbOnceRunOn
            // 
            this.lbOnceRunOn.AutoSize = true;
            this.lbOnceRunOn.Location = new System.Drawing.Point(15, 42);
            this.lbOnceRunOn.Name = "lbOnceRunOn";
            this.lbOnceRunOn.Size = new System.Drawing.Size(52, 13);
            this.lbOnceRunOn.TabIndex = 11;
            this.lbOnceRunOn.Text = "Run on:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.bnRemoveExisting);
            this.groupBox1.Location = new System.Drawing.Point(12, 321);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(375, 106);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Delete all Existing Schedules";
            // 
            // bnCreateSchedule
            // 
            this.bnCreateSchedule.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnCreateSchedule.Location = new System.Drawing.Point(216, 59);
            this.bnCreateSchedule.Name = "bnCreateSchedule";
            this.bnCreateSchedule.Size = new System.Drawing.Size(132, 23);
            this.bnCreateSchedule.TabIndex = 21;
            this.bnCreateSchedule.Text = "Create Schedule";
            this.bnCreateSchedule.UseVisualStyleBackColor = true;
            this.bnCreateSchedule.Click += new System.EventHandler(this.bnCreateSchedule_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(252, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Click to remove all schedule for this report";
            // 
            // FormScheduleReport
            // 
            this.AcceptButton = this.bnCreateSchedule;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(401, 480);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbMain);
            this.Controls.Add(this.bnCancel);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormScheduleReport";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Schedule AuditWizard Report";
            this.Load += new System.EventHandler(this.FormScheduleReport_Load);
            this.gbMain.ResumeLayout(false);
            this.gbMain.PerformLayout();
            this.gbMonthly.ResumeLayout(false);
            this.gbMonthly.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMonthlyFreq)).EndInit();
            this.gbWeekly.ResumeLayout(false);
            this.gbWeekly.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWeeklyFreq)).EndInit();
            this.gbDaily.ResumeLayout(false);
            this.gbDaily.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDailyFreq)).EndInit();
            this.gbOnce.ResumeLayout(false);
            this.gbOnce.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bnCancel;
        private System.Windows.Forms.Button bnRemoveExisting;
        private System.Windows.Forms.GroupBox gbMain;
        private System.Windows.Forms.Label lbAllStartTime;
        private System.Windows.Forms.Label lbAllScheduleReport;
        private System.Windows.Forms.ComboBox cmbScheduleInterval;
        private System.Windows.Forms.GroupBox gbWeekly;
        private System.Windows.Forms.CheckBox cbWeeklySat;
        private System.Windows.Forms.CheckBox cbWeeklySun;
        private System.Windows.Forms.CheckBox cbWeeklyFri;
        private System.Windows.Forms.CheckBox cbWeeklyThu;
        private System.Windows.Forms.CheckBox cbWeeklyWed;
        private System.Windows.Forms.CheckBox cbWeeklyTue;
        private System.Windows.Forms.CheckBox cbWeeklyMon;
        private System.Windows.Forms.Label lbWeeklyWeeks;
        private System.Windows.Forms.NumericUpDown nudWeeklyFreq;
        private System.Windows.Forms.Label lbWeeklyEvery;
        private System.Windows.Forms.GroupBox gbDaily;
        private System.Windows.Forms.Label lbDailyDays;
        private System.Windows.Forms.Label lbDailyEvery;
        private System.Windows.Forms.NumericUpDown nudDailyFreq;
        private System.Windows.Forms.GroupBox gbOnce;
        private System.Windows.Forms.DateTimePicker dtpOnce;
        private System.Windows.Forms.Label lbOnceRunOn;
        private System.Windows.Forms.GroupBox gbMonthly;
        private System.Windows.Forms.Label lbMonthlyMonths;
        private System.Windows.Forms.NumericUpDown nudMonthlyFreq;
        private System.Windows.Forms.DateTimePicker nudStartTime;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bnCreateSchedule;
        private System.Windows.Forms.Label label1;
    }
}