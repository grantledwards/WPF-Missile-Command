using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace WPFMissileCommand
{
    /*
     * False Explosion class. This explosion is meant to be drawn, but not trigger other objects to explode. It is
     * only for drawing on top of cities and bases when they are destroyed.
     */
    class FalseExplosion
    {
        private int x, y, targetSize, curSize;
        private float visualSize, rate;
        public FalseExplosion(int newx, int newy, int ts)
        {
            SoundPlayer SP = new SoundPlayer();
            SP.SoundLocation = "sounds/thud.wav";
            SP.Play();
            rate = (float)1.0;
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
            else if (curSize < ((targetSize * 2)))
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
    }
}
