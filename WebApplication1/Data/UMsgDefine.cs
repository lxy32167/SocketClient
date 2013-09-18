using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace WebApplication1
{
    public static class UMsgDefine
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct stMsgHead
        {
            public int MsgType;
            public int MsgLength;
        }

    /// <summary>
    /// 用户信息 登陆帧头
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct stUserInfo
    {
        public int UserID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public char[] LoginName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public char[] TrueName;
        public int Tasknum;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct FM_Auth_Req
    {
        public stMsgHead MsgHead;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public char[] UserName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] UserPwd;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct FM_Auth_Rsp
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
    public struct FM_Beacon_Client
    {
        public stMsgHead MsgHead;
        public int UserID;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct FM_Beacon_Server
    {
        public stMsgHead MsgHead;
        public uint ServerTime;
        public int Rspcode;
    }

    /// <summary>
    /// 取卷帧头
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct stGetPaperTask
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
    public struct stPaperInfo 
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
    public struct FM_GetPaper_Req
    {
        public stMsgHead MsgHead;
        public stGetPaperTask GetPaperTask;
        public int CheckFlag;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct FM_GetPaper_Rsp
    {
        public stMsgHead MsgHead;
        public int RspCode;
        public stPaperInfo PaperData;
    }
    /// <summary>
    /// 任务信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct stCourseInfo
    {
        public int CourseID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 68)]
        public char[] CName;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct stClassInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] ClassID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 68)]
        public char[] ClassName;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct tag_ExamTask
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
    public struct stBlockInfo
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
    public struct FM_GetBlkInfo_Req
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
    public struct FM_GetBlkInfo_Rsp
    {
        public stMsgHead MsgHead;
        public int RspCode;
        public stBlockInfo BlockInfo;
    }

    
    /// <summary>
    /// 注销帧
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    [StructLayout(LayoutKind.Sequential)]
    public struct FM_UserLogout
    {
        public stMsgHead MsgHead;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public char[] UserName;
    }

    /// <summary>
    /// 存分帧
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    [StructLayout(LayoutKind.Sequential)]
    public struct stSaveScore
    {
        public int CourseID;
        public int UserID;
        public int PaperNo;
        public int BLKNO;

        public int Check;
        public int isExample;

        public float TotalScore;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public char[] DetailScore;
        public uint TimeStamp;
//        public string MarkInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct FM_SaveScore_Req
    {
        public stMsgHead MsgHead;
        public int RecCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public stSaveScore[] Score;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct FM_SaveScore_Rsp
    {
        public stMsgHead MsgHead;
        public int RspCode;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct stSaveMarkInfo
    {
        public int CourseID;
        public int UserID;
        public int PaperNo;
        public int BLKNO;

        public int Check;

        public int MarkInfoLen;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8159)]
        public char[] MarkInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FM_SaveMarkInfo_Req
    {
        public stMsgHead MsgHead;
        public stSaveMarkInfo MarkInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct FM_SaveMarkInfo_Rsp
    {
        public stMsgHead MsgHead;
        public int RspCode;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FM_GETHISTORYPAPER_REQ
    {
        public stMsgHead MsgHead;
        public int UserID;
        public int CourseID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public char[] ClassID;
        public int BlkNo;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct FM_GETHISTORYPAPER_RSP
    {
        public stMsgHead MsgHead;
        public int RspCode;
        public int Count;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct FM_ChgPwd_Req
    {
        public stMsgHead MsgHead;
        public int UserID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] OldPwd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public char[] NewPwd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public char[] TrueName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public char[] ServeFor;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct FM_ChgPwd_Rsp
    {
        public stMsgHead MsgHead;
        public int RspCode;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct stHistoryPaper
    {
        public int PaperNo;
        public float Score;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public char[] StudentID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public char[] DetailScore;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct CheckData
    {
        public int PaperNo;
        public float Score;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public char[] StudentID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public char[] StudentName;
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
    }
}
