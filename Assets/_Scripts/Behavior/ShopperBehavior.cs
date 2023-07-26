//#define WAITING_AT_ENTRANCE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

public class ShopperBehavior : MonoBehaviour
{
	public bool goneLeft = false;
    public bool goneRight = false;
    Appraisal _appraisal;
	AgentComponent _agentComponent;
	AffectComponent _affectComponent;
	UnityEngine.AI.NavMeshAgent _navmeshAgent;
	GameObject _counter;
	GameObject _cashier;
	[SerializeField]
	private Transform _desiredObj;
	//public List<Transform> CurrentObjs = new List<Transform>();
	public GameObject CurrentObjs;
	Vector3 _exit;
	GameObject _objs;
	private GameObject[] _allObjs;

	[SerializeField]
	public int _desiredObjCnt;
	int _acquiredObjCnt;


	private VR_ShelfComponent _shelfComp; //changed from ShelfComponent
	[SerializeField]
	int _totalObjCnt = 0;
	bool _allConsumed = false;

	public Transform _rightHand, _leftHand;


	private bool _finishedWaitingAtEntrance;

	[SerializeField]
	private Vector3 _closestShelfPos;
	[SerializeField]
	private int _shelfInd;
    [SerializeField]
    int[] _shelfOrder; //shelf visiting order
	private AnimationSelector _animationSelector;
	private GUIHandler _guiHandler;

	//Variables made by Nathan Brilmayer
	private const int numberOfIsles = 11; //hard coded for shopper world
    private const int totalShoppers = 21; //hard coded for shopper world
    
	//Author: Nathan Brilmayer, used for floating counter above AI for aquired objects
	aquiredObjUI _aquiredObjUI;
    aquiredObjCanvasManager _aquiredObjCanvasManager;

    [SerializeField]
	private int _state;
	public int State
	{
		get { return _state; }
		set { _state = value; }
	}

	public enum ShoppingState
	{
		ShelfChanging,
		Paying,
		WaitingInLine,
		GoingToLine,
		GoingToObject,
		PickingUpObject,
		Exiting
	}

    private void Awake() //neccessary that all shoppers have first isle set before createShelfOrder() is run
    {
        _shelfComp = GameObject.Find("Shelves").GetComponent<VR_ShelfComponent>(); //Changed from ShelfComponent to VR_ShelfComponent
        initializeFirstShelf();
    }
    // Use this for initialization
    void Start()
	{
		_appraisal = GetComponent<Appraisal>();
		_agentComponent = GetComponent<AgentComponent>();
		_affectComponent = GetComponent<AffectComponent>();
		_navmeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		_animationSelector = GetComponent<AnimationSelector>();
        _shelfComp = GameObject.Find("Shelves").GetComponent<VR_ShelfComponent>(); //Changed from ShelfComponent to VR_ShelfComponent


        _guiHandler = FindObjectOfType(typeof(GUIHandler)) as GUIHandler;


		_allObjs = new GameObject[_shelfComp.transform.childCount];
		for (int i = 0; i < _allObjs.Length; i++)
			_allObjs[i] = _shelfComp.transform.GetChild(i).gameObject;


		//   _navmeshAgent.radius -= 0.1f; //smaller than regular size
		//_navmeshAgent.speed += 0.6f; //faster than regular speed

		//_navmeshAgent.radius *= 2f;//0.5f;

		_exit = GameObject.Find("Exit").transform.position;

		_counter = GameObject.Find("counter");
		_cashier = GameObject.Find("cashier");

		//IMPORTANT! We cannot use the same model twice because we are using the same avatar?? 

		_rightHand = transform.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
		_leftHand = transform.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftHand);


		_acquiredObjCnt = 0;
		
		_desiredObjCnt = (int)(2f * _affectComponent.Personality[(int)OCEAN.E] + 20f); //correlated to extroversion [10 40]


		InitAppraisalStatus();

		_navmeshAgent.autoRepath = true;
		_navmeshAgent.autoBraking = true;


		Random.InitState(_agentComponent.Id);


