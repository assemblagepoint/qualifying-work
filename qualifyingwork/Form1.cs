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

        private void button1_Click(object sender, EventArgs e)
        {
            char[] message = textBox1.Text.ToCharArray();
            int size = message.Length;
            MemoryMappedFile sharedMemory = MemoryMappedFile.CreateOrOpen("MemoryFile", size * 2 + 4);
            //Создаем объект для записи в разделяемый участок памяти
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(0, size * 2 + 4))
            {
                //запись в разделяемую память
                //запись размера с нулевого байта в разделяемой памяти
                writer.Write(0, size);
                //запись сообщения с четвертого байта в разделяемой памяти
                writer.WriteArray<char>(4, message, 0, message.Length);
            }        
        } //кнопка отправить
        private void button2_Click(object sender, EventArgs e)
        {
            char[] message2;
            int size2;    
            MemoryMappedFile sharedMemory = MemoryMappedFile.OpenExisting("MemoryFile");
            //Сначала считываем размер сообщения, чтобы создать массив данного размера
            //Integer занимает 4 байта, начинается с первого байта, поэтому передаем цифры 0 и 4
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(0, 4, MemoryMappedFileAccess.Read))
            {
                size2 = reader.ReadInt32(0);
            }

            //Считываем сообщение, используя полученный выше размер
            //Сообщение - это строка или массив объектов char, каждый из которых занимает два байта
            //Поэтому вторым параметром передаем число символов умножив на из размер в байтах плюс
            //А первый параметр - смещение - 4 байта, которое занимает размер сообщения
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(4, size2 * 2, MemoryMappedFileAccess.Read))
            {
                //Массив символов сообщения
                message2 = new char[size2];
                reader.ReadArray<char>(0, message2, 0, size2);
            }

            string msg = new string(message2);           
            label2.Text = "Получено сообщение : " + msg;
        } //кнопка получить
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
                data2 = new uint[size_memory - 12];
                data2[0] = Convert.ToUInt32(textBox3.Text);
                data2[1] = Convert.ToUInt32(textBox4.Text);
                data2[2] = Convert.ToUInt32(textBox2.Text);
                //writer.Write(0, 3);               
                writer.WriteArray<uint>(size_header, data2, 12, 3);
            }
        }


        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
