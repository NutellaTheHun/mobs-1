using UnityEngine;
using System.Collections.Generic;

public class ActionType
{
	public string Name;
	public int Layer;
	public float Weight;

	public ActionType(string name, int layer, float weight)
	{
		Name = name;
		Layer = layer;
		Weight = weight;
	}

}

//[RequireComponent (typeof (AgentComponent))]
public class AnimationSelector : MonoBehaviour
{
	public float SpeedX, SpeedZ;
	private int _expression; //for debugging

	private float _scale;
	private UnityEngine.AI.NavMeshAgent _navMeshAgent;
	private AgentComponent _agent;
	private AffectComponent _affect;

	public List<ActionType> Actions = new List<ActionType>();
	//private float _cumSpeed = 0f;
	//private float _moveSpeed = 0f;
	private const int TheStepCnt = 3;



	//DEBUG
	public float VelocityMagnitude; // To see in editor
	public float DesiredVelocityMagnitude; // To see in editor

	public Vector3 LookDir; //To see in gizmos
	public Vector3 AvgDesiredVel;
	public List<Vector3> CumDesiredVel;



	private float _speedDampTime = 0.1f;
	private float _angularSpeedDampTime = 0.25f;
	private float _directionResponseTime = 0.2f;

	Animator _animator;
	private Vector3 _prevPos;

	void Awake()
	{

		Restart();
	}

	void Start()
	{

		AvgDesiredVel = Vector3.zero;

		CumDesiredVel = new List<Vector3>();


		_prevPos = transform.position;
		_navMeshAgent.stoppingDistance = 0f;
	}

	public void Restart()
	{

		AvgDesiredVel = Vector3.zero;
		CumDesiredVel.Clear();

		_navMeshAgent = transform.GetComponent<UnityEngine.AI.NavMeshAgent>();
		_agent = GetComponent<AgentComponent>();
		_affect = GetComponent<AffectComponent>();
		_animator = transform.GetComponent<Animator>();

		_animator.Play("BTMoveForward", 0); //default animation

	}






	public void SetExpression(string expressionName, float weight)
	{

	}


	/*
	public void OnDrawGizmosSelected() {
	    Gizmos.color = Color.red;
	    Gizmos.DrawLine(transform.position, transform.position +LookDir);

	    Gizmos.color = Color.blue;
	    Gizmos.DrawLine(transform.position , transform.position + _navMeshAgent.desiredVelocity);
    }       
	*/


