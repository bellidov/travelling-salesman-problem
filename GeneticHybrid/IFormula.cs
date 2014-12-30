using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticHybrid
{
    public interface IFormula
    {
        double eval(double[] x);
        int getDim();
        List<string> getArgs();
    }

    public class formula : IFormula
    {
        private string exp; // exp - expresion, vyrazhenie, kotoroe nuzhno vychisliat
        private List<string> myargs = new List<string>(); // spisok naimenovanij argumentov
        private double[] args; // chislovie argumenti
        private string[] operators = { "+", "-", "*", "/", "^", "sin", "cos"};

        public formula(string exp)  // formula: ex. x1+x2
        {
            this.exp = exp;
            //// poluchayu iz vyrazhenia vse peremennie ////
            bool flag = false; // true - nachalas peremennay s bukvi
            int count = 0;
            int start = 0, len = 1;
            for (int i = 0; i < this.exp.Length; i++)
            {
                if (char.IsLetter(this.exp[i]) && !flag)
                {
                    count++;
                    start = i;
                    flag = true;
                }

                if (flag && this.exp[i] == '(')
                {
                    len = i - start;
                    if (!myargs.Contains(this.exp.Substring(start, len)))
                        myargs.Add(this.exp.Substring(start, len));
                    flag = false;
                }

                if (flag && (isOperator(this.exp[i]) || this.exp[i] == ')'))
                {
                    len = i - start;
                    if (!myargs.Contains(this.exp.Substring(start, len)))
                        myargs.Add(this.exp.Substring(start, len));
                    flag = false;
                }
                if (flag && i == this.exp.Length - 1 && this.exp[i] != ')')
                {
                    len = i - start + 1;
                    if (!myargs.Contains(this.exp.Substring(start, len)))
                        myargs.Add(this.exp.Substring(start, len));
                    flag = false;
                }
            }

            myargs.RemoveAll(isOperator); // ubirayu vse vozmozhnie funktsii, ostavliayu tolko peremennie
            myargs.Sort();
        }

        private bool isOperator(string s)
        {
            return operators.Contains(s);
        }

        public double eval(double[] x)
        {
            this.args = x; // beru chislovie argumenty
            double result = getValue(this.exp); // vyzivaetsa rekursivnaya funktsia getValue
            return result;
        }

        public List<string> getArgs()
        {
            return myargs; // vychislialsa v konstruktore, eto naimenovanij peremennix
        }

        public int getDim()
        {
            return getArgs().Count; // razmernost vxodnogo argumenta/funktsii
        }

        ///////////////
        // rekursivnaya funktsia dlia vychislenia znachenia vyrazhenia:
        ///////////////

        private double getValue(string exp)
        {
            exp = raskritSkobki(exp); // rakryvaet lishnie skobki

            if (exp.StartsWith("-(") && LenSkobok(exp.Substring(1)) == exp.Length - 1)
            {
                return -1 * getValue(exp.Substring(2, exp.Length - 3));
            }

            if (exp.StartsWith("sin(") && LenSkobok(exp.Substring(3)) == exp.Length-3)
            {
                return Math.Sin(getValue(exp.Substring(4, exp.Length - 5)));
            }

            if (exp.StartsWith("cos(") && LenSkobok(exp.Substring(3)) == exp.Length - 3)
            {
                return Math.Cos(getValue(exp.Substring(4, exp.Length - 5)));
            }


            if (isNumber(exp)) // esli vyrazhenie prostoe, t.e. soderzhit tolko chislo
            {
                return Convert.ToDouble(exp); // to prevrashayu ego v double
            }
            else if (myargs.Contains(exp)) // esli dannoe vrazhenie yavliaetsa peremennoi
            {
                return this.args[myargs.IndexOf(exp)]; // to vozvrashayu sootvetstvuyushe znachenie iz massiva
            }
            else
            {
                List<string> op = partition(exp); // inache, razbivayu na 2 vyrazhenia + operator

                if (op[op.Count - 1].Equals("+")) // i vychisliayu
                    return getValue(op[0]) + getValue(op[1]);
                else if (op[op.Count - 1].Equals("*"))
                    return getValue(op[0]) * getValue(op[1]);
                else if (op[op.Count - 1].Equals("-"))
                    return getValue(op[0]) - getValue(op[1]);
                else if (op[op.Count - 1].Equals("/"))
                    return getValue(op[0]) / getValue(op[1]);
                else if (op[op.Count - 1].Equals("^"))
                    return Math.Pow(getValue(op[0]), getValue(op[1]));
                else return 0;
            }
        }

        // razdeliaet vyrazhenie na 3 chasti: 2 vyrazhenia i 1 operator
        private List<string> partition(string exp)
        {
            List<string> parts = new List<string>();
            string operat = null;
            string temp = null;

            int i = 0, j = 0, p = 0, q = exp.Length - 1, count = 0;
            int skobki = 0;
            while (count < exp.Length - 1)
            {
                bool tempBool = exp[count].Equals('-');
                if (count == 0 && tempBool) // na sluchai vyrazhenij, kotorie nachinayutsa s minusa
                {
                    if (isBlock(exp.Substring(1, exp.Length - 1)))
                    {
                        parts.Add("0");
                        parts.Add(exp.Substring(1, exp.Length - 1));
                        parts.Add("-");

                        return parts;
                    }
                    else
                    {
                        count++;
                        continue;
                    }
                }

                if (exp[count] == '(') // esli nachinaetsa skobka 
                {
                    count += LenSkobok(exp.Substring(count)); // to propuskayu ee
                    if (count >= exp.Length) break;  //proveriayu, ne vyshel li ya s massiva
                }

                // proveriau, ne nachinaetsa li vyrazhenie s "sin", "cos", etc
                if (exp.Substring(count).StartsWith("sin") || exp.Substring(count).StartsWith("cos"))
                {
                    count += (3 + LenSkobok(exp.Substring(count+3))); // to propuskayu ee
                    if (count >= exp.Length) break;  //proveriayu, ne vyshel li ya s massiva
                }

                if (isOperator(exp[count]) && skobki == 0) //
                {
                    temp = new string(new char[] { exp[count] });

                    if (operat == null)
                    {
                        operat = temp;
                    }

                    if (temp.Equals("^") && !operat.Equals("*") && !operat.Equals("/") && !operat.Equals("-"))
                    {
                        operat = new string(new char[] { exp[count] });
                        j = count - 1;
                        p = count + 1;
                        count++;
                        continue;
                    }

                    if ((temp.Equals("*") || temp.Equals("/")) && (!operat.Equals("-")))
                    {
                        operat = new string(new char[] { exp[count] });
                        j = count - 1;
                        p = count + 1;
                        count++;
                        continue;
                    }
                    else if (temp.Equals("-"))
                    {
                        operat = new string(new char[] { exp[count] });
                        j = count - 1;
                        p = count + 1;
                        count++;
                        continue;
                    }
                    else if (temp.Equals("+") || temp.Equals("-"))
                    {
                        operat = new string(new char[] { exp[count] });
                        j = count - 1;
                        p = count + 1;
                        break; 
                    }
                  //  else if()

                    count++;
                }
                else
                    count++;
            }
            parts.Add(exp.Substring(i, j - i + 1));
            parts.Add(exp.Substring(p, q - p + 1));
            parts.Add(operat);

            return parts;
        }

        //////////////////////////////////////
        //nizhe prosto vspomogatelnie metodi//
        //////////////////////////////////////

        private bool isOperator(char opera)
        {
            if (opera == '+' || opera == '-' || opera == '*' || opera == '/' || opera == '^')
                return true;
            else return false;
        }

        private bool isNumber(string exp) // proveriaet, yavliaetsa li vyrazhenie chislom
        {
            bool result = true;
            int i = 0;
            while (i < exp.Length)
            {
                if (i == 0 && exp[i].Equals('-'))
                {
                    i++;
                    continue;
                }
                if (!char.IsDigit(exp[i]) && exp[i] != ',')
                    return false;
                i++;
            }
            return result;
            //int res;
            //return Int32.TryParse(exp, out res);
        }

        //naprimer: a*b*c => true.  a*b+c => false
        private bool isBlock(string exp)
        {
            int count = 0;
            bool key = true;
            while (count < exp.Length)
            {
                if (exp[count] == '(') // esli nachinaetsa skobka 
                {
                    count += LenSkobok(exp.Substring(count)); // to propuskayu ee
                    if (count >= exp.Length) break;  //proveriayu, ne vyshel li ya s massiva
                }

                if (exp[count] == '+' || exp[count] == '-')
                {
                    key = false;
                    break;
                }

                count++;
            }
            return key;
        }

        private string raskritSkobki(string exp) // razkrivaet vse nenuzhnie skobki
        {
            if (exp[0] == '(' && exp[exp.Length - 1] == ')') // esli nuzhno - ubirayu skobki chtoby ne meshali
            {
                if (LenSkobok(exp) == exp.Length)
                {
                    char[] buff = new char[exp.Length - 2];
                    for (int i = 1; i < exp.Length - 1; i++)
                        buff[i - 1] = exp[i];
                    return raskritSkobki(new string(buff));//new string(buff); -- tut rekursia
                }
                else return exp;
            }
            else return exp;
        }

        private int LenSkobok(string exp) // dlina odnogo bloka, vnutri skobok
        {
            int count = 0, skobki = 0;
            while (count < exp.Length)
            {
                if (exp[count] == '(')
                    skobki++;
                else if (exp[count] == ')')
                    skobki--;
                if (skobki == 0)
                    break;
                count++;
            }
            return count + 1;
        }
    }
}
