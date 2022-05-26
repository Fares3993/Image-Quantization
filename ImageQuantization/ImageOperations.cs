using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

///Algorithms Project
///Intelligent Scissors
///

namespace ImageQuantization
{
    //structs :
    //=========
    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>  
    public struct RGBPixel
    {
        public byte red, green, blue;
    }
    //********************************************************
    public struct RGBPixelD
    {
        public double red, green, blue;
    }
    //_________________________________________________________________________________________________________________________________________________________________________________________________________________
    //structs :
    //=========
    public struct ColorData
    {
        public RGBPixel color;
        public int color_indix;
    }
    //********************************************************
    public struct Point
    {
        public int indexOFpoint;
        public double cost;
        public int nextPoint;
    }
    //********************************************************
    //classes :
    //=========
    public class MinimumSpaningTree
    {
        public double Sum;
        public List<Point> MSTree;
        public int[] NextColors;
        public List<ColorData> color_index_list;


        public MinimumSpaningTree(double TreeCost, List<Point> tree, int[] N_Color, List<ColorData> idx)
        {
            Sum = TreeCost;
            MSTree = tree;
            NextColors = N_Color;
            color_index_list = idx;
        }

    }

    //********************************************************
    public class PriorityQueue
    {
        public List<Point> Points = new List<Point>();

        public int returnParent(int idx)
        {
            if (idx < 1)
            {
                return -1;
            }
            int newIndex = (idx - 1) / 2;
            return newIndex;
        }//the compliexty is Θ(1)
        //---------------------------
        public int LeftChild(int idx)
        {
            int newIndex = 2 * idx + 1;
            return newIndex;
        }//the compliexty is Θ(1)
        //---------------------------
        public int RightChild(int idx)
        {
            int newIndex = 2 * idx + 2;
            return newIndex;
        }//the compliexty is Θ(1)
        //---------------------------
        public void AscendingTheHeap(int idx) //Minimum Heapify
        {
            int MinValue = idx;
            int L = LeftChild(idx);
            int R = RightChild(idx);
            if (L < Points.Count && Points[L].cost < Points[MinValue].cost)
            {
                MinValue = L;
            }
            if (R < Points.Count && Points[R].cost < Points[MinValue].cost)
            {
                MinValue = R;
            }
            if (MinValue != idx)
            {
                swap(idx, MinValue);//Θ(1)
                AscendingTheHeap(MinValue);
            }
        }//the complexity is O(log(N))
        //---------------------------
        public Point returnMinHeap()
        {
            if (Points.Count > 0)
            {
                Point min = Points[0];
                Points[0] = Points[Points.Count - 1];
                Points.RemoveAt(Points.Count - 1);
                AscendingTheHeap(0);//O(Log(N))
                return min;
            }
            else
            {
                throw new InvalidOperationException("the Priority_Queue is underflow");
            }
        }//the complexity is O(log(N))
        //---------------------------
        public void Append(Point element)
        {
            Points.Add(element); //Θ(1)
            HeapRearrangement(Points.Count -1);//O(Log(N))
        }//the complexity is O(log(N))
        //---------------------------
        public void HeapRearrangement(int idx)
        {
            int parent = returnParent(idx);//Θ(1)
            if (parent >= 0 && Points[parent].cost > Points[idx].cost)
            {
                swap(idx, parent);
                HeapRearrangement(parent);
            }
        }//the complexity is O(log(N)) T(N) = T(N/2) + Θ(1) --> Case2 master method
        //---------------------------
        public bool isEmpty()
        {
            if (Points.Count > 0)
                return false;
            else
                return true;
        }//the compliexty is Θ(1)
        //---------------------------
        public void swap(int idx, int min)
        {
            Point temporary = Points[idx];
            Points[idx] = Points[min];
            Points[min] = temporary;
        }//the compliexty is Θ(1)
    }
//_________________________________________________________________________________________________________________________________________________________________________________________________________________
    /// <summary>
    /// Library of static functions that deal with images
    /// </summary>
    public class ImageOperations
    {
        /// <summary>
        /// Open an image and load it into 2D array of colors (size: Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        public static RGBPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            RGBPixel[,] Buffer = new RGBPixel[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[2];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[0];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }

        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[2] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[0] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }


