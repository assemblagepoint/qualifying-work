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

        private void button1_Click(object sender, EventArgs e)
        {
            //Ввод выражения для записи в общую память
            char[] message = textBox1.Text.ToCharArray();
            //Размер введенного сообщения
            int size = message.Length;

            //Создание участка разделяемой памяти
            //Первый параметр - название участка, 
            //второй - длина участка памяти в байтах: тип char  занимает 2 байта 
            //плюс четыре байта для одного объекта типа Integer
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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Массив для сообщения из общей памяти
            char[] message2;
            //Размер введенного сообщения
            int size2;

            //Получение существующего участка разделяемой памяти
            //Параметр - название участка
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
            textBox2.Text = msg;
            label2.Text = "Получено сообщение : " + msg;
        }

        private void button4_Click(object sender, EventArgs e)
        {         
            char[] message2;
            ushort[] data;
            ushort size_memory; //размер памяти
            ushort size_header; //размер заголовка

            MemoryMappedFile sharedMemory = MemoryMappedFile.OpenExisting("SIMITSharedMemory");
            //Сначала считываем размер сообщения, чтобы создать массив данного размера
            //Integer занимает 4 байта, начинается с первого байта, поэтому передаем цифры 0 и 4
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
            //Считываем сообщение, используя полученный выше размер
            //Сообщение - это строка или массив объектов char, каждый из которых занимает два байта
            //Поэтому вторым параметром передаем число символов умножив на из размер в байтах плюс
            //А первый параметр - смещение - 112 байта, которое занимает размер сообщения
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(112, size_memory * 2, MemoryMappedFileAccess.Read))
            {
                //Массив символов сообщения               
                data = new ushort[size_memory];
                reader.ReadArray<ushort>(0, data, 0, size_memory);
            }

  
            label5.Text = "1) " + data[0].ToString();
            label6.Text = "2) " + data[1].ToString();
            label8.Text = "3) " + data[2].ToString();
            label9.Text = "4) " + data[3].ToString();
            
        }
    }
}
