using UnityEngine;
using System.Collections;

public class BuildGui : MonoBehaviour {
    private bool isDragging = false;
    public Transform dragObject;
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 200, 100), "Drag A DragCube") && !isDragging )
        {
            isDragging = true;
            Instantiate(dragObject, BuildBlock.GetMousePositionAtNullPlane(), transform.rotation);
          
        }
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
        
	}
}
