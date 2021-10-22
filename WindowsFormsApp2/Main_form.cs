using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using System.Security;

using Encryption.Helpers;

namespace WindowsFormsApp2
{

    public partial class Main_form : Form
    {
        byte[] cloud_bytes;
        
        string k;

        Helpers Encrypting = new Helpers();

        Prompt_form parentForm;
        public Main_form(Prompt_form parent)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            parentForm = parent;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (o.ShowDialog()==DialogResult.OK)
            {
                textBox1.Text = File.ReadAllText(o.FileName, Encoding.GetEncoding(1251));
                var s = File.ReadAllText(o.FileName, Encoding.GetEncoding(1251));
                cloud_bytes = Encoding.GetEncoding(1251).GetBytes(s);
            }
            else
            {
                MessageBox.Show("Помилка. Файл не обрано");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = saveFileDialog1.FileName;
            System.IO.File.WriteAllText(filename, textBox2.Text, Encoding.GetEncoding(1251));
        }
        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (o.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = File.ReadAllText(o.FileName, Encoding.GetEncoding(1251));
                var s = File.ReadAllText(o.FileName, Encoding.GetEncoding(1251));
                cloud_bytes = Encoding.GetEncoding(1251).GetBytes(s);
            }
            else
            {
                MessageBox.Show("Помилка. Файл не обрано");
            }
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            k = Convert.ToString(textBox3.Text);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.Checked == true)
            {
                button6.Enabled = false;
                textBox3.ReadOnly = false;
            }
            else
            {
                button6.Enabled = true;
                textBox3.ReadOnly = true;
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            var input = textBox1.Text;
            k = textBox3.Text;

            var answer = Encrypting.Encode(input, k);
            cloud_bytes = answer;

            textBox2.Text = Encrypting.BytesToString(answer);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
           // var input = Encoding.UTF8.GetBytes(textBox1.Text);
            k = textBox3.Text;
            var s = textBox1.Text;
            try
            {
                cloud_bytes = Convert.FromBase64String(s);
                string answer = Encrypting.Decode(cloud_bytes, k);

                if (answer == "")
                {
                    MessageBox.Show("Помилка. Текст не рошифрувався, невірний ключ або початковий текст не зашифровано правильно");
                }

                textBox2.Text = answer;
            }
            catch 
            {
                MessageBox.Show("Помилка. Текст для розшифрування має бути у форматі base64");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            k = Convert.ToString(textBox3.Text);
            var s = Convert.ToString(textBox1.Text);
            cloud_bytes = Encoding.GetEncoding(1251).GetBytes(s);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = saveFileDialog1.FileName;
            System.IO.File.WriteAllText(filename, textBox1.Text, Encoding.Default);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.FormOwnerClosing)
                parentForm.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox2.Text;
            //cloud_bytes = Convert.FromBase64String(textBox2.Text);
            textBox2.Text = "";
        }
    }
}

        

