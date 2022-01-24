using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIList: MonoBehaviour
{
    public Button itemDisplayPrefab;
    public InputField quantityPanel;
    public List<SElement> myElementList = new List<SElement>();
    Dictionary<Button, ListItem> myDictionary = new Dictionary<Button, ListItem>();

    void Start()
    {
        foreach (SElement item in myElementList)
        {
            Button theButton = (Button)Instantiate(itemDisplayPrefab);
            theButton.transform.SetParent(this.transform, false);
            ListItem buttonInfo = new ListItem(1, item.name, theButton.GetComponentInChildren<Text>());
            myDictionary.Add(theButton, buttonInfo);
        }

        GameObject.Destroy(itemDisplayPrefab.gameObject);
    }

    public void updateQuantity(Button theButton)
    {
        ListItem myInfo;
        myDictionary.TryGetValue(theButton, out myInfo);
        quantityPanel.text = myInfo.getQuantity().ToString();
    }

    public void recordQuantity()
    {

    }
}
