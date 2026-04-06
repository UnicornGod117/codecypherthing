using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

        private async void EncryptFileButton_Click(object sender, EventArgs e)
        {
            byte[]? passphrase = null;
            try
            {
                passphrase = GetPassphraseFromForm1();

                using var ofd = new OpenFileDialog
                {
                    Title = "Select a file to encrypt",
                    CheckFileExists = true,
                    InitialDirectory = AppSettings.GetLastUsedDirectory() ?? string.Empty
                };

                if (ofd.ShowDialog() != DialogResult.OK) return;

                string inPath = ofd.FileName;
                AppSettings.SetLastUsedDirectory(Path.GetDirectoryName(inPath));

                using var sfd = new SaveFileDialog
                {
                    Title = "Save encrypted file as",
                    FileName = Path.GetFileName(inPath) + ".enc",
                    OverwritePrompt = true,
                    InitialDirectory = AppSettings.GetLastUsedDirectory() ?? string.Empty
                };

                if (sfd.ShowDialog() != DialogResult.OK) return;

                string outPath = sfd.FileName;

                ToggleUI(false);
                long fileLen = new FileInfo(inPath).Length;
                fileProgressBar.Maximum = 100;
                fileProgressBar.Value = 0;
                fileProgressBar.Visible = true;
                statusLabel.Text = "Encrypting...";
                statusLabel.Visible = true;

                var progress = new Progress<long>(bytes =>
                {
                    if (fileLen > 0)
                        fileProgressBar.Value = (int)(100 * bytes / fileLen);
                });

                // File encryption: strict and durable
                await Task.Run(() => CryptoEngineBlueprint.EncryptFile(inPath, outPath, passphrase, progress: progress));

                if (!this.IsDisposed)
                    MessageBox.Show("File encrypted successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed)
                    MessageBox.Show("Encryption failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (passphrase != null) CryptographicOperations.ZeroMemory(passphrase);
                if (!this.IsDisposed)
                {
                    ToggleUI(true);
                    fileProgressBar.Visible = false;
                    statusLabel.Visible = false;
                }
            }
        }

        private async void DecryptFileButton_Click(object sender, EventArgs e)
        {
            byte[]? passphrase = null;
            try
            {
                passphrase = GetPassphraseFromForm1();

                using var ofd = new OpenFileDialog
                {
                    Title = "Select an encrypted file (.enc)",
                    CheckFileExists = true,
                    InitialDirectory = AppSettings.GetLastUsedDirectory() ?? string.Empty
                };

                if (ofd.ShowDialog() != DialogResult.OK) return;

                string inEncPath = ofd.FileName;
                AppSettings.SetLastUsedDirectory(Path.GetDirectoryName(inEncPath));

                using var sfd = new SaveFileDialog
                {
                    Title = "Save decrypted file as",
                    FileName = Path.GetFileNameWithoutExtension(inEncPath),
                    OverwritePrompt = true,
                    InitialDirectory = AppSettings.GetLastUsedDirectory() ?? string.Empty
                };

                if (sfd.ShowDialog() != DialogResult.OK) return;

                string outPath = sfd.FileName;

                ToggleUI(false);
                long fileLen = new FileInfo(inEncPath).Length; // Note: .enc is slightly larger than original but close enough for progress
                fileProgressBar.Maximum = 100;
                fileProgressBar.Value = 0;
                fileProgressBar.Visible = true;
                statusLabel.Text = "Decrypting...";
                statusLabel.Visible = true;

                var progress = new Progress<long>(bytes =>
                {
                    if (fileLen > 0)
                        fileProgressBar.Value = Math.Min(100, (int)(100 * bytes / fileLen));
                });

                bool ok = await Task.Run(() => CryptoEngineBlueprint.DecryptFile(inEncPath, outPath, passphrase, progress: progress));

                if (!this.IsDisposed)
                {
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
            }
            catch (Exception ex)
            {
                if (!this.IsDisposed)
                    MessageBox.Show("Decryption failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (passphrase != null) CryptographicOperations.ZeroMemory(passphrase);
                if (!this.IsDisposed)
                {
                    ToggleUI(true);
                    fileProgressBar.Visible = false;
                    statusLabel.Visible = false;
                }
            }
        }

        private void ExportKeysButton_Click(object sender, EventArgs e)
        {
            byte[]? keyParts = null;
            byte[]? masterPassBytes = null;
            try
            {
                // Step 1: prompt for optional master password
                bool useVaultMode = false;

                using var prompt = new PasswordPromptDialog(
                    "Enter a master password to protect this key file (Vault Mode).\n" +
                    "Click Skip to use Token Mode (lower security — hardcoded internal password).",
                    PasswordPromptDialog.ButtonMode.OkSkip);

                DialogResult promptResult = prompt.ShowDialog(this);

                if (promptResult == DialogResult.Cancel)
                    return; // user aborted entirely

                if (promptResult == DialogResult.OK)
                {
                    masterPassBytes = Encoding.UTF8.GetBytes(prompt.Password);
                    useVaultMode = true;
                }
                else // DialogResult.No == Skip → Token Mode
                {
                    DialogResult warn = MessageBox.Show(
                        "⚠️ SECURITY ALERT: You are exporting your keys without a master password (Token Mode).\n\n" +
                        "Token Mode uses a hardcoded internal password baked into the application. " +
                        "Anyone with a copy of EncSypher can attempt to decrypt a Token-mode key file.\n\n" +
                        "Treat this file like a physical master key — do NOT share it publicly or store it in " +
                        "an untrusted location.\n\n" +
                        "Do you want to continue with Token Mode?",
                        "Security Alert — Token Mode",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (warn != DialogResult.Yes) return;

                    useVaultMode = false;
                }

                // Step 2: read keys from Form1
                Form1? form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
                if (form1 == null)
                {
                    MessageBox.Show("Form1 is not open.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                keyParts = form1.GetSecureKeyParts();

                // Step 3: choose save location
                using var sfd = new SaveFileDialog
                {
                    Title            = "Export key file",
                    Filter           = "EncSypher Key File (*.keyenc)|*.keyenc|All Files (*.*)|*.*",
                    DefaultExt       = "keyenc",
                    FileName         = "mykeys.keyenc",
                    OverwritePrompt  = true,
                    InitialDirectory = AppSettings.GetLastUsedDirectory() ?? string.Empty
                };

                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                AppSettings.SetLastUsedDirectory(Path.GetDirectoryName(sfd.FileName));

                // Step 4: export and write
                int mode = useVaultMode ? 1 : 0;
                byte[] keyEncData = CryptoEngineBlueprint.ExportKeys(keyParts, mode, masterPassBytes);
                File.WriteAllBytes(sfd.FileName, keyEncData);
                Array.Clear(keyEncData, 0, keyEncData.Length);

                MessageBox.Show(
                    $"Keys exported successfully ({(useVaultMode ? "Vault Mode" : "Token Mode")}).",
                    "Export Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export failed:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (keyParts != null) CryptographicOperations.ZeroMemory(keyParts);
                if (masterPassBytes != null) CryptographicOperations.ZeroMemory(masterPassBytes);
            }
        }

        private void ImportKeysButton_Click(object sender, EventArgs e)
        {
            byte[]? masterPassBytes = null;
            try
            {
                // Step 1: open .keyenc file
                using var ofd = new OpenFileDialog
                {
                    Title            = "Import key file",
                    Filter           = "EncSypher Key File (*.keyenc)|*.keyenc|All Files (*.*)|*.*",
                    CheckFileExists  = true,
                    InitialDirectory = AppSettings.GetLastUsedDirectory() ?? string.Empty
                };

                if (ofd.ShowDialog(this) != DialogResult.OK) return;

                AppSettings.SetLastUsedDirectory(Path.GetDirectoryName(ofd.FileName));

                byte[] data = File.ReadAllBytes(ofd.FileName);

                // Step 2: validate minimum size and peek at mode byte (index 5)
                if (data.Length < 6)
                {
                    MessageBox.Show("File is too small to be a valid .keyenc file.",
                        "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Array.Clear(data, 0, data.Length);
                    return;
                }

                byte modeFlag = data[5];

                if (modeFlag == 0x01) // Vault mode — ask for password
                {
                    using var prompt = new PasswordPromptDialog(
                        "This key file is Vault-protected.\nEnter the master password to decrypt it:",
                        PasswordPromptDialog.ButtonMode.OkCancel);

                    if (prompt.ShowDialog(this) != DialogResult.OK)
                    {
                        Array.Clear(data, 0, data.Length);
                        return;
                    }

                    masterPassBytes = Encoding.UTF8.GetBytes(prompt.Password);
                }

                // Step 3: decrypt
                var (k1, k2, k3, k4) = CryptoEngineBlueprint.ImportKeys(data, masterPassBytes);
                Array.Clear(data, 0, data.Length);

                // Step 4: get Form1 and enable masking BEFORE populating key boxes
                Form1? form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
                if (form1 == null)
                {
                    MessageBox.Show("Form1 is not open.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                form1.Keybox1.UseSystemPasswordChar = true;
                form1.keybox2.UseSystemPasswordChar = true;
                form1.keybox3.UseSystemPasswordChar = true;
                form1.keybox4.UseSystemPasswordChar = true;

                // Sync the Hide Keys checkbox on this form
                hideKeysCheckbox.Checked = true;

                // Step 5: populate
                form1.Keybox1.Text = k1;
                form1.keybox2.Text = k2;
                form1.keybox3.Text = k3;
                form1.keybox4.Text = k4;

                MessageBox.Show("Keys imported successfully.",
                    "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                MessageBox.Show(
                    "Import failed: wrong password, or the file has been tampered with.",
                    "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (InvalidDataException ex)
            {
                MessageBox.Show("Import failed: " + ex.Message,
                    "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Import failed:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (masterPassBytes != null) CryptographicOperations.ZeroMemory(masterPassBytes);
            }
        }

        private byte[] GetPassphraseFromForm1()
        {
            Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (form1 == null) throw new InvalidOperationException("Form1 is not open.");

            return form1.GetSecurePassphrase();
        }

        private void ToggleUI(bool enabled)
        {
            EncryptFileButton.Enabled = enabled;
            DecryptFileButton.Enabled = enabled;
            ExportKeysButton.Enabled = enabled;
            ImportKeysButton.Enabled = enabled;
            randomnisekeyvaluebutton.Enabled = enabled;
            form2close.Enabled = enabled;
            hideKeysCheckbox.Enabled = enabled;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
