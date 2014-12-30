using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticHybrid
{
    public interface IDrawer
    {
        void drawLine(int x1, int y1, int x2, int y2);
        void drawPoint(int x, int y);
    }

    public class Drawer : IDrawer
    {
        private Graphics g;
        private Pen p;

        public Drawer(Graphics g, Pen p)
        {
            this.g = g;
            this.p = p;
        }

        public void drawLine(int x1, int y1, int x2, int y2)
        {
            g.DrawLine(p, x1, y1, x2, y2);
        }

        public void drawPoint(int x, int y)
        {
            SolidBrush b = new SolidBrush(Color.Blue);
            g.FillEllipse(b, x, y, 5, 5);
        }
    }
}
