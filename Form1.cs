using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace eq_cluster
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int fenlei_quantity = Int32.Parse(textBox2.Text);
                StreamReader sr = new StreamReader(this.textBox1.Text, System.Text.Encoding.GetEncoding("gb2312"));   // 定义输入文件
                StreamWriter sw = new StreamWriter("..\\..\\result_log.txt", true, System.Text.Encoding.GetEncoding("gb2312"));  // 定义输出文件
                List<string[]> list_arr = new List<string[]>();           // 定义初始集合
                string temp_line = sr.ReadLine();                        // 读入输入文件的第一行数据
                while (temp_line != null)                                  // 开始执行循环，以输入文件的行数来做判断
                {
                    list_arr.Add(temp_line.Split('\t'));                 // 将输入文件的每行，以/t分成多个数组，既将每行的数据分成若干列
                    temp_line = sr.ReadLine();
                }
                int length_x = list_arr.Count;                           // 得到数组的行
                int length_y = list_arr[0].GetLength(0);                 // 得到数组的列

                string[,] arr_initial = new string[length_x, length_y];   // 定义初始数组
                for (int i = 0; i < length_x; i++)                      // 将数组打印到输出文件
                {
                    for (int j = 0; j < length_y; j++)
                    {
                        arr_initial[i, j] = list_arr[i][j];             // 将泛型集合转换成数组，方便后面的数据使用
                        sw.Write(arr_initial[i, j].ToString() + "\t");                // 打印到输出文件
                    }
                    sw.Write("\r\n");
                }
                sw.Write("\r\n重心：");
                sw.Flush();                                             // 清空写入文件的缓存

                double[] zhongxin = new double[length_y];                 // 定义中心数组
                for (int j = 1; j < length_y; j++)                       // 循环在求重心的值
                {
                    double temp_add = 0;
                    for (int i = 1; i < length_x; i++)
                    {
                        temp_add += double.Parse(arr_initial[i, j]);
                    }
                    zhongxin[j] = temp_add / (length_x - 1);
                    sw.Write("\tx" + j.ToString() + " = " + zhongxin[j].ToString("0.00"));
                }
                sw.Write("\r\n");
                sw.Flush();                                             // 清空写入文件的缓存

                double[] juli = new double[length_x];                 // 定义与中心的距离的数组
                sw.Write("\r\n距离：");
                for (int i = 1; i < length_x; i++)                      // 循环计算中心的距离
                {
                    double temp_add = 0;
                    for (int j = 1; j < length_y; j++)
                    {
                        temp_add += Math.Pow((double.Parse(arr_initial[i, j]) - zhongxin[j]), 2);
                    }
                    juli[i] = Math.Sqrt(temp_add);
                    sw.Write("\td" + i + " = " + juli[i].ToString("0.00"));
                }
                sw.Write("\r\n");
                sw.Flush();


                List<List<int>> julei_b = new List<List<int>>();          //定义初始的聚类集合
                for (int i = 1; i < length_x; i++)
                {
                    List<int> julei_a = new List<int>();
                    julei_a.Add(i);
                    julei_b.Add(julei_a);
                }

                for (; julei_b.Count > fenlei_quantity; )               //当聚类进行，将数据聚为10类的时候停止循环
                {
                    double min_julicha = 10000;                         // 定义一个距离最小值，初始值为10000
                    int min_x = 1; int min_y = 2;

                    // 循环计算距离差，并找出最小值 
                    for (int i = 0; i < julei_b.Count; i++)
                    {
                        for (int j = i + 1; j < julei_b.Count; j++)
                        {
                            double temp_add = 0;
                            int count_no = 0;
                            for (int m = 0; m < julei_b[i].Count; m++)
                            {
                                for (int n = 0; n < julei_b[j].Count; n++)
                                {
                                    count_no++;
                                    temp_add += Math.Abs(juli[julei_b[i][m]] - juli[julei_b[j][n]]);
                                }
                            }
                            double julicha = temp_add / count_no;
                            if (julicha < min_julicha)
                            {
                                min_julicha = julicha;
                                min_x = i;
                                min_y = j;
                            }
                        }
                    }
                    string result = "";
                    for (int ii = 0; ii < julei_b[min_x].Count; ii++)
                    {
                        result += arr_initial[julei_b[min_x][ii], 0] + "/";
                    }
                    for (int jj = 0; jj < julei_b[min_y].Count; jj++)
                    {
                        result += arr_initial[julei_b[min_y][jj], 0] + "/";
                        julei_b[min_x].Add(julei_b[min_y][jj]);
                    }
                    sw.Write("\r\n");
                    sw.Write("最小距离差为：\t min_d=" + min_julicha.ToString("0.00") + "\t" + result + "\r\n");
                    sw.Flush();
                    julei_b.RemoveAt(min_y);

                    sw.Flush();
                }
                //展示最后结果
                string last_result = "\r\n最后结果是：\n\n  ";
                string show = "";
                for (int i = 0; i < julei_b.Count; i++)
                {
                    for (int j = 0; j < julei_b[i].Count; j++)
                    {
                        last_result += arr_initial[julei_b[i][j], 0] + "，";
                        show += arr_initial[julei_b[i][j], 0] + " ";
                    }
                    last_result += "/\n\n  ";
                    show += "\n";
                }
                sw.Write(last_result);
                sw.Flush();
                sw.Close();
                sr.Close();
                MessageBox.Show(show);
                textBox1.Text = "";
                textBox2.Text = "";
            }
            catch
            {
                MessageBox.Show("Sorry,there are some errors!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();     //显示选择文件对话框
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = openFileDialog1.FileName;          //显示文件路径
            }            
        }
    }
}
