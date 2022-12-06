using System.Timers;
using System.Threading.Tasks;
using AngleSharp.Common;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;

namespace ReservationGUI
{
    public partial class Form1 : Form
    {
        public static System.Timers.Timer timer = new System.Timers.Timer(1000 * 120);
        public static string notify = "";

        public class Globals
        {
            public static Form1 form;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Globals.form = this;
            textBox1.Text = Properties.Settings.Default.textBox1;
            textBox2.Text = Properties.Settings.Default.textBox2;
            checkBox1.Checked = Properties.Settings.Default.checkBox1;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.textBox1 = textBox1.Text;
            Properties.Settings.Default.textBox2 = textBox2.Text;
            Properties.Settings.Default.checkBox1 = checkBox1.Checked;
            Properties.Settings.Default.Save();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar));
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;

            Thread thread1 = new Thread(MyMain);
            thread1.Start();

            if (notify != null)
            {
                textBox3.Text = notify;
            }

            timer = new System.Timers.Timer(1000 * 120);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
        }


        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            MyMain();
            if (notify != null)
            {
                textBox3.Text = notify;
            }
        }

        // -------------------------------------------------------------------------------
        private static void MyMain()
        {
            timer.Enabled = false;

            Browser browser = new Browser(); 
            notify = browser.Get(Int32.Parse(Globals.form.textBox1.Text), Int32.Parse(Globals.form.textBox2.Text));

            //if (notify != null)
            //{
            //    Globals.form.textBox3.AppendText(notify);
            //}

            MessageBox.Show(notify);

            timer.Enabled = Globals.form.checkBox1.Checked;

            if (!timer.Enabled)
            {
                Globals.form.button1.Enabled = true;
                Globals.form.button2.Enabled = false;
            }

            return;
        }


    }
}