using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace simProcess
{
    class Timer: Eventcs
    {       
        void Eventcs.execute()
        {           
            UnitySimClock.instance.clock.scheduleEvent(this, 0.1);
        }
    }
}
