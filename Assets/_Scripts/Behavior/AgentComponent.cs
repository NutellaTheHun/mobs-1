using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent (typeof (AffectComponent))]
public class AgentComponent : MonoBehaviour, GeneralStateComponent
{
	public MeshCreator VisibilityMesh;
	ParticleSystem _particleSystem;
	UnityEngine.AI.NavMeshAgent _navMeshAgent;
	Appraisal _appraisal;
	AffectComponent _affectComponent;
	Animator _animator;
    //private AnimationSelector _animationSelector; //yelling in EmotionalBehaviorUpdate() commented out for VR switch, MType.Disdainful and MType.Bored commented out
    private VRShopperAnimationController _shopperAnimationController;
	//public GameObject IndicatorAgent;
	//public GameObject IndicatorParticle;
	//public GameObject IndicatorCircle;


	public int Id;
	public float WalkingSpeed = 1f;//0.02f;//1f; //Computed according to Personality
	private Vector3 _initialPos;
	public float Damage;
	public int GroupId;
	public float PanicThreshold = 0.5f; //between 0 and 1 computed according to Personality ( Initial Personality 0)
	public float WaitingThreshold = 3f;
	public float PanicLevel;
	public ArrayList CollidingAgents;
	public float TimeLastFight = 0f; //end time of the last fight
	public float TimeLastFall = 0f; //end time of the last fight
	public float WaitDuration; //Duration to wait until a new path is set. Proportional to patience
	private float VisibilityAngle = 60;//Mathf.PI/ 3f; //Mathf.PI * 2f; //Mathf.PI / 3f;
	float _visibilityDist;

	Color _expressionColor;
	private float _colorTime;

	const float _maxSpeed = 3.8f;//0.076f;// 3.8f * Time.fixedDeltaTime;

	Vector3 _prevPos; //position in a previous frame
	private float _posChange;
	private float _timeLastPosChange;
	public bool WaitedTooLong = false;
	public bool WaitingOnPurpose = false;
	public bool IsFightStarter = false;
	public bool IsWatchingFight = false; //is the agent watching a fight. updated in fightbehavior
	public Vector3 Impact; //character momentum
			       //DEBUG
	public bool IsPushed = false;
	public bool IsFallen = false;
	public bool StartedWaiting = false;
	public bool FinishedWaiting = false;
	public Vector3 HandPos; //for picking up objects
	public bool NavMeshAgentWorking;

	// Use this for initialization
	void Awake()
	{
		VisibilityMesh = null;
		CollidingAgents = new ArrayList(); //agents that are colliding with me

		if (!GetComponent<UnityEngine.AI.NavMeshAgent>())
			gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();

		//_animationSelector = GetComponent<AnimationSelector>();
		_navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		_appraisal = GetComponent<Appraisal>();
		_affectComponent = GetComponent<AffectComponent>();
		_navMeshAgent.speed = WalkingSpeed; //maximum speed

		_navMeshAgent.angularSpeed = 360;
		_navMeshAgent.updatePosition = true; //false ? for resetting the position
		GetComponent<PersonalityMapper>().PersonalityToSteering(); //initialize steering parameters according to Personality
		_initialPos = transform.position;

		//Set collider size :WARNING: Temporary
		SphereCollider col = this.GetComponent<SphereCollider>();
		//col.radius = 2f; //to test escapes and ascribe --normally 4f
		col.radius = 4f; 

		_visibilityDist = col.radius; //* 2f;
		_prevPos = transform.position;
		_navMeshAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;//ObstacleAvoidanceType.NoObstacleAvoidance; 
		_timeLastPosChange = Time.time;

		Impact = Vector3.zero;
	}

