using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace WPFMissileCommand
{
    /*
     * Explosion class. Stores the position and state of an explosion. Each call to Update changes its size. Update returns false
     * once it is done with its lifespan. It has a getDist method, which is used to find the distance to an Explosion from any point.
     */
    public class Explosion
    {
        private int x, y, targetSize, curSize;
        private float visualSize, rate;
        public Explosion(int newx, int newy, int ts)
        {
            SoundPlayer SP = new SoundPlayer();
            SP.SoundLocation = "sounds/thud.wav";
            SP.Play();
            rate = (float)1.2;
            x = newx;
            y = newy;
            targetSize = ts;
            curSize = 0;
            visualSize = 0;
        }
        public bool Update()
        {
            if (curSize < targetSize)
            {
                curSize++;
                visualSize += rate;
                return true;
            }
            else if (curSize < ((targetSize * 2) - 2))
            {
                curSize++;
                visualSize -= rate;
                return true;
            }

            return false;
        }

        public int[] getCoords()
        {
            int[] ret = { (int)this.x, (int)this.y };
            return ret;
        }
        public int getSize()
        { return (int)visualSize; }

        public int getDist(int otherX, int otherY)
        {
            return (int)(Math.Sqrt(Math.Pow(((int)Math.Abs(otherX - x)), 2) + Math.Pow(((int)Math.Abs(otherY - y)), 2)));
        }

        public bool isInside(int otherX, int otherY)
        {
            return (visualSize > (int)(Math.Sqrt(Math.Pow(((int)Math.Abs(otherX - x)), 2) + Math.Pow(((int)Math.Abs(otherY - y)), 2))));
        }
    }
}
