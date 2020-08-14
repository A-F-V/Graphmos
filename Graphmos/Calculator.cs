using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
namespace Graphmos
{
    public static class Converter
    {
        public static BinaryTree InfixToTree(string s)
        {
            s = s.FiltererNegative(new char[] { ' ' });
            BinaryTree output = new BinaryTree();
            output.Root = new BinaryTree.Node() { data = s, type = BinaryTree.NType.None };
            output.Root.InfixNode();
            return output;

        }

        public static BinaryTree.NType SearchforPossibility(string s, int startPos, out int final, out string Data)
        {
            final = startPos;
            #region Find Mode
            int bracketCount = 0;
            bool FunctionNameDone = false;
            bool Algebra = false;
            bool Bracket = false;
            bool Number = false;
            bool UnaryOperator = false;
            if (s[startPos].IsLetter())
            {
                if (s[startPos].ToString() == s[startPos].ToString().ToLower())
                {
                    Algebra = true;
                }
                else
                    UnaryOperator = true;
            }
            else if (s[startPos] > 47 && s[startPos] < 58)
            {
                Number = true;
            }
            else if (s[startPos] == '(')
            {
                Bracket = true;
                bracketCount++;
            }
            else
            {
                final = startPos + 1;
                Data = s[startPos].ToString();
                return BinaryTree.NType.Operator;
            }
            #endregion
            for (int pos = startPos + 1; pos < s.Length; pos++)
            {
                if (Bracket)
                {
                    if (s[pos] == ')') bracketCount--;
                    if (s[pos] == '(') bracketCount++;
                    if (bracketCount == 0)
                    {
                        final = pos + 1;
                        Data = s.Snip(startPos + 1, pos - 1);
                        return BinaryTree.NType.Bracket;
                    }
                }
                if (Number)
                {
                    if (!((s[pos] > 47 && s[pos] < 58) || s[pos] == '.'))
                    {
                        final = pos;
                        Data = s.Snip(startPos, pos - 1);
                        return BinaryTree.NType.Number;
                    }

                }
                if (Algebra)
                {
                    if (!((s[pos].IsLetter()) && s[pos].ToString() == s[pos].ToString().ToLower()))
                    {
                        final = pos;
                        Data = s.Snip(startPos, pos - 1);
                        return BinaryTree.NType.Algebra;
                    }

                }
                if (UnaryOperator)
                {
                    if (!FunctionNameDone)
                    {
                        if (!s[pos].IsLetter())
                        {
                            bracketCount++;
                            FunctionNameDone = true;
                        }
                    }
                    else
                    {
                        if (s[pos] == ')') bracketCount--;
                        if (s[pos] == '(') bracketCount++;
                        if (bracketCount == 0)
                        {
                            final = pos + 1;
                            Data = s.Snip(startPos, pos);
                            return BinaryTree.NType.UOperator;
                        }
                    }
                }

            }
            final = s.Length;
            Data = s.Snip(startPos, s.Length - 1);
            return Algebra ? BinaryTree.NType.Algebra : Number ? BinaryTree.NType.Number : BinaryTree.NType.None;
        }

    }
    public class BinaryTree
    {
        public BinaryTree Copy()
        {
            BinaryTree o = new BinaryTree();
            o.Root = Root.Copy();
            o.IsMapped = IsMapped;
            return o;
        }
        public enum NType { Number, Algebra, Operator, UOperator, Bracket, None, DOperator }
        public static char[] Operators = new char[] { '+', '-', '/', '*', '^' };
        public class Node
        {
            public Node Left = null;
            public Node Right = null;
            public string data = null;
            public NType type = NType.Number;
            public BinaryTree ToTree()
            {
                BinaryTree o = new BinaryTree();
                o.Root = this;
                return o;
            }
            public void InfixNode()
            {
                List<NType> Complexity = new List<NType>();
                int pos = 0;
                string temp = "";
                int minLevel = 5;
                while (pos != data.Length)
                {
                    NType t = Converter.SearchforPossibility(data, pos, out pos, out temp);
                    Complexity.Add(t);
                    if (t == NType.Operator)
                    {
                        if (temp == "+" || temp == "-") minLevel = 0;
                        if ((temp == "*" || temp == "/")) minLevel = minLevel > 1 ? 1 : minLevel;
                        if ((temp == "^")) minLevel = minLevel > 2 ? 2 : minLevel;
                    }


                }
                if (Complexity.Count == 1)
                {

                    if (Complexity[0] == NType.Number || Complexity[0] == NType.Algebra)
                    {
                        data = temp;
                        type = Complexity[0];
                    }
                    else if (Complexity[0] == NType.Bracket)
                    {
                        data = data.Snip(1, data.Length - 2);

                        this.InfixNode();
                    }
                    else
                    {
                        bool DOP = false;
                        string t = RemoveUOperator(data, out data);
                        int cutter = 0;
                        int bracketCount = 0;
                        for (int p = 0; p < t.Length; p++)
                        {
                            if (t[p] == '(') bracketCount++;
                            if (t[p] == ')') bracketCount--;
                            if (t[p] == ',' && bracketCount == 0)
                            {
                                cutter = p;
                                DOP = true;
                                break;
                            }
                        }
                        if (DOP)
                        {
                            type = NType.DOperator;
                            Left = new Node() { data = t.Snip(0, cutter - 1), type = NType.None };
                            Left.InfixNode();
                            Right = new Node() { data = t.Snip(cutter + 1, t.Length - 1), type = NType.None };
                            Right.InfixNode();
                        }
                        else
                        {
                            type = NType.UOperator;
                            Left = new Node() { data = t, type = NType.None };
                            Left.InfixNode();
                        }
                    }
                }
                else
                {
                    int bracketCount = 0;
                    bool done = false;
                    for (int npos = data.Length - 1; npos >= 0; npos--)
                    {
                        if (done) break;
                        if (data[npos] == '(') bracketCount++;
                        if (data[npos] == ')') bracketCount--;
                        if (bracketCount == 0 && Operators.Contains(data[npos]))
                        {
                            switch (data[npos])
                            {
                                case '+':
                                case '-':
                                    Left = new Node() { type = NType.None, data = data.Snip(0, npos - 1) };
                                    Right = new Node() { type = NType.None, data = data.Snip(npos + 1, data.Length - 1) };
                                    Left.InfixNode();
                                    Right.InfixNode();
                                    data = data[npos].ToString();
                                    type = NType.Operator;
                                    done = true;
                                    break;
                                case '/':
                                case '*':
                                    if (minLevel == 1)
                                    {
                                        Left = new Node() { type = NType.None, data = data.Snip(0, npos - 1) };
                                        Right = new Node() { type = NType.None, data = data.Snip(npos + 1, data.Length - 1) };
                                        Left.InfixNode();
                                        Right.InfixNode();
                                        data = data[npos].ToString();
                                        type = NType.Operator;
                                        done = true;

                                    }
                                    break;
                                case '^':
                                    if (minLevel == 2)
                                    {
                                        Left = new Node() { type = NType.None, data = data.Snip(0, npos - 1) };
                                        Right = new Node() { type = NType.None, data = data.Snip(npos + 1, data.Length - 1) };
                                        Left.InfixNode();
                                        Right.InfixNode();
                                        data = data[npos].ToString();
                                        type = NType.Operator;
                                        done = true;

                                    }
                                    break;
                            }
                        }
                    }
                }
                if (type == NType.None) throw new Exception("Invalid Input");

            }
            public double Evaluate()
            {
                if (type == NType.Number) return Double.Parse(data);
                else
                {
                    if (type == NType.Operator)
                    {
                        return EvaluateOperators(data, Left.Evaluate(), Right.Evaluate());
                    }
                    else if (type == NType.UOperator)
                    {
                        return EvaluateUOperators(data, Left.Evaluate());
                    }
                    else
                    {
                        return EvaluateDOperators(data, Left.ToTree(), Right.ToTree());
                    }
                }
            }
            public double EvaluateUOperators(string Operator, double number)
            {
                switch (Operator)
                {
                    case "MOD":
                        return Math.Abs(number);
                    case "LN":
                        return Math.Log(number);
                    case "SQRT":
                        return Math.Sqrt(number);
                    case "NEG":
                        return -number;
                    case "FACT":
                        return ((int)number).Factorial();
                    case "SIN":
                        return Math.Sin(number);
                    case "COS":
                        return Math.Cos(number);
                    case "CEIL":
                        return Math.Ceiling(number);
                    case "FLOOR":
                        return Math.Floor(number);

                    default:
                        throw new NotImplementedException();
                }
            }
            public double EvaluateOperators(string Operator, double a, double b)
            {
                switch (Operator)
                {
                    case "+":
                        return a + b;
                    case "-":
                        return a - b;
                    case "/":
                        return a / b;
                    case "*":
                        return a * b;
                    case "^":
                        return Math.Pow(a, b);
                }
                return 0;
            }
            public double EvaluateDOperators(string Operator, BinaryTree a, BinaryTree b)
            {
                switch (Operator)
                {
                    case "SUM":
                        double sum = 0;
                        double c = b.Evaluate();
                        for (int i = 0; i < c + 1; i++)
                        {
                            sum += a.ApplyMappingAndEvaluate(new Mapping() { Data = new List<Mapping.Map>() { new Mapping.Map("n", i) } });

                        }
                        return sum;
                    case "DIFN":
                        double x = b.Evaluate();
                        double dx = 0.00000000001;
                        double dy = a.ApplyMappingAndEvaluate(new Mapping() { Data = new List<Mapping.Map>() { new Mapping.Map("n", x + dx) } })
                            - a.ApplyMappingAndEvaluate(new Mapping() { Data = new List<Mapping.Map>() { new Mapping.Map("n", x) } });
                        return dy / dx;
                    case "LOG":
                        return Math.Log(a.Evaluate(), b.Evaluate());
                    case "CHOOSE":
                    case "PERM":
                        int n = (int)a.Evaluate();
                        int r = (int)b.Evaluate();
                        if (r < 0 || n <= 0 || n < r) throw new Exception();
                        if (Operator == "CHOOSE")
                        {
                            return NumericalManipulation.Choose(n, r);
                        }
                        return NumericalManipulation.Choose(n, r) * r.Factorial();

                        break;
                    default:
                        throw new NotImplementedException();

                }

            }
            public void ApplyMap(Mapping m)
            {

                if (type == NType.Algebra)
                {

                    bool match = false;
                    foreach (var item in m.Data)
                    {
                        if (item.point == data)
                        {
                            if (item.IsValue)
                            {
                                data = item.value.ToString();
                                type = NType.Number;
                            }

                            else
                            {
                                data = item.Expression.Root.data;
                                Left = item.Expression.Root.Left;
                                Right = item.Expression.Root.Right;
                                type = item.Expression.Root.type;
                            }
                            match = true;
                            break;

                        }
                    }
                    if (!match && data != "n") throw new Exception("Not Properly Mapped");

                }
                if (!((type == NType.DOperator) && (m.Data.Count == 1 && m.Data[0].point == "n")))
                {
                    if (Left != null)
                    {

                        Left.ApplyMap(m);
                    }
                    if (Right != null)
                    {

                        Right.ApplyMap(m);
                    }
                }
                else
                {
                    if (Right != null)
                    {
                        Right.ApplyMap(m);
                    }
                }
            }
            public void ApplyCompound(Mapping m)
            {

                if (type == NType.Algebra)
                {

                    foreach (var item in m.Data)
                    {
                        if (item.point == data)
                        {
                            if (item.IsValue)
                            {
                                data = item.value.ToString();
                                type = NType.Number;
                            }

                            else
                            {
                                data = item.Expression.Root.data;
                                Left = item.Expression.Root.Left;
                                Right = item.Expression.Root.Right;
                                type = item.Expression.Root.type;
                            }

                            break;

                        }
                    }


                }
                if (Left != null)
                {

                    Left.ApplyCompound(m);
                }
                if (Right != null)
                {

                    Right.ApplyCompound(m);
                }

            }
            public Node Copy()
            {
                return new Node() { data = data.Copy(), Left = Left == null ? null : Left.Copy(), Right = Right == null ? null : Right.Copy(), type = type };
            }
            private string RemoveUOperator(string s, out string old)
            {
                old = "";
                int pos = 0;
                while (true)
                {
                    if (!s[pos].IsLetter())
                    {
                        break;

                    }
                    else
                    {
                        old = old + s[pos];
                        pos++;
                    }

                }
                return s.Snip(pos + 1, s.Length - 2);
            }
            public List<string> FindAlgebra()
            {
                List<string> variables = new List<string>();

                List<string> vtemp;
                if (Left != null)
                {
                    vtemp = Left.FindAlgebra();
                    foreach (var item in vtemp)
                    {

                        variables.Add(item);
                    }
                }

                if (Right != null)
                {
                    vtemp = Right.FindAlgebra();
                    foreach (var item in vtemp)
                    {
                        variables.Add(item);
                    }
                }
                if (type == NType.Algebra && data != "n")
                {
                    variables.Add(data);
                    return variables;
                }


                return variables;
            }
        }
        private bool ExtractAlgebra(Node n)
        {
            bool good = n.type == NType.Algebra && n.data != "n";
            if (n.Left != null)
            {
                good = good | ExtractAlgebra(n.Left);
            }
            if (n.Right != null)
            {
                good = good | ExtractAlgebra(n.Right);
            }
            return good;
        }
        public bool IsAlgebraic
        {

            get
            {
                return ExtractAlgebra(Root);
            }
        }
        public bool IsMapped = false;
        public Node Root = null;
        public double Evaluate()
        {
            if (IsAlgebraic && !IsMapped) throw new Exception("Not Mapped");
            return Root.Copy().Evaluate();
        }
        public BinaryTree ApplyMappings(Mapping m)
        {

            BinaryTree x = Copy();
            x.IsMapped = true;
            x.Root.ApplyMap(m);
            return x;


        }
        public BinaryTree ApplyCompound(Mapping m)
        {
            BinaryTree x = Copy();
            x.IsMapped = true;
            x.Root.ApplyCompound(m);
            return x;
        }
        public double ApplyMappingAndEvaluate(Mapping m)
        {
            return ApplyMappings(m).Evaluate();
        }
        public List<string> FindAlgebraicVariables()
        {
            var temp = Root.FindAlgebra();
            List<string> output = new List<string>();
            foreach (var item in temp)
            {
                if (!output.Contains(item))
                    output.Add(item);
            }
            output.Sort();
            return output;
        }
        public void ApplyTestmapping()
        {
            if (IsAlgebraic)
            {
                List<string> list = FindAlgebraicVariables();
                Mapping m = new Mapping();
                foreach (var item in list)
                {
                    m.Add(new Mapping.Map(item, 0));
                }
                ApplyMappingAndEvaluate(m);
            }
        }

    }
    public class Mapping
    {
        public class Map
        {
            public Map(string p, double v)
            {
                point = p;
                value = v;
                IsValue = true;
            }
            public Map(string p, BinaryTree E)
            {
                Expression = E;
                point = p;
                IsValue = false;
            }
            public BinaryTree Expression;
            public bool IsValue;
            public string point;
            public double value;
        }
        public List<Map> Data = new List<Map>();
        public void Add(Map m)
        {
            Data.Add(m);
        }
    }


