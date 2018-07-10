using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFMissileCommand
{
    /*
     * When a missile branches, new missiles are created and the old one is deleted. Tail takes the place of the deleted missile
     * and is owned by one of the child missiles. It is just for visual effect.
     */
    class Tail
    {
        private double endX, endY, startX, startY;

        public Tail(double startx, double starty, double endx, double endy )
        {
            this.endX = endx;
            this.endY = endy;
            this.startX = startx;
            this.startY = starty;
        }
        
        public double[] GetEndCoords()
        {
            double[] ret = {this.endX, this.endY };
            return ret;
        }

        public double[] GetStartCoords()
        {
            double[] ret = { this.startX, this.startY };
            return ret;
        }
    }
}
