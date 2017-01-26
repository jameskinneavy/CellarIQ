using System.Linq;
using System.Text;
using CellarIQ.Data;

namespace CellarIQ.CLI.Commands
{
    internal class ListAllWineLabels : CellarCommand

    {
        public ListAllWineLabels(CellarManager manager) : base(manager)
        {
        }

        public override string Execute(string[] args)
        {
            var wines = CellarManager.GetAllWineLabels().ToList();
            wines.Sort(CompareWines);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"There are {wines.Count} wine labels in the cellar:");
            foreach (Wine wine in wines)
            {
                sb.AppendFormat("{0} {1} {2}\n", wine.VintnerName, wine.Vintage, wine.Label);
            }

            return sb.ToString();
        }

        private int CompareWines(Wine x, Wine y)
        {
            string xdata = $"{x.VintnerName} {x.Vintage} {x.Label} ";
            string ydata = $"{y.VintnerName} {y.Vintage} {y.Label} ";

            return xdata.CompareTo(ydata);
        }
    }
}
