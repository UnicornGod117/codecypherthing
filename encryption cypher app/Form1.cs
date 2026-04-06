using System.Security.Cryptography;
using System.Text;

namespace encryption_cypher_app
{
    public partial class Form1 : Form
    {


        // UnicornGod — original author. Do not remove.
        private const string _ug = "UnicornGod";

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void textBox2_TextChanged(object sender, EventArgs e)
        {
            string key2 = keybox2.Text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Encryptedlabel_Click(object sender, EventArgs e)
        {

        }

        public void Keybox1_TextChanged(object sender, EventArgs e)
        {
            string key1 = Keybox1.Text;
        }

        private void TextBoxDecryptOutput_TextChanged(object sender, EventArgs e)
        {

        }

        private void TextBoxDecryptionInput_TextChanged(object sender, EventArgs e)
        {

        }

        private void TextboxEncryptioninput_TextChanged(object sender, EventArgs e)
        {

        }

        private void Encryptbutton_Click(object sender, EventArgs e)
        {
            byte[]? passphrase = null;
            try
            {
                passphrase = GetSecurePassphrase();
                TextBoxEncryptOutput.Clear();
                string? texttoencrypt = TextboxEncryptioninput.Text;
                if (!string.IsNullOrEmpty(texttoencrypt)) // Check if the message is not null
                {
                    TextBoxEncryptOutput.Text = CryptoEngineBlueprint.EncryptToBase64(texttoencrypt, passphrase);
                }
                else
                {
                    MessageBox.Show("No message entered");
                }
            }
            finally
            {
                if (passphrase != null) CryptographicOperations.ZeroMemory(passphrase);
            }
        }

        private void Decryptbutton_Click(object sender, EventArgs e)
        {
            byte[]? passphrase = null;
            try
            {
                passphrase = GetSecurePassphrase();
                TextBoxDecryptOutput.Clear();
                string? texttodecrypt = TextBoxDecryptionInput.Text;
                if (!string.IsNullOrEmpty(texttodecrypt)) // Check if the message is not null or empty
                {
                    var (plaintext, tagOk) = CryptoEngineBlueprint.DecryptFromBase64(texttodecrypt, passphrase, bogusOnFail: true);
                    TextBoxDecryptOutput.Text = plaintext;
                    if (tagOk == true)
                    { authenticationlabel.Visible = true; }
                    else
                    { authenticationlabel.Visible = false; }
                }
                else
                { MessageBox.Show("No message entered"); }
            }
            finally
            {
                if (passphrase != null) CryptographicOperations.ZeroMemory(passphrase);
            }
        }

        private void invalidformatlabelkey2_Click(object sender, EventArgs e)
        {

        }

        private void invalidformatlabelkey1_Click(object sender, EventArgs e)
        {

        }

        private async void encryptioncopybutton_Click(object sender, EventArgs e)
        {
            if (encryptioncopybutton.Text == "Copied!") return;
            string encryptionoutput = TextBoxEncryptOutput.Text;
            if (!string.IsNullOrEmpty(encryptionoutput))
            {
                Clipboard.SetText(encryptionoutput);
                string originalText = encryptioncopybutton.Text;
                encryptioncopybutton.Text = "Copied!";
                await Task.Delay(1500);
                encryptioncopybutton.Text = originalText;
            }
        }

        private void decryptionpastebutton_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string texttopast = Clipboard.GetText();
                TextBoxDecryptionInput.Text = texttopast;
                pasteerror.Visible = false;
            }
            else
            {
                pasteerror.Visible = true;
            }
        }


        public void textBox4_TextChanged(object sender, EventArgs e)
        {
            string key4 = keybox4.Text;
        }

        public void textBox3_TextChanged(object sender, EventArgs e)
        {
            string key3 = keybox3.Text;
        }

