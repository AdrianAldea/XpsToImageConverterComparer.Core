using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Xps.Packaging;
using System.Linq;
using System.Diagnostics;

namespace XpsToImgConverter
{
    public class XpsToImgConverter
    {
        static SemaphoreSlim _semaphore = new SemaphoreSlim(4);

        /// <summary>
        /// Convert all xps from path to images
        /// Each page will be converted to a image
        /// </summary>
        /// <param name="xpsFilesList"></param>
        public void ConvertXpsToImg(List<string> xpsFilesList)
        {
            try
            {
                foreach (var item in xpsFilesList)
                {
                    Thread thread = new Thread(LimitThreads_SaveXpsPageToBitmap);
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Name = "Thread for convert to img";
                    thread.Start(item);

                }

            }
            catch (AggregateException ex)
            {
                foreach (var exception in ex.InnerExceptions)
                {
                    Console.WriteLine("Exception: ");
                    Console.WriteLine(ex);
                    Console.WriteLine("StackTrace: ");
                    Console.WriteLine(exception.StackTrace);
                }
            }
        }

        private void LimitThreads_SaveXpsPageToBitmap(object obj)
        {
            _semaphore.Wait();
            SaveXpsPageToBitmap((string)obj);
            _semaphore.Release();
        }

        /// <summary>
        /// Convert xps file to image
        /// </summary>
        /// <param name="xpsFileName"></param>
        private void SaveXpsPageToBitmap(string xpsFileName)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            XpsDocument xpsDoc = new XpsDocument(xpsFileName, System.IO.FileAccess.Read);

            FixedDocumentSequence docSeq = xpsDoc.GetFixedDocumentSequence();

            // You can get the total page count from docSeq.PageCount
            for (int pageNum = 0; pageNum < docSeq.DocumentPaginator.PageCount; ++pageNum)
            {
                DocumentPage docPage = docSeq.DocumentPaginator.GetPage(pageNum);
                BitmapImage bitmap = new BitmapImage();
                RenderTargetBitmap renderTarget =
                    new RenderTargetBitmap((int)docPage.Size.Width,
                                            (int)docPage.Size.Height,
                                            96, // WPF (Avalon) units are 96dpi based
                                            96,
                                            System.Windows.Media.PixelFormats.Default);

                renderTarget.Render(docPage.Visual);

                BmpBitmapEncoder encoder = new BmpBitmapEncoder();  // Choose type here ie: JpegBitmapEncoder, etc
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                FileStream pageOutStream =
                    new FileStream(xpsFileName + ".Page" + pageNum + ".bmp", FileMode.Create, FileAccess.Write);
                encoder.Save(pageOutStream);
                pageOutStream.Close();
            }

            sw.Stop();

            Console.WriteLine("Xps from file: " + xpsFileName + " converted in " + (sw.ElapsedMilliseconds / 1000).ToString());
        }

        public XpsToImgConverter()
        {
            // TODO: Complete member initialization
        }
    }
}
