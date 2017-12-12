using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace USBInterface
{
    public class DeviceScanner
    {
        public class DeviceRemovedArgs
        {
            public DeviceRemovedArgs(string path)
            {
                Path = path;
            }
            public string Path { get; }
        }

        public class DeviceArrivedArgs
        {
            public DeviceArrivedArgs(string path)
            {
                Path = path;
            }

            public string Path { get; }
        }

        public event EventHandler<DeviceArrivedArgs> DeviceArrived;
        public event EventHandler<DeviceRemovedArgs> DeviceRemoved;

        private readonly List<string> _connectedDevices = new List<string>();

        // for async reading    
        private readonly object _syncLock = new object();
        private Thread _scannerThread;
        private volatile bool _asyncScanOn;

        private int _scanIntervalMillisecs = 10;
        public int ScanIntervalInMillisecs
        {
            get { lock (_syncLock) { return _scanIntervalMillisecs; } }
            set { lock (_syncLock) { _scanIntervalMillisecs = value; } }
        }

        public bool IsScanning => _asyncScanOn;

        private readonly ushort _vendorId;
        private readonly ushort _productId;

        // Use this class to monitor when your devices connects.
        // Note that scanning for device when it is open by another process will return FALSE
        // even though the device is connected (because the device is unavailiable)
        public DeviceScanner(ushort vendorId, ushort productId, int scanIntervalMillisecs = 100)
        {
            _vendorId = vendorId;
            _productId = productId;
            ScanIntervalInMillisecs = scanIntervalMillisecs;
        }

        // scanning for device when it is open by another process will return false
        public static List<HidDeviceInfo> ScanOnce(ushort vid, ushort pid)
        {
            var list = new List<HidDeviceInfo>();

            var pDev = HidApi.hid_enumerate(vid, pid);
            while (pDev != IntPtr.Zero)
            {
                var dev = (HidDeviceInfo)Marshal.PtrToStructure(pDev, typeof(HidDeviceInfo));
                list.Add(dev);
                // freeing the enumeration releases the device, 
                // do it as soon as you can, so we dont block device from others
                HidApi.hid_free_enumeration(pDev);
                pDev = dev.next;
            }
            return list;
        }

        public void StartAsyncScan()
        {
            // Build the thread to listen for reads
            if (_asyncScanOn)
            {
                // dont run more than one thread
                return;
            }
            _asyncScanOn = true;
            _scannerThread = new Thread(ScanLoop) {Name = "HidApiAsyncDeviceScanThread"};
            _scannerThread.Start();
        }

        public void StopAsyncScan()
        {
            _asyncScanOn = false;
        }

        private void ScanLoop()
        {
            var culture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // The read has a timeout parameter, so every X milliseconds
            // we check if the user wants us to continue scanning.
            while (_asyncScanOn)
            {
                try
                {
                    var deviceInfo = ScanOnce(_vendorId, _productId);

                    var newlyConnectedDevices = deviceInfo.Where(di => !_connectedDevices.Contains(di.path)).ToArray();
                    var removedDevicePaths = _connectedDevices.Where(cd => deviceInfo.All(di => di.path != cd)).ToArray();

                    foreach (var newlyConnectedDevice in newlyConnectedDevices)
                    {
                        DeviceArrived?.Invoke(this, new DeviceArrivedArgs(newlyConnectedDevice.path));
                        _connectedDevices.Add(newlyConnectedDevice.path);
                    }

                    foreach (var removedDevicePath in removedDevicePaths)
                    {
                        DeviceRemoved?.Invoke(this, new DeviceRemovedArgs(removedDevicePath));
                        _connectedDevices.Remove(removedDevicePath);
                    }
                }
                catch (Exception e)
                {
                    // stop scan, user can manually restart again with StartAsyncScan()
                    Console.WriteLine(e.ToString());
                    _asyncScanOn = false;
                }
                // when read 0 bytes, sleep and read again
                Thread.Sleep(ScanIntervalInMillisecs);
            }
        }
    }
}
