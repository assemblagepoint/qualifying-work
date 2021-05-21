using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace qualifyingwork
{
    public partial class ControlGA : Form
    {
        Form MainForm;

        public static System.Timers.Timer timer;

        public ControlGA(Form MainForm)
        {
            InitializeComponent();
            this.MainForm = MainForm;

            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
            timer.Enabled = true;
        }

        static void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
 
        }

        ushort size_memory; //размер памяти в байтах
        ushort size_header; //размер заголовка в байтах

        //сигналы В модель
        private bool Start; //Пуск ГА, Пуск
        private bool Stop; //Останов ГА, Ост-в
        private bool EmergencyStop; //Аварийный останов ГА
        private bool ShieldsRls_2KA; //Сброс Б/П щитов
        private bool ACK; //ACK
        private bool Breakjack_dropped; //Тормозные домкраты опущены
        private bool Air_no_pressure; //Нет давления технологического воздуха
        private bool SPAZ_state; //Контроль положения СПАЗ
        private bool Stopor = true; //Стопор введен
        private bool ManualMode; //Режим управления ГА, Ручной режим.
        private bool VG; //Включение воздушного выключателя, В-Г: Вкл
        private bool VG_off; //отключение воздушного выключателя, В-Г: Выкл
        private bool RP3_sw = false; //АГП Включен
        private float T_zal; //Температура в маш. зале
        private float H_lim_Sh1; //Щит А верх
        private float L_lim_Sh1; //Щит А низ
        private float H_lim_Sh2; //Щит Б верх
        private float L_lim_Sh2; //Щит А низ
        private float T_smooth_GA18 = 2; //Реакция регулятора
        private float Q = 0; //Мощность реактивная
        private float U_steti = 13; //Напряжение сети
        private float Slider_P = 50; //Мощность активная
       
        //сигналы ИЗ модели
        private float Shield1_pos; //положение щита А
        private float Shield2_pos; //положение щита Б
        private float H_ResVal; //напор
        private float N_turb; //обороты
        private float NA_idle; //пусковое открытие НА
        private float N_turb_idle; //обороты на ХХ
        private bool Not_Ready_mode; // не готов к пуску
        private bool Ready_mode; // готов к пуску
        private bool Start_mode; // пуск
        private bool Load_mode; // в сети
        private bool Stop_mode; // останов
        private bool ES_DF_mode; // А.останов (защиты)
        private bool ES_Runawa1_mode; // А.останов (1-ступень)
        private bool ES_Runawa2_mode; // А.останов (2-ступень)
        private bool SK_load_mode; // СК
        private bool Auto_mode; //Режим автомат
        private bool Man_mode; //Режим ручной 
        private bool RP3; // АГП вкл
        private bool RP3_off; // АГП выкл
        private bool I_VG; // В-Г: Вкл
        private bool I_VG_off; // В-Г: Выкл


        //Закрытие формы
        private void ControlGA_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.Visible = true;
        }

        MemoryMappedFile sharedMemory1 = MemoryMappedFile.CreateOrOpen("SIMITSharedMemory", 204); //костыль для запуска

        MemoryMappedFile sharedMemory = MemoryMappedFile.OpenExisting("SIMITSharedMemory");

        private void button4_Click(object sender, EventArgs e) //кнопка получить параметры
        {
            //using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(0, 8, MemoryMappedFileAccess.Read))
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(0, 8, MemoryMappedFileAccess.Read))//проверка костыля
            {
                size_memory = reader.ReadUInt16(0);
                size_header = reader.ReadUInt16(4);
            }
            label7.Text += size_memory.ToString();
            label10.Text += size_header.ToString();
        }

        #region МЕТОДЫ ОТПРАВКА

        //Кнопка ПУСК (Пуск ГА)
        private void button12_Click(object sender, EventArgs e)
        {
            _Start();
        }

        private void _Start()
        {
            Start = true;
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(0, Start);
            }
        }

        //Кнопка СТОП (Останов ГА)
        private void button11_Click(object sender, EventArgs e)
        {
            _Stop();
        }

        private void _Stop()
        {
            Stop = true;
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(1, Stop);
            }
        }

        //Кнопка 1КА (Аварийный останов ГА)
        private void button10_Click(object sender, EventArgs e)
        {
            _EmergencyStop();
        }

        private void _EmergencyStop()
        {
            EmergencyStop = true;
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(2, EmergencyStop);
            }
        }

        //Кнопка 2КА (Сброс Б/П щитов)
        private void button9_Click(object sender, EventArgs e)
        {
            _ShieldsRls_2KA();
        }

        private void _ShieldsRls_2KA()
        {
            ShieldsRls_2KA = true;
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(3, ShieldsRls_2KA);
            }
        }

        //Кнопка АСК (Автоматическая система контроля)
        private void button8_Click(object sender, EventArgs e)
        {
            _ACK();
        }

        private void _ACK()
        {
            ACK = true;
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(50, ACK);
            }
        }

        //Выключатель генератора Выкл.
        private void button1_Click(object sender, EventArgs e)
        {
            _VG_off();
        }

        private void _VG_off()
        {
            VG_off = true;
            VG = false;
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(11, VG_off);
                writer.Write(10, VG);
            }
        }

        //Выключатель генератора Вкл.
        private void button6_Click(object sender, EventArgs e)
        {
            _VG();
        }

        private void _VG()
        {
            VG = true;
            VG_off = false;
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(10, VG);
                writer.Write(11, VG_off);
            }
        }

        //Автомат гашения поля
        private void button7_Click(object sender, EventArgs e)
        {
            _RP3_sw();
        }

        private void _RP3_sw()
        {
            if (RP3_sw == false)
            {
                RP3_sw = true;
            }
            else
            {
                RP3_sw = false;
            }
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(12, RP3_sw);
            }
        }

        //Кнопка Домкраты поднять
        private void button13_Click(object sender, EventArgs e)
        {
            _Breakjack_dropped();
        }

        private void _Breakjack_dropped()
        {
            Breakjack_dropped = true;
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(4, Breakjack_dropped);
            }
        }

        //Кнопка Тех воздух перекрыть
        private void button16_Click(object sender, EventArgs e)
        {
            _Air_no_pressure();
        }

        private void _Air_no_pressure()
        {
            Air_no_pressure = true;
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(5, Air_no_pressure);
            }
        }

        //Кнопка Стопор вывести
        private void button15_Click(object sender, EventArgs e)
        {
            _Stopor();
        }

        private void _Stopor()
        {
            if (Stopor == true)
            {
                Stopor = false;
            }
            else
            {
                Stopor = true;
            }
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(7, Stopor);
            }
        }

        //Кнопка Инверт.СПАЗ
        private void button14_Click(object sender, EventArgs e)
        {
            _SPAZ_state();
        }

        private void _SPAZ_state()
        {
            if (SPAZ_state == false)
            {
                SPAZ_state = true;
            }
            else
            {
                SPAZ_state = false;
            }

            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(6, SPAZ_state);
            }
        }

        //РЕЖИМ
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            _ManualMode();
        }

        private void _ManualMode()
        {
            if (trackBar1.Value == 0)//Автомат
            {
                ManualMode = false;
            }
            else //Ручной
            {
                ManualMode = true;
            }
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(9, ManualMode);
            }
        }

        //Температура в маш. зале
        private void textBox8_Enter(object sender, EventArgs e)
        {
            _T_zal();
        }

        private void _T_zal()
        {
            T_zal = Convert.ToSingle(textBox8.Text);
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(14, T_zal);
            }
        }

        //Щит А верх
        private void textBox7_Enter(object sender, EventArgs e)
        {
            _H_lim_Sh1();
        }

        private void _H_lim_Sh1()
        {
            H_lim_Sh1 = Convert.ToSingle(textBox7.Text);
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(18, H_lim_Sh1);
            }
        }

        //Щит А низ
        private void textBox6_Enter(object sender, EventArgs e)
        {
            _L_lim_Sh1();
        }

        private void _L_lim_Sh1()
        {
            L_lim_Sh1 = Convert.ToSingle(textBox6.Text);
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(22, L_lim_Sh1);
            }
        }

        //Щит Б верх
        private void textBox1_Enter(object sender, EventArgs e)
        {
            _H_lim_Sh2();
        }

        private void _H_lim_Sh2()
        {
            H_lim_Sh2 = Convert.ToSingle(textBox1.Text);
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(26, H_lim_Sh2);
            }
        }

        //Щит Б низ
        private void textBox5_Enter(object sender, EventArgs e)
        {
            _L_lim_Sh2();
        }

        private void _L_lim_Sh2()
        {
            L_lim_Sh2 = Convert.ToSingle(textBox5.Text);
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(30, L_lim_Sh2);
            }
        }

        //Реакция регулятора
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            _T_smooth_GA18();
        }
        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            _T_smooth_GA18();
        }

        private void _T_smooth_GA18()
        {
            T_smooth_GA18 = trackBar2.Value * (float)0.01;
            textBox10.Text = T_smooth_GA18.ToString();
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(34, T_smooth_GA18);
            }
        }

        //Мощность реактивная
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            _Q();
        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            _Q();
        }

        private void _Q()
        {
            Q = (float)trackBar3.Value;
            textBox11.Text = Q.ToString();
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(38, Q);
            }
        }

        //Напряжение сети
        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            _U_steti();
        }

        private void trackBar4_ValueChanged(object sender, EventArgs e)
        {
            _U_steti();
        }

        private void _U_steti()
        {
            U_steti = trackBar4.Value * (float)0.1;
            textBox12.Text = U_steti.ToString();
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(42, U_steti);
            }
        }

        //Мощность активная
        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            _Slider_P();
        }

        private void trackBar5_ValueChanged(object sender, EventArgs e)
        {
            _Slider_P();
        }

        private void _Slider_P()
        {
            Slider_P = (float)trackBar5.Value;
            textBox13.Text = Slider_P.ToString();
            using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Write))
            {
                writer.Write(46, Slider_P);
            }
        }

        #endregion

        #region   МЕТОДЫ СЧИТЫВАНИЕ

        //метод считывания положение щита А
        private void _Shield1_pos()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                Shield1_pos = reader.ReadSingle(70);
            }
            textBox2.Text = Shield1_pos.ToString();
        }   

        //метод считывания положение щита Б
        private void _Shield2_pos()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                Shield2_pos = reader.ReadSingle(74);
            }
            textBox3.Text = Shield2_pos.ToString();
        }

        //метод считывания напора
        private void _H_ResVal()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                H_ResVal = reader.ReadSingle(78);
            }
            textBox4.Text = H_ResVal.ToString();
        }

        //метод считывания оборотов
        private void _N_turb()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                N_turb = reader.ReadSingle(82);
            }
            textBox9.Text = N_turb.ToString();
        }

        //метод считывания пускового открытия НА
        private void _NA_idle()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                NA_idle = reader.ReadSingle(86);
            }
            textBox14.Text = NA_idle.ToString();
        }

        //метод считывания обороты на ХХ
        private void _N_turb_idle()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                N_turb_idle = reader.ReadSingle(90);
            }
            textBox15.Text = N_turb_idle.ToString();
        }

        //метод считывания не готов к пуску
        private void _Not_Ready_mode()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                Not_Ready_mode = reader.ReadBoolean(94);
            }
            if (Not_Ready_mode == true)
                panel9.BackColor = Color.Yellow;
            else
                panel9.BackColor = Color.Lavender;
        }

        //метод считывания готов к пуску
        private void _Ready_mode()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                Ready_mode = reader.ReadBoolean(95);
            }
            if (Ready_mode == true)
                panel8.BackColor = Color.Green;
            else
                panel8.BackColor = Color.Lavender;
        }

        //метод считывания пуск
        private void _Start_mode()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                Start_mode = reader.ReadBoolean(96);
            }
            if (Start_mode == true)
                panel7.BackColor = Color.Yellow;
            else
                panel7.BackColor = Color.Lavender;
        }

        //метод считывания в сети
        private void _Load_mode()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                Load_mode = reader.ReadBoolean(97);
            }
            if (Load_mode == true)
                panel6.BackColor = Color.Green;
            else
                panel6.BackColor = Color.Lavender;
        }

        //метод считывания останов
        private void _Stop_mode()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                Stop_mode = reader.ReadBoolean(98);
            }
            if (Stop_mode == true)
                panel5.BackColor = Color.Yellow;
            else
                panel5.BackColor = Color.Lavender;
        }

        //метод считывания А.останов (защиты)
        private void _ES_DF_mode()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                ES_DF_mode = reader.ReadBoolean(99);
            }
            if (ES_DF_mode == true)
                panel4.BackColor = Color.Red;
            else
                panel4.BackColor = Color.Lavender;
        }

        //метод считывания А.останов (1-ступень)
        private void _ES_Runawa1_mode()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                ES_Runawa1_mode = reader.ReadBoolean(100);
            }
            if (ES_Runawa1_mode == true)
                panel3.BackColor = Color.Red;
            else
                panel3.BackColor = Color.Lavender;
        }

        //метод считывания А.останов (2-ступень)
        private void _ES_Runawa2_mode()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                ES_Runawa2_mode = reader.ReadBoolean(101);
            }
            if (ES_Runawa2_mode == true)
                panel2.BackColor = Color.Red;
            else
                panel2.BackColor = Color.Lavender;
        }

        //метод считывания СК
        private void _SK_load_mode()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                SK_load_mode = reader.ReadBoolean(102);
            }
            if (SK_load_mode == true)
                panel2.BackColor = Color.Green;
            else
                panel2.BackColor = Color.Lavender;
        }

        //метод считывания Режим автомат
        private void _Auto_mode()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                Auto_mode = reader.ReadBoolean(103);
            }
            if (Auto_mode == true)
                panel10.BackColor = Color.Green;
            else
                panel10.BackColor = Color.Lavender;
        }

        //метод считывания Режим ручной
        private void _Man_mode()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                Man_mode = reader.ReadBoolean(104);
            }
            if (Man_mode == true)
                panel11.BackColor = Color.Green;
            else
                panel11.BackColor = Color.Lavender;
        }

        //метод считывания АГП вкл
        private void _RP3()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                RP3 = reader.ReadBoolean(105);
            }
            if (RP3 == true)
                panel15.BackColor = Color.Green;
            else
                panel15.BackColor = Color.Lavender;
        }

        //метод считывания АГП выкл
        private void _RP3_off()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                RP3_off = reader.ReadBoolean(106);
            }
            if (RP3_off == true)
                panel14.BackColor = Color.Green;
            else
                panel14.BackColor = Color.Lavender;
        }

        //метод считывания  В-Г: Вкл
        private void _I_VG()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                I_VG = reader.ReadBoolean(107);
            }
            if (I_VG == true)
                panel13.BackColor = Color.Green;
            else
                panel13.BackColor = Color.Lavender;
        }

        //метод считывания  В-Г: Выкл
        private void _I_VG_off()
        {
            using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(size_header, size_memory, MemoryMappedFileAccess.Read))
            {
                I_VG_off = reader.ReadBoolean(108);
            }
            if (I_VG_off == true)
                panel12.BackColor = Color.Green;
            else
                panel12.BackColor = Color.Lavender;
        }
        #endregion
    }
}