        /// <summary>
        /// Apply Gaussian smoothing filter to enhance the edge detection 
        /// </summary>
        /// <param name="ImageMatrix">Colored image matrix</param>
        /// <param name="filterSize">Gaussian mask size</param>
        /// <param name="sigma">Gaussian sigma</param>
        /// <returns>smoothed color image</returns>
        public static RGBPixel[,] GaussianFilter1D(RGBPixel[,] ImageMatrix, int filterSize, double sigma)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);

            RGBPixelD[,] VerFiltered = new RGBPixelD[Height, Width];
            RGBPixel[,] Filtered = new RGBPixel[Height, Width];


            // Create Filter in Spatial Domain:
            //=================================
            //make the filter ODD size
            if (filterSize % 2 == 0) filterSize++;

            double[] Filter = new double[filterSize];

            //Compute Filter in Spatial Domain :
            //==================================
            double Sum1 = 0;
            int HalfSize = filterSize / 2;
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                //Filter[y+HalfSize] = (1.0 / (Math.Sqrt(2 * 22.0/7.0) * Segma)) * Math.Exp(-(double)(y*y) / (double)(2 * Segma * Segma)) ;
                Filter[y + HalfSize] = Math.Exp(-(double)(y * y) / (double)(2 * sigma * sigma));
                Sum1 += Filter[y + HalfSize];
            }
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                Filter[y + HalfSize] /= Sum1;
            }

            //Filter Original Image Vertically:
            //=================================
            int ii, jj;
            RGBPixelD Sum;
            RGBPixel Item1;
            RGBPixelD Item2;

            for (int j = 0; j < Width; j++)
                for (int i = 0; i < Height; i++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int y = -HalfSize; y <= HalfSize; y++)
                    {
                        ii = i + y;
                        if (ii >= 0 && ii < Height)
                        {
                            Item1 = ImageMatrix[ii, j];
                            Sum.red += Filter[y + HalfSize] * Item1.red;
                            Sum.green += Filter[y + HalfSize] * Item1.green;
                            Sum.blue += Filter[y + HalfSize] * Item1.blue;
                        }
                    }
                    VerFiltered[i, j] = Sum;
                }

            //Filter Resulting Image Horizontally:
            //===================================
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int x = -HalfSize; x <= HalfSize; x++)
                    {
                        jj = j + x;
                        if (jj >= 0 && jj < Width)
                        {
                            Item2 = VerFiltered[i, jj];
                            Sum.red += Filter[x + HalfSize] * Item2.red;
                            Sum.green += Filter[x + HalfSize] * Item2.green;
                            Sum.blue += Filter[x + HalfSize] * Item2.blue;
                        }
                    }
                    Filtered[i, j].red = (byte)Sum.red;
                    Filtered[i, j].green = (byte)Sum.green;
                    Filtered[i, j].blue = (byte)Sum.blue;
                }

            return Filtered;
        }
//_________________________________________________________________________________________________________________________________________________________________________________________________________________
        //Difinations:
        //============
        //O() : Upper bound order
        //Θ() : Exact bound order
        //Ω() : Lower bound order
        //any statment is O(1)
        //{complexity loop = #iteration * body} 
        //{complexity if = order(condition) + body} 
        //__________________________________________________
        //functions :
        //===========
        public static List<RGBPixel> DistinctColors = new List<RGBPixel>();//Θ(1)
        public static void Extract_Distinct_Color(RGBPixel[,] ImageMatrix)
        {
            List<RGBPixel> DCList = new List<RGBPixel>();//Θ(1)
            bool[, ,] flag = new bool[256, 256, 256];//Θ(1)

            for (int height = 0; height < GetHeight(ImageMatrix); height++)//#Iteration N => N : Height
            {
                for (int width = 0; width < GetWidth(ImageMatrix); width++)//#Iteration M => M : Width
                {
                    if (!(flag[ImageMatrix[height, width].red, ImageMatrix[height, width].green, ImageMatrix[height, width].blue]))//Θ(1)
                    {
                        DCList.Add(ImageMatrix[height, width]);//Θ(1)
                        flag[ImageMatrix[height, width].red, ImageMatrix[height, width].green, ImageMatrix[height, width].blue] = true;//Θ(1)
                    }
                }
            }//complexity this loop is Θ( N * M ) => N ~ M then complexity is Θ( N^2 )

            DistinctColors = DCList;//Θ(1)
        }//Total complexity is Θ( N^2 )
//_________________________________________________________________________________________________________________________________________________________________________________________________________________
        public static List<ColorData> index_list = new List<ColorData>();//Θ(1)
        public static void list_of_index_color(List<RGBPixel> DC)
        {
            List<ColorData> index_color = new List<ColorData>();//Θ(1)

            for (int indexOfColor = 0; indexOfColor < DC.Count; indexOfColor++)//#Iteration D : D number of distinct colors
            {
                ColorData color_data = new ColorData();//Θ(1)
                color_data.color_indix = indexOfColor;//Θ(1)
                color_data.color = DC[indexOfColor];//Θ(1)
                index_color.Add(color_data);//Θ(1)
            }//complexity this loop is Θ( D )
            index_list = index_color;
        }//Total complexity is Θ( D )