	void OnAnimatorIK(int layerIndex)
	{

		if (_animator.GetCurrentAnimatorStateInfo(2).IsName("pickingUp") && !_agent.IsFighting())
		{
			
			float reach = _animator.GetFloat("RightHandReach");  //param is a curve           
			_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, reach);
			_animator.SetIKPosition(AvatarIKGoal.RightHand, _agent.HandPos);
			//_animator.SetLookAtPosition(_agent.HandPos);
			// _animator.SetLookAtWeight(1.0f);
		}
		else
		{
			_animator.SetLookAtWeight(0f);
		}

	}
	protected void UpdateLocomotion()
	{
		float angle = Vector3.Angle(transform.position + transform.forward, transform.position + _navMeshAgent.desiredVelocity);

		float speed = _navMeshAgent.velocity.magnitude; // _navMeshAgent.desiredVelocity.magnitude;//
		CallLocomotionParameters(speed, angle);

		/*
		float pushAngle = Vector3.Angle(transform.position + transform.forward, transform.position - _prevPos);

		if (pushAngle > 150) {
		    _animator.SetTrigger("StepBack");
		}
		*/
		_prevPos = transform.position;

		/*
	       if (AgentDone()) {
		  //  _animator.SetTrigger("StepBack");
		    _navMeshAgent.ResetPath();


		  Do(0, angle);
		   // Do(0, 0);
		}
		else {
		    //float speed = _navMeshAgent.desiredVelocity.magnitude;
		    float speed = _navMeshAgent.velocity.magnitude;

	      //      Vector3 velocity = Quaternion.Inverse(transform.rotation) * _navMeshAgent.desiredVelocity;


		  // angle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;

		   Do(speed, angle);
		    //Do(speed, 0);

	       }
		*/

	}

	public void CallLocomotionParameters(float speed, float direction)
	{
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

	public bool AgentDone()
	{
		return !_navMeshAgent.pathPending && AgentStopping();
	}

	protected bool AgentStopping()
	{
		return _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;
	}



	public void SelectAction(string actionName)
	{

		
		// _animator.SetBool("IsFallen", false);



		switch (actionName)
		{
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
			case "RECEIVEPUNCH":
				_animator.SetTrigger("ReceivePunch");
				break;
			default:
				break;
		}

	}

	public void Update()
	{



		//FOR DEBUG
		VelocityMagnitude = _navMeshAgent.velocity.magnitude;
		DesiredVelocityMagnitude = _navMeshAgent.desiredVelocity.magnitude;

		//LookDir = transform.forward;//transform.rotation * Vector3.forward;


		//Find average desired velocity for the last  ThestepCnt steps
		if (CumDesiredVel.Count > TheStepCnt)
			CumDesiredVel.RemoveAt(0);
		else
		{
			CumDesiredVel.Add(_navMeshAgent.desiredVelocity);
		}

		AvgDesiredVel = Vector3.zero;
		foreach (Vector3 c in CumDesiredVel)
			AvgDesiredVel += c;
		AvgDesiredVel /= CumDesiredVel.Count;



		UpdateLocomotion();

		/*
		Vector3 nextVel = AvgDesiredVel; // _navMeshAgent.desiredVelocity

		Vector3 currPos = transform.position;
		Vector3 nextPos = transform.position + nextVel;


		//Project nextPos on looking direction


		Vector3 np = Vector3.Project(nextVel, LookDir) + currPos;
		Vector3 newVel = new Vector3((nextPos - np).magnitude, 0, (np - currPos).magnitude);


		float angle = Vector3.Angle(nextVel, LookDir);



		newVel.Normalize(); //normalized in 0-1


		SpeedX = MathDefs.AngleDir(LookDir, nextVel, Vector3.up) * newVel.x;

		  //All should be between -1 and 1
		//SpeedX = newVel.x;

		SpeedZ = newVel.z;
		if (angle > 90)
		    SpeedZ *= -1;
	    //    if (angle > 180)
	      //      SpeedX *= -1;


		_animator.SetFloat("X", SpeedX);
		_animator.SetFloat("Z", SpeedZ);


		//if (_navMeshAgent.velocity.magnitude > 3.6f)
		if (_navMeshAgent.velocity.magnitude > 2f)
		    _animator.SetBool("IsRunning", true);
		else
		    _animator.SetBool("IsRunning", false);

		if(_navMeshAgent.velocity.magnitude < 0.1f)
		    _animator.SetBool("IsMoving", false);
		else
		    _animator.SetBool("IsMoving", true);


		*/


		_animator.SetBool("IsFallen", _agent.IsFallen);

		_animator.SetBool("IsFighting", _agent.IsFighting());  //&& !_navMeshAgent.updatePosition);
		_animator.SetBool("Fighter0", _agent.IsFightStarter);



		_animator.stabilizeFeet = true;

		//SetExpression(_affect.EkmanStr[_affect.DominantEkmanEmotion], _affect.Ekman[_affect.DominantEkmanEmotion]);

		//Should overwrite the face
		//SetExpression(_affect.EkmanStr[_affect.GetExpressionInd()], _affect.GetExpressionValue());

		_animator.SetInteger("ExpressionInd", _affect.GetExpressionInd());
		_animator.SetFloat("ExpressionWeight", _affect.GetExpressionValue());



		//  _animator.SetFloat("VelSideways", SpeedX);
		//  _animator.SetFloat("VelForward",SpeedZ); 


		/*Vector3 currPos = transform.position;
		Vector3 diffVec = currPos - _lastPos;
		float dist = Vector3.Distance(currPos, _lastPos);
		float currSpeed = Mathf.Abs(dist)/Time.deltaTime;


		//_animator.SetFloat("MoveSpeed", currSpeed);
		_animator.SetFloat("VelSideways", diffVec.x);
		_animator.SetFloat("VelForward", Mathf.Abs(diffVec.z)); 
		_lastPos = currPos;
		_speedCnt++;
		_cumSpeed += _navMeshAgent.desiredVelocity.magnitude;
		//if (_speedCnt >= TheStepCnt) { //Take the average of TheStepCnt steps
		//    if (_cumSpeed < 0.1f){// && _currAction[0].Equals("BTMoveForward")) {
		//        _animator.SetInteger("MovementState", 0);     
		//    }
		//    else {//if(_currAction[0].Equals("idle")){
		//        _animator.SetInteger("MovementState", 1);     
		//    }
		//    _cumSpeed = 0;
		//    _speedCnt = 0;
		//}

		*/

		//expressions

		/*
		if (Input.GetKeyDown(KeyCode.Q))
		    _navMeshAgent.speed -= 0.1f;
		    _navMeshAgent.speed -= 0.1f;
		if (Input.GetKeyDown(KeyCode.W))
		    _navMeshAgent.speed += 0.1f;

		*/


		//_animator.Play("BTMoveForward", 0); //always running in the background




		//turning while moving forward
		//        float angle = Vector3.Angle(_navMeshAgent.velocity.normalized, transform.forward);        
		//_animator.SetFloat("TurnDirection", angle);
		//       _animator.Play("BTTurn");
	}

}

