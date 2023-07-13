
using UnityEngine;
using System.Collections;


public class CameraAgent : MonoBehaviour
{
    public Transform FollowedAgent;
    
    void Start() { Init(); }
    void OnEnable() { Init(); }

    public void Init() {
        transform.position = FollowedAgent.position;

    }

    void LateUpdate()    {
        //Follow from the face
        transform.position = FollowedAgent.position + Vector3.up * 1.5f + FollowedAgent.GetComponent<UnityEngine.AI.NavMeshAgent>().velocity.normalized *2;
        transform.LookAt(FollowedAgent.position + Vector3.up * 1.5f);

        
        //First person
        /*
        transform.position = FollowedAgent.position + Vector3.up * 1.5f ;
        transform.rotation = FollowedAgent.rotation;
        */
    }

}
