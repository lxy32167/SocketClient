using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public static class DataModule
    {
        public struct stVars
        {
            int ImgNum; //题块涉及图像数目
            List<UMyRecords.stImgInfo> ImgInfo; //题块涉及图像位置信息列表
            int ImgNo; //当前题块图像编号，从0开始，0表示本题块的第一张图片，依此类推
            List<int> ImgID2ImgNo;
        }
    }
}