using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace WebApplication1
{
    public static class BlkUseClass
    {
        public struct ClassArray
        {
            public int CourseID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public char[] ClassID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 68)]
            public char[] ClassName;
        }
        public struct BlkArray
        {
            public int CourseID;
            public int BlkNo;
        }

    }
}
