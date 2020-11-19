using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace T4E5
{
    class Program
    {
        static int counter = 0;

        static void Main(string[] args)
        {
            MyTimer t = new MyTimer(incrementer);
            string op = "";
            do
            {
                Console.WriteLine("Press any key to start.");
                Console.ReadKey();

                Console.WriteLine("Press any key to pause.");
                t.Run();
                Console.ReadKey();

                t.Pause();
                Console.WriteLine("Paused. Press 1 to restart or RETURN to quit.");
                op = Console.ReadLine();
            } while (op == "1");
        }

        static void incrementer()
        {
            counter++;
            Console.WriteLine(counter);
        }
    }

    class MyTimer
    {
        private PeriodicFunction fn;
        private Thread fnThread;
        private bool running;
        private object l;

        public int Delay;

        public MyTimer(PeriodicFunction fn)
        {
            this.fn = fn;
            this.running = false;
            this.fnThread = new Thread(Launch);
            this.fnThread.IsBackground = true;
            this.l = new object();

            this.Delay = 1000;

            this.fnThread.Start();
        }

        public delegate void PeriodicFunction();

        public void Run()
        {
            lock(l)
            {
                this.running = true;
                Monitor.Pulse(l);
            }
        }

        public void Pause()
        {
            lock(l)
            {
                this.running = false;
            }
        }

        private void Launch()
        {
            while (true)
            {
                lock (l)
                {
                    while (!running)
                    {
                        Monitor.Wait(l);
                    }
                }

                fn();
                Thread.Sleep(Delay);
            }
        }
    }
}
