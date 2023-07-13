
using UnityEngine;
using System.Collections;



public class CashierComponent : MonoBehaviour {


	public void LookAt(Vector3 dest, float speed) {
		Vector3 lookDir = dest - transform.position;
		lookDir.y = 0;

		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), Time.time * speed);
	}


	private void OnTriggerStay(Collider collider) {

		
		if(collider.CompareTag("RealPlayer")) 
			LookAt(collider.transform.position, 0.2f);


	}

}




