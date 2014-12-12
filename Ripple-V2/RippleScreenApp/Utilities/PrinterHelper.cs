using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RippleScreenApp.Utilities
{
    public static class PrinterHelper
    {
        public static void PrintDiscountCoupon(string companyName, string discountProductName, string discountValueOnProduct)
        {
            int copiesToPrint = 1;
            string printTemplateFileName = @"\Assets\Docs\PrinterReceipt.pptx";
            string qrCodeImageName = @"\Assets\Images\QREncode.jpg";
            string printFileName = @"\Assets\Docs\printReceipt.pptx";
            string printTemplateFilePath = Helper.GetAssetURI(printTemplateFileName);
            string qrCodeImagepath = Helper.GetAssetURI(qrCodeImageName);
            string printReceiptFilePath = Helper.GetAssetURI(printFileName);
            Microsoft.Office.Interop.PowerPoint.Presentation work = null;
            Microsoft.Office.Interop.PowerPoint.Application app = new Microsoft.Office.Interop.PowerPoint.Application();

            try
            {
                if (File.Exists(printReceiptFilePath))
                {
                    File.Delete(printReceiptFilePath);
                }
                if (File.Exists(qrCodeImagepath))
                {
                    File.Delete(qrCodeImagepath);
                }

                Microsoft.Office.Interop.PowerPoint.Presentations presprint = app.Presentations;
                work = presprint.Open(printTemplateFilePath, Microsoft.Office.Core.MsoTriState.msoCTrue, Microsoft.Office.Core.MsoTriState.msoCTrue, Microsoft.Office.Core.MsoTriState.msoFalse);
                work.PrintOptions.PrintInBackground = Microsoft.Office.Core.MsoTriState.msoFalse;
                Microsoft.Office.Interop.PowerPoint.Slide slide = work.Slides[1];
                foreach (var item in slide.Shapes)
                {
                    var shape = (Microsoft.Office.Interop.PowerPoint.Shape)item;

                    if (shape.HasTextFrame == MsoTriState.msoTrue)
                    {
                        if (shape.TextFrame.HasText == MsoTriState.msoTrue)
                        {
                            var textRange = shape.TextFrame.TextRange;
                            var text = textRange.Text;
                            if (text.Contains("10%"))
                            {
                                text = text.Replace("10", discountValueOnProduct);
                                shape.TextFrame.TextRange.Text = text;
                            }
                            else if (text.Contains("Microsoft"))
                            {
                                text = text.Replace("Microsoft", companyName);
                                shape.TextFrame.TextRange.Text = text;
                            }
                            else if (text.Contains("Windows Phone 8"))
                            {
                                text = text.Replace("Windows Phone 8", discountProductName);
                                shape.TextFrame.TextRange.Text = text;
                            }

                        }
                    }
                    else
                    {
                        if (shape.Name.ToString() == "Picture 2")
                        {
                            shape.Delete();
                            //Add QRCode to print
                            RippleCommonUtilities.HelperMethods.GenerateQRCode("http://projectripple.azurewebsites.net/Ripple.aspx", qrCodeImagepath);
                            slide.Shapes.AddPicture(qrCodeImagepath, MsoTriState.msoFalse, MsoTriState.msoTrue, 560, 90, 80, 80);
                        }
                    }
                }

                work.SaveAs(printReceiptFilePath);
                work.PrintOut();
                work.Close();
                app.Quit();

                //delete the PrintReceipt File
                File.Delete(printReceiptFilePath);
                //Delete the QRCOde image
                File.Delete(qrCodeImagepath);
            }
            catch (System.Exception ex)
            {
                work.Close();
                app.Quit();
                //delete the PrintReceipt File
                File.Delete(printReceiptFilePath);
                //Delete the QRCOde image
                File.Delete(qrCodeImagepath);
                RippleCommonUtilities.LoggingHelper.LogTrace(1, "Went wrong in  Print Discount Coupon at Screen side: {0}", ex.Message);
            }
        }
    }
}
