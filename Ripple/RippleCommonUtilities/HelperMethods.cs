using System.Drawing;
using com.google.zxing.common;
using com.google.zxing;
using com.google.zxing.qrcode;
using System.Collections;
using System;
using System.Windows;

namespace RippleCommonUtilities
{
    public static class HelperMethods
    {
        public static Bitmap GenerateQRCode(string URL)
        {
            QRCodeWriter writer = new QRCodeWriter();
            Hashtable hints = new Hashtable();

            hints.Add(EncodeHintType.ERROR_CORRECTION, com.google.zxing.qrcode.decoder.ErrorCorrectionLevel.M);
            hints.Add("Version", "7");
            ByteMatrix byteIMGNew = writer.encode(URL, BarcodeFormat.QR_CODE, 350, 350, hints);
            sbyte[][] imgNew = byteIMGNew.Array;
            Bitmap bmp1 = new Bitmap(byteIMGNew.Width, byteIMGNew.Height);
            Graphics g1 = Graphics.FromImage(bmp1);
            g1.Clear(System.Drawing.Color.White);
            for (int i = 0; i <= imgNew.Length - 1; i++)
            {
                for (int j = 0; j <= imgNew[i].Length - 1; j++)
                {
                    if (imgNew[j][i] == 0)
                    {
                        g1.FillRectangle(System.Drawing.Brushes.Black, i, j, 1, 1);
                    }
                    else
                    {
                        g1.FillRectangle(System.Drawing.Brushes.White, i, j, 1, 1);
                    }
                }
            }
            return bmp1;
        }

        public static void GenerateQRCode(string URL, String TargetPath)
        {
            QRCodeWriter writer = new QRCodeWriter();
            Hashtable hints = new Hashtable();

            hints.Add(EncodeHintType.ERROR_CORRECTION, com.google.zxing.qrcode.decoder.ErrorCorrectionLevel.M);
            hints.Add("Version", "7");
            ByteMatrix byteIMGNew = writer.encode(URL, BarcodeFormat.QR_CODE, 350, 350, hints);
            sbyte[][] imgNew = byteIMGNew.Array;
            Bitmap bmp1 = new Bitmap(byteIMGNew.Width, byteIMGNew.Height);
            Graphics g1 = Graphics.FromImage(bmp1);
            g1.Clear(System.Drawing.Color.White);
            for (int i = 0; i <= imgNew.Length - 1; i++)
            {
                for (int j = 0; j <= imgNew[i].Length - 1; j++)
                {
                    if (imgNew[j][i] == 0)
                    {
                        g1.FillRectangle(System.Drawing.Brushes.Black, i, j, 1, 1);
                    }
                    else
                    {
                        g1.FillRectangle(System.Drawing.Brushes.White, i, j, 1, 1);
                    }
                }
            }
            bmp1.Save(TargetPath, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        public static void ClickOnFloorToGetFocus()
        {
            int middleWidth = Convert.ToInt32(Math.Floor((double)((int)SystemParameters.PrimaryScreenWidth / 2)));
            int middleHeight = Convert.ToInt32(Math.Floor((double)((int)SystemParameters.PrimaryScreenHeight / 2)));
            RippleCommonUtilities.OSNativeMethods.SendMouseInput(middleWidth, middleHeight, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, true);
            RippleCommonUtilities.OSNativeMethods.SendMouseInput(middleWidth, middleHeight, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, false);
        }
    }
}
