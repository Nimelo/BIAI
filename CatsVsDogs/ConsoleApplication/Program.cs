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
            int SHREDEDELEMENTS = 0;

            //var cats = Perform(@"E:\Uczelnia\sem6\BIAI\train\cats", 50, 0, 50, 5, out SHREDEDELEMENTS);
            //var dogs = Perform(@"E:\Uczelnia\sem6\BIAI\train\dogs", 50, 0, 50, 5, out SHREDEDELEMENTS);

            //var catHistograms = cats.HueHistogram(SECTIONS);
            //var dogHistograms = dogs.HueHistogram(SECTIONS);

            //// ConsoleOutput(SECTIONS, catHistograms, dogHistograms);


            List<int[]> sourceInput = new List<int[]>();
            List<int> sourceOut = new List<int>();

            //AddToSource(ref sourceInput, ref sourceOut, catHistograms, 0, SHREDEDELEMENTS);
            //AddToSource(ref sourceInput, ref sourceOut, dogHistograms, 1, SHREDEDELEMENTS);
            Console.WriteLine("LoadingData");
            for(int i = 0; i < 500; i+=50)
            {
                
                AddToSource(ref sourceInput, ref sourceOut, Perform(@"E:\Uczelnia\sem6\BIAI\train\cats", 50, i, 50, 5, out SHREDEDELEMENTS).HueHistogram(SECTIONS), 0, SHREDEDELEMENTS);
                AddToSource(ref sourceInput, ref sourceOut, Perform(@"E:\Uczelnia\sem6\BIAI\train\dogs", 50, i, 50, 5, out SHREDEDELEMENTS).HueHistogram(SECTIONS), 1, SHREDEDELEMENTS);
                if(i % 1000 == 0)
                {
                    Console.WriteLine(i + 1000);
                    Console.WriteLine(DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime);
                }
            }
           


            int[][] inputs;
            int[] outputs;

            DecisionTree tree = GenerateDecisionTree(SHREDEDELEMENTS, SECTIONS, 2);
            ID3Learning id3learning = new ID3Learning(tree);
           // MergeInputOutput(sourceInput, sourceOut, SHREDEDELEMENTS, out inputs, out outputs);
            //AddToSource(catHistograms, dogHistograms, SHREDEDELEMENTS, out inputs, out outputs);

            // Learn the training instances!
            id3learning.Run(sourceInput.ToArray(), sourceOut.ToArray());
           // Console.WriteLine(id3learning.ComputeError(inputs, outputs));

            int findedCats = 0;
            int findedDogs = 0;

            Console.WriteLine("Query");
            int index = 0;
            for(int iteration = 0; iteration < 12500; iteration += 50)
            {
                if(iteration % 1000 == 0)
                {
                    Console.WriteLine(iteration);
                    Console.WriteLine(DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime);
                }
                var test = Perform(@"E:\Uczelnia\sem6\BIAI\test1\test1", 50, iteration, 50, 5, out SHREDEDELEMENTS);
                var testHistograms = test.HueHistogram(SECTIONS);
                foreach(var item in testHistograms)
                {
                    int[] query = item.ToArray();
                    int output = tree.Compute(query);

                    if(output == 0)
                    {
                        findedCats++;
                    }
                    else
                    {
                        findedDogs++;
                    }

                    File.AppendAllText("dogscats.txt", index + 1 + "," + output + Environment.NewLine);
                    index++;
                }

            }
            Console.WriteLine("Dogs: {0}\nCats: {1}", findedDogs, findedCats);
            Console.WriteLine(DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime);

            // File.Create(@"dogscats.txt");

        }

        private static void AddToSource(ref List<int[]> source, ref List<int> sourceOut, IEnumerable<IEnumerable<int>> collection, int param, int shreddedElements)
        {
            foreach(var item in collection)
            {
                var tmp = new int[shreddedElements];
                Array.Copy(item.ToArray(), tmp, shreddedElements);

                source.Add(tmp);
                sourceOut.Add(param);
            }
        }

        private static void MergeInputOutput(List<int[]> source, List<int> sourceOut, int shreddedElements, out int[][] inputs, out int[] outputs)
        {
            int yk = 0;
            inputs = new int[source.Count()][];
            outputs = new int[source.Count()];

            foreach(var item in source)
            {
                inputs[yk] = new int[shreddedElements];
                Array.Copy(item.ToArray(), inputs[yk], shreddedElements);
                outputs[yk] = sourceOut.ElementAt(yk);
                yk++;
            }
        }


        private static void ConsoleOutput(int SECTIONS, IEnumerable<IEnumerable<int>> catHistograms, IEnumerable<IEnumerable<int>> dogHistograms)
        {
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
        }

        private static DecisionTree GenerateDecisionTree(int amountOfDecisionVariables, int sections, int amountOfClassCount)
        {
            List<DecisionVariable> attributes = new List<DecisionVariable>();
            for(int i = 0; i < amountOfDecisionVariables; i++)
            {
                attributes.Add(new DecisionVariable(i.ToString(), new AForge.IntRange(0, sections)));
            }

            DecisionTree tree = new DecisionTree(attributes.ToArray(), amountOfClassCount);
            return tree;
        }

        private static List<BitmapFeature> Perform(string path, int amount, int from, int roundTo, int shredBy, out int shrededElementsPerBitmap)
        {
            Size avgSize = new Size(400, 350);

            //1. Load collection
            var bmpCollection = BitmapFactory.BitmapFactory.LoadFromDirectory(path, amount, from).ToList();

            //2. Calculate average size of images from collection
            //avgSize = bmpCollection.AverageSize().RoundSquareTo(roundTo);

            //3. Resize images
            bmpCollection = bmpCollection.ResizeImages(avgSize).ToList();

            //4. Shred images
            var shreddedCollection = bmpCollection.ShredImages(shredBy).ToList();

            shrededElementsPerBitmap = shreddedCollection.First().Count();

            //5. Calculate features
            List<BitmapFeature> bmpFeatures = new List<BitmapFeature>();
            shreddedCollection.ForEach(sc => bmpFeatures.Add(new BitmapFeature(sc)));

            return bmpFeatures;
        }
    }
}



