using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticHybrid
{
    class comp : IComparer<IGenotype> //nuzhen dlia sortirovki spiska population
    {
        public int Compare(IGenotype x, IGenotype y)
        {
            if (x.getRang() > y.getRang())
                return 1;
            else if (x.getRang() < y.getRang())
                return -1;
            else return 0;
        }
    }
}
