using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageComparer
{
    public class ImageComparer
    {
        public void CompareImagesFromPaths(List<string> xpsFilePath, List<string> xpsOriginalFilePath)
        {
            List<Tuple<string, string>> pairImagesToCompareList = new List<Tuple<string, string>>();

            PreparePairsOfImagesToCompare(xpsFilePath, xpsOriginalFilePath, pairImagesToCompareList);

            //foreach (var item in pairImagesToCompareList)
            //{
            //    CompareImage(item.Item1, item.Item2);
            //}

            Parallel.ForEach(pairImagesToCompareList, (item) =>
                {
                    CompareImage(item.Item1, item.Item2);
                }
                );

        }

        private static void PreparePairsOfImagesToCompare(List<string> xpsFilePath, List<string> xpsOriginalFilePath, List<Tuple<string, string>> pairImagesToCompareList)
        {
            if (xpsFilePath != null & xpsOriginalFilePath != null)
            {
                if (xpsFilePath.Count == xpsOriginalFilePath.Count)
                {
                    xpsFilePath.Sort();
                    xpsOriginalFilePath.Sort();

                    for (int i = 0; i < xpsFilePath.Count; i++)
                    {
                        Tuple<string, string> pairOfImagesToCompare = new Tuple<string, string>(xpsFilePath[i], xpsOriginalFilePath[i]);
                        pairImagesToCompareList.Add(pairOfImagesToCompare);
                    }
                }
            }
        }

        public void CompareImage(string imageOne, string imageTwo)
        {
            Image<Bgr, Byte> My_Image = new Image<Bgr, byte>(imageOne);
            Image<Bgr, Byte> My_Image2 = new Image<Bgr, byte>(imageTwo);

            Image<Bgr, Byte> My_Image3 = My_Image2.AbsDiff(My_Image);

            byte[, ,] data1 = My_Image.Data;
            byte[, ,] data2 = My_Image2.Data;

            int nrOfPixels = 0;
            for (int i = My_Image.Rows - 1; i >= 0; i--)
            {
                for (int j = My_Image.Cols - 1; j >= 0; j--)
                {
                    if (data1[i, j, 0] != data2[i, j, 0] && data1[i, j, 1] != data2[i, j, 1] && data1[i, j, 2] != data2[i, j, 2])
                    {
                        nrOfPixels++;
                    }
                }
            }

            Console.WriteLine("Image " + imageOne + " has " + nrOfPixels + " different pixels.");
        }
    }
}