		//int randInd = Random.Range(0, 11);

		
        createShelfOrder();
        /*if (randInd == 0)
		{
			_shelfOrder[0] = 0; _shelfOrder[1] = 1; _shelfOrder[2] = 2; _shelfOrder[3] = 3; _shelfOrder[4] = 4; _shelfOrder[5] = 5;
		}
		else if (randInd == 1)
		{
			_shelfOrder[0] = 1; _shelfOrder[1] = 2; _shelfOrder[2] = 3; _shelfOrder[3] = 4; _shelfOrder[4] = 5; _shelfOrder[5] = 0;
		}
		else if (randInd == 2)
		{
			_shelfOrder[0] = 2; _shelfOrder[1] = 3; _shelfOrder[2] = 4; _shelfOrder[3] = 5; _shelfOrder[4] = 1; _shelfOrder[5] = 0;
		}
		else if (randInd == 3)
		{
			_shelfOrder[0] = 3; _shelfOrder[1] = 4; _shelfOrder[2] = 5; _shelfOrder[3] = 2; _shelfOrder[4] = 1; _shelfOrder[5] = 0;
		}
		else if (randInd == 4)
		{
			_shelfOrder[0] = 4; _shelfOrder[1] = 5; _shelfOrder[2] = 3; _shelfOrder[3] = 2; _shelfOrder[4] = 1; _shelfOrder[5] = 0;
		}
		else if (randInd == 5)
		{
			_shelfOrder[0] = 5; _shelfOrder[1] = 4; _shelfOrder[2] = 3; _shelfOrder[3] = 2; _shelfOrder[4] = 1; _shelfOrder[5] = 0;
		}*/


        _objs = GameObject.Find("Objects" + _shelfOrder[0]);


		State = (int)ShoppingState.GoingToObject;



		_navmeshAgent.speed += 0.6f; //faster than usual


		CurrentObjs = new GameObject("AchievedObjects");
		CurrentObjs.transform.parent = this.transform;
        
		_aquiredObjCanvasManager = GameObject.Find("Canvas").GetComponentInChildren<aquiredObjCanvasManager>();

