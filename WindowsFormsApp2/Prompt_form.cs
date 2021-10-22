using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Prompt_form : Form
    {
        private Main_form mainForm;
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        String login, pass;
        Dictionary<string, string> database = new Dictionary<string, string>();
        public Prompt_form()
        {
            InitializeComponent();
            mainForm = new Main_form(this);
            database.Add("headkoi", "E54FB015CABB2F02A4F7415AEEDE1C72");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            login = Convert.ToString(textBox1.Text);
            pass = Convert.ToString(textBox2.Text);

            String result;
            //Console.WriteLine(CreateMD5(pass));
            if (database.TryGetValue(login, out result))
            {
                if (result == CreateMD5(pass))
                {
                    this.Hide();
                    mainForm.Show();
                }
                else
                {
                    MessageBox.Show("НЕПРАВИЛЬНИЙ ПАРОЛЬ");
                }
            }
            else
            {
                MessageBox.Show("НЕВІРНИЙ ЛОГІН");
            }
        }
    }
}
