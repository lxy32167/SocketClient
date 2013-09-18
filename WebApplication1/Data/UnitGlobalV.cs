using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public class UnitGlobalV
    {
        static public Boolean TestFlag = false;
        static public int TimeInverval = 5000;
        static public int Beacontime = 20 * 1000;
        static public Boolean IsCover = false;
        static public Boolean IsShowScP = false;
        static public int LockTime = 10 * 60;
        static public double sysTimeDis;
        static public DateTime javaTime = new DateTime(1970, 1, 1);
        static public DateTime delphiTime = new DateTime(1899, 12, 30);
        static public TimeSpan ts = javaTime - delphiTime;

    }
}