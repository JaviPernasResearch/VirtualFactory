using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour
{
    int quantity;
    String myName;
    Text myPanel;

    public ListItem(int quantity, String myName, Text myPanel)
    {
        this.quantity = quantity;
        this.myName = myName;
        this.myPanel = myPanel;

        myPanel.text = "Capacidad de " + myName;
    }

    public int getQuantity()
    {
        return quantity;
    }
}

