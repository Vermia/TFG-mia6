using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BehResetButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start(){
        GetComponent<Button>().onClick.AddListener(OnMouseDown);
    }

    // Update is called once per frame
    void Update(){
        
    }

    void OnMouseDown(){
        Debug.Log("Reset");

        GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>().retractOrExpand();
    }
}