    public static class NumericalManipulation
    {
        public static int[] Primes(int n)
        {
            List<int> primes = new List<int>();
            int count = 0;
            int num = 2;
            while (count < n)
            {
                bool IsPrime = true;
                double sq = Math.Sqrt(num);
                foreach (var v in primes)
                {
                    if (num % v == 0)
                    {
                        IsPrime = false;
                        break;
                    }
                    if (sq < v) break;

                }
                if (IsPrime) { primes.Add(num); count++; }
                num++;
            }
            return primes.ToArray();
        }
        public static int[] PrimesUpto(int n)
        {
            List<int> primes = new List<int>();
            int num = 2;
            while (num <= n)
            {
                bool IsPrime = true;
                double sq = Math.Sqrt(num);
                foreach (var v in primes)
                {
                    if (num % v == 0)
                    {
                        IsPrime = false;
                        break;
                    }
                    if (sq < v) break;

                }
                if (IsPrime) { primes.Add(num); }
                num++;
            }
            return primes.ToArray();
        }
        public static int Factorial(this int n)
        {
            if (n % 1 != 0 && n < 0) throw new Exception();
            int outputFact = 1;
            if (n == 0) return 1;
            for (int i = 1; i < n + 1; i++)
            {
                outputFact *= i;
            }
            return outputFact;
        }
        public static int Choose(int n, int r)
        {
            List<List<int>> data = new List<List<int>>();
            data.Add(new List<int>() { 1 });
            for (int i = 1; i < n + 1; i++)
            {
                data.Add(new List<int>() { 1 });
                for (int pos = 0; pos < i - 1; pos++)
                {
                    data[i].Add(data[i - 1][pos] + data[i - 1][pos + 1]);
                }
                data[i].Add(1);
            }
            return data[n][r];
        }
        public static int BaseConverter(int a, int b, int num)
        {
            if (a > 10 || a < 2 || b > 10 || b < 2) return 0;
            return DenaryToA(b, AtoDenary(a, num));
        }
        public static int AtoDenary(int a, int num)
        {
            int power = 0;
            if (num == 0) return 0;
            int count = 0;
            while (num > 0)
            {
                count += Convert.ToInt32((num % 10) * Math.Pow(a, power++));
                num /= 10;
            }
            return count;
        }
        public static int DenaryToA(int a, int num)
        {
            List<int> newl = new List<int>();
            if (num == 0) return 0;
            while (num > 0)
            {
                newl.Add(num % a);
                num /= a;
            }
            newl.Reverse();
            return Convert.ToInt32(String.Join("", newl.ToArray()));

        }
    }
    public static class StringCalculation
    {
        public static int EnglishWordCount(string rawData)
        {
            char[] IllegalCharacters = new char[] {'\"',' ','\\','1','2','3','4','5','6','7','8','9','0',
                '!','.','£','$','%','^','&','*','(',')','{','}',',',':',';','@','#','~','?','/' };
            string[] EnglishLanguageWords = File.ReadAllLines(@"C:\Users\User\Documents\Visual Studio 2017\Projects\AFVLib\Core\source\EnglishWords.txt");
            for (int i = 0; i < EnglishLanguageWords.Length; i++)
            {
                EnglishLanguageWords[i] = EnglishLanguageWords[i].ToLower();
            }
            string[] newData = rawData.ToLower().Split(IllegalCharacters);

            int count = 0;
            foreach (var item in newData)
            {
                if (EnglishLanguageWords.Contains(item)) count++;
            }
            return count;
        }
    }
    public static class StringManipulation
    {

