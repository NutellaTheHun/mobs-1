using UnityEngine;
using System.Collections;

public class MoveTarget : MonoBehaviour
{
    public Camera targetCamera;

    // Update is called once per frame
    void LateUpdate()
    {
        
        if (Input.GetMouseButton(0) && Input.GetKey("x"))
        {
            Ray cursorRay = this.targetCamera.ScreenPointToRay(Input.mousePosition);			
            RaycastHit hit;
            if (Physics.Raycast(cursorRay, out hit))
            {
				
                transform.position = hit.point;
            }
        }
    }
}
