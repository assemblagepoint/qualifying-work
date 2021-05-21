using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace qualifyingwork
{
    public partial class Info : Form
    {
        Form MainForm;
        public Info(Form MainForm)
        {
            InitializeComponent();
            this.MainForm = MainForm;
        }

        //закрытие
        private void Info_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.Visible = true;
        }

        //сведения о системе
        private void InfoSystem_button_Click(object sender, EventArgs e)
        {
            GetInfo();
        }

        // Открытие html файла
        void GetInfo()
        {
            try
            {
                Process.Start(Environment.CurrentDirectory + "\\userguide\\help.html");
            }
            catch
            {
                MessageBox.Show("Невозможно открыть справку. Файл справки поврежден или не найден.");
            }
        }

    }
}
