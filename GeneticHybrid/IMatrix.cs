using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticHybrid
{
    interface IVector
    {
        double readV(int pos);
        void writeV(int pos, double val);
        int getSize();
    }

    class SparseVector : IVector
    {
        private int size;
        private Dictionary<int, double> map;

        public SparseVector(int _size)
        {
            this.size = _size;
            this.map = new Dictionary<int, double>();
        }

        public double readV(int pos)
        {
            if (map.ContainsKey(pos))
                return map[pos];
            return 0;
        }

        public void writeV(int pos, double val)
        {
            if (map.ContainsKey(pos))
                map[pos] = val;
            else
                map.Add(pos, val);
        }

        public int getSize()
        {
            return this.size;
        }
    }

    interface IMatrix
    {
        double readM(int row, int column);
        void writeM(int row, int column, double value);
        int getSizeRows();
        int getSizeCols();
    }

    interface IHider : IMatrix
    {
        IHider Hide_row(int row);
        IHider Hide_col(int col);
    }

    interface IAdder : IMatrix
    {
        IAdder addRow(int row);
        IAdder addCol(int col);
    }



    abstract class ASomeMatrix : IMatrix
    {
        private List<IVector> vectors;
        private int sizeRows;
        private int sizeCols;

        public ASomeMatrix(int sizeRows, int sizeCols)
        {
            this.sizeRows = sizeRows;
            this.sizeCols = sizeCols;

            vectors = new List<IVector>();

            for (int i = 0; i < sizeRows; i++)
            {
                vectors.Add(create(sizeCols));
            }
        }

        protected abstract IVector create(int size);

        public int getSizeRows()
        {
            return sizeRows;
        }

        public int getSizeCols()
        {
            return sizeCols;
        }

        public double readM(int row, int column)
        {
            return vectors[row].readV(column);
        }

        public void writeM(int row, int column, double value)
        {
            if (row == sizeRows)
            {
                vectors.Add(new SparseVector(column));
                sizeRows++;
                sizeCols++;
            }

            vectors[row].writeV(column, value);

        }
    }


    class SparseMatrix : ASomeMatrix
    {
        public SparseMatrix(int cols, int rows)
            : base(cols, rows) { }

        protected override IVector create(int size)
        {
            return new SparseVector(size);
        }
    }

    class AddDecoratorCol : IAdder
    {
        private IMatrix matrix;
        private int addedCol;

        public AddDecoratorCol(IMatrix matrix, int addedCol)
        {
            if ((addedCol < 0) || (addedCol > matrix.getSizeCols()))
                throw new IndexOutOfRangeException();

            this.matrix = matrix;
            this.addedCol = addedCol;

        }

        public IAdder addRow(int row)
        {
            return new AddDecoratorRow(this, row);
        }

        public IAdder addCol(int col)
        {
            return new AddDecoratorCol(this, col);
        }

        public double readM(int row, int col)
        {
            if (col < addedCol)
            {
                return matrix.readM(row, col);
            }
            if (col > addedCol && col < this.getSizeCols())
                return matrix.readM(row, col - 1);

            if (col == addedCol)
            {
                return matrix.readM(row, this.getSizeCols() - 1);
            }

            return 0;

        }

        public void writeM(int row, int column, double value)
        {
            matrix.writeM(row, column, value);
        }

        public int getSizeRows()
        {
            return matrix.getSizeRows();
        }

        public int getSizeCols()
        {
            return matrix.getSizeCols() + 1;
        }
    }

    class AddDecoratorRow : IAdder
    {
        private IMatrix matrix;
        private int addedRow;

        public AddDecoratorRow(IMatrix matrix, int addedRow)
        {
            if ((addedRow < 0) || (addedRow > matrix.getSizeRows()))
                throw new IndexOutOfRangeException();

            this.matrix = matrix;
            this.addedRow = addedRow;
        }

        public IAdder addRow(int row)
        {
            return new AddDecoratorRow(this, row);
        }

        public IAdder addCol(int col)
        {
            return new AddDecoratorCol(this, col);
        }

        public double readM(int row, int column)
        {
            throw new NotImplementedException();
        }

        public void writeM(int row, int column, double value)
        {
            throw new NotImplementedException();
        }

        public int getSizeRows()
        {
            return getSizeRows() + 1;
        }

        public int getSizeCols()
        {
            return getSizeCols();
        }
    }

    class HideDecoratorCol : IHider
    {
        IMatrix matrix;
        int hidden_col;

        public HideDecoratorCol(IMatrix matrix, int col)
        {
            if ((col < 0) || (col > matrix.getSizeCols()))
                throw new IndexOutOfRangeException();

            this.matrix = matrix;
            hidden_col = col;
        }

        public IHider Hide_row(int row)
        {
            return new HideDecoratorRow(this, row);
        }

        public IHider Hide_col(int col)
        {
            return new HideDecoratorCol(this, col);
        }

        public int getSizeRows()
        {
            return matrix.getSizeRows();
        }

        public int getSizeCols()
        {
            return (matrix.getSizeCols() - 1);
        }

        public double readM(int row, int col)
        {
            if (col < hidden_col)
            {
                return matrix.readM(row, col);
            }
            else
                return matrix.readM(row, col + 1);
        }

        public void writeM(int row, int col, double value)
        {
            if (col < hidden_col)
            {
                matrix.writeM(row, col, value);
            }
            else
                matrix.writeM(row, col + 1, value);
        }
    }


    class HideDecoratorRow : IHider
    {
        IMatrix matrix;
        int hidden_row;
        //TODO: row < matrix.row_num! 
        public HideDecoratorRow(IMatrix matrix, int row)
        {
            if ((row < 0) || (row > matrix.getSizeRows()))
                throw new IndexOutOfRangeException();

            this.matrix = matrix;
            hidden_row = row;
        }

        public IHider Hide_row(int row)
        {
            return new HideDecoratorRow(this, row);
        }

        public IHider Hide_col(int col)
        {
            return new HideDecoratorCol(this, col);
        }

        public int getSizeRows()
        {
            return (matrix.getSizeRows() - 1);
        }

        public int getSizeCols()
        {
            return matrix.getSizeCols();
        }

        public double readM(int row, int col)
        {
            if (row < hidden_row)
            {
                return matrix.readM(row, col);
            }
            else
            {
                return matrix.readM(row + 1, col);
            }
        }

        public void writeM(int row, int col, double value)
        {
            if (row < hidden_row)
            {
                matrix.writeM(row, col, value);
            }
            else
            {
                matrix.writeM(row + 1, col, value);
            }
        }
    }
}
