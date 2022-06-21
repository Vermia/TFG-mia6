using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start(){
        QualitySettings.vSyncCount=0;
        Application.targetFrameRate=144;

        
    }

    // Update is called once per frame
    void Update(){
        float maxX = BehBoard.widthInSquares*1.28f-7.72f;
        float maxY = -BehBoard.heightInSquares*1.28f+6.72f;

        float speed = 2 * Time.deltaTime;
        transform.position = new Vector3( transform.position.x + Input.GetAxis("Horizontal") *speed, transform.position.y + Input.GetAxis("Vertical")  * speed, -10);
        if(transform.position.x<=2.85) transform.position=new Vector3(2.85f, transform.position.y, -10);
        if(transform.position.y>=-3.0f) transform.position=new Vector3(transform.position.x, -3.0f, -10);
        if(transform.position.x>=maxX) transform.position=new Vector3(maxX, transform.position.y, -10);
        if(transform.position.y<=maxY) transform.position=new Vector3(transform.position.x, maxY, -10);
    }
}