	void Start()
	{
        _navMeshAgent.avoidancePriority = 9;
        //StaggerStart(); //Enables nav mesh agent component to help stagger the crowd of shoppers into the store
        //Added by Nathan Brilmayer FOR VR
        _shopperAnimationController = GetComponent<VRShopperAnimationController>();
        _animator = GetComponentInChildren<Animator>();
        //IndicatorAgent = (GameObject)Instantiate(Resources.Load("Indicator"), transform.position + Vector3.up, transform.rotation);
        //IndicatorParticle = (GameObject)Instantiate(Resources.Load("IndicatorParticle"), transform.position + Vector3.up *2.2f, transform.rotation);
        //IndicatorCircle = (GameObject)Instantiate(Resources.Load("IndicatorCircle"), transform.position + Vector3.up * 2.3f , transform.rotation);
        //IndicatorCircle.transform.parent = transform;
        //IndicatorCircle.transform.localScale = new Vector3(_navMeshAgent.radius * 2, 0, _navMeshAgent.radius * 2);

        //IndicatorParticle.transform.Rotate(-90, 0, 0); //for the particle system to be parallel to the table plane
        //IndicatorAgent.transform.parent = transform;        
        //IndicatorParticle.transform.parent = transform;
        //IndicatorAgent.transform.localScale = new Vector3(_navMeshAgent.radius, 1f, _navMeshAgent.radius); //height for seeing indicator
        //_particleSystem = new ParticleSystem();

        //_particleSystem = IndicatorParticle.GetComponent<ParticleSystem>();
        //IndicatorParticle.GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, 0f); //always invisible 
        //IndicatorParticle.SetActive(false);
        //_particleSystem.enableEmission = false;
        //_expressionColor = new Color(1,1,1);
        _timeLastPosChange = Time.time;

		//if(GetComponentInChildren<AnimationSelector>() &&  GetComponent<AnimationSelector>().enabled) {
		//    IndicatorAgent.SetActive(false);
		//    IndicatorCircle.SetActive(false);
		//}
		//else {
		//    IndicatorAgent.SetActive(true);
		//    IndicatorCircle.SetActive(false);
		//}
		Random.InitState(Id);

	}

    public void StaggerStart()
    {
        Random.InitState(gameObject.GetInstanceID());
        float num = Random.Range(0f, 4f);
		//Debug.Log(transform.name + " " + num);
		StartCoroutine(EnableNavMeshAgent(num));
    }

    IEnumerator EnableNavMeshAgent(float num)
    {
        yield return new WaitForSeconds(num);
		_navMeshAgent.enabled = true;
    }

    public void Restart()
	{
		_navMeshAgent.speed = WalkingSpeed; //maximum speed
		_navMeshAgent.angularSpeed = 360;
		_navMeshAgent.updatePosition = false; //for resetting the position
		GetComponent<PersonalityMapper>().PersonalityToSteering(); //update steering parameters in navmesh according to Personality
		transform.position = _initialPos;
		Damage = 0f;
		_navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

		if (IsFighting())
		{
			DestroyImmediate(GetComponent<FightBehavior>());
			_shopperAnimationController.EnterMovingState();
        }

		_timeLastPosChange = Time.time;
		IsWatchingFight = false;
		Impact = Vector3.zero;
		GetComponent<UnityEngine.AI.NavMeshAgent>().updatePosition = true;
	}

	

	void UpdateWaitingTime()
	{
		if ((Time.time - _timeLastPosChange) >= 5)
		{
			_posChange = Vector3.Distance(transform.position, _prevPos);
			_timeLastPosChange = Time.time;
			if (WaitingOnPurpose || _posChange >= WaitingThreshold)
			{//(Time.deltaTime * _navMeshAgent.speed) / 1.5f) {
				WaitedTooLong = false;
			}
			else
			{
				WaitedTooLong = true;
			}
			_prevPos = transform.position;
		}
	}