//_________________________________________________________________________________________________________________________________________________________________________________________________________________
        public static Dictionary<int, double>[] Graph(RGBPixel[,] ImageMatrix)
        {
            Dictionary<int, double>[] neighboursList = new Dictionary<int, double>[DistinctColors.Count];//Θ(1)
            double redSimilarity, greenSimilarity, blueSimilarity, colorSimilarity;//Θ(1)

            for (int indexOfColor = 0; indexOfColor < DistinctColors.Count; indexOfColor++)//#Iteration D: D number of distinct colors
            {
                neighboursList[indexOfColor] = new Dictionary<int, double>();//Θ(1)
            }//complexity this loop is Θ(D) 

            int index1 = 0, index2 = 0;//Θ(1)

            foreach (RGBPixel color in DistinctColors)//Θ(D): D is number of distinct colors
            {
                foreach (RGBPixel color2 in DistinctColors)//Θ(D): D is number of distinct colors
                {
                    redSimilarity = (color.red - color2.red) * (color.red - color2.red);//Θ(1)
                    greenSimilarity = (color.green - color2.green) * (color.green - color2.green);//Θ(1)
                    blueSimilarity = (color.blue - color2.blue) * (color.blue - color2.blue);//Θ(1)
                    colorSimilarity = Math.Sqrt(redSimilarity + greenSimilarity + blueSimilarity);//Θ(1)
                    neighboursList[index_list[index1].color_indix].Add(index_list[index2].color_indix, colorSimilarity);//Θ(1)
                    index2++;//Θ(1)
                }
                index1++;//Θ(1)
            }//complexity this loop is Θ( D^2 )
            return neighboursList;//Θ(1)
        }//Total complexity is Θ( D^2 )
//_________________________________________________________________________________________________________________________________________________________________________________________________________________
        public static MinimumSpaningTree minSpaningTree(RGBPixel[,] ImageMatrix)
        {
            List<Point> tree = new List<Point>();//Θ(1)
            PriorityQueue minHeap = new PriorityQueue();//Θ(1)

            bool[] isVisited = new bool[DistinctColors.Count];//Θ(1)
            int[] NextColor = new int[DistinctColors.Count];//Θ(1)
            double[] Weight = new double[DistinctColors.Count];//Θ(1)

            for (int indexOfColor = 0; indexOfColor < DistinctColors.Count; indexOfColor++)//Θ(V): V is number of distinct colors (vertices)
            {
                isVisited[indexOfColor] = false;//Θ(1)
                NextColor[indexOfColor] = -1;//Θ(1)
                if (indexOfColor == 0)//Θ(1)
                {
                    Weight[indexOfColor] = 0;//Θ(1)
                }
                else
                {
                    Weight[indexOfColor] = double.MaxValue;//Θ(1)
                }
            }//complexity of loop is Θ( V ) : V is number of distinct colors ( vertices )

            Point root = new Point();//Θ(1)
            root.indexOFpoint = -1;//Θ(1)
            root.cost = 0;//Θ(1)
            root.nextPoint = 0;//Θ(1)
            minHeap.Append(root);//O(log(V)) : V is number of distinct colors ( vertices )

            while (minHeap.isEmpty() != true)//Max #iteration V :V is number of distinct colors {Max length of the priorty queue}
            {
                Point node = minHeap.returnMinHeap();// Stopping Loop =>with complexity is O( Log(V) ): V is number of distinct colors (vertices)
                int nextPoint_index = node.nextPoint;//Θ(1)

                if (isVisited[nextPoint_index] == true)//to skip next steps Θ(1)
                {
                    continue;//Θ(1)
                }

                isVisited[nextPoint_index] = true;//Θ(1)
                RGBPixel color1 = index_list[nextPoint_index].color;//Θ(1)
                tree.Add(node);//Θ(1)

                int index = 0;//Θ(1)
                foreach (RGBPixel color2 in DistinctColors)//#iteration V : V is Number of distinct colors ( vertices )
                {
                    if (index != nextPoint_index)//Θ(1)
                    {
                        double redSimilarity = (color1.red - color2.red) * (color1.red - color2.red);//Θ(1)
                        double greenSimilarity = (color1.green - color2.green) * (color1.green - color2.green);//Θ(1)
                        double blueSimilarity = (color1.blue - color2.blue) * (color1.blue - color2.blue);//Θ(1)
                        double colorSimilarity = Math.Sqrt(redSimilarity + greenSimilarity + blueSimilarity);//Θ(1)

                        if (!(isVisited[index]) && colorSimilarity < Weight[index])//Θ(1)
                        {
                            Weight[index] = colorSimilarity;//Θ(1)
                            NextColor[index] = nextPoint_index;//Θ(1)
                            Point node1 = new Point();//Θ(1)
                            node1.cost = colorSimilarity;//Θ(1)
                            node1.indexOFpoint = nextPoint_index;//Θ(1)
                            node1.nextPoint = index;//Θ(1)
                            minHeap.Append(node1);//O(Log(V)) : V is Number of distinct colors ( vertices )
                        }
                    }
                    index++;//Θ(1)
                }//complexity foreach loop : O(V * Log(V)) Where Log(V) is upper bound
            }//complexity while loop : O(V^2 * Log(V)) 
             //:E belongs to [ v , v^2 ] => total compexity of this loop
             //upper bound is O(E * Log(V))


            double MST_SUM = 0;//Θ(1)
            for (int idx = 0; idx < DistinctColors.Count; idx++)//#iteration V : V is Number of distinct colors ( vertices )
            {
                MST_SUM += Weight[idx];//Θ(1)
            }//complexity loop : Θ(V)

            return new MinimumSpaningTree(MST_SUM, tree, NextColor, index_list);//Θ(1)
        }//total function's complexity is O(E * Log(V))
