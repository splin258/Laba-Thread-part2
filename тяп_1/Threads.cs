using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;


namespace laba_1
{
    public partial class Threads : Form
    {
        public int countThreads;
        public Threads()
        {
            InitializeComponent();
            textBox2.Text = "20"; //количество строк 
            textBox1.Text = "100"; // количество потоков
        }
        int ser = 1; // номер графика

        static List<List<int>> matrix_1 = new List<List<int>>();
        static List<List<int>> matrix_2 = new List<List<int>>();
        static List<string> timers = new List<string>();

        static ManualResetEvent startEvent = new ManualResetEvent(false);

        private void button1_1_Click(object sender, EventArgs e)
        {
            //start
            button1_1.Enabled = false;
            listBox1.Items.Clear();
            chart1.Series[0].Points.Clear();

            int counter = 2;
            ManualResetEvent signal = new ManualResetEvent(false);

            Thread[] create_matrix = new Thread[]
            {
                new Thread(() => {
                    startEvent.WaitOne();
                    init_first_matrix();
                    counter--;

                    if(counter == 0)
                    {
                        signal.Set();
                    }
                }),
                new Thread(() => {
                   startEvent.WaitOne();
                   init_second_matrix();
                    counter--;

                    if(counter == 0)
                    {
                        signal.Set();
                    }
                })
            };

            foreach (Thread thread in create_matrix)
            {
                thread.Start();
            }

            startEvent.Set();
            signal.WaitOne();
            startEvent.Reset();

            //show result creates
            foreach (string timer in timers)
            {
                listBox1.Items.Add(timer);
            }
            timers.Clear();

            int count_threads = Int32.Parse(textBox1.Text);
            int size_matrix = Int32.Parse(textBox2.Text);

            for (int i = 1; i <= count_threads; i++)
            {
                ManualResetEvent signal_counting = new ManualResetEvent(false);

                int range = size_matrix / i;
                Thread[] threads = new Thread[i];

                int j = 0;

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                for (int k = 0; k < i; k++)
                {
                    threads[k] = new Thread(() =>
                    {
                        startEvent.WaitOne();
                        summ_line(j++, range);

                        if (j == i)
                        {
                            signal_counting.Set();
                        }
                    });
                    threads[k].Start();
                }

                startEvent.Set();
                signal_counting.WaitOne();

                stopWatch.Stop();
                listBox1.Items.Add("Counts Threads(" + i + "): " + stopWatch.ElapsedMilliseconds);
                chart1.Series[0].Points.AddXY(i, stopWatch.ElapsedMilliseconds);
            }

            button1_1.Enabled = true;
        }

        private void summ_line(int index, int range)
        {
            int startRange = index * range;
            int size_matrix = Int32.Parse(textBox2.Text);

            for (; startRange < (index + 1) * range; startRange++)
            {
                for (int i = 0; i < size_matrix; i++)
                {
                    matrix_1[startRange][i] = matrix_1[startRange][i] + matrix_2[startRange][i];
                }
            }
        }

        private void init_first_matrix()
        {
            matrix_1.Clear();
            int size_matrix = Int32.Parse(textBox2.Text);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            for (int i = 0; i < size_matrix; i++)
            {
                List<int> row = new List<int>();

                for (int j = 0; j < size_matrix; j++)
                {
                    Random rnd = new Random(new System.DateTime().Millisecond + (j + i + 1));
                    row.Add(rnd.Next(-1000000, 1000000));
                }
                matrix_1.Add(row);
            }

            stopWatch.Stop();

            timers.Add("Create first matrix: " + stopWatch.ElapsedMilliseconds);
        }

        private void init_second_matrix()
        {
            matrix_2.Clear();
            int size_matrix = Int32.Parse(textBox2.Text);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            for (int i = 0; i < size_matrix; i++)
            {
                List<int> row = new List<int>();

                for (int j = 0; j < size_matrix; j++)
                {
                    Random rnd = new Random(new System.DateTime().Millisecond + (j + i + 2));
                    row.Add(rnd.Next(-1000000, 1000000));
                }
                matrix_2.Add(row);
            }

            stopWatch.Stop();

            timers.Add("Create second matrix: " + stopWatch.ElapsedMilliseconds);
        }
    }
}
