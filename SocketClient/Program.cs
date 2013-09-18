using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
namespace SocketClient
{
    [StructLayout(LayoutKind.Sequential)]
    struct stMsgHead
    {
        public int MsgType;
        public int MsgLength;
    }

    /// <summary>
    /// 用户信息 登陆帧头
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct stUserInfo
    {
        public int UserID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public char[] LoginName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public char[] TrueName;
        public int Tasknum;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct FM_Auth_Req
    {
        public stMsgHead MsgHead;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public char[] UserName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] UserPwd;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct FM_Auth_Rsp
    {
        public stMsgHead MsgHead;
        public int Rspcode;
        public uint ServerTime;
        public stUserInfo UserInfo;
    }
    
    /// <summary>
    /// 信标帧头
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct FM_Beacon_Client
    {
        public stMsgHead MsgHead;
        public int UserID;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct FM_Beacon_Server
    {
        public stMsgHead MsgHead;
        public uint ServerTime;
        public int Rspcode;
    }

    /// <summary>
    /// 取卷帧头
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct stGetPaperTask
    {
        public int UserID;
        public int CourseID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] ClassID;
        public int BlkNo;
        public int PaperNo;
        public int PicNo;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct stPaperInfo 
    {
        public int PaperNo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public char[] StudentID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public char[] StudentName;
        public int ImageFormat;
        public int ImageLen;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct FM_GetPaper_Req
    {
        public stMsgHead MsgHead;
        public stGetPaperTask GetPaperTask;
        public int CheckFlag;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct FM_GetPaper_Rsp
    {
        public stMsgHead MsgHead;
        public int RspCode;
        public stPaperInfo PaperData;
    }
    /// <summary>
    /// 任务信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct stCourseInfo
    {
        public int CourseID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 68)]
        public char[] CName;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct stClassInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] ClassID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 68)]
        public char[] ClassName;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct tag_ExamTask
    {
        public stCourseInfo CourseInfo;
        public stClassInfo ClassInfo;
        public int BlkNo;
        public int nPart;
        public int nTotal;
        public int pPart;
        public int MaxTask;
    }
    /// <summary>
    /// 题块信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct stBlockInfo
    {
        public int CourseID;
        public int BlkNo;
        public int CanShowFull;
        public int ScoreFlag;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 254)]
        public char[] CheckRule;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 254)]
        public char[] Position;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 254)]
        public char[] HsPosition;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 254)]
        public char[] CrPosition;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct FM_GetBlkInfo_Req
    {
        public stMsgHead MsgHead;
        public int CourseID;
        public int BlkID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] ClassID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public char[] UserName;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct FM_GetBlkInfo_Rsp
    {
        public stMsgHead MsgHead;
        public int RspCode;
        public stBlockInfo BlockInfo;
    }
    class Program
    {
        private static byte[] result = new byte[1024];
        static double v,v1,sysTimeDis;
        static void Main(string[] args)
        {
            FM_Auth_Req fmLogReq;
            FM_Auth_Rsp fmLogRsp;
            FM_GetPaper_Req fmGetPaperReq;
            FM_GetPaper_Rsp fmGetPaperRsp;
            tag_ExamTask ExamTask;
            FM_GetBlkInfo_Req fmGetBlkReq;
            FM_GetBlkInfo_Rsp fmGetBlkRsp;
            //设定服务器IP地址  
            IPAddress ip = IPAddress.Parse("192.168.244.128");
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 27520)); //配置服务器IP与端口  
                Console.WriteLine("连接服务器成功");
            }
            catch
            {
                Console.WriteLine("连接服务器失败，请按回车键退出！");
                return;
            }

 
            fmLogReq.UserName = "".ToCharArray();
            fmLogReq.UserPwd = "".ToCharArray();
            fmLogReq.MsgHead.MsgType = 16842753;
            fmLogReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(FM_Auth_Req));
            fmLogReq.UserName = "52300100".PadRight(24, '\0').ToCharArray();
            fmLogReq.UserPwd = "0100".PadRight(20, '\0').ToCharArray();
            byte[] Message = StructToBytes(fmLogReq);
            clientSocket.Send(Message, fmLogReq.MsgHead.MsgLength, SocketFlags.None);
            byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(FM_Auth_Rsp))];
            clientSocket.Receive(RcvMessage,Marshal.SizeOf(typeof(FM_Auth_Rsp)),SocketFlags.None);
            Console.WriteLine("Receive OK!");     
            fmLogRsp = (FM_Auth_Rsp)BytesToStruct(RcvMessage,typeof(FM_Auth_Rsp));
            //RcvTask


 //           Thread myThread = new Thread(TBeaconThread);
  //          myThread.Start();

              byte[] RcvTaskMessage = new byte[Marshal.SizeOf(typeof(tag_ExamTask))];
              clientSocket.Receive(RcvTaskMessage, Marshal.SizeOf(typeof(tag_ExamTask)), SocketFlags.None);
   //           Console.WriteLine("Receive task ok!");
              ExamTask = (tag_ExamTask)BytesToStruct(RcvTaskMessage, typeof(tag_ExamTask));
              Console.WriteLine("{0}", new String(ExamTask.ClassInfo.ClassID));
             Console.WriteLine("{0}", new String(ExamTask.ClassInfo.ClassName));
             Console.WriteLine("{0}", ExamTask.CourseInfo.CourseID);
              Console.WriteLine("{0}", ExamTask.MaxTask);

              clientSocket.Receive(RcvTaskMessage, Marshal.SizeOf(typeof(tag_ExamTask)), SocketFlags.None);
    //          Console.WriteLine("Receive task ok!");
              ExamTask = (tag_ExamTask)BytesToStruct(RcvTaskMessage, typeof(tag_ExamTask));
              Console.WriteLine("{0}", new String(ExamTask.ClassInfo.ClassID));
              Console.WriteLine("{0}", ExamTask.CourseInfo.CourseID);
              Console.WriteLine("{0}", ExamTask.MaxTask);

              Console.ReadLine();
            //RcvBlk
              fmGetBlkReq.MsgHead.MsgType = 16842765;
              fmGetBlkReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(FM_GetBlkInfo_Req));
              fmGetBlkReq.CourseID = 1;
              fmGetBlkReq.BlkID = 2;
              fmGetBlkReq.ClassID = "20081301".PadRight(16,'\0').ToCharArray();
              fmGetBlkReq.UserName = "52300100".PadRight(24, '\0').ToCharArray();

              byte[] BlkMessage = StructToBytes(fmGetBlkReq);
              clientSocket.Send(BlkMessage, fmGetBlkReq.MsgHead.MsgLength, SocketFlags.None);
             
              byte[] RcvBlkMessage = new byte[Marshal.SizeOf(typeof(FM_GetBlkInfo_Rsp))];
              clientSocket.Receive(RcvBlkMessage, Marshal.SizeOf(typeof(FM_GetBlkInfo_Rsp)), SocketFlags.None);
    //          Console.WriteLine("Receive OK!");
              fmGetBlkRsp = (FM_GetBlkInfo_Rsp)BytesToStruct(RcvBlkMessage, typeof(FM_GetBlkInfo_Rsp));
    //          if (fmGetBlkRsp.MsgHead.MsgType == 16842766)
   //               Console.WriteLine("Blk Msg Type OK!");
   //           Console.WriteLine("{0}", new String(fmGetBlkRsp.BlockInfo.Position));

   //           Console.WriteLine("{0}", new String(fmGetBlkRsp.BlockInfo.CrPosition));
            //RcvPaper
           ///
            fmGetPaperReq.MsgHead.MsgType = 16842771;
            fmGetPaperReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(FM_GetPaper_Req));
            fmGetPaperReq.GetPaperTask.UserID = 1000;
            fmGetPaperReq.GetPaperTask.CourseID = 1;
            fmGetPaperReq.GetPaperTask.ClassID = "20081301".PadRight(16, '\0').ToCharArray();
            fmGetPaperReq.GetPaperTask.BlkNo = 2;
            fmGetPaperReq.GetPaperTask.PaperNo = 0;
            fmGetPaperReq.GetPaperTask.PicNo = 2;
            fmGetPaperReq.CheckFlag = 0;

            byte[] PaperMessage = StructToBytes(fmGetPaperReq);
            clientSocket.Send(PaperMessage, fmGetPaperReq.MsgHead.MsgLength, SocketFlags.None);

            byte[] RcvPaperMessage = new byte[Marshal.SizeOf(typeof(FM_GetPaper_Rsp))];
            clientSocket.Receive(RcvPaperMessage, Marshal.SizeOf(typeof(FM_GetPaper_Rsp)), SocketFlags.None);
   //         Console.WriteLine("Receive OK!");
            fmGetPaperRsp = (FM_GetPaper_Rsp)BytesToStruct(RcvPaperMessage, typeof(FM_GetPaper_Rsp));
  //          if (fmGetPaperRsp.MsgHead.MsgType == 16842772)
   //             Console.WriteLine("Paper Msg Type OK!");
   //         Console.WriteLine("{0}", fmGetPaperRsp.PaperData.PaperNo);
   
  

 //SaveToFile 
            byte[] RcvMS = new byte[4096];
            int len = 0;

            FileStream fs = new FileStream(@"D:\\test.jpg", FileMode.OpenOrCreate);
            BufferedStream bs = new BufferedStream(fs);
            using (fs)
            {
                using (bs)
                {
                    while (len < fmGetPaperRsp.PaperData.ImageLen)
                    {
                        int count = clientSocket.Receive(RcvMS, RcvMS.Length, SocketFlags.None);
                        bs.Write(RcvMS, 0, count);
                        len += count;
                    }
                    bs.Flush();
                    fs.Flush();
                    bs.Close();
                    fs.Close();
                }
            }


          
        }  
        public static byte[] StructToBytes(object obj)
        {
           //得到结构体的大小
            int size = Marshal.SizeOf(obj);
           //创建byte数组
            byte[] bytes = new byte[size];
           //分配结构体大小的内存空间
           IntPtr structPtr = Marshal.AllocHGlobal(size);
           Marshal.StructureToPtr(obj, structPtr, false);
           //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
           //释放内存空间
          Marshal.FreeHGlobal(structPtr);
           //返回byte数组
           return bytes;

      } 
      public static object BytesToStruct(byte[] bytes, Type type)
         {
             //得到结构的大小
           int size = Marshal.SizeOf(type);
       //    Log(size.ToString(), 1);
           //byte数组长度小于结构的大小
            if (size > bytes.Length)
           {
              //返回空
               return null;
         }
           //分配结构大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
          //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
           //将内存空间转换为目标结构
          object obj = Marshal.PtrToStructure(structPtr, type);
           //释放内存空间
           Marshal.FreeHGlobal(structPtr);
            //返回结构
          return obj;
       }
      public static void TBeaconThread()
      {
          FM_Beacon_Client BeaconReq;
          FM_Beacon_Server BeaconRsp;
          IPAddress ip = IPAddress.Parse("192.168.244.128");
          Socket BeaconclientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                BeaconclientSocket.Connect(new IPEndPoint(ip, 27510)); //配置服务器IP与端口  
                Console.WriteLine("UDP连接服务器成功");
            }
            catch
            {
                Console.WriteLine("UDP连接服务器失败，请按回车键退出！");
                return;
            }
          // Beacon
          BeaconReq.MsgHead.MsgType = 16842755;
          BeaconReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(FM_Beacon_Client));
          BeaconReq.UserID = 0100;
          BeaconRsp.MsgHead.MsgType = 0;
          while(true)
          {
              byte[] BeaMessage = StructToBytes(BeaconReq);

              BeaconclientSocket.Send(BeaMessage, BeaconReq.MsgHead.MsgLength, SocketFlags.None);
              byte[] BeaRcvMessage = new byte[Marshal.SizeOf(typeof(FM_Beacon_Server))];
              BeaconclientSocket.Receive(BeaRcvMessage, Marshal.SizeOf(typeof(FM_Beacon_Server)), SocketFlags.None);

              BeaconRsp = (FM_Beacon_Server)BytesToStruct(BeaRcvMessage, typeof(FM_Beacon_Server));
              Console.WriteLine("Beacon Receive OK!");
              if (BeaconRsp.MsgHead.MsgType == 16842756)
                  Console.WriteLine("Msg Type OK!");
              DateTime javaTime = new DateTime(1970, 1, 1);
              DateTime delphiTime = new DateTime(1899, 12, 30);
              DateTime Now = new DateTime(2013, 4, 19, 16, 31, 22);
              TimeSpan ts2 = Now - javaTime;
              TimeSpan ts = javaTime - delphiTime;
              v = ts.TotalDays + (double)(BeaconRsp.ServerTime+8*60*60)/86400;  //服务器时间（东八区，ServerTime是标准UTC）
              TimeSpan ts1 = DateTime.Now - delphiTime;
              v1 = ts1.TotalDays;                                               //本地时间（东八区）
              sysTimeDis = v- v1;
              Console.WriteLine("{0:F6}",v-v1);
              DateTime sysDisTimeTrue = DateTime.Now.AddDays(sysTimeDis);
              Console.WriteLine("{0}", ts2.TotalSeconds);
              Thread.Sleep(20*1000);
          }
 
      }

    }
}
