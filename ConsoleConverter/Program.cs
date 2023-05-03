using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleConverter
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Console.ReadKey();

            if (args != null)
            {
                //Prepare input arguments for type of application
                List<string> listOfArgs = new List<string>();
                string actionOfApplication = args[0];
                List<string> xpsFilesList = new List<string>();
                listOfArgs.Add(actionOfApplication);

                string typeOfFiles = string.Empty;

                if (actionOfApplication == "convert")
                {
                    typeOfFiles = "*.xps";
                }
                else if (actionOfApplication == "compare")
                {
                    typeOfFiles = "*.bmp";
                }

                string[] xpsFiles = Directory.GetFiles(args[1].Replace('*', ' '), typeOfFiles, SearchOption.AllDirectories);
                string[] xpsOriginalFiles = Directory.GetFiles(args[2].Replace('*', ' '), typeOfFiles, SearchOption.AllDirectories);
                xpsFilesList.AddRange(xpsFiles);
                xpsFilesList.AddRange(xpsOriginalFiles);

                if (actionOfApplication == "convert")
                {
                    XpsToImgConverter.XpsToImgConverter converter = new XpsToImgConverter.XpsToImgConverter();

                    Console.WriteLine("Conversion started. . . ");
                    converter.ConvertXpsToImg(xpsFilesList);
                }
                else
                    if (actionOfApplication == "compare")
                    {
                        ImageComparer.ImageComparer comparer = new ImageComparer.ImageComparer();

                        List<string> imagesPathToCompare = new List<string>();
                        List<string> originalImagesPath = new List<string>();

                        int middleOfList = xpsFilesList.Count / 2;

                        for (int i = 0; i < middleOfList; i++)
                        {
                            imagesPathToCompare.Add(xpsFilesList[i]);
                        }

                        for (int i = middleOfList; i < xpsFilesList.Count; i++)
                        {
                            originalImagesPath.Add(xpsFilesList[i]);
                        }

                        comparer.CompareImagesFromPaths(imagesPathToCompare, originalImagesPath);

                    }
            }

            Console.WriteLine("Job finished! Press any key to exit...");
            Console.ReadKey();
        }
    }
}
