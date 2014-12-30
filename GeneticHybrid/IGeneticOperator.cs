using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GeneticHybrid
{
    interface IGeneticOperator
    {
        List<IGenotype> getPopulation(List<IGenotype> population, IFunction f);
    }



    class InitPopu : IGeneticOperator
    {
        private int Fdim, N;
        private double[] a, b;

        public InitPopu(int Fdim, int N, double[] a, double[] b)
        {
            this.Fdim = Fdim;
            this.N = N;
            this.a = (double[])a.Clone();
            this.b = (double[])b.Clone();
        }

        public List<IGenotype> getPopulation(List<IGenotype> population, IFunction f)
        {
            Random r = new Random();
            //    double[] genotemp = new double[Fdim];
            List<double> genotemp = new List<double>();
            for (int i = 0; i < N; i++)  // N - razmer populiatsii
            {
                int j = 0;  // --nachinayu s 1 t.k. pervoe znachenie fiksirovannoe

                genotemp.Clear();
             //   genotemp.Add(1);   // fiksirovannoe znachenie
                while (j < Fdim)
                {
                    int newValue = r.Next((int)a[j], (int)b[j] + 1); // a[j] + 1 potomu chto vybiraem znachenia ot 2 do 15, pervoe znachenie - 1 - fiksirovannoe
                    if (!genotemp.Contains(newValue))
                    {
                        genotemp.Add(newValue);  // beru sluchainie DOPUSTIMIE parametri argumenta
                        j++;
                    }
                }
                population.Add(new Genotype(genotemp.ToArray(), f));  // dobavliayu v populiatsiu
            }

            return population;
        }
    }



    class ZhadnyInitPopu : IGeneticOperator
    {
        private int Fdim, N;
        private double[] a, b;

        public ZhadnyInitPopu(int Fdim, int N, double[] a, double[] b)
        {
            this.Fdim = Fdim;
            this.N = N;
            this.a = (double[])a.Clone();
            this.b = (double[])b.Clone();
        }

        private Dictionary<int, double> getRow(int indexRow, IFunction f)
        {
            indexRow--;
            Dictionary<int, double> row = new Dictionary<int, double>();
            for (int i = 0; i < f.getDim(); i++)
            {
                if(i != indexRow)
                {
                    row.Add(i + 1, f.getMatrix().readM(indexRow, i));
                }
            }
            return row;
        }

        public List<IGenotype> getPopulation(List<IGenotype> population, IFunction f)
        {
            Random r = new Random();
            for (int n = 0; n < N; n++)  // N - razmer populiatsii
            {
                int i = r.Next(1, 6); // sluchainaya stroka (sluchainiy gorod)
                Dictionary<int, double> row = getRow(i, f);     // stroka dlia poiska, iz nee postepenno udaliayutsa indexi s minimalnimi znacheniami
                List<double> genotemp = new List<double>();         // novaya osob, tuda budut dobavliatsa indexi

                genotemp.Add(i);
                while(row.Count > 0)
                {
                    double min = row.Values.Min();
                    int nextIndex = row.FirstOrDefault(x => x.Value == min).Key;
                    genotemp.Add(nextIndex);
                    row.Remove(nextIndex);
                }
                population.Add(new Genotype(genotemp.ToArray(), f));  // dobavliayu v populiatsiu
            }
            return population;
        }
    }

    class Crossover : IGeneticOperator
    {
        private int crossingRate;

        public Crossover()
        {

        }

        public Crossover(int crossingRate)
        {
            this.crossingRate = crossingRate;
        }

        public virtual List<IGenotype> getPopulation(List<IGenotype> population, IFunction f)
        {
            int L = population[0].getGenotype().Length;
            int N = population.Count;
            int Fdim = f.getDim();
            int cross;

            Random r = new Random();
            cross = r.Next(1, L); // sluchainaya tochka dlia skreshivania    --2 <= cross < L    iz-za fiks
                                                                                         
            double[] buff = new double[L];
            int max = (int)(this.crossingRate * N / 100); // maximalnoe kolichestvo brachnix par

            for (int k = 0; k < max - 1; k = k + 2) // beru po 2 osobi i skreshivayu ix
            {
                double[] child1 = new double[Fdim];
                double[] child2 = new double[Fdim];

                for (int i = 0; i < cross; i++)
                {
                    child1[i] = population[k].getCodedGenotype()[i];
                    child2[i] = population[k + 1].getCodedGenotype()[i];
                }
                for (int i = cross; i < L; i++)
                {
                    child1[i] = population[k + 1].getCodedGenotype()[i];
                    child2[i] = population[k].getCodedGenotype()[i];
                }

                population.Add(new Genotype(Kod.uncode(child1), f));
                population.Add(new Genotype(Kod.uncode(child2), f));
            }
            return population;
        }
    }

    class TwoPointCrossover : IGeneticOperator
    {
        private int crossingRate;
        Random r;

        public TwoPointCrossover(int crossingRate)
        {
            this.crossingRate = crossingRate;
            
        }

        public List<IGenotype> getPopulation(List<IGenotype> population, IFunction f)
        {
            r = new Random();
            int L = population[0].getGenotype().Length;
            int N = population.Count;
            int Fdim = f.getDim();
            int cross1, cross2;
            List<int> crossPoints = new List<int>();
            
            cross1 = r.Next(1, L); // sluchainaya tochka dlia skreshivania  
            crossPoints.Add(cross1);

            do{
                cross2 = r.Next(1, L);
            }
            while(cross2 == cross1);
            crossPoints.Add(cross2);

            crossPoints.Sort();

            int max = (int)(this.crossingRate * N / 100); // maximalnoe kolichestvo brachnix par

            for (int k = 0; k < max - 1; k = k + 2) // beru po 2 osobi i skreshivayu ix
            {
                double[] child1 = new double[Fdim];
                double[] child2 = new double[Fdim];

                for (int i = 0; i < crossPoints[0]; i++)
                {
                    child1[i] = population[k].getCodedGenotype()[i];
                    child2[i] = population[k + 1].getCodedGenotype()[i];
                }

                for (int i = crossPoints[0]; i < crossPoints[1]; i++)
                {
                    child1[i] = population[k + 1].getCodedGenotype()[i];
                    child2[i] = population[k].getCodedGenotype()[i];
                }

                for (int i = crossPoints[1]; i < L; i++)
                {
                    child1[i] = population[k].getCodedGenotype()[i];
                    child2[i] = population[k + 1].getCodedGenotype()[i];
                }

                population.Add(new Genotype(Kod.uncode(child1), f));
                population.Add(new Genotype(Kod.uncode(child2), f));
            }
            return population;
        }
    }

    class DiscreteMutation : IGeneticOperator
    {
        private int mutationRate;
        double[] a, b;

        public DiscreteMutation(int mutationRate, double[] a, double[] b)
        {
            this.a = a;
            this.b = b;
            this.mutationRate = mutationRate;
        }

        public List<IGenotype> getPopulation(List<IGenotype> population, IFunction f)
        {
            int L = population[0].getGenotype().Length; // dlina genotype
            int N = population.Count;
            int Fdim = f.getDim();

            Random r = new Random();
            int count = 0;
            int max = (int)(mutationRate * N / 100);

            while (count < max)
            {
                int ip = r.Next(0, N); // index dlia populiatsii           
                int ig1 = r.Next(0, L); // index dlia mutatsii genotype    // iz-za fiks.
                int ig2 = 0; // vtoroi index dlia mutatsii

                while(true){
                    ig2 = r.Next(0, L); // iz-za fiks.
                    if (ig2 != ig1)
                    {
                        break;
                    }
                }
     //           double value = r.NextDouble() * (b[ig] - a[ig]) + a[ig]; // sluchainoe razreshennoe znachenie dlia gena

                double[] genotemp = new double[Fdim];
                for (int i = 0; i < Fdim; i++)
                {
                    genotemp[i] = population[ip].getGenotype()[i]; // sozdayu mutirovannij gen iz sluchainoi osobi ip iz vsei popuiatsii
                }

                genotemp[ig1] = population[ip].getGenotype()[ig2]; // sobstvenno, mutatsia
                genotemp[ig2] = population[ip].getGenotype()[ig1]; // sobstvenno, mutatsia

                population.Add(new Genotype(genotemp, f));

                count++;
            }
    //        Console.WriteLine("идет мутация...");

            return population;
        }
    }

    class Mutation : IGeneticOperator
    {
        private int mutationRate;
        double[] a, b;


        public Mutation(int mutationRate, double[] a, double[] b)
        {
            this.a = a;
            this.b = b;
            this.mutationRate = mutationRate;
        }

        public List<IGenotype> getPopulation(List<IGenotype> population, IFunction f)
        {
            int L = population[0].getGenotype().Length; // dlina genotype
            int N = population.Count;
            int Fdim = f.getDim();

            Random r = new Random();
            int count = 0;
            int max = (int)(mutationRate * N / 100);

            while(count < max)
            {
                int ip = r.Next(0, N); // index dlia populiatsii
                int ig = r.Next(0, L); // index dlia genotype
                double value = r.NextDouble() * (b[ig] - a[ig]) + a[ig]; // sluchainoe razreshennoe znachenie dlia gena

                double[] genotemp = new double[Fdim];
                for (int i = 0; i < Fdim; i++)
                {
                    genotemp[i] = population[ip].getGenotype()[i]; // sozdayu mutirovannij gen
                }
                genotemp[ig] = value; // sobstvenno, mutatsia

                population.Add(new Genotype(genotemp, f));

                count++;
            }
            return population;
        }
    }

    //elitist strategy
    class Selector : IGeneticOperator
    {
        private int N; // nachalniy zadanniy razmer populiatsii

        public Selector(int N)
        {
            this.N = N;
        }

        public List<IGenotype> getPopulation(List<IGenotype> population, IFunction f)
        {
            population.Sort(new comp());
            population.RemoveRange(N, population.Count-N);
            return population;
        }
    }

    //roulette wheel selection
    class RouletteSelector : IGeneticOperator
    {
        private int N;
        private Random r;

        public RouletteSelector(int N)
        {
            this.N = N;
        }

        public List<IGenotype> getPopulation(List<IGenotype> population, IFunction f)
        {
            r = new Random();
            double suma = 0; // obshy znamenatel dlia vychislenia veroyatnosti kazhdoi osobi
            List<IGenotype> roulette = new List<IGenotype>();

            foreach(var genotype in population)
            {
                suma += 1/genotype.getRang();
            }

            double exp = 0;

     /*       foreach (var genotype in population)
            {
                exp += (1/ genotype.getRang()) / suma;
            }*/

            foreach (var genotype in population)
            {
                int p = Convert.ToInt32((1000 / genotype.getRang()) / suma);
                int count = 0;
                while(count < p)
                {
                    roulette.Add(genotype);
                    count++;
                }
            }

            population = new List<IGenotype>();
            for (int i = 0; i < N; i++)
            {
                population.Add(roulette[r.Next(0, roulette.Count)]);
            }

            return population;
        }
    }

    class TournamentSelector : IGeneticOperator
    {
        private int N;
        private Random r;

        public TournamentSelector(int N)
        {
            this.N = N;
        }

        private IGenotype getBetterFromFroup(List<IGenotype> group)
        {
            IGenotype best = group[0];

            foreach (var genotype in group)
            {
                if (genotype.getRang() <= best.getRang())
                {
                    best = genotype;
                }
            }

            return best;
        }


        public List<IGenotype> getPopulation(List<IGenotype> population, IFunction f)
        {
            r = new Random();

            List<IGenotype> [] groups = new List<IGenotype>[N]; // gruppy dlia turnira

            int sizeGroup = Convert.ToInt32(population.Count / N);  // chislo uchastnikov v kazhdoi gruppe

            //Zapolniayu gruppy
            int index = 0; // index to add to group
            for (int i = 0; i < N - 1; i++)
            {
                groups[i] = new List<IGenotype>();
                for(int j = 0; j < sizeGroup; j++)
                {
                    index = r.Next(0, population.Count);
                    groups[i].Add(population[index]);
                    population.RemoveAt(index);
                }
            }
            int sizeLastGroup = population.Count;
            groups[N - 1] = new List<IGenotype>();
            for (int j = 0; j < sizeLastGroup; j++)
            {
                index = r.Next(0, population.Count);
                groups[N - 1].Add(population[index]);
                population.RemoveAt(index);
            }
            


            List<IGenotype> newPopulation = new List<IGenotype>();  // novaya populatsia posle selektsii
            for (int i = 0; i < N; i++ ) // proidus po kazhdoi gruppe i vyberu lushuyu osob
            {
                newPopulation.Add(getBetterFromFroup(groups[i]));
            }

            return newPopulation;
        }
    }

    class ArithmeticCrossover : Crossover
    {
        private int crossingRate;

        public ArithmeticCrossover(int crossingRate)
        {
            this.crossingRate = crossingRate;
        }

        public override List<IGenotype> getPopulation(List<IGenotype> population, IFunction f)
        {
            int L = population[0].getGenotype().Length;
            int N = population.Count;
            int Fdim = f.getDim();
            double alpha;
            int max = (int)(this.crossingRate * N / 100); // maximalnoe kolichestvo brachnix par

            Random r = new Random();
            alpha = r.NextDouble(); // sluchainaya tochka dlia skreshivania

            for (int k = 0; k < max - 1; k = k + 2) // beru po 2 osobi i skreshivayu ix
            {
                int n = 0, m = 0; // indeksi dlia sluchainix roditelei
                n = new Random().Next(0, population.Count);
                m = new Random().Next(0, population.Count);

                double[] ancestor1 = new double[Fdim];
                double[] ancestor2 = new double[Fdim];

                double[] parent1 = population[n].getGenotype();
                double[] parent2 = population[m].getGenotype();

                for (int i = 0; i < L; i++)
                {
                    ancestor1[i] = alpha*parent1[i] + (1-alpha)*parent2[i];
                    ancestor2[i] = (1-alpha) * parent1[i] + alpha * parent2[i];
                }

                population.Add(new Genotype(ancestor1, f));
                population.Add(new Genotype(ancestor2, f));
            }
            return population;
        }
    }

    class WriterLog : IGeneticOperator
    {
        private string fileName= "data.txt";
        public WriterLog()
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        public List<IGenotype> getPopulation(List<IGenotype> population, IFunction f)
        {
            using (StreamWriter writer = new StreamWriter(fileName, true, Encoding.Default))
            {
                double bestFitness = getBestFitness(population);
                double averageFitness = getFitnessMedium(population);
                writer.WriteLine("{0}, {1}", bestFitness, averageFitness);
            }
            return population;
        }

        // vychisliaet srednuyu prisposoblennost dlia kazhdoi populiatsii
        private double getFitnessMedium(IEnumerable<IGenotype> population)
        {
            return population.Average(genotype => genotype.getRang());
        }

        private double getBestFitness(IEnumerable<IGenotype> population)
        {
            return population.Min(genotype => genotype.getRang());
        }
    }


    class LocalAdaptation : IGeneticOperator
    {
        private IExtremunFinder finder;
        private int dim = 0;
        double[] a, b;

        public LocalAdaptation(double[] a, double[] b)
        {
            this.a = a;
            this.b = b;
        }

        public List<IGenotype> getPopulation(List<IGenotype> population, IFunction f)
        {
            List<IGenotype> popu = new List<IGenotype>(); // to chto budet na vyxode
           
            int count = 0;
            if (dim == 0)
            {
                dim = f.getDim();
            }

            // cycle proxodit po kazhdoi osobi v populiatsii
            while (count < population.Count)
            {
                double[] argMin = (double[])population[count].getGenotype().Clone(); // delayu kopiu tekushego massiva
                IGenotype genoMin; // budet dobavliatsa v populiatsiu popu

                // cycl proxodit po kazhdomu elementu tekushego genotypa
                for (int i = 0; i < dim; i++)
                {
                    genoMin = new Genotype(argMin, f);

                    double[] argTemp = (double[])argMin.Clone(); // vremenno, dlia sravnenia
                    finder = new Strongin(a[i], b[i], argTemp, i, 0.5); // po ocheredi uluchaetsa kazhdiy element genotypa
                    argTemp[i] = finder.argMin(f);
                    IGenotype genoTemp = new Genotype(argTemp, f);

                    // uslovie ostanova
                    if (genoTemp.getRang() <= genoMin.getRang())
                    {
                        //esli bylo ulushenie, to soxraniayu ego
                        argMin = (double[])argTemp.Clone();
                    }
                    else
                    {  
                        //esli net ulushenia, to vyxozhu is tsikla chtoby pereiti k sleduyushei osobi
                        break;
                    }
                    
                }

                genoMin = new Genotype(argMin, f);
                popu.Add(genoMin);
                count++;
            }
            //fin de ciclo

            return popu;
        }
    }

}
