using System.Security.Cryptography;

namespace encryption_cypher_app
{
    public partial class Form1 : Form
    {
        public string key1;
        public string key2;
        public string key3;
        public string key4;
        public string texttoencrypt;
        public string texttodecrypt;


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
            key1 = Keybox1.Text;
            key2 = keybox2.Text;
            key3 = keybox3.Text;
            key4 = keybox4.Text;

            TextBoxEncryptOutput.Clear();
            string? texttoencrypt = TextboxEncryptioninput.Text;
            if (!string.IsNullOrEmpty(texttoencrypt)) // Check if the message is not null
            {TextBoxEncryptOutput.Text = CryptoEngineBlueprint.EncryptToBase64(texttoencrypt, key1, key2, key3, key4);}
            else
            {MessageBox.Show("No message entered");}

        }

        private void Decryptbutton_Click(object sender, EventArgs e)
        {
            key1 = Keybox1.Text;
            key2 = keybox2.Text;
            key3 = keybox3.Text;
            key4 = keybox4.Text;

            TextBoxDecryptOutput.Clear();
            string? texttodecrypt = TextBoxDecryptionInput.Text;
            if (!string.IsNullOrEmpty(texttodecrypt)) // Check if the message is not null or empty
            {
                var (plaintext, tagOk) = CryptoEngineBlueprint.DecryptFromBase64(texttodecrypt, bogusOnFail: true, key1, key2, key3, key4);
                TextBoxDecryptOutput.Text = plaintext;
                if(tagOk == true) 
                {authenticationlabel.Visible = true;}
                else 
                { authenticationlabel.Visible = false;}
            }
            else
            {MessageBox.Show("No message entered");}
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

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
