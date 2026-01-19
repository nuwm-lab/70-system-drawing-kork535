using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LabWork
{
    public class FunctionCalculator
    {
        public double XMin => -1.0;
        public double XMax => 2.3;
        public double Step => 0.7;

        public double CalculateY(double x) => (Math.Exp(2 * x) - 8) / (x + 3);

        public List<(double X, double Y)> GetPoints(double delta)
        {
            var points = new List<(double, double)>();
            for (double x = XMin; x <= XMax + 0.0001; x += delta)
                points.Add((x, CalculateY(x)));
            return points;
        }
    }

    public class MainForm : Form
    {
        private readonly FunctionCalculator _calc = new FunctionCalculator();

        public MainForm()
        {
            this.Text = "Варіант 8: Графік функції";
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.MinimumSize = new Size(500, 400);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int w = ClientSize.Width - 80;
            int h = ClientSize.Height - 80;
            if (w <= 0 || h <= 0) return;

            var plotPoints = _calc.GetPoints(0.02);
            double yMin = plotPoints.Min(p => p.Y);
            double yMax = plotPoints.Max(p => p.Y);

            float scaleX = (float)(w / (_calc.XMax - _calc.XMin));
            float scaleY = (float)(h / (yMax - yMin));

            PointF Map(double x, double y) => new PointF(
                (float)((x - _calc.XMin) * scaleX) + 40,
                (float)(ClientSize.Height - 40 - (y - yMin) * scaleY)
            );

            using (Pen p = new Pen(Color.Silver, 1))
            {
                PointF zero = Map(0, 0);
                g.DrawLine(p, 40, zero.Y, ClientSize.Width - 40, zero.Y);
                g.DrawLine(p, zero.X, 40, zero.X, ClientSize.Height - 40);
            }

            using (Pen p = new Pen(Color.Blue, 2))
            {
                for (int i = 0; i < plotPoints.Count - 1; i++)
                    g.DrawLine(p, Map(plotPoints[i].X, plotPoints[i].Y), Map(plotPoints[i+1].X, plotPoints[i+1].Y));
            }

            for (double x = _calc.XMin; x <= _calc.XMax + 0.01; x += _calc.Step)
            {
                PointF pt = Map(x, _calc.CalculateY(x));
                g.FillEllipse(Brushes.Red, pt.X - 3, pt.Y - 3, 6, 6);
            }
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}