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
namespace ConsoleApplication
{
    /*
        @"E:\GitHubRepo\BIAI\CatsVsDogs\ConsoleApplication\bin\Debug\SomeCats"
        @"E:\Uczelnia\sem6\BIAI\train\train"
     */

    class Program
    {
        static void Main(string[] args)
        {
            int SECTIONS = 10;
            //1:40 dla 1000
            //0:14 dla 100
            var cats = Perform(@"E:\Uczelnia\sem6\BIAI\train\cats", 10);
            var dogs = Perform(@"E:\Uczelnia\sem6\BIAI\train\dogs", 10);

            var catHistograms = cats.HueHistogram(SECTIONS);
            var dogHistograms = dogs.HueHistogram(SECTIONS);

            foreach(var item in catHistograms.Histogram(SECTIONS))
            {
                item.ToList().ForEach(x => Console.Write(x + " "));
                Console.WriteLine();
            }

            Console.WriteLine(DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime);

            foreach(var item in dogHistograms.Histogram(SECTIONS))
            {
                item.ToList().ForEach(x => Console.Write(x + " "));
                Console.WriteLine();
            }

            Console.WriteLine(DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime);




            DataTable table = new DataTable("Example");

            for(int i = 0; i < 10; i++)
            {
                table.Columns.Add(i.ToString());
            }

            table.Rows.Add(catHistograms);

           // Codification codebook = new Codification(table);

            //DataTable symbols = codebook.Apply(table);

            DecisionVariable[] attrubites = 
            {
                  new DecisionVariable("0", new AForge.IntRange(0, 9)),
                 new DecisionVariable("1", new AForge.IntRange(0, 9)),
                  new DecisionVariable("2", new AForge.IntRange(0, 9)),
                   new DecisionVariable("3", new AForge.IntRange(0, 9)),
                  new DecisionVariable("4", new AForge.IntRange(0, 9)),
                   new DecisionVariable("5", new AForge.IntRange(0, 9)),
                  new DecisionVariable("6", new AForge.IntRange(0, 9)),
                   new DecisionVariable("7", new AForge.IntRange(0, 9)),
                  new DecisionVariable("8", new AForge.IntRange(0, 9)),
                   new DecisionVariable("9", new AForge.IntRange(0, 9))
                
            };

            int classCOunt = 2;

            DecisionTree tree = new DecisionTree(attrubites, classCOunt);

            // Create a new instance of the ID3 algorithm
            ID3Learning id3learning = new ID3Learning(tree);

            // Translate our training data into integer symbols using our codebook:
            //DataTable symbols = codebook.Apply(table);

            int yk = 0;
            int[][] inputs = new int[10][];//catHistograms.Count()][];

            foreach(var item in catHistograms)
            {
                inputs[yk] = new int[10];//[item.Count()];
                Array.Copy(item.ToArray(), inputs[yk], 10);//item.ToList().Count());
                yk++;
            }

            int[] outputs = new int[10];

            // Learn the training instances!
            id3learning.Run(inputs, outputs);




            //int[] query = codebook.Translate("Sunny", "Hot", "High", "Strong");

            //int output = tree.Compute(query);

            //string answer = codebook.Translate("PlayTennis", output);
            // answer will be "No".
        }

        private static List<BitmapFeature> Perform(string path, int amount)
        {
            Size avgSize = new Size(500, 500);

            //1. Load collection
            var bmpCollection = BitmapFactory.BitmapFactory.LoadFromDirectory(path, amount).ToList();

            //2. Calculate average size of images from collection
            avgSize = bmpCollection.AverageSize().RoundSquareTo(50);

            //3. Resize images
            bmpCollection = bmpCollection.ResizeImages(avgSize).ToList();

            //4. Shred images
            var shreddedCollection = bmpCollection.ShredImages(50).ToList();

            //5. Calculate features
            List<BitmapFeature> bmpFeatures = new List<BitmapFeature>();
            shreddedCollection.ForEach(sc => bmpFeatures.Add(new BitmapFeature(sc)));

            return bmpFeatures;
        }
    }
}



