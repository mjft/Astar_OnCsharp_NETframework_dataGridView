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

        struct Point : IComparable<Point>
        {
            public int row;
            public int col;
            public String Came_From;
            public int f_cost;//当前（路程）代价
            public int g_cost;//预估代价
            public int cost;
            public int CompareTo(Point other)
            {
                // 排序逻辑：this在前面就意味着指定按照这个属性升序，如果写在后面就是降序
                return this.cost.CompareTo(other.cost); //降序
            }
        }

        Point Start = new Point();//起点
        Point Ending = new Point();//终点
        Point PBlock = new Point();//临时存障碍物
        List<Point> Blocks = new List<Point>();//储存障碍物
        List<Point> Next = new List<Point>();//按搜索顺序储存准备搜索的点
        List<Point> Completed = new List<Point>();//存储已搜索的点

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 20; i++)
            {
                this.dataGridView1.Rows.Add();
                this.dataGridView1.RowHeadersDefaultCellStyle.DataSourceNullValue = "1";
            }
            Start.row = 19;//设初值，用于设置起点
            Start.col = 19;
            Ending.row = 19;
            Ending.col = 19;
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

        private void button_SetStart_Click(object sender, EventArgs e)//设置起点
        {
            this.dataGridView1.Rows[Start.row].Cells[Start.col].Style.BackColor = Color.FromName("White");//清空上一个起点
            Start.row = dataGridView1.CurrentCell.RowIndex;
            Start.col = dataGridView1.CurrentCell.ColumnIndex;
            this.dataGridView1.Rows[Start.row].Cells[Start.col].Style.BackColor = Color.FromName("Lime");//设置单元格颜色
            //this.dataGridView1.Rows[row].Cells[col].Style.ForeColor = Color.Black;//设置字体颜色
        }

        private void button_SetEnding_Click(object sender, EventArgs e)//设置终点
        {
            this.dataGridView1.Rows[Ending.row].Cells[Ending.col].Style.BackColor = Color.FromName("White");//设置单元格颜色
            Ending.row = dataGridView1.CurrentCell.RowIndex;
            Ending.col = dataGridView1.CurrentCell.ColumnIndex;
            this.dataGridView1.Rows[Ending.row].Cells[Ending.col].Style.BackColor = Color.FromName("Red");//设置单元格颜色
        }

        private void button_SetBlock_Click(object sender, EventArgs e)//设置障碍物
        {
            PBlock.row = dataGridView1.CurrentCell.RowIndex;
            PBlock.col = dataGridView1.CurrentCell.ColumnIndex;
            Blocks.Add(PBlock);//存进障碍物表
            this.dataGridView1.Rows[PBlock.row].Cells[PBlock.col].Style.BackColor = Color.FromName("Black");//设置单元格颜色
        }





        private void button_Search_General_Click(object sender, EventArgs e)//普通方法
        {
            Restar();
            Next.Add(Start);//将起点作为第一个要搜索的点
            Point P = Start;//设置循环的判断初值
            while (P.row != Ending.row || P.col != Ending.col)//(Math.Abs(P.row - Ending.row) != 1 && P.col != Ending.col || P.row != Ending.row && Math.Abs(P.col - Ending.col) != 1)
            {
                P = Next[0];//读取下一个要处理的点
                            //逆时针顺序存放，读取时就是顺时针
                if (P.col != 0)//左
                {
                    Point left = P;
                    left.col -= 1;
                    Process(left, "→");
                }
                if (P.row != 19)//下
                {
                    Point down = P;
                    down.row += 1;
                    Process(down, "↑");
                }
                if(P.col != 19)//右
                {
                    Point right = P;
                    right.col += 1;
                    Process(right, "←");
                }
                if(P.row != 0)//上
                {
                    Point up = P;
                    up.row -= 1;
                    Process(up, "↓");
                }

                PrintCompleted(P);//填色
                Completed.Add(P);//处理完后将点放入已处理表
                Next.RemoveAt(0);//删除Next里的刚刚完成搜索的点P
            }

            PrintRoad();//画路径
        }


        private void button_Search_Astar_Click(object sender, EventArgs e)//A star方法
        {
            Restar();
            Next.Add(Start);//将起点作为第一个要搜索的点
            Point P = Start;//设置循环的判断初值
            while (P.row != Ending.row || P.col != Ending.col)// || Next.Count == 0)
            {
                P = Next[0];//读取下一个要处理的点
                            //顺序无关系
                if (P.col != 0)//左
                {
                    Point left = P;
                    left.col -= 1;
                    Process(left, P, "→");
                }
                if (P.row != 19)//下
                {
                    Point down = P;
                    down.row += 1;
                    Process(down, P, "↑");
                }
                if (P.col != 19)//右
                {
                    Point right = P;
                    right.col += 1;
                    Process(right, P, "←");
                }
                if (P.row != 0)//上
                {
                    Point up = P;
                    up.row -= 1;
                    Process(up, P, "↓");
                }

                PrintCompleted(P);//填色
                Completed.Add(P);//处理完后将点放入已处理表
                Next.RemoveAt(0);//删除Next里的刚刚完成搜索的点P

                Next.Sort();//排序,cost低的往前
            }

            PrintRoad();//画路径
        }

        private void button_ShowCost_Click(object sender, EventArgs e)//显示代价
        {
            Point P = Ending;//从终点开始往回画
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
                    {
                        P.Came_From = q.Came_From;
                        P.f_cost = q.f_cost;
                        P.g_cost = q.g_cost;
                    }
                }
                this.dataGridView1.Rows[P.row].Cells[P.col].Value = P.f_cost + "+" + P.g_cost;
            }
        }

        private void button_Restart_Click(object sender, EventArgs e)//清空
        {
            Blocks.Clear();
            Restar();
        }
        //////////////////////////////////////////////函数////////////////////////////////////////////////
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

        private bool IsBlock(Point P)
        {
            for (int i = 0; i < Blocks.Count; i++)
            {
                Point q = Blocks[i];
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

        private void PrintRoad()//画路径
        {
            Point P = Ending;//从终点开始往回画
            while (P.row != Start.row || P.col != Start.col)
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
                    if (P.row == Completed[i].row && P.col == Completed[i].col)
                    {
                        P.Came_From = Completed[i].Came_From;
                        break;
                    }
                }
                this.dataGridView1.Rows[P.row].Cells[P.col].Style.BackColor = Color.FromName("Aqua");
            }
            this.dataGridView1.Rows[Start.row].Cells[Start.col].Style.BackColor = Color.FromName("Lime");
            this.dataGridView1.Rows[Ending.row].Cells[Ending.col].Style.BackColor = Color.FromName("Red");
            this.dataGridView1.Rows[Ending.row].Cells[Ending.col].Value = null;
        }

        private void  Process(Point P, string Came_From)//判断并处理点
        {
            if (!IsBlock(P))
            {
                P.Came_From = Came_From;
                if (!IsCompleted(P) && !IsNext(P))
                {
                    Next.Add(P);
                    PrintNext(P);
                }
            }
        }
        private void Process(Point P, Point Station, string Came_From)//A star判断并处理点
        {
            if (!IsBlock(P))
            {
                P.Came_From = Came_From;
                if (!IsCompleted(P) && !IsNext(P))
                {
                    P.f_cost = Station.f_cost + 1;
                    P.g_cost = Math.Abs(P.row - Ending.row) + Math.Abs(P.col - Ending.col);
                    P.cost = P.f_cost + P.g_cost;
                    Next.Add(P);
                    PrintNext(P);
                }
            }
        }

        private void Restar()
        {
            Completed.Clear();
            Next.Clear();
            for (int i = 0; i < 19; i++)
                for (int j = 0; j < 19; j++)
                {
                    this.dataGridView1.Rows[i].Cells[j].Value = "";
                    this.dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.FromName("White");
                }
            for (int i = 0; i < Blocks.Count; i++)
                this.dataGridView1.Rows[Blocks[i].row].Cells[Blocks[i].col].Style.BackColor = Color.FromName("Black");
        }
        //private void DeepCopy(Point *to,Point *from)

    }
}
