using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace GeneticHybrid
{

    interface IExtremunFinder
    {
        double argMin(IFunction f);
        List<double> getPoints();
    }

    class Strongin : IExtremunFinder
    {
        private double Fmin, Xmin;
        private double a, b, L, error;
        private IFunction f;
        private List<double> X;
        private int k;
        private Random r;
        private double[] toFix;

        private int index;

        public Strongin(double a, double b, double[] toFix, int index, double error)
        {
            this.a = a;
            this.b = b;
            this.error = error;
            this.toFix = toFix;
            this.index = index;

            L = 0; // solo al inicio

            X = new List<double>();

            r = new Random();
            k = 2;     // mozhno li vsegda tak delat? ili inogda nado pobolshe srazu?
            // llenamos la lista con k elementos random
            for (int i = 0; i < k; i++)
                X.Add(r.NextDouble() * (b - a) + a);
            X[0] = a;
            X[k - 1] = b;
            X.Sort();
        }


        private double[] arg(double value)
        {
            toFix[index] = value; // poluchim argument s izmenennim elementom

            if(toFix[0] == 0 && toFix[1] == 0)
            {
                toFix[index] = 0.0001;
            }

            return toFix;
        }

        private double setL()
        {

            double temp;
            for (int i = 0; i < k - 1; i++)
            {
                double f1 = f.getValue(arg(X[i]));
                double f2 = f.getValue(arg(X[i + 1]));

                temp = Math.Abs((f1 - f2) / (X[i] - X[i + 1]));

                if (temp > L)
                    L = temp * 1.5;
                if (L == 0)
                    L = 1;
            }

            return L;
        }

        public double argMin(IFunction f)
        {
            this.f = f;

            // poluchit minimalnoe znachenie sredi vsex X
            Fmin = f.getValue(arg(X[0]));
            Xmin = X[0];
            for (int i = 0; i < k; i++)
            {
                double temp = f.getValue(arg(X[i]));
                if (temp < Fmin)
                {
                    Fmin = temp;
                    Xmin = X[i];
                }
            }



            while (true)
            {
                L = setL();
                //poluchit minimalnie znachenia lomanix linij
                double fmin = getR(X[0], X[1]);
                double xmin = getXr(X[0], X[1]);

                for (int i = 0; i < k - 1; i++)
                {
                    double temp = getR(X[i], X[i + 1]);
                    if (temp > fmin)   // ! zdes otlichie ot metoda Pyavskogo <
                    {
                        fmin = temp;
                        xmin = getXr(X[i], X[i + 1]);
                    }
                }

                // proverka ostanova
                if (Math.Abs(Xmin - xmin) < error)
                {
                    break;
                }
                else
                {
                    X.Add(xmin);
                    X.Sort();
                    k = k + 1;

                    if (f.getValue(arg(xmin)) < Fmin)
                    {
                        Fmin = f.getValue(arg(xmin));
                        Xmin = xmin;
                    }
                }
            }
            return Xmin;
        }

        // gets characteristic for interval
        private double getR(double a, double b)
        {
            double Fa = f.getValue(arg(a));
            double Fb = f.getValue(arg(b));
            double R = (b - a) * L + (Fb - Fa) * (Fb - Fa) / ((b - a) * L) - 2 * (Fb + Fa);
            return R;
            //    return (Fa + Fb) / 2 - L * (b - a) / 2;
        }

        // gets min arg for charact.
        private double getXr(double a, double b)
        {
            double Fa = f.getValue(arg(a));
            double Fb = f.getValue(arg(b));
            double Xr = (b + a) / 2 - (Fb - Fa) / (2 * L);

            return Xr;
        }

        // nuzhno chtoby risovat tochki aproksimatsii
        public List<double> getPoints()
        {
            return X;
        }
    }
}
