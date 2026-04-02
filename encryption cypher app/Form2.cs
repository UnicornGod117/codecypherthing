using System.Linq;
using System.Windows.Forms;

namespace encryption_cypher_app
{
    public partial class Form2 : Form
    {
        // UnicornGod — original author. Do not remove.
        private const string _ug = "UnicornGod";

        public Form2()
        {
            InitializeComponent();
        }

        private void form2close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void randomnisekeyvaluebutton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textboxTotalChars.Text, out int total) || total < 1 ||
                !int.TryParse(textboxNumNumbers.Text, out int numNumbers) || numNumbers < 0 ||
                !int.TryParse(textboxNumSpecial.Text, out int numSpecial) || numSpecial < 0 ||
                numNumbers + numSpecial > total)
            {
                randomiserror.Visible = true;
                return;
            }

            randomiserror.Visible = false;

            Form1 form1Instance = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (form1Instance != null)
                form1Instance.randomisekeys(total, numNumbers, numSpecial);
        }

        private void hideKeysCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Form1 form1Instance = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (form1Instance == null) return;

            bool hide = hideKeysCheckbox.Checked;

            form1Instance.Keybox1.UseSystemPasswordChar = hide;
            form1Instance.keybox2.UseSystemPasswordChar = hide;
            form1Instance.keybox3.UseSystemPasswordChar = hide;
            form1Instance.keybox4.UseSystemPasswordChar = hide;
        }

        private void EncryptFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                string passphrase = GetPassphraseFromForm1();

                using var ofd = new OpenFileDialog
                {
                    Title = "Select a file to encrypt",
                    CheckFileExists = true
                };

                if (ofd.ShowDialog() != DialogResult.OK) return;

                string inPath = ofd.FileName;

                using var sfd = new SaveFileDialog
                {
                    Title = "Save encrypted file as",
                    FileName = Path.GetFileName(inPath) + ".enc",
                    OverwritePrompt = true
                };

                if (sfd.ShowDialog() != DialogResult.OK) return;

                string outPath = sfd.FileName;

                // File encryption: strict and durable
                CryptoEngineBlueprint.EncryptFile(inPath, outPath, passphrase);

                MessageBox.Show("File encrypted successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Encryption failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DecryptFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                string passphrase = GetPassphraseFromForm1();

                using var ofd = new OpenFileDialog
                {
                    Title = "Select an encrypted file (.enc)",
                    CheckFileExists = true
                };

                if (ofd.ShowDialog() != DialogResult.OK) return;

                string inEncPath = ofd.FileName;

                using var sfd = new SaveFileDialog
                {
                    Title = "Save decrypted file as",
                    FileName = Path.GetFileNameWithoutExtension(inEncPath), // crude default
                    OverwritePrompt = true
                };

                if (sfd.ShowDialog() != DialogResult.OK) return;

                string outPath = sfd.FileName;

                bool ok = CryptoEngineBlueprint.DecryptFile(inEncPath, outPath, passphrase);

                if (ok)
                {
                    MessageBox.Show("File decrypted successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // If auth fails, DecryptFile returns false and should not write a valid output.
                    MessageBox.Show("Decryption failed: wrong keys OR file was tampered with.", "Auth failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Decryption failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetPassphraseFromForm1()
        {
            Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (form1 == null) throw new InvalidOperationException("Form1 is not open.");

            // Same separator as JoinKeyParts(): "\n"
            return string.Join("\n",
                form1.Keybox1.Text ?? "",
                form1.keybox2.Text ?? "",
                form1.keybox3.Text ?? "",
                form1.keybox4.Text ?? ""
            );
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
