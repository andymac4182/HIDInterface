using System;
using System.Runtime.InteropServices;

namespace USBInterface
{
    // Used from https://stackoverflow.com/questions/29298336/using-a-c-struct-in-c-sharp
    [StructLayout(LayoutKind.Sequential)]
    public struct HidDeviceInfo
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public String path;
        public ushort vendor_id;
        public ushort product_id;
        [MarshalAs(UnmanagedType.LPWStr)]
        public String serial_number;
        public ushort release_number;
        [MarshalAs(UnmanagedType.LPWStr)]
        public String manufacturer_string;
        [MarshalAs(UnmanagedType.LPWStr)]
        public String product_string;
        public ushort usage_page;
        public ushort usage;
        public int interface_number;
        public IntPtr next;
    };
}