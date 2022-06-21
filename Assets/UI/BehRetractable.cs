using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehRetractable : MonoBehaviour{
    public float positionX;
    public float positionY;

    void Awake(){

    }

    void Start(){
        
    }

    void Update(){
        
    }

    public void comeHere(){
        transform.localPosition = new Vector3(positionX, positionY, 0);
    }

    public void goAway(){
        transform.localPosition = new Vector3(10000, 10000, 0);
    }
}
