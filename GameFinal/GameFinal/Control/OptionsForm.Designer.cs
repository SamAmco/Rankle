namespace GameFinal
{
    partial class OptionsForm
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
            this.serverRadioButton = new System.Windows.Forms.RadioButton();
            this.clientRadioButton = new System.Windows.Forms.RadioButton();
            this.runModeBox = new System.Windows.Forms.GroupBox();
            this.iPGroupBox = new System.Windows.Forms.GroupBox();
            this.iPBox4 = new System.Windows.Forms.TextBox();
            this.iPBox3 = new System.Windows.Forms.TextBox();
            this.iPBox2 = new System.Windows.Forms.TextBox();
            this.iPBox1 = new System.Windows.Forms.TextBox();
            this.goButton = new System.Windows.Forms.Button();
            this.serverSettingsGroup = new System.Windows.Forms.GroupBox();
            this.mapNumberLabel = new System.Windows.Forms.Label();
            this.mapNumberBox = new System.Windows.Forms.TextBox();
            this.FullScreenCheckBox = new System.Windows.Forms.CheckBox();
            this.graphicsSettingsGroup = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.skinComboBox = new System.Windows.Forms.ComboBox();
            this.skinPictureBox = new System.Windows.Forms.PictureBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.resolutionsComboBox = new System.Windows.Forms.ComboBox();
            this.runModeBox.SuspendLayout();
            this.iPGroupBox.SuspendLayout();
            this.serverSettingsGroup.SuspendLayout();
            this.graphicsSettingsGroup.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.skinPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // serverRadioButton
            // 
            this.serverRadioButton.AutoSize = true;
            this.serverRadioButton.Location = new System.Drawing.Point(6, 19);
            this.serverRadioButton.Name = "serverRadioButton";
            this.serverRadioButton.Size = new System.Drawing.Size(56, 17);
            this.serverRadioButton.TabIndex = 0;
            this.serverRadioButton.TabStop = true;
            this.serverRadioButton.Text = "Server";
            this.serverRadioButton.UseVisualStyleBackColor = true;
            this.serverRadioButton.CheckedChanged += new System.EventHandler(this.serverRadioButton_CheckedChanged);
            // 
            // clientRadioButton
            // 
            this.clientRadioButton.AutoSize = true;
            this.clientRadioButton.Location = new System.Drawing.Point(6, 42);
            this.clientRadioButton.Name = "clientRadioButton";
            this.clientRadioButton.Size = new System.Drawing.Size(51, 17);
            this.clientRadioButton.TabIndex = 1;
            this.clientRadioButton.TabStop = true;
            this.clientRadioButton.Text = "Client";
            this.clientRadioButton.UseVisualStyleBackColor = true;
            this.clientRadioButton.CheckedChanged += new System.EventHandler(this.clientRadioButton_CheckedChanged);
            // 
            // runModeBox
            // 
            this.runModeBox.Controls.Add(this.serverRadioButton);
            this.runModeBox.Controls.Add(this.clientRadioButton);
            this.runModeBox.Location = new System.Drawing.Point(14, 12);
            this.runModeBox.Name = "runModeBox";
            this.runModeBox.Size = new System.Drawing.Size(200, 69);
            this.runModeBox.TabIndex = 2;
            this.runModeBox.TabStop = false;
            this.runModeBox.Text = "Run Mode";
            // 
            // iPGroupBox
            // 
            this.iPGroupBox.Controls.Add(this.iPBox4);
            this.iPGroupBox.Controls.Add(this.iPBox3);
            this.iPGroupBox.Controls.Add(this.iPBox2);
            this.iPGroupBox.Controls.Add(this.iPBox1);
            this.iPGroupBox.Enabled = false;
            this.iPGroupBox.Location = new System.Drawing.Point(12, 87);
            this.iPGroupBox.Name = "iPGroupBox";
            this.iPGroupBox.Size = new System.Drawing.Size(200, 48);
            this.iPGroupBox.TabIndex = 3;
            this.iPGroupBox.TabStop = false;
            this.iPGroupBox.Text = "Connect IP";
            // 
            // iPBox4
            // 
            this.iPBox4.Location = new System.Drawing.Point(137, 19);
            this.iPBox4.MaxLength = 3;
            this.iPBox4.Name = "iPBox4";
            this.iPBox4.Size = new System.Drawing.Size(31, 20);
            this.iPBox4.TabIndex = 3;
            // 
            // iPBox3
            // 
            this.iPBox3.Location = new System.Drawing.Point(100, 19);
            this.iPBox3.MaxLength = 3;
            this.iPBox3.Name = "iPBox3";
            this.iPBox3.Size = new System.Drawing.Size(31, 20);
            this.iPBox3.TabIndex = 2;
            // 
            // iPBox2
            // 
            this.iPBox2.Location = new System.Drawing.Point(63, 19);
            this.iPBox2.MaxLength = 3;
            this.iPBox2.Name = "iPBox2";
            this.iPBox2.Size = new System.Drawing.Size(31, 20);
            this.iPBox2.TabIndex = 1;
            // 
            // iPBox1
            // 
            this.iPBox1.Location = new System.Drawing.Point(26, 19);
            this.iPBox1.MaxLength = 3;
            this.iPBox1.Name = "iPBox1";
            this.iPBox1.Size = new System.Drawing.Size(31, 20);
            this.iPBox1.TabIndex = 0;
            // 
            // goButton
            // 
            this.goButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.goButton.Font = new System.Drawing.Font("Motorwerk", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.goButton.Location = new System.Drawing.Point(220, 343);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(169, 29);
            this.goButton.TabIndex = 4;
            this.goButton.Text = "GO!";
            this.goButton.UseVisualStyleBackColor = true;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // serverSettingsGroup
            // 
            this.serverSettingsGroup.Controls.Add(this.mapNumberLabel);
            this.serverSettingsGroup.Controls.Add(this.mapNumberBox);
            this.serverSettingsGroup.Location = new System.Drawing.Point(14, 141);
            this.serverSettingsGroup.Name = "serverSettingsGroup";
            this.serverSettingsGroup.Size = new System.Drawing.Size(197, 49);
            this.serverSettingsGroup.TabIndex = 5;
            this.serverSettingsGroup.TabStop = false;
            this.serverSettingsGroup.Text = "Server Settings";
            // 
            // mapNumberLabel
            // 
            this.mapNumberLabel.AutoSize = true;
            this.mapNumberLabel.Location = new System.Drawing.Point(70, 22);
            this.mapNumberLabel.Name = "mapNumberLabel";
            this.mapNumberLabel.Size = new System.Drawing.Size(90, 13);
            this.mapNumberLabel.TabIndex = 1;
            this.mapNumberLabel.Text = "Map number (1-8)";
            // 
            // mapNumberBox
            // 
            this.mapNumberBox.Location = new System.Drawing.Point(24, 19);
            this.mapNumberBox.MaxLength = 3;
            this.mapNumberBox.Name = "mapNumberBox";
            this.mapNumberBox.Size = new System.Drawing.Size(31, 20);
            this.mapNumberBox.TabIndex = 4;
            this.mapNumberBox.Text = "4";
            // 
            // FullScreenCheckBox
            // 
            this.FullScreenCheckBox.AutoSize = true;
            this.FullScreenCheckBox.Location = new System.Drawing.Point(24, 19);
            this.FullScreenCheckBox.Name = "FullScreenCheckBox";
            this.FullScreenCheckBox.Size = new System.Drawing.Size(79, 17);
            this.FullScreenCheckBox.TabIndex = 6;
            this.FullScreenCheckBox.Text = "Full Screen";
            this.FullScreenCheckBox.UseVisualStyleBackColor = true;
            // 
            // graphicsSettingsGroup
            // 
            this.graphicsSettingsGroup.Controls.Add(this.resolutionsComboBox);
            this.graphicsSettingsGroup.Controls.Add(this.FullScreenCheckBox);
            this.graphicsSettingsGroup.Location = new System.Drawing.Point(14, 196);
            this.graphicsSettingsGroup.Name = "graphicsSettingsGroup";
            this.graphicsSettingsGroup.Size = new System.Drawing.Size(197, 99);
            this.graphicsSettingsGroup.TabIndex = 7;
            this.graphicsSettingsGroup.TabStop = false;
            this.graphicsSettingsGroup.Text = "graphicsSettings";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.skinComboBox);
            this.groupBox1.Controls.Add(this.skinPictureBox);
            this.groupBox1.Controls.Add(this.nameTextBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(220, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(169, 325);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Character";
            // 
            // skinComboBox
            // 
            this.skinComboBox.FormattingEnabled = true;
            this.skinComboBox.Location = new System.Drawing.Point(69, 51);
            this.skinComboBox.Name = "skinComboBox";
            this.skinComboBox.Size = new System.Drawing.Size(94, 21);
            this.skinComboBox.TabIndex = 3;
            this.skinComboBox.SelectedIndexChanged += new System.EventHandler(this.skinComboBox_SelectedIndexChanged);
            // 
            // skinPictureBox
            // 
            this.skinPictureBox.Location = new System.Drawing.Point(6, 75);
            this.skinPictureBox.Name = "skinPictureBox";
            this.skinPictureBox.Size = new System.Drawing.Size(157, 242);
            this.skinPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.skinPictureBox.TabIndex = 2;
            this.skinPictureBox.TabStop = false;
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(54, 18);
            this.nameTextBox.MaxLength = 20;
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(109, 20);
            this.nameTextBox.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Tank Skin";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Name: ";
            // 
            // resolutionsComboBox
            // 
            this.resolutionsComboBox.FormattingEnabled = true;
            this.resolutionsComboBox.Items.AddRange(new object[] {
            "640, 480",
            "720, 480",
            "720, 576",
            "800, 600",
            "1024, 768",
            "1152, 864",
            "1280, 720",
            "1280, 768",
            "1280, 800",
            "1366, 768",
            "1600, 900",
            "1920, 1080"});
            this.resolutionsComboBox.Location = new System.Drawing.Point(24, 43);
            this.resolutionsComboBox.Name = "resolutionsComboBox";
            this.resolutionsComboBox.Size = new System.Drawing.Size(136, 21);
            this.resolutionsComboBox.TabIndex = 7;
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 380);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.graphicsSettingsGroup);
            this.Controls.Add(this.serverSettingsGroup);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.iPGroupBox);
            this.Controls.Add(this.runModeBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Name = "OptionsForm";
            this.Text = "OptionsForm";
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OptionsForm_KeyDown);
            this.runModeBox.ResumeLayout(false);
            this.runModeBox.PerformLayout();
            this.iPGroupBox.ResumeLayout(false);
            this.iPGroupBox.PerformLayout();
            this.serverSettingsGroup.ResumeLayout(false);
            this.serverSettingsGroup.PerformLayout();
            this.graphicsSettingsGroup.ResumeLayout(false);
            this.graphicsSettingsGroup.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.skinPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton serverRadioButton;
        private System.Windows.Forms.RadioButton clientRadioButton;
        private System.Windows.Forms.GroupBox runModeBox;
        private System.Windows.Forms.GroupBox iPGroupBox;
        private System.Windows.Forms.TextBox iPBox4;
        private System.Windows.Forms.TextBox iPBox3;
        private System.Windows.Forms.TextBox iPBox2;
        private System.Windows.Forms.TextBox iPBox1;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.GroupBox serverSettingsGroup;
        private System.Windows.Forms.Label mapNumberLabel;
        private System.Windows.Forms.TextBox mapNumberBox;
        private System.Windows.Forms.CheckBox FullScreenCheckBox;
        private System.Windows.Forms.GroupBox graphicsSettingsGroup;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox skinComboBox;
        private System.Windows.Forms.PictureBox skinPictureBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox resolutionsComboBox;
    }
}