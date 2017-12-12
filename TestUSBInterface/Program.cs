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

        public static void Enter(object s, DeviceScanner.DeviceArrivedArgs deviceArrivedArgs)
        {
            Console.WriteLine($"Device arrived - {deviceArrivedArgs.Path}");
        }
        public static void Exit(object s, DeviceScanner.DeviceRemovedArgs deviceRemovedArgs)
        {
            Console.WriteLine($"Device removed - {deviceRemovedArgs.Path}");
        }

        static void Main(string[] args)
        {
            var scanner = new DeviceScanner(0x20A0, 0x4241);
            scanner.DeviceArrived += Enter;
            scanner.DeviceRemoved += Exit;
            scanner.StartAsyncScan();
            Console.WriteLine("Scanning");
            Console.ReadKey();
        }
    }


}
