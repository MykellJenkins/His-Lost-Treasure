using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Slot : MonoBehaviour
{
    public GameObject item;
    public int ID;
    public string type;
    public string description;
    public bool empty;
    public Sprite Icon;

    public void UpdateSlot()
    {
        this.GetComponent<Image>().sprite = Icon;
    }
}
