using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace Graphmos
{
    public partial class Form1 : Form
    {
        public float LowX, HighX, LowY, HighY;
        public int sizeX, sizeY;
        int? oldX, oldY;
        int a = 0;
        string input = "";
        public const int IntervalLength = 20;
        public const int rate = 800;
        Font font = new Font("Times New Roman", 10.0f);

        private void MW(object sender, MouseEventArgs e)
        {

            float inc = (float)Math.Pow(2, (e.Delta / 120));
            if (HighX * inc - LowX * inc != 0 && HighY * inc - LowY * inc != 0)
            {
                if (!(HighX * inc > float.MaxValue / 10 || HighY * inc > float.MaxValue / 10 || LowX * inc < float.MinValue / 10 || LowY * inc < float.MinValue / 10)
                    && !(HighX * inc - LowX * inc < 1E-10 || HighY * inc - LowY * inc < 1E-10))
                {
                    LowX -= (HighX - LowX) * (inc - 1) / 2;
                    LowY -= (HighY - LowY) * (inc - 1) / 2;
                    HighY += (HighY - LowY) * (inc - 1) / 2;
                    HighX += (HighX - LowX) * (inc - 1) / 2;
                    Trans = new Translator() { Lx = LowX, Ly = LowY, Tx = HighX, Ty = HighY, h = sizeY, w = sizeX };
                    GraphPanel.Invalidate();
                }
            }
        }
        private void Hover(object sender, EventArgs e)
        {
            GraphPanel.Focus();
        }
        private void EquationIO_TextChanged(object sender, EventArgs e)
        {
            input = EquationIO.Text;
            GraphPanel.Invalidate();

        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            a = trackBar1.Value;
            GraphPanel.Invalidate();
        }
        private void Move(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (oldX == null)
                {
                    oldX = e.X;
                    oldY = e.Y;
                }
                else
                {
                    //Move Along based on distance in IS
                    PointIS trans = PointIS.FromPoint(new Point(e.X - (int)oldX, (e.Y - (int)oldY)), Trans);
                    if (!(float.IsInfinity(trans.X) || float.IsInfinity(trans.Y)))
                    {
                        if (!(HighX + trans.X > float.MaxValue / 10 || HighY + trans.Y > float.MaxValue / 10 || LowX - trans.X < float.MinValue / 10 || LowY - trans.Y < float.MinValue / 10))
                        {
                            LowX += trans.X;
                            HighX += trans.X;
                            LowY += trans.Y;
                            HighY += trans.Y;
                            oldY = e.Y;
                            oldX = e.X;
                            Trans = new Translator() { Lx = LowX, Ly = LowY, Tx = HighX, Ty = HighY, h = sizeY, w = sizeX };

                            GraphPanel.Invalidate();
                        }
                    }
                }
            }
            else
            {
                oldX = oldY = null;
            }
        }
        private void GraphPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            RenderAxis(g);
            foreach (var curve in input.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    var temp = new Curve(curve, a, LowX, HighX, LowY, HighY, rate, (float)sizeX / rate);
                    temp.Render(Pens.Red, g, Trans);
                }
                catch { continue; }
            }
            g.Dispose();
        }

        public Translator Trans;
        public Form1()
        {

            InitializeComponent();
            DoubleBuffered = true;
            LowX = LowY = -5;
            HighX = HighY = 5;
            sizeX = 800;
            sizeY = 600;
            DoubleBuffered = true;
            Trans = new Translator() { Lx = LowX, Ly = LowY, Tx = HighX, Ty = HighY, h = sizeY, w = sizeX };
            GraphPanel.Invalidate();
        }

        public void RenderAxis(Graphics G)
        {
            G.DrawRectangle(Pens.Black, 0, 0, sizeX - 1, sizeY - 1);
            float intervalX = (float)(HighX - LowX) / 10;
            for (float rep = LowX + (intervalX - LowX % intervalX); rep < HighX; rep += intervalX)
            {

                if (rep == 0)
                {
                    G.DrawString("0", font, Brushes.Black, new PointIS(rep, (IntervalLength * (float)(HighY - LowY) / (100 * sizeY))).ToPoint(Trans));
                    continue;
                }
                G.DrawLine(Pens.LightGray, new PointIS(rep, LowY).ToPoint(Trans), new PointIS(rep, HighY).ToPoint(Trans));
                G.DrawLine(Pens.Black, new PointIS(rep, (IntervalLength * (float)(HighY - LowY) / (2 * sizeY))).ToPoint(Trans),
                    new PointIS(rep, (-IntervalLength * (float)(HighY - LowY) / (2 * sizeY))).ToPoint(Trans));

                if (rep > 1000 || rep < -1000 || (rep < 0.001f && rep > -0.001f))
                    G.DrawString(rep.ToString("#.##e+00"), font, Brushes.Black, new PointIS(rep, (IntervalLength * (float)(HighY - LowY) / (100 * sizeY))).ToPoint(Trans));
                else
                    G.DrawString(rep.ToString("####.####"), font, Brushes.Black, new PointIS(rep, (IntervalLength * (float)(HighY - LowY) / (100 * sizeY))).ToPoint(Trans));

            }
            float intervalY = (float)(HighY - LowY) / 10;
            for (float rep = LowY + (intervalY - LowY % intervalY); rep < HighY; rep += intervalY)
            {
                if (rep == 0)
                {
                    G.DrawLine(Pens.Black, new PointIS((IntervalLength * (float)(HighX - LowX) / (2 * sizeX)), rep).ToPoint(Trans),
                  new PointIS((-IntervalLength * (float)(HighX - LowX) / (2 * sizeX)), rep).ToPoint(Trans));
                    continue;
                }
                G.DrawLine(Pens.LightGray, new PointIS(LowX, rep).ToPoint(Trans), new PointIS(HighX, rep).ToPoint(Trans));
                G.DrawLine(Pens.Black, new PointIS((IntervalLength * (float)(HighX - LowX) / (2 * sizeX)), rep).ToPoint(Trans),
                  new PointIS((-IntervalLength * (float)(HighX - LowX) / (2 * sizeX)), rep).ToPoint(Trans));

                if (rep > 1000 || rep < -1000 || (rep < 0.001f && rep > -0.001f))
                    G.DrawString(rep.ToString("#.##E+00"), font, Brushes.Black, new PointIS((IntervalLength * (float)(HighX - LowX) / (2 * sizeX)), rep).ToPoint(Trans));
                else
                    G.DrawString(rep.ToString("####.####"), font, Brushes.Black, new PointIS((IntervalLength * (float)(HighX - LowX) / (2 * sizeX)), rep).ToPoint(Trans));

            }
            G.DrawLine(Pens.Black, new PointIS(LowX, 0).ToPoint(Trans), new PointIS(HighX, 0).ToPoint(Trans));
            G.DrawLine(Pens.Black, new PointIS(0, LowY).ToPoint(Trans), new PointIS(0, HighY).ToPoint(Trans));

        }
    }
    public class Curve
    {
        BinaryTree Tree;
        List<PointIS> Items;
        float sRate;
        public Curve(string exp, int A, float minx, float maxx, float miny, float maxy, float rate, float sRate)
        {
            this.sRate = sRate;
            Tree = Converter.InfixToTree(exp);
            Tree = Tree.ApplyCompound(new Mapping() { Data = new List<Mapping.Map>() { new Mapping.Map("a", A) } });
            var variables = Tree.FindAlgebraicVariables();
            Items = new List<PointIS>();
            if (variables.Count == 0 || (variables.Contains("x") && variables.Count == 1))
            {
                float rRate = (float)(maxx - minx) / rate;
                for (float x = minx; x < maxx; x += rRate)
                {
                    try
                    {
                        float v = (float)Tree.ApplyMappingAndEvaluate(new Mapping() { Data = new List<Mapping.Map>() { new Mapping.Map("x", x) } });
                        if (v.Equals(Single.NaN) || float.IsInfinity(v)) continue;
                        Items.Add(new PointIS(x, v)); //ADD POINT
                    }
                    catch { }
                }
            }
            else throw new Exception();
        }
        public void Render(Pen P, Graphics g, Translator T)
        {
            List<Point> x = new List<Point>();
            int i = 0;
            int count = 0;

            while (i != Items.Count)
            {
                var inspect = Items[i].ToPoint(T);

                if (count == 0)
                {
                    if (inspect.Y > 600 || inspect.Y < -0) { i++; continue; }
                    x = new List<Point>();
                    if (inspect.X > 2 && inspect.X < 798)
                    {
                        if (inspect.Y < 150) x.Add(new Point(inspect.X, 0));
                        else if (inspect.Y > 450) x.Add(new Point(inspect.X, 600));
                    }
                    x.Add(inspect);
                }
                else
                {
                    if ((inspect.Y > 600 || inspect.Y < -0) || (inspect.X - sRate - x[x.Count - 1].X) / inspect.X > 1f || (inspect.X - sRate - x[x.Count - 1].X) / inspect.X < -1f)
                    {
                        if (x.Count == 1)
                        {
                            g.DrawRectangle(P, x[x.Count - 1].X, x[x.Count - 1].Y, 1, 1);
                        }
                        else if (x.Count == 2)
                        {
                            g.DrawLine(P, x[x.Count - 1].X, x[x.Count - 1].Y, x[x.Count - 2].X, x[x.Count - 2].Y);
                        }
                        else
                        {
                            g.DrawLines(P, x.ToArray());
                        }
                        count = 0;
                        continue;
                    }


                }
                x.Add(inspect);
                i++;
                count++;

            }
            if (x.Count == 0) ;
            else if (x.Count == 1)
            {
                g.DrawRectangle(P, x[x.Count - 1].X, x[x.Count - 1].Y, 1, 1);
            }
            else if (x.Count == 2)
            {
                g.DrawLine(P, x[x.Count - 1].X, x[x.Count - 1].Y, x[x.Count - 2].X, x[x.Count - 2].Y);
            }
            else
            {
                var inspect = x[x.Count - 1];
                if (inspect.Y < 150) x.Add(new Point(inspect.X, 0));
                else if (inspect.Y > 450) x.Add(new Point(inspect.X, 600));
                g.DrawLines(P, x.ToArray());
            }

        }
    }
    public class PointIS
    {
        public float X, Y;
        public PointIS(float x, float y)
        {
            X = x;
            Y = y;

        }
        public static PointIS FromPoint(Point p, Translator t)
        {
            return new PointIS((p.X) * (float)(t.Tx - t.Lx) / t.w, -(float)(p.Y * (t.Ty - t.Ly)) / t.h);
        }
        public Point ToPoint(float Tx, float Ty, float Lx, float Ly, int h, int w)
        {

            return new Point((int)((X - Lx) * (w) / (Tx - Lx)), (int)((Ty - Y) * (h) / (float)(Ty - Ly)));
        }
        public Point ToPoint(Translator t)
        {
            return ToPoint(t.Tx, t.Ty, t.Lx, t.Ly, t.h, t.w);
        }
        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
    public class Translator
    {
        public float Tx, Ty, Lx, Ly;
        public int h, w;
    }
}
