using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    public static class ColorExtension
    {
        public static int Section(this Color color, int amountOfSection)
        {
            return (int)( color.GetHue() * amountOfSection / 360.0 );
        }
    }
}
