namespace encryption_cypher_app
{
    partial class Form2
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
            form2titlelabel = new Label();
            form2close = new Button();
            randomnisekeyvaluebutton = new Button();
            labelTotalChars = new Label();
            textboxTotalChars = new TextBox();
            labelNumNumbers = new Label();
            textboxNumNumbers = new TextBox();
            labelNumSpecial = new Label();
            textboxNumSpecial = new TextBox();
            randomiserror = new Label();
            hideKeysCheckbox = new CheckBox();
            EncryptFileButton = new Button();
            DecryptFileButton = new Button();
            author = new Label();
            ExportKeysButton = new Button();
            ImportKeysButton = new Button();
            fileProgressBar = new ProgressBar();
            statusLabel = new Label();
            SuspendLayout();
            // 
            // form2titlelabel
            // 
            form2titlelabel.AutoSize = true;
            form2titlelabel.ForeColor = SystemColors.Control;
            form2titlelabel.Location = new Point(400, 20);
            form2titlelabel.Name = "form2titlelabel";
            form2titlelabel.Size = new Size(238, 25);
            form2titlelabel.TabIndex = 0;
            form2titlelabel.Text = "Key Generation & File Tools";
            // 
            // form2close
            // 
            form2close.Location = new Point(1030, 12);
            form2close.Name = "form2close";
            form2close.Size = new Size(112, 34);
            form2close.TabIndex = 1;
            form2close.Text = "Close";
            form2close.UseVisualStyleBackColor = true;
            form2close.Click += form2close_Click;
            //
            // labelTotalChars
            //
            labelTotalChars.AutoSize = true;
            labelTotalChars.ForeColor = SystemColors.Control;
            labelTotalChars.Location = new Point(80, 80);
            labelTotalChars.Name = "labelTotalChars";
            labelTotalChars.Size = new Size(200, 25);
            labelTotalChars.TabIndex = 2;
            labelTotalChars.Text = "Total characters per key:";
            //
            // textboxTotalChars
            //
            textboxTotalChars.Location = new Point(80, 110);
            textboxTotalChars.Name = "textboxTotalChars";
            textboxTotalChars.Size = new Size(120, 31);
            textboxTotalChars.TabIndex = 3;
            textboxTotalChars.Text = "16";
            //
            // labelNumNumbers
            //
            labelNumNumbers.AutoSize = true;
            labelNumNumbers.ForeColor = SystemColors.Control;
            labelNumNumbers.Location = new Point(80, 160);
            labelNumNumbers.Name = "labelNumNumbers";
            labelNumNumbers.Size = new Size(180, 25);
            labelNumNumbers.TabIndex = 4;
            labelNumNumbers.Text = "Numbers (digits):";
            //
            // textboxNumNumbers
            //
            textboxNumNumbers.Location = new Point(80, 190);
            textboxNumNumbers.Name = "textboxNumNumbers";
            textboxNumNumbers.Size = new Size(120, 31);
            textboxNumNumbers.TabIndex = 5;
            textboxNumNumbers.Text = "4";
            //
            // labelNumSpecial
            //
            labelNumSpecial.AutoSize = true;
            labelNumSpecial.ForeColor = SystemColors.Control;
            labelNumSpecial.Location = new Point(80, 240);
            labelNumSpecial.Name = "labelNumSpecial";
            labelNumSpecial.Size = new Size(240, 25);
            labelNumSpecial.TabIndex = 6;
            labelNumSpecial.Text = "Special characters (!@# etc.):";
            //
            // textboxNumSpecial
            //
            textboxNumSpecial.Location = new Point(80, 270);
            textboxNumSpecial.Name = "textboxNumSpecial";
            textboxNumSpecial.Size = new Size(120, 31);
            textboxNumSpecial.TabIndex = 7;
            textboxNumSpecial.Text = "2";
            //
            // randomnisekeyvaluebutton
            //
            randomnisekeyvaluebutton.Location = new Point(80, 320);
            randomnisekeyvaluebutton.Name = "randomnisekeyvaluebutton";
            randomnisekeyvaluebutton.Size = new Size(120, 34);
            randomnisekeyvaluebutton.TabIndex = 8;
            randomnisekeyvaluebutton.Text = "Randomise Keys";
            randomnisekeyvaluebutton.UseVisualStyleBackColor = true;
            randomnisekeyvaluebutton.Click += randomnisekeyvaluebutton_Click;
            //
            // randomiserror
            //
            randomiserror.AutoSize = true;
            randomiserror.ForeColor = Color.Red;
            randomiserror.Location = new Point(80, 370);
            randomiserror.Name = "randomiserror";
            randomiserror.Size = new Size(309, 25);
            randomiserror.TabIndex = 9;
            randomiserror.Text = "Invalid input — numbers + specials must not exceed total.";
            randomiserror.Visible = false;
            //
            // hideKeysCheckbox
            //
            hideKeysCheckbox.AutoSize = true;
            hideKeysCheckbox.ForeColor = SystemColors.ButtonFace;
            hideKeysCheckbox.Location = new Point(450, 320);
            hideKeysCheckbox.Name = "hideKeysCheckbox";
            hideKeysCheckbox.Size = new Size(116, 29);
            hideKeysCheckbox.TabIndex = 10;
            hideKeysCheckbox.Text = "Hide Keys";
            hideKeysCheckbox.UseVisualStyleBackColor = true;
            hideKeysCheckbox.CheckedChanged += hideKeysCheckbox_CheckedChanged;
            // 
            // EncryptFileButton
            // 
            EncryptFileButton.Location = new Point(450, 80);
            EncryptFileButton.Name = "EncryptFileButton";
            EncryptFileButton.Size = new Size(139, 38);
            EncryptFileButton.TabIndex = 11;
            EncryptFileButton.Text = "Encrypt File";
            EncryptFileButton.UseVisualStyleBackColor = true;
            EncryptFileButton.Click += EncryptFileButton_Click;
            // 
            // DecryptFileButton
            // 
            DecryptFileButton.Location = new Point(450, 140);
            DecryptFileButton.Name = "DecryptFileButton";
            DecryptFileButton.Size = new Size(139, 38);
            DecryptFileButton.TabIndex = 12;
            DecryptFileButton.Text = "Decrypt File";
            DecryptFileButton.UseVisualStyleBackColor = true;
            DecryptFileButton.Click += DecryptFileButton_Click;
            //
            // ExportKeysButton
            //
            ExportKeysButton.Location = new Point(450, 200);
            ExportKeysButton.Name = "ExportKeysButton";
            ExportKeysButton.Size = new Size(139, 38);
            ExportKeysButton.TabIndex = 14;
            ExportKeysButton.Text = "Export Keys";
            ExportKeysButton.UseVisualStyleBackColor = true;
            ExportKeysButton.Click += ExportKeysButton_Click;
            //
            // ImportKeysButton
            //
            ImportKeysButton.Location = new Point(450, 260);
            ImportKeysButton.Name = "ImportKeysButton";
            ImportKeysButton.Size = new Size(139, 38);
            ImportKeysButton.TabIndex = 15;
            ImportKeysButton.Text = "Import Keys";
            ImportKeysButton.UseVisualStyleBackColor = true;
            ImportKeysButton.Click += ImportKeysButton_Click;
            //
            // author
            // 
            author.AutoSize = true;
            author.ForeColor = Color.FromArgb(30, 0, 0);
            author.Location = new Point(964, 691);
            author.Name = "author";
            author.Size = new Size(187, 25);
            author.TabIndex = 13;
            author.Text = "Made by Unicorn God";
            author.Click += label1_Click;
            // 
            // fileProgressBar
            //
            fileProgressBar.Location = new Point(450, 370);
            fileProgressBar.Name = "fileProgressBar";
            fileProgressBar.Size = new Size(580, 38);
            fileProgressBar.TabIndex = 16;
            fileProgressBar.Visible = false;
            //
            // statusLabel
            //
            statusLabel.AutoSize = true;
            statusLabel.ForeColor = SystemColors.Control;
            statusLabel.Location = new Point(450, 420);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(0, 25);
            statusLabel.TabIndex = 17;
            statusLabel.Visible = false;
            //
            // Form2
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Desktop;
            ClientSize = new Size(1154, 725);
            Controls.Add(statusLabel);
            Controls.Add(fileProgressBar);
            Controls.Add(author);
            Controls.Add(ImportKeysButton);
            Controls.Add(ExportKeysButton);
            Controls.Add(DecryptFileButton);
            Controls.Add(EncryptFileButton);
            Controls.Add(hideKeysCheckbox);
            Controls.Add(randomiserror);
            Controls.Add(labelTotalChars);
            Controls.Add(textboxTotalChars);
            Controls.Add(labelNumNumbers);
            Controls.Add(textboxNumNumbers);
            Controls.Add(labelNumSpecial);
            Controls.Add(textboxNumSpecial);
            Controls.Add(randomnisekeyvaluebutton);
            Controls.Add(form2close);
            Controls.Add(form2titlelabel);
            ForeColor = SystemColors.ActiveCaptionText;
            Name = "Form2";
            Text = "Form2";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label form2titlelabel;
        private Button form2close;
        private Button randomnisekeyvaluebutton;
        private Label labelTotalChars;
        private TextBox textboxTotalChars;
        private Label labelNumNumbers;
        private TextBox textboxNumNumbers;
        private Label labelNumSpecial;
        private TextBox textboxNumSpecial;
        private Label randomiserror;
        private CheckBox hideKeysCheckbox;
        private Button EncryptFileButton;
        private Button DecryptFileButton;
        private Button ExportKeysButton;
        private Button ImportKeysButton;
        private ProgressBar fileProgressBar;
        private Label statusLabel;
        private Label author;
    }
}