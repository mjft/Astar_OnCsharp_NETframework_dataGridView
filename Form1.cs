using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;//StringArray



namespace A_star_Csharp_Test1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                this.dataGridView1.Rows.Add();
                this.dataGridView1.RowHeadersDefaultCellStyle.DataSourceNullValue = "1";
            }
        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)//设置行号
        {
            //显示在HeaderCell上
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                DataGridViewRow r = this.dataGridView1.Rows[i];
                r.HeaderCell.Value = string.Format("{0}", i + 1);
            }
            this.dataGridView1.Refresh();
        }

        /*public class Point
        {
            public int row;
            public int col;
            public String Came_From;
        }*/
        struct Point
        {
            public int row;
            public int col;
            public String Came_From;
        }

        private void button_SetStart_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.CurrentCell.RowIndex;
            int col = dataGridView1.CurrentCell.ColumnIndex;
            label1.Text = (row + 1).ToString();
            label2.Text = (col + 1).ToString();
            this.dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.FromName("Lime");//设置单元格颜色
            this.dataGridView1.Rows[row].Cells[col].Style.ForeColor = Color.Black;//设置字体颜色
            this.dataGridView1.Rows[row].Cells[col].Value = "1";//设置内容
        }

        private void button_SetEnding_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.CurrentCell.RowIndex;
            int col = dataGridView1.CurrentCell.ColumnIndex;
            label1.Text = (row + 1).ToString();
            label2.Text = (col + 1).ToString();
            this.dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.FromName("Red");//设置单元格颜色
            this.dataGridView1.Rows[row].Cells[col].Style.ForeColor = Color.Black;//设置字体颜色
        }

        private void button_SetBlock_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.CurrentCell.RowIndex;
            int col = dataGridView1.CurrentCell.ColumnIndex;
            label1.Text = (row + 1).ToString();
            label2.Text = (col + 1).ToString();
            this.dataGridView1.Rows[row].Cells[col].Style.BackColor = Color.FromName("Black");//设置单元格颜色
            this.dataGridView1.Rows[row].Cells[col].Style.ForeColor = Color.Black;//设置字体颜色
        }

        List<Point> Completed = new List<Point>();
        List<Point> Next = new List<Point>();

        private void button_Search_General_Click(object sender, EventArgs e)//普通方法
        {
            Point Start = new Point();
            Point Ending = new Point();
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    if (this.dataGridView1.Rows[i].Cells[j].Style.BackColor == Color.FromName("Lime"))
                    { Start.row = i; Start.col = j; }
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    if (this.dataGridView1.Rows[i].Cells[j].Style.BackColor == Color.FromName("Red"))
                    { Ending.row = i; Ending.col = j; }
            Next.Add(Start);//将起点作为第一个到达的点
            Point P = Start;//判断初值
            while (P.row != Ending.row || P.col != Ending.col)// || Next.Count == 0)
            {
                P = Next[0];//读取下一个要处理的点
                            //逆时针顺序存放，读取时就是顺时针
                if (P.col != 0)//左
                {
                    Point left = P;
                    left.col -= 1;
                    left.Came_From = "→";
                    if (!IsCompleted(left) && !IsNext(left))
                    {
                        Next.Add(left);
                        PrintNext(left);//填色
                    }

                }

                if (P.row != 9)//下
                {
                    Point down = P;
                    down.row += 1;
                    down.Came_From = "↑";
                    if (!IsCompleted(down) && !IsNext(down))
                    {
                        Next.Add(down);
                        PrintNext(down);
                    }
                }

                if(P.col != 9)//右
                {
                    Point right = P;
                    right.col += 1;
                    right.Came_From = "←";
                    if (!IsCompleted(right) && !IsNext(right))
                    {
                        Next.Add(right);
                        PrintNext(right);
                    }
                }

                if(P.row != 0)//上
                {
                    Point up = P;
                    up.row -= 1;
                    up.Came_From = "↓";
                    if (!IsCompleted(up) && !IsNext(up))
                    {
                        Next.Add(up);
                        PrintNext(up);
                    }
                }

                Completed.Add(P);//处理完后将点放入已处理表
                PrintCompleted(P);//填色
                Next.RemoveAt(0);//删除Next里的P
            }

            P = Ending;
            while (P.row != Start.row || P.col != Start.col)//画路径
            {
                switch (P.Came_From)
                {
                    case "→": P.col += 1; break;
                    case "↑": P.row -= 1; break;
                    case "←": P.col -= 1; break;
                    case "↓": P.row += 1; break;
                }
                for (int i = 0; i < Completed.Count; i++)
                {
                    Point q = Completed[i];
                    if (q.row == P.row && q.col == P.col)
                        P.Came_From = q.Came_From;
                }
                PrintRoad(P);
            }

            this.dataGridView1.Rows[Start.row].Cells[Start.col].Style.BackColor = Color.FromName("Lime");
            this.dataGridView1.Rows[Ending.row].Cells[Ending.col].Style.BackColor = Color.FromName("Red");
        }

        private bool IsNext(Point P)
        {
            for(int i=0;i< Next.Count; i++)
            {
                Point q = Next[i];
                if (q.row == P.row && q.col == P.col)
                    return true;
            }
            return false;
        }

        private bool IsCompleted(Point P)
        {
            for (int i = 0; i < Completed.Count; i++)
            {
                Point q = Completed[i];
                if (q.row == P.row && q.col == P.col)
                    return true;
            }
            return false;
        }

        private void PrintCompleted(Point P)
        {
            this.dataGridView1.Rows[P.row].Cells[P.col].Style.BackColor = Color.FromName("Gray");
            this.dataGridView1.Rows[P.row].Cells[P.col].Value = P.Came_From;//画方向箭头
        }

        private void PrintNext(Point P)
        {
            this.dataGridView1.Rows[P.row].Cells[P.col].Style.BackColor = Color.FromName("BlanchedAlmond");
            this.dataGridView1.Rows[P.row].Cells[P.col].Value = P.Came_From;
        }
        private void PrintRoad(Point P)
        {
            this.dataGridView1.Rows[P.row].Cells[P.col].Style.BackColor = Color.FromName("Aqua");
            this.dataGridView1.Rows[P.row].Cells[P.col].Value = P.Came_From;
        }
        //private void DeepCopy(Point *to,Point *from)

    }
}
