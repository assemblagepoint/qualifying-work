using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace qualifyingwork
{
    public partial class MainForm : Form
    {
        string id; 

        MemoryMappedFile SM;

        public MainForm()
        {
            InitializeComponent();
        }

        //начать управление моделью
        private void button2_Click(object sender, EventArgs e)
        {
            
            ControlGA ga = new ControlGA(this);
            ga.Show();
            this.Visible = false;
        }

        //справка
        private void button3_Click(object sender, EventArgs e)
        {
            Info inf = new Info(this);
            inf.Show();
            this.Visible = false;
        }

        //проверить соединение
        private void button1_Click(object sender, EventArgs e)
        {
            id = textBox1.Text;

            if (textBox1.Text == "")
            {
                label2.Text = "Для проверка соединения введите id модели в поле выше!";
            }
            else
            {
                try
                {
                    SM = MemoryMappedFile.OpenExisting(id); 
                    label2.Text = "Соединение успешно установлено!";
                    label2.BackColor = Color.LightGreen;
                    
                }
                catch
                {
                    label2.Text = "Соединение не установлено!";
                    label2.BackColor = Color.Red;
                   
                }
            }

        }
    }
}