	void LateUpdate()
	{
		//if (IsFallen) {
		//   IndicatorAgent.transform.Rotate(90, 0, 0);
		//}
		if(_navMeshAgent.velocity.magnitude > 0.3)
		{
			_animator.SetBool("VRIK_IsMoving", true);
		}
		else
		{
            _animator.SetBool("VRIK_IsMoving", false);
        }
		if (Impact.magnitude > 0.2)
		{
			SteerTo(transform.position + Impact * Time.deltaTime);
			_navMeshAgent.updateRotation = false;
			_navMeshAgent.updateRotation = false;
			_navMeshAgent.updateRotation = false;
		}
		else
		{
			_navMeshAgent.updateRotation = true;
		}	}
	void FixedUpdate()
	{

		// impact vanishes to zero over time
		Impact = Vector3.Lerp(Impact, Vector3.zero, 2 * Time.deltaTime);

		//AffectUpdate(); //our method
				//_expressionColor = _affectComponent.GetExpressionColor();
				//UpdateIndicator();

		if (IsFighting() == false)
		{
			if (TimeSinceLastFight() > 10) //start healing after 10 seconds
				Heal();
		}
		if (IsDead())
		{
			// _navMeshAgent.Stop();
			_navMeshAgent.updatePosition = false;
			_navMeshAgent.updateRotation = false;
			return;
		}

		//FALLING TURNED OFF
		if (Time.timeSinceLevelLoad > 3) //skip the beginning where agents collide
			CheckFallingStandingUp();

		if (IsFallen)
		{
			_navMeshAgent.updatePosition = false;
			_navMeshAgent.updateRotation = false;
		}

		NavMeshAgentWorking = _navMeshAgent.updatePosition;

		//if(VisibilityMesh != null)
		//    RenderViewingZone();


	}



