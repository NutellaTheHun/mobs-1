#define ASCRIBE
using UnityEngine;
using System.Collections;

public class AudienceBehavior : MonoBehaviour {
    UnityEngine.AI.NavMeshAgent _navMeshAgent;
    Vector3 _targetPos;
    AgentComponent _agentComponent;
    int _targetId;
    GameObject[] _targets;
    public GameObject Target; //current target
	void Start()  {
        Restart();
	
	}

    public void Restart() {
        //InitAppraisalStatus();
        _targets = new GameObject[GameObject.FindGameObjectsWithTag("Target").Length] ;
        _targets = GameObject.FindGameObjectsWithTag("Target");
        
        _agentComponent = GetComponent<AgentComponent>();
        _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
   //     if(Target !=null)
     //       _agentComponent.SteerTo(Target.transform.position);
       // _agentComponent.CurrAction = "";
        
        SetTarget();
        }
    
    void SetTarget() {
        
        
        _targetId = Random.Range(0, _targets.Length);//_agentComponent.Id % _targets.Length;
       Target = _targets[_targetId];

#if ASCRIBE
       Target.transform.position = transform.position;
#endif
        _agentComponent.SteerTo(Target.transform.position);
        
    }

	void FixedUpdate ()  {
        
        float distToTarget = (Target.transform.position - transform.position).magnitude;

        if (distToTarget < 1f) //change target
            SetTarget();

	}

    
}