//_________________________________________________________________________________________________________________________________________________________________________________________________________________
        public static RGBPixelD[, ,] get_colorPalette(MinimumSpaningTree mst, int numOFclusters)
        {
            List<Point>[] adj = new List<Point>[mst.NextColors.Length];//Θ(1)
            RGBPixelD[, ,] Color_palette = new RGBPixelD[256, 256, 256];//Θ(1)
            bool[] isVisited = new bool[mst.NextColors.Length];//Θ(1)


            /*Extract K Clusters*/
            for (int cluster = 0; cluster < numOFclusters - 1; cluster++) //#Iteration K: K is a number of cluster
            {
                int i = 0 ,indexOFmax = 0;//Θ(1)
                double maxCost = double.MinValue;//Θ(1)
                foreach (Point color in mst.MSTree) //#Iteration D: D is number of distinct colors
                {
                    if (color.cost > maxCost)//Θ(1)
                    {
                        maxCost = color.cost;//Θ(1)
                        indexOFmax = i;//Θ(1)
                    }
                    i++;//Θ(1)
                }//complexity of this loop is Θ(D): D is number of distinct colors

                mst.MSTree.RemoveAt(indexOFmax);//O(D)  
            }//total loop Order --> Θ( K * D )
            


            /*convert clustered tree from list  to adjacent list*/
            for (int nextColor = 0; nextColor < mst.NextColors.Length; nextColor++)//#Iteration D: D number of distinct colors
            {
                adj[nextColor] = new List<Point>();//Θ(1)
            }//total loop --> Θ(D) --> D = number of distinct colors
            int firstIteration = 0;//Θ(1)
            foreach (Point color in mst.MSTree)//#Iteration D: D number of distinct colors
            {
                if (firstIteration == 0)//to skip first color in MSTree "root" Θ(1)
                {
                    firstIteration++;//Θ(1)
                    continue;//Θ(1)
                }
                Point point1 = new Point();//Θ(1)
                Point point2 = new Point();//Θ(1)
                point1.cost = color.cost;//Θ(1)
                point1.nextPoint = color.nextPoint;//Θ(1)
                point2.cost = color.cost;//Θ(1)              P1 <-neibours-> P2
                point2.nextPoint = color.indexOFpoint;//Θ(1)
                adj[color.indexOFpoint].Add(point1);//Θ(1)
                adj[color.nextPoint].Add(point2);//Θ(1)
            }//the complexity is Θ(D)
            /*end the convertion*/


            /* to each average_color of the cluster ... to all his adjacent and make a pallete with new color */
            for (int nextColor = 0; nextColor < mst.NextColors.Length; nextColor++) // #Iteration is D: D is number of disinct colors
            {
                if (isVisited[nextColor] == false)//go in with number of clusters only (K)
                {
                    ClusterColor(mst.color_index_list, nextColor, Color_palette, isVisited, adj);//Θ(D) BFS was called,D is number of disinct colors
                }
            }//complexity loop is Θ(D * D + K)--> D^2 : K is number of clusters, D is number of distinct colors       
            /*end loop*/


            return Color_palette;//Θ(1)
        }//  total function's complexity is Θ(D^2) : D is number of distinct colors
