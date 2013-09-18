using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Atalasoft.Imaging.ImageProcessing;
using Atalasoft.Imaging;
using System.IO;
using Atalasoft.Imaging.Codec;
using Atalasoft.PdfDoc;
using Atalasoft.Imaging.Codec.Pdf;
using Atalasoft.Imaging.Codec.Tiff;
using Atalasoft.Imaging.Codec.Jpeg2000;

namespace WebApplication1
{
    public class ImageProcessing
    {
        public ImageProcessing()
        {
        }

        public static void ApplyCommand(ImageCommand command, AtalaImage temp, string path, int frame)
        {
            if (command != null)
            {
                if (command.InPlaceProcessing)
                    command.Apply(temp);
                else
                    temp = command.Apply(temp).Image;
                
                SaveChanges(temp, path, frame);
            }
        }

        public static void SaveChanges(AtalaImage image, string path, int frame)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                /* We're assuming only two file types are being used in 
                 * this application, TIF and PDF. */

                ImageInfo info = RegisteredDecoders.GetImageInfo(fs);
                fs.Position = 0;
                if (info.ImageType == ImageType.Tiff)
                    EditTiff(image, fs, frame, path);
                else
                    EditPDF(image, fs, frame, path);

            }

            File.Delete(path);
            File.Move(path + "_tmp", path);

        }
        public static void EditPDF(AtalaImage image, FileStream fs, int frame, string path)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, new PdfEncoder(), null);
            PdfDocument newDoc = new PdfDocument(ms);

            PdfDocument pDoc = new PdfDocument(fs);

            pDoc.Pages.RemoveAt(frame);
            pDoc.Pages.Insert(frame, newDoc.Pages[0]);

            pDoc.Save(path + "_tmp");

        }

        public static void EditTiff(AtalaImage image, Stream fs, int frame, string path)
        {
            TiffDocument tDoc = new TiffDocument(fs);

            TiffPage tPage = new TiffPage(image);

            tDoc.Pages.RemoveAt(frame);
            tDoc.Pages.Insert(frame, tPage);

            tDoc.Save(path + "_tmp");
        }
    }
}