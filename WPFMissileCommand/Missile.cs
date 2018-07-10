using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFMissileCommand
{
    /*
     * Missile handles its own position and movement, but not its drawing or destruction. It does however discover
     * when it should be destroyed, and the main window deletes it.
     */
    class Missile
    {
        private int startX, startY, targetX, targetY, groundHeight;
        private double thisX, thisY, dX, dY;
        private LinkedList<Explosion> myexplosions;
        private LinkedList<FalseExplosion> cityExplosions;
        private Tail tail;

        public Missile(int sX, int sY, int gH, int tX, double dist, LinkedList<Explosion> e, LinkedList<FalseExplosion> fe)
        {

            startX = sX;
            startY = sY;
            thisX = startX;
            thisY = startY;
            groundHeight = gH;
            myexplosions = e;
            cityExplosions = fe;

            targetX = tX;
            targetY = groundHeight;

            double ratio = dist / (Math.Sqrt(Math.Pow((targetX - startX), 2) + Math.Pow((targetY - startY), 2)));
            dX = ratio * (targetX - startX);
            dY = ratio * (targetY - startY);
        }

        public int Update()
        {
            thisX += dX;
            thisY += dY;

            foreach (Explosion E in myexplosions)
                if (E.getDist((int)thisX, (int)thisY) <= E.getSize())
                {
                    myexplosions.AddFirst(new Explosion((int)thisX, (int)thisY, 30));
                    return 25;
                }

            if ((groundHeight - thisY) > 5)
            {
                return -1;
            }
            else
            {
                cityExplosions.AddFirst(new FalseExplosion((int)targetX, (int)targetY, 35));
                return 0;
            }

        }

        public void SetTail(Tail t)
        { this.tail = t; }

        public Tail GetTail()
        { return tail; }

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
