using UnityEngine;
using System.Collections.Generic;



//[RequireComponent (typeof (AgentComponent))]
public class HumanAnimationSelector : MonoBehaviour {
    public float SpeedX, SpeedZ;
    private int _expression; //for debugging
	
    private float _scale;
    
    
    public List<ActionType> Actions = new List<ActionType>();
    ////private float _cumSpeed = 0f;
    ////private float _moveSpeed = 0f;
    //private const int TheStepCnt = 3;

    

    private float _speedDampTime = 0.1f;
	private float _angularSpeedDampTime = 0.25f;
	private float _directionResponseTime = 0.2f;

	Animator _animator;
    HumanComponent _humanComponent;
    

	private Vector3 _prevPos;
    

    void Awake() {

        Restart();
    }

	void Start() {

        //_firstPersonController = GetComponent <FirstPersonController>();
        _prevPos = transform.position;
	    
	}

	public void Restart() {

        _humanComponent = transform.GetComponent<HumanComponent>();

        _animator = transform.GetComponent<Animator>();

        _animator.Play("BTMoveForward", 0); //default animation

	}
    public void FixedUpdate() {



        

        UpdateLocomotion();



    }


    protected void UpdateLocomotion() {
        //float angle = Vector3.Angle(transform.position + transform.forward, transform.position + _navMeshAgent.desiredVelocity);

        //float speed = _navMeshAgent.velocity.magnitude; // _navMeshAgent.desiredVelocity.magnitude;//



        float speed = Vector3.Magnitude(transform.position - _prevPos) / Time.deltaTime;//*10;
        float angle = Vector3.Angle(_prevPos + transform.forward, transform.position + transform.forward);

        
        CallLocomotionParameters(speed, angle);

      
        _prevPos = transform.position;


    }

    public void CallLocomotionParameters(float speed, float direction) {
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
        
        bool inTransition = _animator.IsInTransition(0);
        bool inIdle = state.IsName("Idle");
        bool inTurn = state.IsName("TurnOnSpot") || state.IsName("PlantNTurnLeft") || state.IsName("PlantNTurnRight");
        bool inWalkRun = state.IsName("BTMoveForward");

        float speedDampTime = inIdle ? 0 : _speedDampTime;
        float angularSpeedDampTime = inWalkRun || inTransition ? _angularSpeedDampTime : 0;
        float directionDampTime = inTurn || inTransition ? 1000000 : 0;

        float angularSpeed = direction / _directionResponseTime;

        //_animator.SetFloat("Speed", speed, speedDampTime, Time.deltaTime);
        //_animator.SetFloat("AngularSpeed", angularSpeed, angularSpeedDampTime, Time.deltaTime);
        //_animator.SetFloat("Direction", direction, directionDampTime, Time.deltaTime);

        _animator.SetFloat("AngularSpeed", direction);//directionDampTime, Time.deltaTime);
        _animator.SetFloat("Direction", direction);
        _animator.SetFloat("Speed", speed);
        
    }






    public void SelectAction(string actionName) {        

        switch (actionName) {
            case "YELL0":
                
                _animator.SetTrigger("Yell0");
                break;
            case "YELL1":                
                _animator.SetTrigger("Yell1");
                break;
            case "PICKUP":
                
                _animator.SetTrigger("PickUp");
                break;
            case "DISAPPOINTED":                
                    _animator.SetTrigger("GetDisappointed");
                break;                      
            case "WRITHING":
                _animator.SetBool("IsFallen", true);
                break;
            case "PUNCH":
                _animator.SetTrigger("Punch");                
                break;
            default:
                break;

        }

    }

    
}

