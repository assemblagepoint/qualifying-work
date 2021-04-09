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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        uint[] data; //массив для хранения параметров типа uint
        uint[] data2; //массив для хранения параметров типа uint
        ushort size_memory; //размер памяти в байтах
        ushort size_header; //размер заголовка в байтах

        private void button4_Click(object sender, EventArgs e) //кнопка получить параметры
        {
            MemoryMappedFile sharedMemory = MemoryMappedFile.OpenExisting("SIMITSharedMemory");
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(0, 4, MemoryMappedFileAccess.Read))
            {
                size_memory = reader.ReadUInt16(0);
            }
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(4, 4, MemoryMappedFileAccess.Read))
            {
                size_header = reader.ReadUInt16(0);
            }
            label7.Text = "Размер общей памяти: " + size_memory.ToString();
            label10.Text = "Размер заголовка: " + size_header.ToString();
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                data = new uint[size_memory];
                reader.ReadArray<uint>(0, data, 0, size_memory);
            }
            label5.Text = "1) " + data[0].ToString();
            label6.Text = "2) " + data[1].ToString();
            label8.Text = "3) " + data[2].ToString();
        }

        private void button3_Click(object sender, EventArgs e) //кнопка отправить параметры
        {
            MemoryMappedFile sharedMemory = MemoryMappedFile.OpenExisting("SIMITSharedMemory");
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header + 12, size_memory, MemoryMappedFileAccess.Write))
            {
                data2 = new uint[3];
                data2[0] = Convert.ToUInt32(textBox3.Text);
                data2[1] = Convert.ToUInt32(textBox4.Text);
                data2[2] = Convert.ToUInt32(textBox2.Text);

                writer.Write(0, data2[0]);
                writer.Write(4, data2[1]);
                writer.Write(8, data2[2]);
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void eventLog1_EntryWritten(object sender, System.Diagnostics.EntryWrittenEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
