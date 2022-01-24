using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BehStartButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClick(){
        Debug.Log("Start");
        BehBoard.startGame();
    }
}