//_________________________________________________________________________________________________________________________________________________________________________________________________________________
        public static void ClusterColor(List<ColorData> ColorIndex, int nextIndex, RGBPixelD[, ,] pallete, bool[] isVisited, List<Point>[] adj)
        {//using BFS
            int counter = 1;//Θ(1)
            RGBPixelD AverageColor = new RGBPixelD();//Θ(1)
            Queue<int> queue = new Queue<int>();//Θ(1)
            List<RGBPixel> clustersColor = new List<RGBPixel>();//Θ(1)
            queue.Enqueue(nextIndex);//Θ(1)


            while (queue.Count != 0) 
            {
                int root = queue.Dequeue();
                RGBPixel color = ColorIndex[root].color;
                clustersColor.Add(color);
                AverageColor.red += color.red;
                AverageColor.blue += color.blue;
                AverageColor.green += color.green;

                /*to number of points in cluster */
                foreach (Point root_ in adj[root])
                {
                    if (isVisited[root_.nextPoint] == false)
                    {
                        queue.Enqueue(root_.nextPoint);
                        counter++;
                    }
                } // complexity is Θ(adj(D)) : D = number of distinct color
                /*end number of points in cluster*/

                isVisited[root] = true;
            }//complexity is Θ(D) and runing time=(E+D) : E = number of edges in cluster {maximum E is D-1}, D is is #Distinct colors


            AverageColor.red /= counter; 
            AverageColor.blue /= counter; 
            AverageColor.green /= counter; 


            //assign the the cluster color with its represented color in the pallete
            foreach (RGBPixel clusterColor in clustersColor) 
            {
                pallete[clusterColor.red, clusterColor.blue, clusterColor.green] = AverageColor;
            }//complexity is O(D) : D = number of Distinct colors in this cluster 


        }// total function's complexity is Θ(D) : D is number of Distinct colors in this cluster 
//_________________________________________________________________________________________________________________________________________________________________________________________________________________
        // replace the color of each pixal in the image, with average color of cluster in the pallete
        public static void ImageQuantize(RGBPixel[,] ImageMatrix, RGBPixelD[, ,] pallete)
        {
            bool[, ,] isVisited = new bool[256, 256, 256];//Θ(1)
            int red, green, blue;//Θ(1)
            StreamWriter sr = new StreamWriter("colorPallete.txt");//Θ(1)
            sr.Write("red\t\t");//Θ(1)
            sr.Write("green\t\t");//Θ(1)
            sr.WriteLine("blue");//Θ(1)


            int W = GetWidth(ImageMatrix); //Θ(1)
            int H = GetHeight(ImageMatrix);//Θ(1)
            for (int i = 0; i < H; i++)//#iteration N : N = height
            {
                for (int j = 0; j < W; j++)//#iteration M : M = width
                {
                    var index = ImageMatrix[i, j]; //Θ(1)
                    var color = pallete[index.red, index.blue, index.green]; //Θ(1)
                    ImageMatrix[i, j].red = (byte)color.red; //Θ(1)
                    ImageMatrix[i, j].green = (byte)color.green; //Θ(1) 
                    ImageMatrix[i, j].blue = (byte)color.blue;//Θ(1)


                    //to write the average color in cluster only one in file
                    red = ImageMatrix[i, j].red;//Θ(1)
                    green = ImageMatrix[i, j].green;//Θ(1)
                    blue = ImageMatrix[i, j].blue;//Θ(1)
                    if (isVisited[red, green, blue] == false)//when write in file don't repeat the color for same cluster Θ(1)
                    {
                        sr.Write(ImageMatrix[i, j].red);//Θ(1)
                        sr.Write("\t\t");//Θ(1)
                        sr.Write(ImageMatrix[i, j].green);//Θ(1)
                        sr.Write("\t\t");//Θ(1)
                        sr.WriteLine(ImageMatrix[i, j].blue);//Θ(1)
                        isVisited[red, green, blue] = true;//Θ(1)
                    }
                }
            }
            sr.Close();//Θ(1)
        }//Total_complexity is Θ( N * M ) = Θ( N^2 ) : N (height) ~ M (width)  
//_________________________________________________________________________________________________________________________________________________________________________________________________________________
    }//end ImageOperations
}
