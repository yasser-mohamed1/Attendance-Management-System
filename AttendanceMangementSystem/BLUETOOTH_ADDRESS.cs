using System;
using System.Runtime.InteropServices;

namespace AttendanceMangementSystem
{
    [StructLayout(LayoutKind.Explicit)]

    internal class BLUETOOTH_ADDRESS
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.I8)]
        public Int64 ullLong;
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.U1)]
        public Byte rgBytes_0;
        [FieldOffset(1)]
        [MarshalAs(UnmanagedType.U1)]
        public Byte rgBytes_1;
        [FieldOffset(2)]
        [MarshalAs(UnmanagedType.U1)]
        public Byte rgBytes_2;
        [FieldOffset(3)]
        [MarshalAs(UnmanagedType.U1)]
        public Byte rgBytes_3;
        [FieldOffset(4)]
        [MarshalAs(UnmanagedType.U1)]
        public Byte rgBytes_4;
        [FieldOffset(5)]
        [MarshalAs(UnmanagedType.U1)]
        public Byte rgBytes_5;
    }
}
