using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace WPFMissileCommand
{
    /*
     * Plane handles its own position and movement, but not its drawing or destruction. It does however discover
     * when it should be destroyed, and the main window deletes it.
     */
    class Plane
    {
        private Random rand;
        private int Y, type;
        private double dX, curX;
        private LinkedList<Explosion> myexplosions;

        public Plane(int startY, int type, double dist, LinkedList<Explosion> e)
        {
            SoundPlayer SP = new SoundPlayer();
            if(type == 1)
                SP.SoundLocation = "sounds/alien.wav";
            else
                SP.SoundLocation = "sounds/plane.wav";
            SP.Play();

            rand = new Random();
            this.type = type;
            //  missileChance = mc;

            int leftOrRight = NewRand(2);
            if (leftOrRight == 0)
            {
                curX = -20;
                dX = dist;
            }
            else
            {
                curX = 670;
                dX = -dist;
            }

            Y = startY;

            myexplosions = e;
        }

        public int Update()
        {
            curX += dX;

            foreach (Explosion E in myexplosions)
                if (E.getDist((int)curX, (int)Y) <= E.getSize())
                {
                    myexplosions.AddFirst(new Explosion((int)curX, (int)Y, 30));
                    return 100;
                }

            if (curX > -25 && curX < 675)
                return -1;
            else
                return 0;

        }

        private int NewRand(int mod)
        { return rand.Next() % mod; }

        public int[] GetMyCoords()
        {
            int[] ret = { (int)this.curX, (int)this.Y };
            return ret;
        }

        public bool GetDirection()
        { return (dX > 0); }

        public int GetPlaneType()
        { return type; }
    }
}
