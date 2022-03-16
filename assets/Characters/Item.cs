using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ItemTypes{
    noItem, gun
}



public class Item{
    public ItemTypes type;
    public int charges;



    public Item(){
        type = ItemTypes.gun; charges=5;
    }

    public Item(ItemTypes ptype, int pcharges){
        type=ptype; charges = pcharges;
    }
}