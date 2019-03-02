using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kasatelnaya
{

    public partial class Form1 : Form
    {

        bool CreateMaPoint = false;
        float dX = 0;
        float dY = 0;
        public List<List<MaPoint>> mahes = new List<List<MaPoint>>();
        MaPoint[,] B;
        float [,] U;
        float[,] Ut;
        float [,] N;
        //public List<MaPoint> mahs = new List<MaPoint>();

        public struct MaPoint
        {
            public float x, y, z,num;
            public bool isIClicked;

            public MaPoint(float p1, float p2, bool clicker)
            {
                x = p1;
                y = p2;
                z = 0;
                num = 0;
                isIClicked = clicker;
            }

            public MaPoint(float p1, float p2, float p3)
            {
                x = p1;
                y = p2;
                z = p3;
                num = 0;
                isIClicked = false;
            }

            public static MaPoint operator +(MaPoint a, MaPoint b)
            {
                MaPoint c = new MaPoint(a.x + b.x, a.y + b.y, a.z + b.z);
                return c;
            }
            public static MaPoint operator -(MaPoint a, MaPoint b)
            {
                MaPoint c = new MaPoint(a.x - b.x, a.y - b.y, a.z - b.z);
                return c;
            }

            public static MaPoint operator *(float s, MaPoint a)
            {
                MaPoint c = new MaPoint(a.x *s, a.y *s, a.z *s);
                return c;
            }

            public static MaPoint operator *(MaPoint a, float s)
            {
                MaPoint c = new MaPoint(a.x * s, a.y * s, a.z * s);
                return c;
            }

            public static MaPoint operator *(MaPoint a, MaPoint b)
            {
                MaPoint c = new MaPoint(a.x * b.x, a.y * b.y, a.z * b.z);
                return c;
            }
        };

        static MaPoint[,] Mult(MaPoint[,] a, float[,] b)
        {
            if (a.GetLength(1) != b.GetLength(0)) throw new Exception("Матрицы нельзя перемножить");
            MaPoint[,] r = new MaPoint[a.GetLength(0), b.GetLength(1)];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    for (int k = 0; k < b.GetLength(0); k++)
                    {
                        r[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return r;
        }

        static MaPoint[,] Mult(float[,] a, MaPoint[,] b)
        {
            if (a.GetLength(1) != b.GetLength(0)) throw new Exception("Матрицы нельзя перемножить");
            MaPoint[,] r = new MaPoint[a.GetLength(0), b.GetLength(1)];  
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    for (int k = 0; k < b.GetLength(0); k++)
                    {
                        r[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return r;
        }

        static float[,] newU (float i)
        {
           float [,] U = new float[,] { {(float)Math.Pow(i, 3), (float)Math.Pow(i, 2), i, 1}};

            return U;
        }

        static float[,] newUt(float i)
        {
            float[,] Ut = new float[,] { { (float)Math.Pow(i, 3) }, { (float)Math.Pow(i, 2) }, { i }, { 1 } };

            return Ut;
        }

        public Form1()
        {
       
            B = new MaPoint[,] { { new MaPoint(-15, 0, 15), new MaPoint(-15, 5, 5), new MaPoint(-15, 5, -5), new MaPoint(-15, 0, -15) },
                { new MaPoint(-5,5,15), new MaPoint(-5,5,5),new MaPoint(-5,5,-5),new MaPoint(-5,5,-15)},
                {new MaPoint(5,5,15),new MaPoint(5,5,5),new MaPoint(5,5,-5),new MaPoint(5,5,-15)},
                {new MaPoint(15,0,15),new MaPoint(15,5,5),new MaPoint(15,5,-5),new MaPoint(15,0,-15)}};
            N = new float[,] { { -1F, 3F, -3F, 1F }, { 3F, -6F, 3F, 0F }, { -3F, 3F, 0F, 0F }, { 1F, 0F, 0F, 0F } };
            U = new float[,] { { 0.125F, 0.25F, 0.5F, 1F} };
            Ut = new float[,] { { 0.125F }, { 0.25F} , { 0.5F}, { 1 } };
            InitializeComponent();

         
            

        }

        MaPoint getBezierPoint(List<MaPoint> points, int numPoints, float t)
        {
            List<MaPoint> tmp = new List<MaPoint>();
            tmp = points.GetRange(0, points.Count);

            int i = numPoints - 1;
            while (i > 0)
            {
                for (int k = 0; k < i; k++)
                    tmp[k] = tmp[k] + t * (tmp[k + 1] - tmp[k]);
                i--;
            }
            return tmp[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mahes.Clear();
            pictureBox1.Invalidate();
        }


       

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Pen Ospen = new Pen(Color.Black, 1);
            Pen Linecpen = new Pen(Color.Gray, 2);
            Pen Redpen = new Pen(Color.Red, 3);
            SolidBrush brush = new SolidBrush(Color.Blue);
            SolidBrush Linebrush = new SolidBrush(Color.Green);
        
           
            e.Graphics.DrawLine(Ospen, 551, 0, 551, 801);
            e.Graphics.DrawLine(Ospen, 0, 319, 1319, 319);
           // label2.Text = B[1, 2].z.ToString();
            
            for (float i = 0; i < 1; i += (float)0.025)
                for (float j = 0; j < 1; j += (float)0.025)
                    if (B.Length > 0)
            {
                        MaPoint[,] P = Mult(Mult(newU(i), Mult(N, Mult(B, N))), newUt(j));
                        e.Graphics.FillEllipse(Linebrush, 10*P[0,0].x - 1 +550, 10*P[0, 0].y - 1 + 318, 3, 3);
            }

            for (int i = 0; i < B.GetLength(0); i++)
                for (int j = 0; j < B.GetLength(1); j++)
                {
                    if (i != B.GetLength(0) - 1)
                        e.Graphics.DrawLine(Linecpen, 10 * B[i, j].x + 550, 10 * B[i, j].y + 318, 10 * B[i + 1, j].x + 550, 10 * B[i + 1, j].y + 318);
                    if (j != B.GetLength(1) - 1)
                        e.Graphics.DrawLine(Linecpen, 10 * B[i, j].x + 550, 10 * B[i, j].y + 318, 10 * B[i, j + 1].x + 550, 10 * B[i, j + 1].y + 318);
                }


            for (int j = 0; j < B.GetLength(1); j++)
                for (int i = 0; i < B.GetLength(0); i++)
            {
                e.Graphics.FillEllipse(brush, 10*B[j,i].x - 5+550, 10*B[j,i].y - 5+318,10, 10);
            }

        }

      private void button4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.W)
            {
                for (int j = 0; j < B.GetLength(1); j++)
                    for (int i = 0; i < B.GetLength(0); i++)
                {
                    B[j,i] = new MaPoint(B[j,i].x, B[j, i].y * (float)Math.Cos(0.0872665) + B[j, i].z * (float)Math.Sin(-0.0872665),
                                                     -B[j, i].y * (float)Math.Sin(-0.0872665) + B[j, i].z * (float)Math.Cos(0.0872665));
                }
                pictureBox1.Invalidate();
            }
            if (e.KeyData == Keys.S)
            {
                for (int j = 0; j < B.GetLength(1); j++)
                    for (int i = 0; i < B.GetLength(0); i++)
                    {
                    B[j,i] = new MaPoint(B[j,i].x, B[j,i].y * (float)Math.Cos(0.0872665) + B[j,i].z * (float)Math.Sin(0.0872665),
                                                     -B[j,i].y * (float)Math.Sin(0.0872665) + B[j,i].z * (float)Math.Cos(0.0872665));
                }
                pictureBox1.Invalidate();
            }
            if (e.KeyData == Keys.D)
            {
                for(int j = 0; j < B.GetLength(1); j++)
                    for (int i = 0; i < B.GetLength(0); i++)
                {
                    B[j,i] = new MaPoint(B[j,i].x * (float)Math.Cos(0.0872665) + B[j,i].z * (float)Math.Sin(0.0872665), B[j,i].y,
                                                     -B[j,i].x * (float)Math.Sin(0.0872665) + B[j,i].z * (float)Math.Cos(0.0872665));
                }
                pictureBox1.Invalidate();
            }
            if (e.KeyData == Keys.A)
            {
                for(int j = 0; j < B.GetLength(1); j++)
                    for (int i = 0; i < B.GetLength(0); i++)
                {
                    B[j,i] = new MaPoint(B[j,i].x * (float)Math.Cos(-0.0872665) + B[j,i].z * (float)Math.Sin(-0.0872665), B[j,i].y,
                                                      -B[j,i].x * (float)Math.Sin(-0.0872665) + B[j,i].z * (float)Math.Cos(-0.0872665));
                }
                pictureBox1.Invalidate();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            B[0, 0].x = (float)numericUpDown1.Value;
            pictureBox1.Invalidate();
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            B[0, 1].x = (float)numericUpDown2.Value;
            pictureBox1.Invalidate();
        }
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            B[0, 2].x = (float)numericUpDown3.Value;
            pictureBox1.Invalidate();
        }
        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            B[0, 3].x = (float)numericUpDown4.Value;
            pictureBox1.Invalidate();
        }
        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            B[1, 0].x = (float)numericUpDown5.Value;
            pictureBox1.Invalidate();
        }
        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            B[1, 1].x = (float)numericUpDown6.Value;
            pictureBox1.Invalidate();
        }
        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            B[1, 2].x = (float)numericUpDown7.Value;
            pictureBox1.Invalidate();
        }
        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            B[1, 3].x = (float)numericUpDown8.Value;
            pictureBox1.Invalidate();
        }
        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            B[2, 0].x = (float)numericUpDown9.Value;
            pictureBox1.Invalidate();
        }
        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            B[2, 1].x = (float)numericUpDown10.Value;
            pictureBox1.Invalidate();
        }
        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            B[2, 2].x = (float)numericUpDown11.Value;
            pictureBox1.Invalidate();
        }
        private void numericUpDown12_ValueChanged(object sender, EventArgs e)
        {
            B[2, 3].x = (float)numericUpDown12.Value;
            pictureBox1.Invalidate();
        }
        private void numericUpDown13_ValueChanged(object sender, EventArgs e)
        {
            B[3, 0].x = (float)numericUpDown13.Value;
            pictureBox1.Invalidate();
        }
        private void numericUpDown14_ValueChanged(object sender, EventArgs e)
        {
            B[3, 1].x = (float)numericUpDown14.Value;
            pictureBox1.Invalidate();
        }
        private void numericUpDown15_ValueChanged(object sender, EventArgs e)
        {
            B[3, 2].x = (float)numericUpDown15.Value;
            pictureBox1.Invalidate();
        }
        private void numericUpDown16_ValueChanged(object sender, EventArgs e)
        {
            B[3, 3].x = (float)numericUpDown16.Value;
            pictureBox1.Invalidate();
        }
        //// //// /////
        private void numericUpDown17_ValueChanged(object sender, EventArgs e)
        {
            B[0, 0].y = (float)numericUpDown17.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown18_ValueChanged(object sender, EventArgs e)
        {
            B[0, 1].y = (float)numericUpDown18.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown19_ValueChanged(object sender, EventArgs e)
        {
            B[0, 2].y = (float)numericUpDown19.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown20_ValueChanged(object sender, EventArgs e)
        {
            B[0, 3].y = (float)numericUpDown20.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown21_ValueChanged(object sender, EventArgs e)
        {
            B[1, 0].y = (float)numericUpDown21.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown22_ValueChanged(object sender, EventArgs e)
        {
            B[1, 1].y = (float)numericUpDown22.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown23_ValueChanged(object sender, EventArgs e)
        {
            B[1, 2].y = (float)numericUpDown23.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown24_ValueChanged(object sender, EventArgs e)
        {
            B[1, 3].y = (float)numericUpDown24.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown25_ValueChanged(object sender, EventArgs e)
        {
            B[2,0].y = (float)numericUpDown25.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown26_ValueChanged(object sender, EventArgs e)
        {
            B[2, 1].y = (float)numericUpDown26.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown27_ValueChanged(object sender, EventArgs e)
        {
            B[2, 2].y = (float)numericUpDown27.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown28_ValueChanged(object sender, EventArgs e)
        {
            B[2, 3].y = (float)numericUpDown28.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown29_ValueChanged(object sender, EventArgs e)
        {
            B[3, 0].y = (float)numericUpDown29.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown30_ValueChanged(object sender, EventArgs e)
        {
            B[3, 1].y = (float)numericUpDown30.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown31_ValueChanged(object sender, EventArgs e)
        {
            B[3, 2].y = (float)numericUpDown31.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown32_ValueChanged(object sender, EventArgs e)
        {
            B[3, 3].y = (float)numericUpDown32.Value;
            pictureBox1.Invalidate();
        }
        /////////////////////
        private void numericUpDown33_ValueChanged(object sender, EventArgs e)
        {
            B[3, 3].z = (float)numericUpDown33.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown34_ValueChanged(object sender, EventArgs e)
        {
            B[3, 2].z = (float)numericUpDown34.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown35_ValueChanged(object sender, EventArgs e)
        {
            B[3, 1].z = (float)numericUpDown35.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown36_ValueChanged(object sender, EventArgs e)
        {
            B[3, 0].z = (float)numericUpDown36.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown37_ValueChanged(object sender, EventArgs e)
        {
            B[2, 3].z = (float)numericUpDown37.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown38_ValueChanged(object sender, EventArgs e)
        {
            B[2, 2].z = (float)numericUpDown38.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown39_ValueChanged(object sender, EventArgs e)
        {
            B[2, 1].z = (float)numericUpDown39.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown40_ValueChanged(object sender, EventArgs e)
        {
            B[2, 0].z = (float)numericUpDown40.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown41_ValueChanged(object sender, EventArgs e)
        {
            B[1, 3].z = (float)numericUpDown41.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown42_ValueChanged(object sender, EventArgs e)
        {
            B[1, 2].z = (float)numericUpDown42.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown43_ValueChanged(object sender, EventArgs e)
        {
            B[1, 1].z = (float)numericUpDown43.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown44_ValueChanged(object sender, EventArgs e)
        {
            B[1, 0].z = (float)numericUpDown44.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown45_ValueChanged(object sender, EventArgs e)
        {
            B[0, 3].z = (float)numericUpDown45.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown46_ValueChanged(object sender, EventArgs e)
        {
            B[0, 2].z = (float)numericUpDown46.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown47_ValueChanged(object sender, EventArgs e)
        {
            B[0, 1].z = (float)numericUpDown47.Value;
            pictureBox1.Invalidate();
        }

        private void numericUpDown48_ValueChanged(object sender, EventArgs e)
        {
            B[0, 0].z = (float)numericUpDown48.Value;
            pictureBox1.Invalidate();
        }

    
    }
}
