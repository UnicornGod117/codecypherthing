namespace encryption_cypher_app
{
    partial class Form1
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
        public void InitializeComponent()
        {
            Keylabel1 = new Label();
            Keybox1 = new TextBox();
            keybox2 = new TextBox();
            keylabel2 = new Label();
            Encryptbutton = new Button();
            Decryptbutton = new Button();
            Decryptlabel = new Label();
            Encryptlabel = new Label();
            TextBoxDecryptionInput = new TextBox();
            TextboxEncryptioninput = new TextBox();
            TextBoxDecryptOutput = new TextBox();
            TextBoxEncryptOutput = new TextBox();
            decryptedlabel = new Label();
            Encryptedlabel = new Label();
            invalidformatlabelkey1 = new Label();
            invalidformatlabelkey2 = new Label();
            encryptioncopybutton = new Button();
            decryptionpastebutton = new Button();
            pasteerror = new Label();
            keylabel4 = new Label();
            keybox4 = new TextBox();
            keybox3 = new TextBox();
            keylabel3 = new Label();
            invalidformatlabelkey4 = new Label();
            invalidformatlabelkey3 = new Label();
            gotoform2button = new Button();
            authenticationlabel = new Label();
            VersionNumber = new Label();
            SuspendLayout();
            // 
            // Keylabel1
            // 
            Keylabel1.AutoSize = true;
            Keylabel1.ForeColor = SystemColors.Control;
            Keylabel1.Location = new Point(470, 58);
            Keylabel1.Name = "Keylabel1";
            Keylabel1.Size = new Size(55, 25);
            Keylabel1.TabIndex = 0;
            Keylabel1.Text = "Key 1";
            Keylabel1.TextAlign = ContentAlignment.TopRight;
            Keylabel1.Click += label1_Click;
            // 
            // Keybox1
            // 
            Keybox1.Location = new Point(274, 86);
            Keybox1.Name = "Keybox1";
            Keybox1.Size = new Size(467, 31);
            Keybox1.TabIndex = 1;
            Keybox1.TextChanged += Keybox1_TextChanged;
            // 
            // keybox2
            // 
            keybox2.Location = new Point(274, 187);
            keybox2.Name = "keybox2";
            keybox2.Size = new Size(467, 31);
            keybox2.TabIndex = 2;
            keybox2.TextChanged += textBox2_TextChanged;
            // 
            // keylabel2
            // 
            keylabel2.AutoSize = true;
            keylabel2.ForeColor = SystemColors.Control;
            keylabel2.Location = new Point(470, 159);
            keylabel2.Name = "keylabel2";
            keylabel2.Size = new Size(55, 25);
            keylabel2.TabIndex = 3;
            keylabel2.Text = "Key 2";
            keylabel2.TextAlign = ContentAlignment.TopRight;
            // 
            // Encryptbutton
            // 
            Encryptbutton.ForeColor = SystemColors.Desktop;
            Encryptbutton.Location = new Point(180, 462);
            Encryptbutton.Name = "Encryptbutton";
            Encryptbutton.Size = new Size(129, 34);
            Encryptbutton.TabIndex = 4;
            Encryptbutton.Text = "Encrypt";
            Encryptbutton.UseVisualStyleBackColor = true;
            Encryptbutton.Click += Encryptbutton_Click;
            // 
            // Decryptbutton
            // 
            Decryptbutton.ForeColor = SystemColors.Desktop;
            Decryptbutton.Location = new Point(1159, 462);
            Decryptbutton.Name = "Decryptbutton";
            Decryptbutton.Size = new Size(132, 34);
            Decryptbutton.TabIndex = 5;
            Decryptbutton.Text = "Decrypt";
            Decryptbutton.UseVisualStyleBackColor = true;
            Decryptbutton.Click += Decryptbutton_Click;
            // 
            // Decryptlabel
            // 
            Decryptlabel.AutoSize = true;
            Decryptlabel.ForeColor = SystemColors.Control;
            Decryptlabel.Location = new Point(1052, 312);
            Decryptlabel.Name = "Decryptlabel";
            Decryptlabel.Size = new Size(375, 25);
            Decryptlabel.TabIndex = 6;
            Decryptlabel.Text = "Decrypt here! (put your text to decrypt below)";
            // 
            // Encryptlabel
            // 
            Encryptlabel.AutoSize = true;
            Encryptlabel.ForeColor = SystemColors.Control;
            Encryptlabel.Location = new Point(81, 312);
            Encryptlabel.Name = "Encryptlabel";
            Encryptlabel.Size = new Size(371, 25);
            Encryptlabel.TabIndex = 7;
            Encryptlabel.Text = "Encrypt here! (put your text to encrypt below)";
            // 
            // TextBoxDecryptionInput
            // 
            TextBoxDecryptionInput.Location = new Point(1052, 353);
            TextBoxDecryptionInput.Multiline = true;
            TextBoxDecryptionInput.Name = "TextBoxDecryptionInput";
            TextBoxDecryptionInput.Size = new Size(359, 76);
            TextBoxDecryptionInput.TabIndex = 8;
            TextBoxDecryptionInput.TextChanged += TextBoxDecryptionInput_TextChanged;
            // 
            // TextboxEncryptioninput
            // 
            TextboxEncryptioninput.Location = new Point(81, 353);
            TextboxEncryptioninput.Multiline = true;
            TextboxEncryptioninput.Name = "TextboxEncryptioninput";
            TextboxEncryptioninput.Size = new Size(359, 76);
            TextboxEncryptioninput.TabIndex = 9;
            TextboxEncryptioninput.TextChanged += TextboxEncryptioninput_TextChanged;
            // 
            // TextBoxDecryptOutput
            // 
            TextBoxDecryptOutput.Location = new Point(1052, 562);
            TextBoxDecryptOutput.Multiline = true;
            TextBoxDecryptOutput.Name = "TextBoxDecryptOutput";
            TextBoxDecryptOutput.ReadOnly = true;
            TextBoxDecryptOutput.Size = new Size(359, 255);
            TextBoxDecryptOutput.TabIndex = 10;
            TextBoxDecryptOutput.TextChanged += TextBoxDecryptOutput_TextChanged;
            // 
            // TextBoxEncryptOutput
            // 
            TextBoxEncryptOutput.Location = new Point(81, 568);
            TextBoxEncryptOutput.Multiline = true;
            TextBoxEncryptOutput.Name = "TextBoxEncryptOutput";
            TextBoxEncryptOutput.ReadOnly = true;
            TextBoxEncryptOutput.Size = new Size(359, 249);
            TextBoxEncryptOutput.TabIndex = 11;
            // 
            // decryptedlabel
            // 
            decryptedlabel.AutoSize = true;
            decryptedlabel.ForeColor = SystemColors.Control;
            decryptedlabel.Location = new Point(1159, 533);
            decryptedlabel.Name = "decryptedlabel";
            decryptedlabel.Size = new Size(132, 25);
            decryptedlabel.TabIndex = 12;
            decryptedlabel.Text = "Decrypted text:";
            // 
            // Encryptedlabel
            // 
            Encryptedlabel.AutoSize = true;
            Encryptedlabel.ForeColor = SystemColors.Control;
            Encryptedlabel.Location = new Point(180, 533);
            Encryptedlabel.Name = "Encryptedlabel";
            Encryptedlabel.Size = new Size(129, 25);
            Encryptedlabel.TabIndex = 13;
            Encryptedlabel.Text = "Encrypted text:";
            Encryptedlabel.Click += Encryptedlabel_Click;
            // 
            // invalidformatlabelkey1
            // 
            invalidformatlabelkey1.AutoSize = true;
            invalidformatlabelkey1.ForeColor = Color.Red;
            invalidformatlabelkey1.Location = new Point(299, 120);
            invalidformatlabelkey1.Name = "invalidformatlabelkey1";
            invalidformatlabelkey1.Size = new Size(393, 25);
            invalidformatlabelkey1.TabIndex = 14;
            invalidformatlabelkey1.Text = "INVALID FORMAT (ensure key is ONLY numbers)";
            invalidformatlabelkey1.Visible = false;
            invalidformatlabelkey1.Click += invalidformatlabelkey1_Click;
            // 
            // invalidformatlabelkey2
            // 
            invalidformatlabelkey2.AutoSize = true;
            invalidformatlabelkey2.ForeColor = Color.Red;
            invalidformatlabelkey2.Location = new Point(299, 221);
            invalidformatlabelkey2.Name = "invalidformatlabelkey2";
            invalidformatlabelkey2.Size = new Size(393, 25);
            invalidformatlabelkey2.TabIndex = 15;
            invalidformatlabelkey2.Text = "INVALID FORMAT (ensure key is ONLY numbers)";
            invalidformatlabelkey2.Visible = false;
            invalidformatlabelkey2.Click += invalidformatlabelkey2_Click;
            // 
            // encryptioncopybutton
            // 
            encryptioncopybutton.ForeColor = SystemColors.Desktop;
            encryptioncopybutton.Location = new Point(446, 783);
            encryptioncopybutton.Name = "encryptioncopybutton";
            encryptioncopybutton.Size = new Size(112, 34);
            encryptioncopybutton.TabIndex = 16;
            encryptioncopybutton.Text = "Copy";
            encryptioncopybutton.UseVisualStyleBackColor = true;
            encryptioncopybutton.Click += encryptioncopybutton_Click;
            // 
            // decryptionpastebutton
            // 
            decryptionpastebutton.ForeColor = SystemColors.Desktop;
            decryptionpastebutton.Location = new Point(916, 349);
            decryptionpastebutton.Name = "decryptionpastebutton";
            decryptionpastebutton.Size = new Size(112, 34);
            decryptionpastebutton.TabIndex = 17;
            decryptionpastebutton.Text = "Paste";
            decryptionpastebutton.UseVisualStyleBackColor = true;
            decryptionpastebutton.Click += decryptionpastebutton_Click;
            // 
            // pasteerror
            // 
            pasteerror.AutoSize = true;
            pasteerror.ForeColor = Color.Red;
            pasteerror.Location = new Point(894, 388);
            pasteerror.Name = "pasteerror";
            pasteerror.Size = new Size(152, 25);
            pasteerror.TabIndex = 18;
            pasteerror.Text = "Nothing to paste!";
            pasteerror.Visible = false;
            pasteerror.Click += label1_Click_1;
            // 
            // keylabel4
            // 
            keylabel4.AutoSize = true;
            keylabel4.ForeColor = SystemColors.Control;
            keylabel4.Location = new Point(977, 159);
            keylabel4.Name = "keylabel4";
            keylabel4.Size = new Size(55, 25);
            keylabel4.TabIndex = 22;
            keylabel4.Text = "Key 4";
            keylabel4.TextAlign = ContentAlignment.TopRight;
            // 
            // keybox4
            // 
            keybox4.Location = new Point(781, 187);
            keybox4.Name = "keybox4";
            keybox4.Size = new Size(467, 31);
            keybox4.TabIndex = 21;
            keybox4.TextChanged += textBox4_TextChanged;
            // 
            // keybox3
            // 
            keybox3.Location = new Point(781, 86);
            keybox3.Name = "keybox3";
            keybox3.Size = new Size(467, 31);
            keybox3.TabIndex = 20;
            keybox3.TextChanged += textBox3_TextChanged;
            // 
            // keylabel3
            // 
            keylabel3.AutoSize = true;
            keylabel3.ForeColor = SystemColors.Control;
            keylabel3.Location = new Point(977, 58);
            keylabel3.Name = "keylabel3";
            keylabel3.Size = new Size(55, 25);
            keylabel3.TabIndex = 19;
            keylabel3.Text = "Key 3";
            keylabel3.TextAlign = ContentAlignment.TopRight;
            keylabel3.Click += label2_Click;
            // 
            // invalidformatlabelkey4
            // 
            invalidformatlabelkey4.AutoSize = true;
            invalidformatlabelkey4.ForeColor = Color.Red;
            invalidformatlabelkey4.Location = new Point(818, 221);
            invalidformatlabelkey4.Name = "invalidformatlabelkey4";
            invalidformatlabelkey4.Size = new Size(393, 25);
            invalidformatlabelkey4.TabIndex = 24;
            invalidformatlabelkey4.Text = "INVALID FORMAT (ensure key is ONLY numbers)";
            invalidformatlabelkey4.Visible = false;
            // 
            // invalidformatlabelkey3
            // 
            invalidformatlabelkey3.AutoSize = true;
            invalidformatlabelkey3.ForeColor = Color.Red;
            invalidformatlabelkey3.Location = new Point(818, 120);
            invalidformatlabelkey3.Name = "invalidformatlabelkey3";
            invalidformatlabelkey3.Size = new Size(393, 25);
            invalidformatlabelkey3.TabIndex = 23;
            invalidformatlabelkey3.Text = "INVALID FORMAT (ensure key is ONLY numbers)";
            invalidformatlabelkey3.Visible = false;
            // 
            // gotoform2button
            // 
            gotoform2button.ForeColor = SystemColors.Desktop;
            gotoform2button.Location = new Point(637, 487);
            gotoform2button.Name = "gotoform2button";
            gotoform2button.Size = new Size(221, 71);
            gotoform2button.TabIndex = 25;
            gotoform2button.Text = "Extra";
            gotoform2button.UseVisualStyleBackColor = true;
            gotoform2button.Click += gotoform2button_Click;
            // 
            // authenticationlabel
            // 
            authenticationlabel.AutoSize = true;
            authenticationlabel.ForeColor = Color.Chartreuse;
            authenticationlabel.Location = new Point(1118, 499);
            authenticationlabel.Name = "authenticationlabel";
            authenticationlabel.Size = new Size(211, 25);
            authenticationlabel.TabIndex = 26;
            authenticationlabel.Text = "Authentication successful";
            authenticationlabel.Visible = false;
            // 
            // VersionNumber
            // 
            VersionNumber.AutoSize = true;
            VersionNumber.Location = new Point(1409, 24);
            VersionNumber.Name = "VersionNumber";
            VersionNumber.Size = new Size(99, 25);
            VersionNumber.TabIndex = 27;
            VersionNumber.Text = "Version 2.7";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.WindowText;
            ClientSize = new Size(1511, 922);
            Controls.Add(VersionNumber);
            Controls.Add(authenticationlabel);
            Controls.Add(gotoform2button);
            Controls.Add(invalidformatlabelkey4);
            Controls.Add(invalidformatlabelkey3);
            Controls.Add(keylabel4);
            Controls.Add(keybox4);
            Controls.Add(keybox3);
            Controls.Add(keylabel3);
            Controls.Add(pasteerror);
            Controls.Add(decryptionpastebutton);
            Controls.Add(encryptioncopybutton);
            Controls.Add(invalidformatlabelkey2);
            Controls.Add(invalidformatlabelkey1);
            Controls.Add(Encryptedlabel);
            Controls.Add(decryptedlabel);
            Controls.Add(TextBoxEncryptOutput);
            Controls.Add(TextBoxDecryptOutput);
            Controls.Add(TextboxEncryptioninput);
            Controls.Add(TextBoxDecryptionInput);
            Controls.Add(Encryptlabel);
            Controls.Add(Decryptlabel);
            Controls.Add(Decryptbutton);
            Controls.Add(Encryptbutton);
            Controls.Add(keylabel2);
            Controls.Add(keybox2);
            Controls.Add(Keybox1);
            Controls.Add(Keylabel1);
            ForeColor = SystemColors.ControlLightLight;
            Name = "Form1";
            Text = "ahhh 💀";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Keylabel1;
        public TextBox Keybox1;
        public TextBox keybox2;
        private Label keylabel2;
        private Button Encryptbutton;
        private Button Decryptbutton;
        private Label Decryptlabel;
        private Label Encryptlabel;
        private TextBox TextBoxDecryptionInput;
        private TextBox TextboxEncryptioninput;
        private TextBox TextBoxDecryptOutput;
        private TextBox TextBoxEncryptOutput;
        private Label decryptedlabel;
        private Label Encryptedlabel;
        private Label invalidformatlabelkey1;
        private Label invalidformatlabelkey2;
        private Button encryptioncopybutton;
        private Button decryptionpastebutton;
        private Label pasteerror;
        private Label keylabel4;
        public TextBox keybox4;
        public TextBox keybox3;
        private Label keylabel3;
        private Label invalidformatlabelkey4;
        private Label invalidformatlabelkey3;
        private Button gotoform2button;
        private Label authenticationlabel;
        private Label VersionNumber;
    }
}
