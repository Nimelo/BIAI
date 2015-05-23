using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    class Program
    {
        static void Main(string[] args)
        {
            var tmp = File.ReadAllLines(@"E:\GitHubRepo\BIAI\CatsVsDogs\ConsoleApplication\bin\Debug\dogscats.txt");

            Random e = new Random();
            File.WriteAllLines("dogscats.csv", tmp.Take(1));
            foreach(var item in tmp.Skip(1))
            {
                var tmp2 = item.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                int value = Int16.Parse(tmp2[0]);

                int value2 = int.Parse(tmp2[1]);

                if(value2 == -1)
                    value2 = e.Next(1);

                File.AppendAllText("dogscats.csv", value + 1 + "," + value2 + Environment.NewLine);

            }
        }
    }
}
