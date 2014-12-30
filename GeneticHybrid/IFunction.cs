using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticHybrid
{
    interface IFunction
    {
        double getValue(double[] x);
        int getDim();
        IMatrix getMatrix();
    }

    class MatrixFunction : IFunction
    {
        IMatrix matrix;


        public MatrixFunction(IMatrix matrix)
        {
            this.matrix = matrix;
        }

        public IMatrix getMatrix()
        {
            return matrix;
        }

        public double getValue(params double[] x)
        {
            double value = 0;
            int size = matrix.getSizeCols() - 1;
            for (int i = 0; i < size; i++)
            {
                int _x = (int)x[i];
                int _y = (int)x[i + 1];
                double temp = matrix.readM(_x - 1,  _y - 1);
                value += temp;
            }
            int xx = (int)x[matrix.getSizeRows() - 1];
            int yy = (int)x[0];
            double tempo = matrix.readM(xx - 1,  yy - 1);
            value += tempo;
            return value;
        }

        public int getDim()
        {
            return matrix.getSizeCols();
        }
    }

    class MultiLipshevFun : IFunction
    {
        IFormula fm;
        public MultiLipshevFun(IFormula fm)
        {
            this.fm = fm;
        }

        public double getValue(double[] x)
        {
            return fm.eval(x);
        }

        public int getDim()
        {
            return fm.getDim();
        }


        public IMatrix getMatrix()
        {
            throw new NotImplementedException();
        }
    }
}
