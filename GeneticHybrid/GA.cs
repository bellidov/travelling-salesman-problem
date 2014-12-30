using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace GeneticHybrid
{

    class GA
    {
        private int N;//razmer populiatsii
        private int M = 1; // razmer odnogo genotipa = 1 t.k. bez kodirovania poka
        private List<IGenotype> population; //soderzhit vozmozhnie reshenia
        private double[] a, b; // tochki nachala i kontsa vozmozhnix znachenij osobi
        IFunction f;
        private int generations;
        private List<IGeneticOperator> operators;
        private IGeneticOperator initPopu;

        public GA(double[] a, double[] b, int generations, int popuSize)
        {
            this.a = a;
            this.b = b;
            population = new List<IGenotype>();
            this.generations = generations;
            this.N = popuSize;
        }

        //ustanavlivaet geneticheskie operatori
        public void setOperators(params IGeneticOperator[] oper){
            operators = new List<IGeneticOperator>();
            foreach (var v in oper)
            {
                operators.Add(v);
            }
        }

        public void setInitOperator(IGeneticOperator initPopu)
        {
            this.initPopu = initPopu;
        }

        //sam geneticheskiy algoritm soderzhitsa v etom metode
        public double[] FindMinArg(IFunction f)
        {
            int Fdim = f.getDim();
            int count = 0;
            this.f = f;
            // dlina osobi (genotypa):
            int L = M * Fdim;   // in this case L genotype (osob) =  dim of function because we are working without coder
           
            initPopulation(Fdim); // poluchaet sluchainuyu nachalnuyu populiatsiu

            double fitnessValue = population[0].getRang();

                while (count < generations) // poka schetchik ne dostignet zadannoe chislo
                {
                    // vypolniautsa vse 3 geneticheskie operatori podriad
                    foreach (var v in operators)
                    {
                        this.population = v.getPopulation(this.population, f);
                    }
                    
                    ////   uslovie ostanova
                    double buff = population.Min(genotype=>genotype.getRang());

  //                  Console.WriteLine(buff);

                    if (buff >= fitnessValue)
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                        fitnessValue = buff;
                    }
                }

            return population[0].getGenotype(); // iz-za sortirovki, samaya udachnaya osob vsegda v nachale spiska
        }

        // vozvrashaet ves nador vozmozhnix reshenij soderzhashixsia v populiatsii
        public List<IGenotype> getPopulation()
        {
            return this.population;
        }

        // random generator for first population
        private void initPopulation(int Fdim)     
        {
            population = initPopu.getPopulation(population, f);
            /*
            Random r = new Random();
        //    double[] genotemp = new double[Fdim];
            List<double> genotemp = new List<double>();
            for (int i = 0; i < N; i++)  // N - razmer populiatsii
            {
                int j = 1;  // nachinayu s 1 t.k. pervoe znachenie fiksirovannoe
                
                genotemp.Clear();
                genotemp.Add(1);   // fiksirovannoe znachenie
                while (j < Fdim)
                {
                    int newValue = r.Next((int)a[j] + 1, (int)b[j] + 1); // a[j] + 1 potomu chto vybiraem znachenia ot 2 do 15, pervoe znachenie - 1 - fiksirovannoe
                    if (!genotemp.Contains(newValue))
                    {
                        genotemp.Add(newValue);  // beru sluchainie DOPUSTIMIE parametri argumenta
                        j++;
                    }
                }
                population.Add(new Genotype(genotemp.ToArray(), f));  // dobavliayu v populiatsiu
            }
             */
        }


        ///////////////////////
        // draw the resultat //
        ///////////////////////

        double Ymin, Ymax, Xmin, Xmax; //koordinaty dlia grafiki
        double mx, my; // margin dlia grafiki
        List<double> P = null;

        public void Draw(IDrawer d, int index, double[] borders)
        {
            
            this.Xmin = borders[0];
            this.Xmax = borders[1];
            this.Ymin = borders[2];
            this.Ymax = borders[3];
            this.mx = 0.01;//  borders[4];
            this.my = 0.05; // borders[5];

            // razvivayu os' na shagi i soxraniau tochki v P
            int N = 500; // kolichestvo tochek dlia otrisovki

                P = new List<double>();
                double shag = (b[index] - a[index]) / N;
                for (int i = 0; i < N; i++)
                    P.Add(a[index] + i * shag);

            // beru minArg i tam meniayu element indeksa index
            double[] _a = (double[])population[0].getGenotype().Clone();
            double[] _b = (double[])_a.Clone();

            //risuyu grafik funtksii
            for (int i = 0; i < N - 1; i++)
            {
                _a[index] = P[i];
                _b[index] = P[i + 1];
                
                double test = f.getValue(_a);

                // kostyl
                double funValue = f.getValue(_b);
                if (!Double.IsNaN(test) && !Double.IsNaN(funValue))
                {
                    int xc1 = convertX(_a[index]);
                    int yc1 = convertY(test);
                    int xc2 = convertX(_b[index]);
                    int yc2 = convertY(funValue);

                    d.drawLine(xc1, yc1, xc2, yc2);
                }
            }

            //risuyu tochku minimuma
            d.drawPoint(convertX(population[0].getGenotype()[index]), 
                            convertY(population[0].getRang()));

            foreach (var v in population)
            {
                d.drawPoint(convertX(v.getGenotype()[index]),
                                convertY(v.getRang()));
            }
        }

        private int convertX(double x)
        {
            int temp = (int)(600*((x - Xmin)*(1-mx) + mx*(Xmax - x))/(Xmax-Xmin));
            return temp;
        }

        private int convertY(double y)
        {
            int temp = (int)(400 * ((Ymax - y) * (1 - my) + (y - Ymin) * my) / (Ymax - Ymin));
            return temp;
        }
    }
}
