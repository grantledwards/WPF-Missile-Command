using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace WPFMissileCommand
{
    class Flak
    {
        /*
     * Flak handles its own position and movement, but not its drawing or destruction. It does however discover
     * when it should be destroyed, and the main window deletes it.
     */
        private int startX, startY, targetX, targetY;
        private double thisX, thisY, dX, dY;
        private LinkedList<Explosion> myexplosions;

        public Flak(int sX, int sY, int tX, int tY, double dist, LinkedList<Explosion> e)
        {
            SoundPlayer SP = new SoundPlayer();
            SP.SoundLocation = "sounds/missile.wav";
            SP.Play();
            startX = sX;
            startY = sY;
            thisX = sX;
            thisY = sY;
            myexplosions = e;

            targetX = tX;
            targetY = tY;

            double ratio = dist / (Math.Sqrt(Math.Pow((targetX - startX), 2) + Math.Pow((targetY - startY), 2)));
            dX = ratio * (targetX - startX);
            dY = ratio * (targetY - startY);
        }

        public bool Update()
        {
            thisX += dX;
            thisY += dY;

            if (thisY <= (targetY))
            {
                myexplosions.AddFirst(new Explosion((int)targetX, (int)targetY, 30));
                return false;
            }
            return true;
        }

        public int[] GetMyCoords()
        {
            int[] ret = { (int)this.thisX, (int)this.thisY };
            return ret;
        }

        public int[] GetStartCoords()
        {
            int[] ret = { this.startX, this.startY };
            return ret;
        }
    }
}
