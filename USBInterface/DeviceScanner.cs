using System;
using System.Globalization;
using System.Threading;

namespace USBInterface
{
    public class DeviceScanner
    { 
        public event EventHandler DeviceArrived;
        public event EventHandler DeviceRemoved;

        public bool IsDeviceConnected => _deviceConnected;

        // for async reading
        private readonly object _syncLock = new object();
        private Thread _scannerThread;
        private volatile bool _asyncScanOn;

        private volatile bool _deviceConnected;

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
        public static bool ScanOnce(ushort vid, ushort pid)
        {
            return HidApi.hid_enumerate(vid, pid) != IntPtr.Zero;
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
                    var deviceInfo = HidApi.hid_enumerate(_vendorId, _productId);
                    var deviceOnBus = deviceInfo != IntPtr.Zero;
                    // freeing the enumeration releases the device, 
                    // do it as soon as you can, so we dont block device from others
                    HidApi.hid_free_enumeration(deviceInfo);
                    if (deviceOnBus && ! _deviceConnected)
                    {
                        // just found new device
                        _deviceConnected = true;
                        DeviceArrived?.Invoke(this, EventArgs.Empty);
                    }
                    if (! deviceOnBus && _deviceConnected)
                    {
                        // just lost device connection
                        _deviceConnected = false;
                        DeviceRemoved?.Invoke(this, EventArgs.Empty);
                    }
                }
                catch (Exception)
                {
                    // stop scan, user can manually restart again with StartAsyncScan()
                    _asyncScanOn = false;
                }
                // when read 0 bytes, sleep and read again
                Thread.Sleep(ScanIntervalInMillisecs);
            }
        }
    }
}
