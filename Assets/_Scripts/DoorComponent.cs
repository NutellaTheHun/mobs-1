using UnityEngine;
using System.Collections;

public class DoorComponent : MonoBehaviour {


    
    private Vector3 _stopPos;
    public Vector3 Vel;
    private float _speed;

    void Awake() {
        
        _speed = 0.2f;

        _stopPos = transform.position + Vel;
    }

    
	// Update is called once per frame
	void Update () {
	    Open();
	}

    void Open() {

        if (Vector3.Distance(transform.position, _stopPos) > 0.1f) 
            transform.position += _speed*Vel*Time.deltaTime;            
       /* else {
            GameObject[] agents = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject a in agents) {
                if (a.GetComponent<ShopperBehavior>())
                    a.SendMessage("DoorOpen");
            }
        }*/
    }

    

}

