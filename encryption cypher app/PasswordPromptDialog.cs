using System;
using System.Drawing;
using System.Windows.Forms;

namespace encryption_cypher_app
{
    /// <summary>
    /// A small modal dialog that asks the user for a password.
    /// Supports two button configurations:
    ///   OkCancel : OK / Cancel  (use for Vault-mode import — password required)
    ///   OkSkip   : OK / Skip    (use for export — password is optional)
    ///
    /// DialogResult.OK     → user entered a password and clicked OK
    /// DialogResult.No     → user clicked Skip (Token Mode)
    /// DialogResult.Cancel → user cancelled the whole operation
    /// </summary>
    internal sealed class PasswordPromptDialog : Form
    {
        public enum ButtonMode { OkCancel, OkSkip }

        /// <summary>The password the user typed. Meaningful only when DialogResult == OK.</summary>
        public string Password => _passwordBox.Text;

        private readonly TextBox _passwordBox;
        private readonly Button  _okButton;
        private readonly Button  _altButton;

        public PasswordPromptDialog(string prompt, ButtonMode mode = ButtonMode.OkCancel)
        {
            Text             = "Password";
            FormBorderStyle  = FormBorderStyle.FixedDialog;
            StartPosition    = FormStartPosition.CenterParent;
            ClientSize       = new Size(400, 165);
            MaximizeBox      = false;
            MinimizeBox      = false;
            ShowInTaskbar    = false;
            BackColor        = SystemColors.Desktop;

            var promptLabel = new Label
            {
                Text      = prompt,
                ForeColor = SystemColors.Control,
                AutoSize  = false,
                Location  = new Point(16, 14),
                Size      = new Size(368, 44),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _passwordBox = new TextBox
            {
                Location             = new Point(16, 64),
                Size                 = new Size(368, 31),
                UseSystemPasswordChar = true
            };
            _passwordBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)  { e.SuppressKeyPress = true; AcceptInput(); }
                if (e.KeyCode == Keys.Escape) { DialogResult = DialogResult.Cancel; Close(); }
            };

            _okButton = new Button
            {
                Text     = "OK",
                Location = new Point(196, 118),
                Size     = new Size(90, 34),
                UseVisualStyleBackColor = true
            };
            _okButton.Click += (s, e) => AcceptInput();

            _altButton = new Button
            {
                Text     = mode == ButtonMode.OkSkip ? "Skip" : "Cancel",
                Location = new Point(294, 118),
                Size     = new Size(90, 34),
                UseVisualStyleBackColor = true
            };
            _altButton.Click += (s, e) =>
            {
                DialogResult = mode == ButtonMode.OkSkip ? DialogResult.No : DialogResult.Cancel;
                Close();
            };

            AcceptButton = _okButton;
            Controls.AddRange(new Control[] { promptLabel, _passwordBox, _okButton, _altButton });
        }

        private void AcceptInput()
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