        /// <summary>
        /// Filters a string, removing any characters not in the sieve.
        /// </summary>
        /// <param name="data">original string you wish to filter</param>
        /// <param name="sieve">sieve containing all the characters which are allowed in the filtered.</param>
        /// <returns></returns>
        public static string Filterer(this string data, char[] sieve)
        {
            string newData = "";
            foreach (var chart in data)
            {
                if (sieve.Contains(chart)) newData += chart;
            }
            return newData;
        }
        /// <summary>
        /// Filters a string, removing any characters in the sieve.
        /// </summary>
        /// <param name="data">original string you wish to filter</param>
        /// <param name="sieve">sieve containing all the characters which are not allowed in the filtered.</param>
        /// <returns></returns>
        public static string FiltererNegative(this string data, char[] sieve)
        {
            string newData = "";
            foreach (var chart in data)
            {
                if (!sieve.Contains(chart)) newData += chart;
            }
            return newData;
        }
        public static string Snip(this string data, int start, int end)
        {
            string n = "";
            for (int pos = start; pos < end + 1; pos++)
            {
                n = n + data[pos];
            }
            return n;
        }
        public static bool ContainsLower(this string data)
        {
            foreach (var item in data)
            {
                if (((int)item > 96 && (int)item < 124)) return true;
            }
            return false;
        }
        public static string Copy(this string data)
        {
            string x = "";
            foreach (var item in data)
            {
                x = x + item;
            }
            return x;
        }
        /// <summary>
        /// Snips a string array from start to end, including start and end
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string[] Snip(this string[] data, int start, int end)
        {
            string[] output = new string[end - start + 1];
            for (int pos = start; pos < end + 1; pos++)
            {
                output[pos - start] = data[pos];
            }
            return output;
        }
    }
    public static class CharacterManipulation
    {
        public static bool IsLetter(this char x)
        {
            return ((int)x > 64 && (int)x < 91) || ((int)x > 96 && (int)x < 124);
        }
    }

}