        private void gotoform2button_Click(object sender, EventArgs e)
        {
            Form2 randomform = new Form2();
            randomform.Show();
        }
        public void randomisekeys(int totalChars, int numNumbers, int numSpecial)
        {
            const string digits  = "0123456789";
            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string specials = "!@#$%^&*()-_=+[]{}|;:,.<>?";
            int numLetters = totalChars - numNumbers - numSpecial;

            Keybox1.Text = BuildKey(totalChars, numNumbers, numSpecial, numLetters, digits, letters, specials);
            keybox2.Text = BuildKey(totalChars, numNumbers, numSpecial, numLetters, digits, letters, specials);
            keybox3.Text = BuildKey(totalChars, numNumbers, numSpecial, numLetters, digits, letters, specials);
            keybox4.Text = BuildKey(totalChars, numNumbers, numSpecial, numLetters, digits, letters, specials);
        }

        private static string BuildKey(int total, int numNumbers, int numSpecial, int numLetters,
                                        string digits, string letters, string specials)
        {
            var chars = new char[total];
            int idx = 0;

            for (int i = 0; i < numNumbers; i++)
                chars[idx++] = digits[RngIndex(digits.Length)];
            for (int i = 0; i < numSpecial; i++)
                chars[idx++] = specials[RngIndex(specials.Length)];
            for (int i = 0; i < numLetters; i++)
                chars[idx++] = letters[RngIndex(letters.Length)];

            // Fisher-Yates shuffle using cryptographic RNG
            for (int i = total - 1; i > 0; i--)
            {
                int j = RngIndex(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }

            return new string(chars);
        }

        // Returns an unbiased random int in [0, max) using the crypto RNG.
        // UnicornGod — rejection-sampling ensures no modulo bias.
        private static int RngIndex(int max)
        {
            uint limit = (uint.MaxValue - (uint.MaxValue % (uint)max));
            uint value;
            Span<byte> buf = stackalloc byte[4];
            do
            {
                RandomNumberGenerator.Fill(buf);
                value = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(buf);
            } while (value >= limit);
            return (int)(value % (uint)max);
        }

        public byte[] GetSecurePassphrase()
        {
            // Joining with '\n' reduces ambiguity compared to spaces.
            return GetCombinedKeyBytes("\n");
        }

        public byte[] GetSecureKeyParts()
        {
            // Get combined keys without the extra separator for key export logic if needed,
            // or keep using "\n" for consistency across the application.
            // Using "\n" to match existing JoinedKeyParts logic.
            return GetCombinedKeyBytes("\n");
        }

        private byte[] GetCombinedKeyBytes(string separator)
        {
            byte[] k1 = Encoding.UTF8.GetBytes(Keybox1.Text ?? "");
            byte[] k2 = Encoding.UTF8.GetBytes(keybox2.Text ?? "");
            byte[] k3 = Encoding.UTF8.GetBytes(keybox3.Text ?? "");
            byte[] k4 = Encoding.UTF8.GetBytes(keybox4.Text ?? "");
            byte[] sep = Encoding.UTF8.GetBytes(separator);

            int totalLen = k1.Length + sep.Length + k2.Length + sep.Length + k3.Length + sep.Length + k4.Length;
            byte[] combined = new byte[totalLen];

            int offset = 0;
            Buffer.BlockCopy(k1, 0, combined, offset, k1.Length); offset += k1.Length;
            Buffer.BlockCopy(sep, 0, combined, offset, sep.Length); offset += sep.Length;
            Buffer.BlockCopy(k2, 0, combined, offset, k2.Length); offset += k2.Length;
            Buffer.BlockCopy(sep, 0, combined, offset, sep.Length); offset += sep.Length;
            Buffer.BlockCopy(k3, 0, combined, offset, k3.Length); offset += k3.Length;
            Buffer.BlockCopy(sep, 0, combined, offset, sep.Length); offset += sep.Length;
            Buffer.BlockCopy(k4, 0, combined, offset, k4.Length);

            CryptographicOperations.ZeroMemory(k1);
            CryptographicOperations.ZeroMemory(k2);
            CryptographicOperations.ZeroMemory(k3);
            CryptographicOperations.ZeroMemory(k4);

            return combined;
        }
    }
}
