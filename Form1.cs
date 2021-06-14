using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        bool G_IsUsingLimint = false;
        double[] G_Scaling_Y = new double[2];
        List<CalculatedPoint> LB_Results = new List<CalculatedPoint>();
        public Form1()
        {
            InitializeComponent();
            changeLimitsTBEnable();
            G_Scaling_Y[0] = 0;
            G_Scaling_Y[0] = 80;
        }
        void changeLimitsTBEnable()
        {
            TB_X1_MAX.Enabled = G_IsUsingLimint;
            TB_X2_MAX.Enabled = G_IsUsingLimint;
            TB_INTERVAL_X1.Enabled = G_IsUsingLimint;
            TB_INTERVAL_X2.Enabled = G_IsUsingLimint;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            G_IsUsingLimint = IsUsingLimits.Checked;
            changeLimitsTBEnable();
        }

        private void B_CalculateY_Click(object sender, EventArgs e)
        {
            DGV_DataTable.Rows.Clear();
            List<CalculatedPoint> points = new List<CalculatedPoint>();
            LB_Results.Clear();

            double cb_X1 = convertTextToDouble(ref TB_X1);
            double cb_X2 = convertTextToDouble(ref TB_X2);

            if (G_IsUsingLimint && (cb_X1 != 0 || cb_X2 != 0))
            {
                TB_INTERVAL_X1.Text = TB_INTERVAL_X1.Text.Replace('.', ',');
                TB_INTERVAL_X2.Text = TB_INTERVAL_X2.Text.Replace('.', ',');

                double cb_INTERVAL_X1 = convertTextToDouble(ref TB_INTERVAL_X1);
                double cb_INTERVAL_X2 = convertTextToDouble(ref TB_INTERVAL_X2);
                double cb_X1_MAX = convertTextToDouble(ref TB_X1_MAX);
                double cb_X2_MAX = convertTextToDouble(ref TB_X2_MAX);

                for (double i_x1 = cb_X1, i_x2 = cb_X2;
                    (i_x1 < cb_X1_MAX || i_x2 < cb_X2_MAX);
                    i_x1 += cb_INTERVAL_X1, i_x2 += cb_INTERVAL_X2)
                {
                    points.Add(new CalculatedPoint(i_x1, i_x2));
                }
            }
            else
            {
                points.Add(new CalculatedPoint(cb_X1, cb_X2));
            }

            foreach (CalculatedPoint point in points)
            {
                if (!double.IsNaN(point.Y))
                {
                    G_Scaling_Y[0] = Math.Min(point.Y, G_Scaling_Y[0]);
                    G_Scaling_Y[1] = Math.Max(point.Y, G_Scaling_Y[1]);
                }
            }

            CH_Graphic.Series[0].Points.Clear();
            CH_Graphic.Series[1].Points.Clear();
            CH_Graphic.ChartAreas[0].AxisY.ScaleView.Zoom(G_Scaling_Y[0], G_Scaling_Y[1]);

            int counter = 0;
            foreach (CalculatedPoint point in points)
            {
                LB_Results.Add(point);

                DGV_DataTable.Rows.Add();
                for (int i = 0; i < 3; i++)
                {
                    DGV_DataTable.Rows[counter].Cells[i].Value = point.DataForDGV()[i];
                }
                counter++;

                CH_Graphic.Series[0].Points.AddXY(point.X1, point.Y);
                CH_Graphic.Series[1].Points.AddXY(point.X2, point.Y);
            }
        }
        private double convertTextToDouble(ref TextBox TB)
        {
            double output = 0;

            try
            {
                output = Convert.ToDouble(TB.Text);
            }
            catch
            {
                MessageBox.Show("Can`t convert " + TB.Name + ".Text to double.\n0 is result.");
            }

            return output;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

namespace WindowsFormsApp1
{
    public class CalculatedPoint
    {
        public double Y;
        public double X1;
        public double X2;

        public CalculatedPoint(double X1, double X2)
        {
            calculate_Y(X1, X2, ref Y);
            this.X1 = X1;
            this.X2 = X2;
        }
        public static double calculate_Y(double X1, double X2, ref double res_Y)
        {
            double Y = 0;
            double TopPart = 0;
            double BotPart = 1;

            TopPart = (Math.Cos(2 * X2) + (X1 / X2));


            BotPart = (16 * X1 * X2);


            Y = Math.Pow((TopPart / BotPart), 0.5);


            res_Y = Y;
            return Y;
        }
        public string[] DataForDGV()
        {
            return new string[] { X1.ToString("0.000"), X2.ToString("0.000"), Y.ToString("0.000") };
        }
        public override string ToString()
        {
            string output = string.Empty;

            output += "| Y: " + Y.ToString("0.00000000") + " |\t| " + "X1: " + X1.ToString("0.000") + " |\t| " + "X2: " + X2.ToString("0.000") + " |\n";

            return output;
        }
    }
}
