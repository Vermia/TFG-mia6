using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vector2i{
    public int x, y;
    public Vector2i(int px, int py){
        x=px;y=py;
    }
}

public class BehSquare : MonoBehaviour
{
    public int i{get; private set;}
    public int j{get;private set;}
    public GameObject occupant;

    // Start is called before the first frame update
    void Start() {
        /*Font arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        GameObject canvasGO = new GameObject();
        canvasGO.name = "Canvas";
        canvasGO.AddComponent<Canvas>();
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Get canvas from the GameObject.
        Canvas canvas;
        canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Create the Text GameObject.
        GameObject textGO = new GameObject();
        textGO.transform.parent = canvasGO.transform;
        textGO.AddComponent<Text>();
       // textGO.transform.localPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Set Text component properties.
        text = textGO.GetComponent<Text>();
        text.text = "[" + i + ", " + j + "]";
        text.fontSize = 20;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = arial;
        */
    }

    // Update is called once per frame
    void Update(){
        //text.GetComponent<RectTransform>().localPosition = Camera.main.WorldToScreenPoint( new Vector2(  transform.localPosition.x-10.3333f, transform.localPosition.y-5f )  );
    }

    void setPosition(Vector2i p){
        i=p.x; j=p.y;
    }

    void setOccupant(GameObject poccupant){
        occupant = poccupant;
        //transform.position = new Vector2( occupant.transform.position.x, occupant.transform.position.y );
    }
}


