﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace USBInterface
{
    internal class HidApi
    {
        #region Native Methods
#if WIN64
        public const string DLL_FILE_NAME = "hidapi64.dll";
#else
        public const string DLL_FILE_NAME = "hidapi.dll";
#endif

        /// Return Type: int
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hid_init();


        /// Return Type: int
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hid_exit();

        /// Return Type: hid_device_info*
        ///vendor_id: unsigned short
        ///product_id: unsigned short
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr hid_enumerate(ushort vendor_id, ushort product_id);

        /// Return Type: hid_device*
        ///vendor_id: unsigned short
        ///product_id: unsigned short
        ///serial_number: wchar_t*
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr hid_open(ushort vendor_id, ushort product_id, [In] string serial_number);


        /// Return Type: hid_device*
        ///path: char*
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr hid_open_path([In] string path);


        /// Return Type: int
        ///device: hid_device*
        ///data: unsigned char*
        ///length: size_t->unsigned int
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hid_write(IntPtr device, [In] byte[] data, uint length);


        /// Return Type: int
        ///dev: hid_device*
        ///data: unsigned char*
        ///length: size_t->unsigned int
        ///milliseconds: int
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hid_read_timeout(IntPtr device, [Out] byte[] buf_data, uint length, int milliseconds);


        /// Return Type: int
        ///device: hid_device*
        ///data: unsigned char*
        ///length: size_t->unsigned int
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hid_read(IntPtr device, [Out] byte[] buf_data, uint length);


        /// Return Type: int
        ///device: hid_device*
        ///nonblock: int
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl)]
        public extern static int hid_set_nonblocking(IntPtr device, int nonblock);


        /// Return Type: int
        ///device: hid_device*
        ///data: char*
        ///length: size_t->unsigned int
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hid_send_feature_report(IntPtr device, [In] byte[] data, uint length);


        /// Return Type: int
        ///device: hid_device*
        ///data: unsigned char*
        ///length: size_t->unsigned int
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int hid_get_feature_report(IntPtr device, [Out] byte[] buf_data, uint length);


        /// Return Type: void
        ///device: hid_device*
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl)]
        public extern static void hid_close(IntPtr device);


        /// Return Type: int
        ///device: hid_device*
        ///string: wchar_t*
        ///maxlen: size_t->unsigned int
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int hid_get_manufacturer_string(IntPtr device, StringBuilder buf_string, uint length);


        /// Return Type: int
        ///device: hid_device*
        ///string: wchar_t*
        ///maxlen: size_t->unsigned int
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int hid_get_product_string(IntPtr device, StringBuilder buf_string, uint length);


        /// Return Type: int
        ///device: hid_device*
        ///string: wchar_t*
        ///maxlen: size_t->unsigned int
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int hid_get_serial_number_string(IntPtr device, StringBuilder buf_serial, uint maxlen);


        /// Return Type: int
        ///device: hid_device*
        ///string_index: int
        ///string: wchar_t*
        ///maxlen: size_t->unsigned int
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern int hid_get_indexed_string(IntPtr device, int string_index, StringBuilder buf_string, uint maxlen);


        /// Return Type: wchar_t*
        ///device: hid_device*
        [DllImport(DLL_FILE_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public static extern IntPtr hid_error(IntPtr device);



        #endregion
    }
}
