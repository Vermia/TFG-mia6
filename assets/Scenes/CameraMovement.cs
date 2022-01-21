using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount=0;
        Application.targetFrameRate=144;
    }

    // Update is called once per frame
    void Update(){
        float speed = 2 * Time.deltaTime;
        transform.position = new Vector3( transform.position.x + Input.GetAxis("Horizontal") *speed, transform.position.y + Input.GetAxis("Vertical")  * speed, -10);
    }
}
