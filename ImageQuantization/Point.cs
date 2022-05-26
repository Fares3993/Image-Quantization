using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    public class Point
    {
        public int numberOFpoint;
        public double cost;
        public int nextPoint;
        public Point() { }
        public void set_key(double val)
        {
            this.cost = val;
        }
    }
}
