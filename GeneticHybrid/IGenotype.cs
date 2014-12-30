using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticHybrid
{
    public interface IGenotype
    {
        double getRang();
        double[] getGenotype();
        double[] getCodedGenotype();
    }

    class Genotype : IGenotype
    {
        private double[] genotype;
        private double rang;

        public Genotype(double[] genotype, IFunction f)
        {
       /*     this.genotype = new double[genotype.Length];//genotype;

            for (int i = 0; i < genotype.Length; i++ ) // klonirovanie
                this.genotype[i] = genotype[i];
            */
            this.genotype = (double[])genotype.Clone();
            rang = f.getValue(genotype);
        }

        public double getRang()
        {
            return rang;
        }

        public double[] getGenotype() 
        {
            return genotype;
        }

        public override string ToString()
        {
            return string.Format("{1} : ({0})", string.Join(", ", genotype), rang);
        }


        public double[] getCodedGenotype()
        {
            return Kod.code(genotype);
        }
    }
}
