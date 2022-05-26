using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
namespace ImageQuantization
{
    public partial class display_content : Form
    {
        MinimumSpaningTree MST;
        string DistinctColor;
        public display_content(MinimumSpaningTree mst, string DC)
        {
            InitializeComponent();
            MST = mst;
            DistinctColor = DC;
        }

        private void display_content_Load(object sender, EventArgs e)
        {
            textBox1.Text = DistinctColor;
            textBox2.Text = MST.Sum.ToString();
            textBox3.Text = MainForm.time_mSec;
            textBox4.Text = MainForm.time_sec;

            StreamReader sr = new StreamReader("colorPallete.txt");
            List<string> lines = new List<string>();
            while(!sr.EndOfStream)
            {
                lines.Add(sr.ReadLine());
            }
            sr.Close();
            DataTable table = new DataTable();
            table.Columns.Add("Cluster Number",typeof(int));
            table.Columns.Add("Red", typeof(int));
            table.Columns.Add("Green", typeof(int));
            table.Columns.Add("Blue", typeof(int));

            for(int i = 1; i < lines.Count; i++)
            {
                string line = lines[i];
                string[] word = Regex.Split(line, "\t\t");
                table.Rows.Add(i,int.Parse(word[0]), int.Parse(word[1]), int.Parse(word[2]));
            }
            dataGridView1.DataSource=table;          
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
