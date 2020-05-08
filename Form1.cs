using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace lab9
{
    public partial class Form1 : Form
    {
        int Radius;
        int QuantThreads;
        int Speed;
        int X;//левый верхний угол квадрата
        int Y;
        double res = 0;//используя метод Монте-Карло
        double StdRes = 0;//используя стандартную формулу
        int PointRect = 0;//количество точек в квадрате
        int PointCrc = 0;//количество точек в круге
        List<Color> Colors = new List<Color>();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Check())
            {
                StdRes = Math.Round((Math.PI * Radius * Radius), 0);
                res = 0;
                textBox_ResultSrandart.Text = StdRes.ToString();
                Colors.Clear();
                Painting();
                Async_tasks();
            }
        }
        public async Task Async_tasks()
        {
            Task[] tasks = new Task[QuantThreads];

            Random rand = new Random();
            for (int i = 0; i < tasks.Length; i++)
            {
                Colors.Add(Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256)));//задание цвета потокам
                tasks[i] = await Task.Run(async () => Work(i));
            }
        }

        public async Task Work(int i)
        {
            Random rnd = new Random();

            while (Math.Abs(res - StdRes) >= 20)
            {
                int x = rnd.Next(X, X + 2 * Radius);//координаты точки в квадрате
                int y = rnd.Next(Y, Y + 2 * Radius);

                //проверка попадания точки в круг
                if (((X + Radius) - x) * ((X + Radius) - x) + ((Y + Radius) - y) * ((Y + Radius) - y) <= Radius * Radius)
                {
                    Interlocked.Increment(ref PointCrc);
                }
                Interlocked.Increment(ref PointRect);

                //отображение точки
                Brush b = new SolidBrush(Colors[i]);
                Point(x, y, b);

                //вычисление по методу МК
                int localres = (4 * Radius * Radius * PointCrc/ PointRect);
                Interlocked.Exchange(ref res, localres);

                textBox_ResultMK.Invoke(new Action(() => textBox_ResultMK.Text = res.ToString()));
                await Task.Delay(Speed);
            }
        }

        /// <summary>
        /// Отрисовка квадрата и круга
        /// </summary>
        private void Painting()
        {
            Graphics g = pictureBox1.CreateGraphics();
            g.Clear(pictureBox1.BackColor);

            X = (pictureBox1.Right - pictureBox1.Left) / 2 - Radius;
            Y = (pictureBox1.Bottom - pictureBox1.Top) / 2 - Radius;

            Rectangle r = new Rectangle(X, Y, 2 * Radius, 2 * Radius);
            Brush b = new SolidBrush(Color.Violet);
            Pen p = new Pen(b, 3);
            g.DrawRectangle(p, r);

            b = new SolidBrush(Color.Yellow);
            p = new Pen(b, 3);
            g.DrawEllipse(p, r);
        }

        /// <summary>
        /// Отрисовка точки
        /// </summary>
        /// <param name="x">координата</param>
        /// <param name="y">координата</param>
        /// <param name="b">цвет</param>
        private void Point(int x, int y, Brush b)
        {
            Graphics g = pictureBox1.CreateGraphics();

            Pen p = new Pen(b);
            Rectangle point = new Rectangle(x, y, 3, 3);
            g.DrawRectangle(p, point);
            g.FillRectangle(b, point);
        }

        /// <summary>
        /// Проверка заполненных полей
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            bool res = true;
            if (CheckInt(textBox6.Text))
            {
                Int32.TryParse(textBox6.Text, out Radius);
                if (Radius * 2 > pictureBox1.Width || Radius * 2 > pictureBox1.Height)
                {
                    MessageBox.Show("Введите, пожалуйста, радиус меньше!");
                    return false;
                }
                if (Radius<40)
                {
                    MessageBox.Show("Введите, пожалуйста, радиус больше!");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Некорректный радиус!");
                return false;
            }
            if (CheckInt(textBox7.Text))
            {
                Int32.TryParse(textBox7.Text, out QuantThreads);
            }
            else
            {
                MessageBox.Show("Некорректное число потоков!");
                return false;
            }
            if (CheckInt(textBox8.Text))
            {
                Int32.TryParse(textBox8.Text, out Speed);
            }
            else
            {
                MessageBox.Show("Некорректная скорость!");
                return false;
            }
            return res;
        }

        //Число больше 0
        public bool CheckInt(string s)
        {
            int n;
            if (Int32.TryParse(textBox6.Text, out n))
            {
                if (n >= 0)
                {
                    return true;
                }
            }
            return false;
        }

    }
}

