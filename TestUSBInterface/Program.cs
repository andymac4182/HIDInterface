using System;
using USBInterface;

namespace TestUSBInterface
{
    class Program
    {
        public static void Handle(object s, ReportEventArgs a)
        {
            Console.WriteLine(string.Join(", ", a.Data));
        }

        public static void Enter(object s, EventArgs a)
        {
            Console.WriteLine("device arrived");
        }
        public static void Exit(object s, EventArgs a)
        {
            Console.WriteLine("device removed");
        }

        static void Main(string[] args)
        {
            // setup a scanner before hand
            var scanner = new DeviceScanner(0x20A0, 0x4241);
            scanner.DeviceArrived += Enter;
            scanner.DeviceRemoved += Exit;
            scanner.StartAsyncScan();
            Console.WriteLine("asd");

            // this should probably happen in enter() function
            try
            {
                // this can all happen inside a using(...) statement
                var dev = new USBDevice(0x4d8, 0x3f, null, false, 31);

                Console.WriteLine(dev.Description());

                // add handle for data read
                dev.InputReportArrivedEvent += Handle;
                // after adding the handle start reading
                dev.StartAsyncRead();
                // can add more handles at any time
                dev.InputReportArrivedEvent += Handle;

                // write some data
                var data = new byte[32];
                data[0] = 0x00;
                data[1] = 0x23;
                dev.Write(data);

                dev.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }
    }


}
