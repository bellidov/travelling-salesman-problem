using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeneticHybrid
{
    public partial class MyForm : Form
    {
        public MyForm()
        {
            InitializeComponent();
    //        g = Graphics.FromHwnd(panel.Handle);
            p = new Pen(Color.Red);
            comboBox1.Items.Add(initpopu1);
            comboBox1.Items.Add(initpopu2);
            comboBox2.Items.Add(crossover1);
            comboBox2.Items.Add(crossover2);
            comboBox3.Items.Add(selector1);
 //           comboBox3.Items.Add(selector2);
            comboBox3.Items.Add(selector3);

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
        }

        private const string initpopu1 = "Sluchaino";
        private const string initpopu2 = "Zhadniy algoritm";
        private const string crossover1 = "Odnotochechny";
        private const string crossover2 = "Dvutochechny";
        private const string selector1 = "Elitist strategy";
        private const string selector2 = "Roulette Selector";
        private const string selector3 = "Tournament Selector";

        Graphics g;
        Pen p;

        double[] a; // min (left) values for args 
        double[] b; // max (right) values for args
        IFunction f;
        GA finder;
        int dim;
        double[] minArg;
        double minValue;

        private void SetDataToGen()
        {
            dim = f.getDim();
            // dlia kommivoyagera:
            a = new double[dim]; //{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
            b = new double[dim]; //{ 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15};

            for (int i = 0; i < dim; i++)
            {
                a[i] = 1;
                b[i] = dim;
            }

            /*
             postanovka zadachi
             * kakie operatory
             * experiment
             * 
             */


            //create a object Genetic Algoritm
            finder = new GA(a, b, Convert.ToInt32(generations.Text), Convert.ToInt32(populationSize.Text));

            IGeneticOperator InitPopuAlgoritm = null;
            IGeneticOperator CrossoverAlgoritm = null;
            IGeneticOperator SelectorAlgoritm = null;

            switch (comboBox1.Text)
            {
                case initpopu1:
                    InitPopuAlgoritm = new InitPopu(dim, Convert.ToInt32(populationSize.Text), a, b);
                    break;
                case initpopu2:
                    InitPopuAlgoritm = new ZhadnyInitPopu(dim, Convert.ToInt32(populationSize.Text), a, b);
                    break;
            }

            switch (comboBox2.Text)
            {
                case crossover1:
                    CrossoverAlgoritm = new Crossover(Convert.ToInt32(crossingRate.Text));
                    break;
                case crossover2:
                    CrossoverAlgoritm = new TwoPointCrossover(Convert.ToInt32(crossingRate.Text));
                    break;
            }

            switch (comboBox3.Text)
            {
                case selector1:
                    SelectorAlgoritm = new Selector(Convert.ToInt32(populationSize.Text));
                    break;
                case selector2:
                    SelectorAlgoritm = new RouletteSelector(Convert.ToInt32(populationSize.Text));
                    break;
                case selector3:
                    SelectorAlgoritm = new TournamentSelector(Convert.ToInt32(populationSize.Text));
                    break;
            }

            //set genetic operators
            finder.setInitOperator(InitPopuAlgoritm);       // algoritm dlia sozdania nachalnoi populiatsii
            finder.setOperators(CrossoverAlgoritm,
                                new DiscreteMutation(Convert.ToInt32(mutationRate.Text), a, b),
                                SelectorAlgoritm
                          );
        }



        private void RunButton_Click(object sender, EventArgs e)
        {
            SetDataToGen();

            double[] optArg = finder.FindMinArg(f); //////// zdes proisxodit poisk reshenia
            minArg = optArg;
            minValue = f.getValue(minArg);

            minArgTextBox.Text = "";
            foreach (var v in optArg)
            {
                minArgTextBox.Text += Math.Round(v, 2).ToString() + "; ";
            }
            minValueTextBox.Text = Math.Round(f.getValue(optArg), 2).ToString();       
        }




        OpenFileDialog openf = new OpenFileDialog();
        IMatrix matrix;

        // load the matrix file
    //    private List<string> linesList = new List<string>();
        private void button1_Click(object sender, EventArgs e)
        {

            if (openf.ShowDialog() == DialogResult.OK)
            {
                string[] lines = System.IO.File.ReadAllLines(@openf.FileName);
                string[] words = new string[lines.Length];
                matrix = new SparseMatrix(lines.Length, lines.Length);
                Console.WriteLine(openf.FileName);
                for (int i = 0; i < lines.Length; i++)
                {
                    words = lines[i].Split(' ');
                    for (int j = 0; j < lines.Length; j++)
                    {
                        string pios = words[j].Replace(".", ",");
                        double temp = Convert.ToDouble(pios);
                        matrix.writeM(i, j, temp);
                    }
                }
            }

            f = new MatrixFunction(matrix);

            for (int i = 0; i < f.getDim(); i++)
            {
                for (int j = 0; j < f.getDim(); j++)
                {
                    Console.Write(matrix.readM(i, j) + " ");
                }
                Console.WriteLine();
            }

        }

        private void MyForm_Load(object sender, EventArgs e)
        {

        }
    }
}
