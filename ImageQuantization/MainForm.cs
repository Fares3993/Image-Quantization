using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();//Θ(1)
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();//Θ(1)
        }

        public static string time_mSec;//Θ(1)
        public static string time_sec;//Θ(1)
        private void Operations(int numOfCluster)
        {
            double before = System.Environment.TickCount;
            ImageOperations.Extract_Distinct_Color(ImageMatrix);
            ImageOperations.list_of_index_color(ImageOperations.DistinctColors);
            MinimumSpaningTree mst = ImageOperations.minSpaningTree(ImageMatrix);
            RGBPixelD[, ,] pallete = ImageOperations.get_colorPalette(mst, Convert.ToInt32(nudMaskSize.Text));
            string dis = ImageOperations.DistinctColors.Count.ToString();
            display_content d = new display_content(mst, ImageOperations.DistinctColors.Count.ToString());
            ImageOperations.ImageQuantize(ImageMatrix, pallete);//Θ(N^2) : N is width or height {width ~ height}
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);//Θ(N^2) : N is width or height {width ~ height}
            double after = System.Environment.TickCount;//Θ(1)
            double result1 = (after - before);//Θ(1)
            time_mSec = result1.ToString() + " M-Sec";//Θ(1)
            double result = result1 / 1000;//Θ(1)
            time_sec = result.ToString() + " Sec";//Θ(1)
            d.Show();//Θ(1)
            //ImageOperations.DistinctColors = null;
            //ImageOperations.index_list = null;            
        }//complexity of this function is Θ(N^2)
        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);//Θ(1)
            int maskSize = (int)nudMaskSize.Value;//Θ(1)
            //ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
            //ImageOperations.DisplayImage(ImageMatrix, pictureBox2);//Θ(N^2) : N is width or height {width ~ height}
            Operations(maskSize);//Θ(N^2)
        }//complexity is Θ(N^2)

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Image image = pictureBox2.Image;

            SaveFileDialog s = new SaveFileDialog();
            s.FileName = "Image";
            s.DefaultExt = ".Jpg";
            s.Filter = "Image (.jpg)|*.jpg";
            s.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            s.RestoreDirectory = true;
            if (s.ShowDialog() == DialogResult.OK)
            {
                string filename = s.FileName;
                using (System.IO.FileStream fstream = new System.IO.FileStream(filename, System.IO.FileMode.Create))
                {
                    image.Save(fstream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    fstream.Close();
                }
            }
        }

       
       
    }
}