        _aquiredObjCanvasManager.initializeShopperCounterUI(this);
    }
	public void setAquiredUI(aquiredObjUI aou)
	{
		_aquiredObjUI = aou;
    }
    public int getAquiredObjCount()
	{
		return _acquiredObjCnt;
	}
    private void initializeFirstShelf() //ensures all shoppers have shelf[0] set so shelfComponents index of shoppers per isle is set
	{
        _shelfOrder = new int[numberOfIsles]; //Randomize shelf order for now
		_shelfOrder[0] = Random.Range(0, 10);
		//_shelfOrder[0] = 1;
        _shelfComp.incrementShopper(_shelfOrder[0]);
    }
    private void createShelfOrder()
	{
		float isleP = (float)_shelfOrder[0] / (float)numberOfIsles;
		float shopperP = (float)_shelfComp.getShopperCount(_shelfOrder[0]) / (float)totalShoppers;
        float Normalized_goLeft_p = (isleP + shopperP) /2;
		float randomF = Random.Range(0f, 1f);
         if (Normalized_goLeft_p < randomF)
         {
             goLeftwards();
			goneLeft = true;
         }
         else
         {
             goRightwards();
			goneRight = true;
         }
    }
	private void goLeftwards()
	{
        if (_shelfOrder[0] == _shelfOrder.Length - 1)
		{
            for (int i = 1; i < _shelfOrder[0]; i++)
            {
                _shelfOrder[i] = _shelfOrder[0] - i;
            }
			return;
        }

        for (int i = 1; i <= _shelfOrder[0]; i++)
		{
			_shelfOrder[i] = _shelfOrder[0] - i;
        }

        int temp = 1;
        for (int i = _shelfOrder[0]+1; i < numberOfIsles; i++)
        {
            _shelfOrder[i] = _shelfOrder[0] + temp;
            temp++;
        }
    }

    private void goRightwards()
    {
        if (_shelfOrder[0] == 0)
		{
            for (int i = 1; i < numberOfIsles; i++)
            {
                _shelfOrder[i] = _shelfOrder[0] + i;
            }
			return;
        }

        for (int i = 1; i < numberOfIsles - _shelfOrder[0]; i++)
		{
			_shelfOrder[i] = _shelfOrder[0] + i;
        }

		int temp = 1;
        for (int i = numberOfIsles - _shelfOrder[0]; i < numberOfIsles; i++)
        {
            _shelfOrder[i] = _shelfOrder[0] - temp;
			temp++;
        }
    }


    public IEnumerator WaitAtEntrance(int seconds)
	{
		_finishedWaitingAtEntrance = false;
		yield return new WaitForSeconds(seconds);

		_finishedWaitingAtEntrance = true;


		//Dont't forget to set started waiting to false before state change
	}

	public void Restart()
	{
		InitAppraisalStatus();
		UpdateState();
		_acquiredObjCnt = 0;

		GameObject _objs = GameObject.Find("Objects" + _shelfOrder[0]);

		for (int i = 0; i < _objs.transform.childCount; i++)
		{
			_objs.transform.GetChild(i).gameObject.SetActive(true);
			_objs.transform.GetChild(i).gameObject.GetComponent<ObjComponent>().Achieved = false;
		}
	}



	void Update()
	{


		if (!_agentComponent.IsFighting())
		{
			UpdateState();
			UpdateAppraisalStatus();



			CurrentObjs.SetActive(true);

			//Debug.Log("fighting");
			//Check fighting
			//Fight with disapproved agents
			//Disapproved agents are the ones who achieve my desired object before me
			foreach (GameObject c in _agentComponent.CollidingAgents)
			{

				if (c.GetComponent<ShopperBehavior>() != null && _agentComponent.IsGoodToFight(c, 5f))
				{
					_agentComponent.StartFight(c, true);
					c.GetComponent<AgentComponent>().StartFight(this.gameObject, false);

				}

			}

		}

        else {
			CurrentObjs.SetActive(false); //don't show items when fighting

		}
  //      else  if (GetComponent<FightBehavior>().Opponent.CompareTag("Player") )
  //      { //Fighting
		//	CurrentObjs.SetActive(false);
		//	//foreach (Transform co in CurrentObjs)
		//	//  co.gameObject.SetActive(false);

		//	//The winner gets the items
		//	if(GetComponent<FightBehavior>().Opponent.GetComponent<AgentComponent>().Damage > _agentComponent.Damage) { //opponent yields to me
		//		GetComponent<FightBehavior>().Opponent.GetComponent<ShopperBehavior>().YieldObjects(this.gameObject);
				
				
		//	}
		//	else { //agent yield to opponent
		//		YieldObjects(GetComponent<FightBehavior>().Opponent);
				
		//	}

		//}

		//else if(GetComponent<FightBehavior>().Opponent.CompareTag("RealPlayer")) { //Fighting
		//	CurrentObjs.SetActive(false);
		//	//foreach (Transform co in CurrentObjs)
		//	//  co.gameObject.SetActive(false);

		//	//The winner gets the items
		//	if(GetComponent<FightBehavior>().Opponent.GetComponent<HumanComponent>().Damage > _agentComponent.Damage) { //human yields to agent
		//		GetComponent<FightBehavior>().Opponent.GetComponent<HumanShoppingBehavior>().YieldObjects(this.gameObject);
		//	}
		//	else { //agent yields to human
		//		YieldObjects(GetComponent<FightBehavior>().Opponent);
		//	}

		//}
	}



	//Arrange positions of object
	public void SortObjects()
	{

		for (int i = 0; i < CurrentObjs.transform.childCount; i++)
			CurrentObjs.transform.GetChild(i).transform.position = CurrentObjs.transform.position - 0.05f * Vector3.up + i * 0.05f * Vector3.up;


	}

	//Give all your objects to your opponent
	public void YieldObjects(GameObject opponent)
	{

		List<Transform> children = new List<Transform>();
		int childCnt = CurrentObjs.transform.childCount;
		for(int i = 0; i < childCnt; i++)
			children.Add(CurrentObjs.transform.GetChild(i));



		foreach(Transform c in children) {
			if(opponent.CompareTag("RealPlayer")) {
				c.parent = opponent.GetComponent<HumanShoppingBehavior>().CurrentObjs.transform;
				opponent.GetComponent<HumanShoppingBehavior>().SortObjects();
			}
			
            else {
				c.parent = opponent.GetComponent<ShopperBehavior>().CurrentObjs.transform;
				opponent.GetComponent<ShopperBehavior>().SortObjects();
		
		
		}

		
		}
		CurrentObjs.SetActive(true);
	}


	void PickedObject()
	{
		_agentComponent.FinishedWaiting = true;
	}

	void UpdateState()
	{
		float minDist = 100000f;
		_totalObjCnt = 0;



		switch (State)
		{
			case (int)ShoppingState.GoingToObject:
				{
					//Find closest object with the least effective density
					Transform closestObj = null;

                        for (int i = 0; i < _objs.transform.childCount; i++)
                        {	//find closest object
                            Transform obj = _objs.transform.GetChild(i);
							if(obj.CompareTag("Ipad"))
							{
								if (obj.gameObject.activeInHierarchy == false || obj.GetComponent<ObjComponent>().Achieved == true)
									continue;
								_totalObjCnt++;
								// if the object is too crowded and I am not the closest person wait in this state and I am not hopeful
								if (_affectComponent.Emotion[(int)EType.Hope] < 0.3f && obj.GetComponent<ObjComponent>().Density > 20 && obj.GetComponent<ObjComponent>().ClosestAgent != this.gameObject)
								{
									_agentComponent.LookAt(obj.transform.position, 0.01f); //just look at the object
									_navmeshAgent.updateRotation = true;
									continue;
								}

								float dist = Vector3.Distance(obj.transform.position, transform.position);
								if (dist < minDist)
								{
									minDist = dist;
									closestObj = obj;
								}
							}
                        }
                        _desiredObj = closestObj;

					
					if (_desiredObj != null)
					{
						_agentComponent.SteerTo(_desiredObj.transform.position);
						float dist = Vector2.Distance(new Vector2(_desiredObj.transform.position.x, _desiredObj.transform.position.z), new Vector2(transform.position.x, transform.position.z));

						if (dist < 1f)
						{//Pick up object                        
							State = (int)ShoppingState.PickingUpObject;
						}

					}
					else
					{
						if (_totalObjCnt == 0)
						{ //all objects in my shelf are consumed
							_shelfInd++;
							// if ( _affectComponent.Ekman[(int)EkmanType.Afraid] > 0.5) {
							if (_shelfInd > 10) //change to 12 for superstore, NATHAN
							{ //all objects in the store are consumed
								_allConsumed = true;


								if (_acquiredObjCnt > 0)
								{//if I bought something
								 //Let them all pay
									State = (int)ShoppingState.GoingToLine;
									//if (_affectComponent.Emotion[(int)EType.Reproach] < 0.5f)
									//	State = (int)ShoppingState.GoingToLine;
									//else
									//	State = (int)ShoppingState.Exiting; //go out without paying                          
								}
								else
								{ // I have nothing to buy
									State = (int)ShoppingState.Exiting;
								}
							}
							else
							{
								State = (int)ShoppingState.ShelfChanging;
							}
						}

					}


				}
				break;
			case (int)ShoppingState.PickingUpObject:

				if (_agentComponent.StartedWaiting == false)
				{
					_agentComponent.LookAt(_desiredObj.transform.position, 0.1f);
					//   if(_desiredObj.GetComponent<ObjComponent>().Achieved == false && _desiredObj.GetComponent<ObjComponent>().ClosestAgent.Equals(this.gameObject)) {
					_animationSelector.SelectAction("PICKUP");
					_agentComponent.HandPos = _desiredObj.position; //+ Vector3.up * 0.1f;    
											// }


					// StartCoroutine(_agentComponent.WaitAWhile(2));
					_agentComponent.StartedWaiting = true;
					_agentComponent.FinishedWaiting = false;

				}

				else if (_agentComponent.StartedWaiting && !_agentComponent.FinishedWaiting && _desiredObj.GetComponent<ObjComponent>().Achieved == false)
				{ //started waiting
					if (_desiredObj.GetComponent<ObjComponent>().ClosestAgent.Equals(this.gameObject))
						_desiredObj.position = _rightHand.position;
					_agentComponent.HandPos = _desiredObj.position;//+ Vector3.up * 0.1f;
					PickedObject();
				}

				if (_agentComponent.FinishedWaiting)
				{
					if (_desiredObj.GetComponent<ObjComponent>().Achieved == false)
					{ //make sure someone else didn't pick it before me                

						//   CurrentObjs.Add(_desiredObj);
						_desiredObj.parent = CurrentObjs.transform;
						_desiredObj.position = CurrentObjs.transform.position - 0.05f * Vector3.up + (CurrentObjs.transform.childCount - 1) * 0.05f * Vector3.up;
						//  _desiredObj.position = _leftHand.position;
						_desiredObj.GetComponent<ObjComponent>().AchievingAgent = this.gameObject;
						_desiredObj.GetComponent<ObjComponent>().Achieved = true;
						_acquiredObjCnt++;

						_aquiredObjUI.setAquiredObjCount(_acquiredObjCnt); //Added by Nathan Brilmayer for floating aquiredObj count UI

                        _desiredObj.GetComponent<ObjComponent>().ObjPickupSuccess();
                        //    _agentComponent.HandPos = _desiredObj.position + Vector3.up * 0.1f;


                    }
					else //it is achieved
					     //FUNDA: Why?
					     //_agentComponent.LookAt(_desiredObj.GetComponent<ObjComponent>().AchievingAgent.transform.position, 0.01f);

						_navmeshAgent.updateRotation = true;
					_agentComponent.StartedWaiting = false;
					if (_acquiredObjCnt >= _desiredObjCnt)
					{

						if (_affectComponent.Emotion[(int)EType.Reproach] < 0.5f)
							State = (int)ShoppingState.GoingToLine; //go to line
						else
							State = (int)ShoppingState.Exiting; //exit without paying

					}
					else
						State = (int)ShoppingState.GoingToObject; //go to another object                        

				}

				break;
			case (int)ShoppingState.GoingToLine:
				_counter.GetComponent<LineHandler>().GetInLine(this.gameObject); //get in line if not already in
				if (_counter.GetComponent<LineHandler>().IsInLine(this.gameObject))
					State = (int)ShoppingState.WaitingInLine;
				else
					_agentComponent.SteerTo(_counter.GetComponent<LineHandler>().LineEnd);

				break;
			case (int)ShoppingState.WaitingInLine:

				_agentComponent.SteerTo(_counter.GetComponent<LineHandler>().FindLineEndBeforeAgent(this.gameObject));
				if (_counter.GetComponent<LineHandler>().IsFirst(this.gameObject))
					State = (int)ShoppingState.Paying;

				break;

			case (int)ShoppingState.Paying:
				_agentComponent.Watch(_cashier);
				//_agentComponent.LookAt(_cashier.transform.position);
				if (_agentComponent.StartedWaiting == false)
					StartCoroutine(_agentComponent.WaitAWhile(10));

				if (_agentComponent.FinishedWaiting)
				{
					_navmeshAgent.updateRotation = true; //change after lookat
					_counter.GetComponent<LineHandler>().GetOutLine(this.gameObject);
					_agentComponent.StartedWaiting = false;
					State = (int)ShoppingState.Exiting;
				}
				break;

			case (int)ShoppingState.Exiting:
				_agentComponent.SteerTo(_exit);
				//if they are outside the store
				if (Vector3.Distance(transform.position, _exit) < 20)
				{
					_appraisal.Restart(); //clear all appraisal components
					_affectComponent.ContagionMode = false;
				}
				break;

			case (int)ShoppingState.ShelfChanging:
				_closestShelfPos = _shelfComp.FindClosestIsleWaypoint(transform.position, _shelfOrder[_shelfInd]);


				_agentComponent.SteerTo(_closestShelfPos);

//				Debug.DrawLine(transform.position, _closestShelfPos);

				_objs = _allObjs[_shelfOrder[_shelfInd]];


				if (Vector3.Distance(_closestShelfPos, transform.position) < 2f) //close enough
				{
                    State = (int)ShoppingState.GoingToObject;
                }
				break;

		}

	}
	void LateUpdate()
	{
		//if (_currentObj != null) {
		//for (int i = 0; i < CurrentObjs.Count; i++) {

		CurrentObjs.transform.position = _leftHand.position;
		//   for (int i = 0; i < CurrentObjs.transform.childCount; i++) {
		//CurrentObjs.transform.GetChild(i).position = _hand.position - 0.05f * Vector3.up + i * 0.05f * Vector3.up;
		//CurrentObjs[i].position = _hand.position - 0.05f* Vector3.up + i * 0.05f * Vector3.up;
		//CurrentObjs[i].gameObject.SetActive(true);
		//   _agentComponent.HandPos = _currentObjs[i].transform.position;

		//   }
	}

	void InitAppraisalStatus()
	{


		if (_affectComponent.Personality[(int)OCEAN.N] > 0.5f)  //if neurotic feel distress about the crowded scene
			_appraisal.AddGoal("sales", 0.25f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectIrrelevant);
		if (_affectComponent.Personality[(int)OCEAN.N] > 0.85f)
		{ //Fear
		  //if *really* neurotic feel fear
		  //Hope to get  desired items
			for (int i = 0; i < _desiredObjCnt; i++)
				// Add as many fearful emotions as the number of desired objects which can sum up to 0.1          
				_appraisal.AddGoal("sales", 0.05f / _desiredObjCnt, AppDef.Displeased, AppDef.ConsequenceForSelf,
						   AppDef.ProspectRelevant, AppDef.Unconfirmed);
		}
		else
		{
			//Hope to get  desired items
			for (int i = 0; i < _desiredObjCnt; i++)
				// Add as many hopeful emotions as the number of desired objects which can sum up to 0.4          
				_appraisal.AddGoal("sales", 0.4f / _desiredObjCnt, AppDef.Pleased, AppDef.ConsequenceForSelf,
						   AppDef.ProspectRelevant, AppDef.Unconfirmed);
		}



		//Standard about other shoppers
		//      if (_affectComponent.Personality[(int)OCEAN.N] > 0f)  //if neurotic feel jealousy
		//     _appraisal.AddStandard(0.3f, AppDef.Disapproving, AppDef.FocusingOnOther, transform.parent.gameObject);	//shoppers in general --no specific shopper	            
		//Attitude towards objects in the store
		_appraisal.AddAttitude(null, 0.2f, AppDef.Liking); //General liking

	}


	void UpdateAppraisalStatus()
	{
		if (State == (int)ShoppingState.PickingUpObject)
		{
			if (_desiredObj != null && _desiredObj.GetComponent<ObjComponent>().Achieved && _desiredObj.GetComponent<ObjComponent>().AchievingAgent != this.gameObject)
			{ //someone else achieved it just when i was trying to get it                
			  //Change hope to disappointment for 1 object
				float wt = _appraisal.RemoveGoal("sales", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
				if (wt != 0)
				{
					_appraisal.AddGoal("sales", wt + 0.1f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed); //slightly higher than hope
																					 //Resentment towards other shoppers
					_appraisal.AddGoal("sales", 0.1f, AppDef.Displeased, AppDef.ConsequenceForOther, transform.parent.gameObject, AppDef.DesirableForOther);
					//High disapproval towards that specific agent who achieved my object before me                   
					_appraisal.AddStandard(0.5f, AppDef.Disapproving, AppDef.FocusingOnOther, _desiredObj.GetComponent<ObjComponent>().AchievingAgent); //Causes reproach

				}

				//Change fear to fearsconfirmed for 1 object
				wt = _appraisal.RemoveGoal("sales", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
				if (wt != 0)
					_appraisal.AddGoal("sales", wt / 5f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);
			}

			else
			{
				//I achieved                
				//Change hope to satisfaction                  
				float wt = _appraisal.RemoveGoal("sales", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
				if (wt != 0)
				{
					_appraisal.AddGoal("sales", wt, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);
					//If neurotic, gloating towards other shoppers
					if (_affectComponent.Personality[(int)OCEAN.A] < 0f)
						_appraisal.AddGoal("sales", 0.1f, AppDef.Pleased, AppDef.ConsequenceForOther, transform.parent.gameObject, AppDef.UndesirableForOther);
				}

				//Change fear to relief
				wt = _appraisal.RemoveGoal("sales", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
				if (wt != 0)
					_appraisal.AddGoal("sales", wt, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed);
			}

		}

		else if (State == (int)ShoppingState.GoingToObject)
		{
			//if someone else took my desired object
			if (IsDesiredObjectMissed())
			{
				bool exists = _appraisal.DoesStandardExist(_desiredObj.GetComponent<ObjComponent>().AchievingAgent, AppDef.Disapproving);
				if (!exists && _affectComponent.Personality[(int)OCEAN.A] < 0f) //if disagreeable add disapproval  towards that specific agent who achieved my object before me  
					_appraisal.AddStandard(0.4f, AppDef.Disapproving, AppDef.FocusingOnOther, _desiredObj.GetComponent<ObjComponent>().AchievingAgent); //Causes reproach

			}
		}

		else if (_acquiredObjCnt >= _desiredObjCnt)
		{
			//Change hope to satisfaction                  
			float wt = _appraisal.RemoveGoal("sales", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
			if (wt != 0)
			{
				_appraisal.AddGoal("sales", 0.6f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);
				//If neurotic, gloating towards other shoppers
				if (_affectComponent.Personality[(int)OCEAN.N] > 0f)
					_appraisal.AddGoal("sales", 0.3f, AppDef.Pleased, AppDef.ConsequenceForOther, transform.parent.gameObject, AppDef.UndesirableForOther);
			}

			//Change fear to relief

			wt = _appraisal.RemoveGoal("sales", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
			if (wt != 0)
				_appraisal.AddGoal("sales", wt, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed);


		}
		else
		{

			if (_allConsumed)
			{ // all the objects in the store are consumed

				//Change hope to disappointment
				float wt = _appraisal.RemoveGoal("sales", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
				if (wt != 0)
				{
					_appraisal.AddGoal("sales", 0.6f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed);
					//Resentment towards other shoppers
					_appraisal.AddGoal("sales", 0.3f, AppDef.Displeased, AppDef.ConsequenceForOther, transform.parent.gameObject, AppDef.DesirableForOther);
				}

				//Change fear to fearsconfirmed
				wt = _appraisal.RemoveGoal("sales", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
				if (wt != 0)
					_appraisal.AddGoal("sales", wt / 5f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);


				//Add disapproval to the store for not having any more\
				if (!_appraisal.DoesStandardExist(_cashier, AppDef.Disapproving))
					_appraisal.AddStandard(0.5f, AppDef.Disapproving, AppDef.FocusingOnOther, _cashier); //Causes reproach
			}
			else if (State == (int)ShoppingState.ShelfChanging)
			{

				//Decrease hope
				float wt = _appraisal.RemoveGoal("sales", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
				if (wt > 0.001f)
				{
					_appraisal.AddGoal("sales", wt * 3f / 4f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed); //less hope                    
																					   //Resentment towards other shoppers
					_appraisal.AddGoal("sales", 0.05f, AppDef.Displeased, AppDef.ConsequenceForOther, transform.parent.gameObject, AppDef.DesirableForOther);
				}

				//Increase fear
				wt = _appraisal.RemoveGoal("sales", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
				if (wt != 0)
					_appraisal.AddGoal("sales", wt + 0.01f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);

			}

		}
	}


	//Check if someone else took my desired object before me
	bool IsDesiredObjectMissed()
	{
		return (_desiredObj != null && _desiredObj.GetComponent<ObjComponent>().Achieved &&
			_desiredObj.GetComponent<ObjComponent>().AchievingAgent != null &&
			_desiredObj.GetComponent<ObjComponent>().AchievingAgent != this.gameObject);

	}



	void OnDrawGizmosSelected()
	{
		/*   Gizmos.color = Color.blue;
		   if(_navmeshAgent)
		       Gizmos.DrawLine(transform.position, _navmeshAgent.destination);


		   //Draw shelves
		   if (_shelves!=null)
		       for(int i = 0; i < 6; i ++) {
			   Gizmos.DrawLine(_shelves[i].v1, _shelves[i].v2);
			   Gizmos.DrawLine(_shelves[i].v2, _shelves[i].v3);
			   Gizmos.DrawLine(_shelves[i].v3, _shelves[i].v4);
			   Gizmos.DrawLine(_shelves[i].v4, _shelves[i].v1);
		       }*/
	}


}