	//Select behaviors based on emotions
	//Agent component script is executed before role components, so current actions in role components overwrite the ones here
	void EmotionalBehaviorUpdate()
	{
		switch (_affectComponent.GetCurrMoodOctant())
		{
			case (int)MType.Hostile:
				
				if (_affectComponent.GetExpressionRange() == EmotionRange.High)
				{
					_navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, _maxSpeed, Time.deltaTime);
					foreach (GameObject c in CollidingAgents)
					{
						if (IsGoodToFight(c, 5f))
						{
							StartFight(c, true);
							c.GetComponent<GeneralStateComponent>().StartFight(gameObject, false);
						}
					}
				}
				

				//If still not fighting
				/*if (!IsFighting())
				{
					int val = Random.Range(0, 2);
					if (val == 0)
						_animationSelector.SelectAction("YELL0");
					//CurrAction[3] = "yelling0";
					else
						_animationSelector.SelectAction("YELL1");
					//CurrAction[3] = "yelling1";
				}
				else if (_affectComponent.GetExpressionRange() == EmotionRange.Moderate)
				{
					//If not found someone to fight, just yell
					if (!IsFighting())
					{
						int val = Random.Range(0, 2);
						if (val == 0)
							_animationSelector.SelectAction("YELLG0");
						//CurrAction[3] = "yelling0";
						else
							_animationSelector.SelectAction("YELL1");
						//CurrAction[3] = "yelling1";
					}
				}
				else //low anger
					_navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, WalkingSpeed, Time.deltaTime);*/
				break;
			case (int)MType.Exuberant:
				if (IsShopper())
				{
					_navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, _maxSpeed, Time.deltaTime);
				}
				break;
			case (int)MType.Docile:
				_navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, WalkingSpeed, Time.deltaTime);
				break;
			case (int)MType.Relaxed:
				_navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, WalkingSpeed, Time.deltaTime);
				break;
			case (int)MType.Anxious:
				if (_affectComponent.GetExpressionRange() == EmotionRange.High)
				{
					_navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, _maxSpeed, Time.deltaTime);
				}
				else if (_affectComponent.GetExpressionRange() == EmotionRange.Moderate)
					_navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, _maxSpeed, Time.deltaTime);
				else
					_navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, WalkingSpeed, Time.deltaTime);
				break;
			case (int)MType.Bored:
				if (_affectComponent.GetExpressionRange() == EmotionRange.High)
				{
					_navMeshAgent.speed = Mathf.Lerp(_navMeshAgent.speed, WalkingSpeed, Time.deltaTime);
					/*if (MathDefs.GetRandomNumber(5) > 3)
						_animationSelector.SelectAction("DISAPPOINTED");*/
				}
				break;

			case (int)MType.Disdainful:
				/*if (_affectComponent.GetExpressionRange() == EmotionRange.High)
					_animationSelector.SelectAction("DISAPPOINTED");*/
				break;
		}

	}



	void AffectUpdate()
	{
		if (!IsFighting())
		{
			_navMeshAgent.updatePosition = true;
			_navMeshAgent.updateRotation = true;
		}
		float[] eventFactor = _appraisal.ComputeEventFactor();
		//only if susceptible 

		//Emotion contagion
		Dictionary<int, float> lambdaList = new Dictionary<int, float>();//indices of dominant emotions around me
										 //List<IndexValuePair> lambdaList = new List<IndexValuePair>(); //indices of dominant emotions around us
		if (_affectComponent.ContagionMode)
		{
			foreach (GameObject c in CollidingAgents)
			{
				//Ensures that agents within a radius are considered
				//if (!c.CompareTag("RealPlayer") && IsVisible(c, VisibilityAngle)) {
				if (c.CompareTag("Player") && IsVisible(c, VisibilityAngle))
				{

					//if c is in my visual cone and within a certain proximity                
					for (int eInd = 0; eInd < c.GetComponent<AffectComponent>().Emotion.Length; eInd++)
					{
						if (c.GetComponent<AffectComponent>().CanExpress(eInd))
						{
							if (lambdaList.ContainsKey(eInd))
							{
								lambdaList[eInd] += c.GetComponent<AffectComponent>().Emotion[eInd];
								lambdaList[eInd] /= 2; //for normalization
							}
							else
								lambdaList.Add(eInd, c.GetComponent<AffectComponent>().Emotion[eInd]);
						}
					}
				}
			}
		}
		_affectComponent.ComputeEmotion(eventFactor, lambdaList);
		lambdaList.Clear();
		_affectComponent.ComputeMood();
	}

	

	public void SteerTo(Vector3 pos)
	{
		//pos.y = 0f;//_navMeshAgent.transform.position.y; //always assume they are on the same level
		if(_navMeshAgent.enabled == true)
		_navMeshAgent.SetDestination(pos);
		
	}

	///Steer away from the location pos
	public void SteerFrom(Vector3 pos)
	{
		Vector3 dir = transform.position - pos;
		dir.y = 0f;
		dir.Normalize();
		_navMeshAgent.SetDestination(transform.position + dir);
	}

	/// Finds the agents around me
	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("RealPlayer"))
		{
			if (!CollidingAgents.Contains(collider))
				CollidingAgents.Add(collider.gameObject);
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("RealPlayer"))
		{
			CollidingAgents.Remove(collider.gameObject);
		}
	}

	public void AddDamage(float amount)
	{
		Damage += amount * Random.Range(0, 2) * Time.deltaTime;
	}

	public float GetDamage() {
		return Damage;
	}

	public void Heal()
	{
		//if (IsWounded())
		//{
		if(Damage > 0) {
			Damage -= 0.1f * Time.deltaTime;
		}

		
		if (!IsWounded())
			ResetSpeed();
		//}
	}
	public bool IsDead()
	{
		return Damage > 3;
	}

	public bool IsWounded()
	{
		return (!IsDead() && Damage > 1);
	}

	public void AdjustEmotionalSpeed()
	{

	}

	public void IncreaseSpeed(int c)
	{

		_navMeshAgent.speed += c * 0.2f * Time.deltaTime;
		if (_navMeshAgent.speed > _maxSpeed)
			_navMeshAgent.speed = _maxSpeed;
	}

	public void DecreaseSpeed(int c)
	{
		_navMeshAgent.speed -= c * 0.2f * Time.deltaTime;
		if (_navMeshAgent.speed <= WalkingSpeed)
			_navMeshAgent.speed = WalkingSpeed;
	}

	public void ResetSpeed()
	{
		_navMeshAgent.speed = WalkingSpeed;
	}

	public void IncreasePanic()
	{
		PanicLevel += 0.1f * Time.deltaTime;
		if (PanicLevel > 1f)
			PanicLevel = 1f;

		if (IsPanicking())
			IncreaseSpeed(2);

	}
	public void DecreasePanic()
	{
		PanicLevel -= 0.1f * Time.deltaTime;
		if (PanicLevel < 0f)
			PanicLevel = 0f;

		if (IsPanicking() && _navMeshAgent.speed > WalkingSpeed)
			DecreaseSpeed(1);
		else
			_navMeshAgent.speed = WalkingSpeed;
	}

	public void CalmDown()
	{
		PanicLevel = 0f;
		_navMeshAgent.speed = WalkingSpeed;
	}

	public bool IsPanicking()
	{
		return PanicLevel > PanicThreshold;
	}

	/// If other is in my visual cone 
	public bool IsVisible(GameObject other, float viewAngle)
	{
		Vector3 orientation = transform.forward;//GetComponent<NavMeshAgent>().velocity;
		Vector3 distVec = other.transform.position - transform.position;
		orientation.y = distVec.y = 0f;
		orientation.Normalize();
		distVec.Normalize();
		float angle = Vector3.Angle(orientation, distVec);//Mathf.Acos(Vector3.Dot(distVec, orientation));
		if (angle <= VisibilityAngle)
			return true;
		return false;
	}

	public void LookAt(Vector3 dest, float speed)
	{
		Vector3 lookDir = dest - transform.position;
		lookDir.y = 0;
		_navMeshAgent.updateRotation = false; //rotate towards the desired object

		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), Time.time * speed);
	}

	public void Watch(GameObject other)
	{
		Quaternion wantedRotation = Quaternion.LookRotation(other.transform.position - transform.position, Vector3.up);
		transform.rotation = Quaternion.Lerp(transform.rotation, wantedRotation, Time.deltaTime);
	}
	public void Watch(Vector3 pos)
	{
		Quaternion wantedRotation = Quaternion.LookRotation(pos - transform.position, Vector3.up);
		transform.rotation = Quaternion.Lerp(transform.rotation, wantedRotation, Time.deltaTime);
	}
	public void Face(GameObject other)
	{
		if (other.CompareTag("Player"))
		{
			SteerTo(other.transform.position + other.GetComponent<UnityEngine.AI.NavMeshAgent>().desiredVelocity.normalized);
		}
		else if (other.CompareTag("RealPlayer"))
		{
			SteerTo(other.transform.position);
		}
	}

	public bool IsFacing(GameObject other)
	{
		//(other.transform.position - transform.position).magnitude < 2f &&
		return (IsVisible(other, Mathf.PI / 3f) && other.GetComponent<GeneralStateComponent>().IsVisible(this.gameObject, Mathf.PI / 3f));
	}

	public void AddImpact(Vector3 force)
	{

		Impact += force; //dir.normalized * force.magnitude / 2f; //vel/ mass

	}
	public bool IsBeingPushed()
	{
		float pushCoef = Mathf.Abs(_navMeshAgent.velocity.magnitude / _navMeshAgent.speed);
		IsPushed = pushCoef > 5 && pushCoef < 10;
		return IsPushed;
	}

	void Fall()
	{
		
		if (!IsFallen)
		{
			IsFallen = IsBeingPushed() && Random.Range(0, 40) < 1;
			if (IsFallen)
			{
				_navMeshAgent.updatePosition = false;
				_navMeshAgent.updateRotation = false;
				_navMeshAgent.avoidancePriority = 0;
				_navMeshAgent.angularSpeed = 0;
				_navMeshAgent.speed = 0;
				_navMeshAgent.radius = 1.2f;
				//IndicatorAgent.transform.Translate(0, -1.5f, 0);
				UpdateAppraisalStatus();
				TimeLastFall = Time.time;
			}
		}
		

	}

	void StandUp()
	{ //stand up from falling position
		if (IsFallen)
		{  //if enough time has passed
		   //if(Random.Range(0, 10) < 1) {            
			if (Time.time - TimeLastFall > 10)
			{
				_navMeshAgent.updatePosition = true;
				_navMeshAgent.updateRotation = true;
				//    _navMeshAgent.Resume();
				gameObject.GetComponent<PersonalityMapper>().PersonalityToSteering(); //reset to default values
				IsFallen = false;
				//IndicatorAgent.transform.Translate(0, 1.5f, 0);
			}
		}
	}

	void CheckFallingStandingUp()
	{
		Fall();
		StandUp();
		if (IsFallen)
		{
			AddDamage(0.1f); //add damage  
		}

	}


	public bool IsShopper()
	{
		return GetComponent<ShopperBehavior>() != null;
	}

	public bool IsFighting()
	{
		return (GetComponent<FightBehavior>() != null);
	}
	public bool HasFallen()
	{
		return IsFallen;
	}
	public bool CanFight()
	{
		if (IsFighting()) //already fighting
			return false;
		if (IsWounded())
			return false;
		if (IsFallen)
			return false;
		return true;
	}

	public IEnumerator WaitAWhile(int seconds)
	{
		StartedWaiting = true;
		FinishedWaiting = false;
		yield return new WaitForSeconds(seconds);

		FinishedWaiting = true;

		//Dont't forget to set started waiting to false before state change
	}

	///Returns true if a fight conditions are met
	public bool IsGoodToFight(GameObject other, float minDist)
	{
		//dist < 3f seklindeydi
		if (Vector3.Distance(other.transform.position, transform.position) < minDist && CanFight() && other.GetComponent<GeneralStateComponent>().CanFight()){
			// only fight with real player ?
			
		// if (other == null) {
		//     Debug.LogError("Opponent is null in fight");
		//     return false;
		// }
			if (other.CompareTag("RealPlayer"))
			{
				//Debug.Log("fight");
				return true;
			}

			if (other.CompareTag("Player") && (_appraisal.DoesStandardExist(other, AppDef.Disapproving) || _appraisal.DoesStandardExist(other.transform.parent.gameObject, AppDef.Disapproving)))
				return true;
		}
		return false;
	}


	/// Start a fight with the other agent	
	public void StartFight(GameObject other, bool isStarter)
	{
		IsFightStarter = isStarter;
		if (other == null)
		{
			Debug.LogError("Opponent is null in fight");
			return;
		}
		if (GetComponent("FightBehavior") == null)
		{
			this.gameObject.AddComponent<FightBehavior>();
			GetComponent<FightBehavior>().Init(other);
			IsFightStarter = isStarter;
		}
	}

	public float TimeSinceLastFight()
	{
		return Time.time - TimeLastFight;
	}

	public void DeactivateOtherBehaviors()
	{
		

		//Stop fighting    
		if (GetComponent<FightBehavior>())
		{
			GetComponent<FightBehavior>().FinishFight();
		}

		GetComponent<UnityEngine.AI.NavMeshAgent>().updateRotation = true; //in case police vigiling is stopped in the middle

	}


	//Appraisal status for falling
	void UpdateAppraisalStatus()
	{
		if (IsFallen)
			_appraisal.AddGoal("falling", 0.5f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);
		else
		{
			float wt = _appraisal.RemoveGoal("falling", AppDef.Displeased, AppDef.ConsequenceForSelf,
							    AppDef.ProspectRelevant, AppDef.Confirmed);
			if (wt != 0)
			{
				_appraisal.AddGoal("falling", 0.5f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed);
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		if (_navMeshAgent)
			Gizmos.DrawLine(transform.position, transform.position + Impact);
	}
}



