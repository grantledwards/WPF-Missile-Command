using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFMissileCommand
{
    /*
     * Base handles its own position and ammunition, but not its drawing. It returns its
     * current ammo level to the main window.
     */
    class Base
    {
        private int xPos, ammunition;
        private bool destroyed;
        private LinkedList<FalseExplosion> myexplosions;

        public Base(int x, LinkedList<FalseExplosion> e)
        {
            myexplosions = e;
            this.xPos = x;
            destroyed = false;
            ammunition = 10;
        }

        public int GetX()
        { return xPos; }

        public bool IsDestroyed()
        { return destroyed; }
        
        public int GetAmmunition()
        { return ammunition; }

        public bool Spend()
        {
            if (ammunition < 1)
                return false;
            ammunition--;
            return true;
        }
        public void Resupply()
        { ammunition = 10; }
        public void SpendAll()
        { ammunition = 0; }

        public void Update()
        {
         //   ammunition = 0;
          //  if (destroyed)
          //      return;
            foreach (FalseExplosion E in myexplosions)
                if (Math.Abs(E.getCoords()[0] - xPos) < 10)//(E.getDist((int)xPos, 420) <= E.getSize())
                {
                    ammunition = 0;
                    destroyed = true;
                }
        }
    }
}
