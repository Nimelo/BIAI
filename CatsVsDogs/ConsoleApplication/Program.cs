using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitmapHelper;
using CsvHelper;
using System.IO;
using System.Collections;
using SizeHelper;
using Accord.MachineLearning.DecisionTrees;
using System.Data;
using Accord.Statistics.Filters;
using Accord.MachineLearning.DecisionTrees.Learning;
using System.Windows.Forms;
namespace ConsoleApplication
{
    /*
        @"E:\GitHubRepo\BIAI\CatsVsDogs\ConsoleApplication\bin\Debug\SomeCats"
        @"E:\Uczelnia\sem6\BIAI\train\train"
     */

    class Program
    {
        public static void Main(string[] args)
        {
            var bmps = BitmapFactory.BitmapFactory.LoadFromDirectory(@"E:\Uczelnia\sem6\BIAI\train\cats\", 1, 0);

            int shred = 10;
            var bmp = bmps.First().ResizeImage(400, 350).ShredImage(shred).Average().Merge(new Size(400, 350), new Size(shred, shred));

           // bmp.Convert2GrayScaleFast();
           // bmp.threshold(bmp.getOtsuThreshold());

            bmp.Save("tmp.bmp");
            Display(bmp);
            
        }

        public static void Display(Image img)
        {

            Form form = new Form();
            var pb = new PictureBox();
            pb.Height = img.Height;
            pb.Width = img.Width;
            pb.Image = img;
            form.Controls.Add(pb);
            Application.Run(form);

        }

    }
}



