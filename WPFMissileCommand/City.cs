using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFMissileCommand
{
    /*
     * City handles its own position and status, but not its drawing. It tells the main window whether it is destroyed
     */
    class City
    {
        private int xPos;
        private bool destroyed;
        private LinkedList<FalseExplosion> myexplosions;

        public City(int x, LinkedList<FalseExplosion> e)
        {
            this.xPos = x;
            destroyed = false;
            myexplosions = e;
        }

        public int GetX()
        { return xPos; }

        public bool IsDestroyed()
        { return destroyed; }

        public void Restore()
        { destroyed = false; }

        public void Update()
        {
            if (destroyed)
                return;
            foreach (FalseExplosion E in myexplosions)
                if (Math.Abs(E.getCoords()[0] - xPos) < 10)
                    destroyed = true;
        }
    }
}
