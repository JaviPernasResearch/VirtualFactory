using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using simProcess;
using System;

public class SimpleTransporter : SElement, VElement {

	ConstantDouble[] travelTime;
	MultiServer theWorkstation;

	public double speed = 1.0;
	public Transform origin;
	public Transform destination;
	public float height = 1f;
	public int capacity = 1;

	Vector3 odVector;
	float length;

	void Start () {
		travelTime = new ConstantDouble[capacity];
		UnitySimClock.instance.elements.Add (this);
	}
	
	override public void initializeSim()
	{
		if (origin != null & destination != null) {
			odVector = destination.position - origin.position;
			length = odVector.magnitude;
			Debug.Log("Length " + length.ToString() + " " +this.name);

			for (int i=0; i<capacity; i++)
			{
				travelTime[i] = new ConstantDouble(length / speed);
			}
		} else {
			for (int i=0; i<capacity; i++)
			{
				travelTime[i] = new ConstantDouble(1.0);
			}
		}

		theWorkstation = new MultiServer (travelTime, name, UnitySimClock.instance.clock);
		
		theWorkstation.vElement = this;
	}
	override public void startSim()
	{
		if (origin == null) {
			origin = this.transform;
		}
		if (destination == null) {
			destination = this.transform;
		}
		
		theWorkstation.start ();
	}

	// Update is called once per frame
	void FixedUpdate () {
		float p;
		GameObject gItem;

		if (theWorkstation!= null) {
			
			foreach (ServerProcess sProcess in theWorkstation.workInProgress) {
                gItem = (GameObject)sProcess.theItem.vItem;

                p = (((float)Time.time - (float)sProcess.loadTime) / (float)sProcess.lastDelay);

                if (gItem != null && p <= 1)
                {
                    gItem.transform.position = origin.position + odVector * p + new Vector3(0f, height, 0f);
                }			
			}

		}

	}

	
	override public Element getElement()
	{
		return theWorkstation;
	}
	
	
	void VElement.reportState(string msg)
	{
	}
	
	object VElement.generateItem(int type)
	{
		return null;
	}
	
	void VElement.loadItem(Item vItem)
	{
        GameObject gItem;
        gItem = (GameObject)vItem.vItem;

        if (gItem != null)
        {
            gItem.transform.position = origin.position;
        }

    }

    void VElement.unloadItem(Item vItem)
    {
    }

    public override void restartSim()
    {
        Queue<Item> items = theWorkstation.getItems();
        int i = 0;

        foreach (Item it in items)
        {
            GameObject.Destroy((GameObject)it.vItem);
            i++;
        }

        this.startSim();
    }


    //UI
    public override string getReport()
    {
        return null;
    }
}
