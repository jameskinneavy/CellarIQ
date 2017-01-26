using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellarIQ.Common
{
    public static class ColorMapper 
    {
        private static readonly Dictionary<string, string> VarietalColorMap = new Dictionary<string, string>();

        static ColorMapper()
        {
            Map(Varietals.CabernetSauvignon, Colors.Red);
            Map(Varietals.Grenache, Colors.Red);
            Map(Varietals.Merlot, Colors.Red);
            Map(Varietals.Mourvedre, Colors.Red);
            Map(Varietals.PetitSirah, Colors.Red);
            Map(Varietals.PetitVerdot, Colors.Red);
            Map(Varietals.PinotBlanc, Colors.White);
            Map(Varietals.Sangiovese, Colors.Red);
            Map(Varietals.Syrah, Colors.Red);
            Map(Varietals.Tannat, Colors.Red);
            Map(Varietals.Tempranillo, Colors.Red);
            Map(Varietals.Viognier, Colors.White);
            Map(Varietals.Zinfandel, Colors.Red);
        }

        public static void Map(string varietal, string color)
        {
            VarietalColorMap.Add(varietal, color);
        }

        public static string GetVarietalColor(string varietalName)
        {
            
            return VarietalColorMap[varietalName];

        }
    }
}
