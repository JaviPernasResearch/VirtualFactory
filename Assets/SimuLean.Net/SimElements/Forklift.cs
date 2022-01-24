using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace simProcess
{
    public class Forklift : Element, WorkStation
    {
        ServerProcess theProcess;

		int currentItems;
		int capacity;

        string name;

        public bool atPickPoint;
        public bool readyToLeave = false;
        bool isReceiving = false;

        public Forklift(String name, SimClock sClock, int capacity) : base(name, sClock)
        {
            this.name = name;
			this.capacity = capacity;
        }

        public override void start()
        {

            theProcess = new ServerProcess(this, new PoissonProcess(1), 1);

            currentItems = 0;
            atPickPoint = false;
        }

        override public int getQueueLength()
        {
			return currentItems;
        }
        override public int getFreeCapacity()
        {
            return capacity - currentItems;
        }

        string WorkStation.getName()
        {
            return name;
        }

        //Desbloquea la estación de trabajo cuando aguas abajo queda libre
        public override bool unblock()
        {
            if (theProcess.state == 2)
            {
                ArrayList itemsStoraged = theProcess.getItems();
                ArrayList itemsToRemove = new ArrayList();

                foreach (Item it in itemsStoraged)
                {
                    if (getOutput().sendItem(it, this))
                    {
                        itemsToRemove.Add(it);
                        currentItems--;
                        //vElement.reportState("Exit 1"); //Tiene que liberar un item
                    }
                    else
                    {
                        foreach (Item itt in itemsToRemove)
                        {
                            itemsStoraged.Remove(itt);
                        }
                        vElement.reportState("Update items");
                        theProcess.state = 2;
                        return false;
                    }

                    vElement.reportState("Exit all");    
                }
            }
            else
            {
                return false;
            }

            theProcess.state = 0;
            readyToLeave = false;
            theProcess.clearList();
            getInput().notifyAvaliable(this);

            return true;
        }


        public override bool receive(Item theItem)
        {

            if (currentItems >= capacity || theProcess.state != 0)
            {
                return false;
            }
            else
            {
				if(atPickPoint == true)  //Entra cuando la carretilla está en el punto de recogida
				{
                    theProcess.addItem(theItem);
                    currentItems++;

                    if (currentItems >= capacity)
                    {
                        isReceiving = false;
                        vElement.loadItem(theItem);

                        Item myItems = theItem;

                        foreach (Item it in theProcess.getItems())
                        {
                            myItems.addItem(it);
                        }

                        theProcess.state = 1;
                        vElement.unloadItem(myItems);
                    }
                    else
                    {
                        vElement.loadItem(theItem);

                        if (isReceiving != true)
                        {
                            isReceiving = true;
                            simClock.scheduleEvent(this.theProcess, 2);
                        }
                    }

                    return true;
                }else
                {
                    vElement.loadItem(theItem);
                    return false;
                }

            }
        }

        void WorkStation.completeServerProcess(ServerProcess theProcess)
        {
            if (isReceiving == true)
            {
                if (!getInput().notifyAvaliable(this))
                {
                    Item myItems = null;

                    foreach (Item it in theProcess.getItems())
                    {
                        if (myItems == null) //Item container
                        {
                            myItems = it;
                        }

                        myItems.addItem(it);
                    }

                    isReceiving = false;
                    theProcess.state = 1;
                    vElement.unloadItem(myItems);

                }

                return;
            }

            else if (readyToLeave == true)
            {
                ArrayList itemsStoraged = theProcess.getItems();
                ArrayList itemsToRemove = new ArrayList();

                foreach (Item it in theProcess.getItems())
                {

                    if (getOutput().sendItem(it, this))
                    {
                        itemsToRemove.Add(it);
                        currentItems--;
                    }
                    else
                    {
                        foreach (Item itt in itemsToRemove)
                        {
                            itemsStoraged.Remove(itt);
                        }
                        theProcess.state = 2;
                        vElement.reportState("Update Items");
                        return;
                    }
                }

                readyToLeave = false;
                theProcess.clearList();
                theProcess.state = 0;
                getInput().notifyAvaliable(this);

                return;
            }
            
        }

        public override bool checkAvaliability(Item theItem)
        {
            return !(currentItems >= capacity || theProcess.state != 0);
        }

		
		public void pickItem() //Called once the forklift arrives at the origin
        {
			getInput().notifyAvaliable(this);
				
		}
		
		public void leaveItem() //Called once the forklift arrives at the destination
		{
            simClock.scheduleEvent(theProcess, 0.0);

        }

        public ArrayList getItems()
        {
        
            return theProcess.getItems();

        }
    }

}
