using System;
using System.Collections.Generic;
using System.Collections;

namespace simProcess
{
	public class CustomerSink : Element, Eventcs
	{
		static int truckCapacity = 10;

		int numberItems;
		int pendingOrders;
        int capacity;

		IntegerProvider orderQuantity;
		DoubleProvider demand;

		Queue<Item> itemsQ;

        //UI
        int totalShipments;
        int totalOrders;
        int totalTrucks;

		public CustomerSink(int capacity, String name, SimClock state, int minOrder, int maxOrder) : base(name, state)
		{
            this.capacity = capacity;

            numberItems = 0;
			orderQuantity = new IntegerUniformDistribution (minOrder, maxOrder);
			//demand = new DemandProcess (simClock);
            demand = new UniformDistribution(5.0, 10.0);

            itemsQ = new Queue<Item>();

            SimCosts.addCost(SimCosts.storeCapacityCost * capacity);
        }

		public int getNumberItems()
		{
			return numberItems;
		}

        public int getPendingOrders()
        {
            return pendingOrders;
        }


        public override void start()
		{
			numberItems = 0;
			pendingOrders = 0;

            totalOrders = 0;
            totalShipments = 0;
            totalTrucks = 0;

			itemsQ.Clear ();

			simClock.scheduleEvent (this, demand.provideValue ());
		}


		void Eventcs.execute()
		{
            int currentOrder = this.orderQuantity.provideValue();

            this.pendingOrders += currentOrder;
            this.totalOrders += currentOrder;

            simClock.scheduleEvent (this, demand.provideValue ());
		}

		public override bool unblock()
		{
			throw new System.InvalidOperationException("The Sink cannot receive notifications."); //To change body of generated methods, choose Tools | Templates.
		}

		/*public override bool notifyRequest() {
            throw new System.InvalidOperationException("The Sink cannot receive notifications."); //To change body of generated methods, choose Tools | Templates.
        }*/

		override public int getQueueLength()
		{
			return itemsQ.Count;
		}
		override public int getFreeCapacity()
		{
			return -1;
		}



		public void shipTruck() {
			int q;

			q = Math.Min(itemsQ.Count, CustomerSink.truckCapacity);
			q = Math.Min(q, pendingOrders);

            if (q == 0)
            {
                //vElement.unloadItem(null);
                return;
            }

            Item itemsContainer = itemsQ.Peek();

            for (int i = 0; i < q; i++) {
                itemsContainer.addItem(itemsQ.Dequeue());
				//vElement.unloadItem(itemsQ.Dequeue ());
                numberItems--;

                SimCosts.addRevenue(SimCosts.salePrice);
            }

            vElement.unloadItem(itemsContainer);
			pendingOrders -= q;
            totalShipments += q;
            totalTrucks++;

			if (q > 0) {
                SimCosts.addCost(SimCosts.shipmentCost);
            }
		}


		public override bool receive(Item theItem)
		{
            if (itemsQ.Count < capacity)
            {
                numberItems++;
                itemsQ.Enqueue(theItem);
                vElement.loadItem(theItem);
                return true;
            }
            else
                return false;

		}

		public override bool checkAvaliability(Item theItem)
		{
            return !(numberItems >= capacity);
        }

		public Queue<Item> getItems()
		{
			return itemsQ;
		}

        //Restarting
        public void setCapacity(int capacity)
        {
            this.capacity = capacity;
        }

        public int getTotalShipments()
        {
            return totalShipments;
        }
        public int getTotalTrucks()
        {
            return totalTrucks;
        }
        public int getTotalOrders()
        {
            return totalOrders;
        }
    }
}

