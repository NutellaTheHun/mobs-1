//#define WAITING_AT_ENTRANCE
using UnityEngine;
using System.Collections;
using static ObjComponent;


public class ShopperBehavior : MonoBehaviour
{
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

	//Variables for LineHandling and Paying
    LineHandler _LineHandler;
    [SerializeField] public GameObject NextInLinePositionCollider;
	public ShopperBehavior ShopperAheadInLine;
	public ShopperBehavior ShopperBehindInLine;
	//public GameObject PositionBehindSomeone;
	public bool isWaitingBehindSomeone = false;
	private GameObject paymentCollider;
	public bool hasPaid = false;
    [SerializeField] private GameObject FindClosestIpad;
	private GameObject IsShopperCrowdedComponent;
	private ShopperInTheWay shopperInTheWay;
	private DetermineAlternatePath determineAlternatePath;

    //Author: Nathan Brilmayer, used for floating counter above AI for aquired objects
    aquiredObjUI _aquiredObjUI;
    aquiredObjCanvasManager _aquiredObjCanvasManager;
	VRShopperAnimationController _animationController;
	[SerializeField] private IsleCountData _isleCountData;
	[SerializeField] private IpadCountData _ipadCountData;
    private bool IsCrowded = false;
	ShelfSide sideOfIsle = ShelfSide.None;

    [SerializeField]
	private int _state;
    private bool hasSwitchedSides = false;
    private bool isRegistered = false; //for registering with linehandler in State.GoingToLine
	public Vector3 LineDestination;
    private bool hasLeft = false;
    private bool isPickingUpObj = false;
    private bool movingInIsle = false;

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
		_LineHandler = GameObject.Find("counter").GetComponent<LineHandler>();
		paymentCollider = GameObject.Find("PaymentCollider");
		IsShopperCrowdedComponent = transform.Find("IsShopperCrowdedComponent").gameObject;

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

		//_rightHand = transform.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
		//_leftHand = transform.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftHand);


		_acquiredObjCnt = 0;
		
		_desiredObjCnt = (int)(2f * _affectComponent.Personality[(int)OCEAN.E] + 20f); //correlated to extroversion [10 40]


		InitAppraisalStatus();

		_navmeshAgent.autoRepath = true;
		_navmeshAgent.autoBraking = true;


		Random.InitState(_agentComponent.Id);
		
        createShelfOrder();

        _objs = GameObject.Find("Objects" + _shelfOrder[0]);


		//State = (int)ShoppingState.GoingToObject;
		State = (int)ShoppingState.ShelfChanging;



        _navmeshAgent.speed += 0.6f; //faster than usual


		CurrentObjs = new GameObject("AchievedObjects");
		CurrentObjs.transform.parent = this.transform;
        
		_aquiredObjCanvasManager = GameObject.Find("Canvas").GetComponentInChildren<aquiredObjCanvasManager>();
        _aquiredObjCanvasManager.initializeShopperCounterUI(this);
        _animationController = GetComponent<VRShopperAnimationController>();
        shopperInTheWay = GetComponentInChildren<ShopperInTheWay>();
		determineAlternatePath = GetComponentInChildren<DetermineAlternatePath>();
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
         }
         else
         {
             goRightwards();
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
		else
        {	//Fighting
            CurrentObjs.SetActive(false); 
			//Fighting other AI
			if (GetComponent<FightBehavior>().Opponent.CompareTag("Player"))
			{ 
				//The winner gets the items
				if (GetComponent<FightBehavior>().Opponent.GetComponent<AgentComponent>().Damage > _agentComponent.Damage)
				{ //opponent yields to me
					GetComponent<FightBehavior>().Opponent.GetComponent<ShopperBehavior>().YieldObjects(this.gameObject);
				}
				else
				{ //agent yield to opponent
					YieldObjects(GetComponent<FightBehavior>().Opponent);
				}
			}
            //Fighting player
            else if (GetComponent<FightBehavior>().Opponent.CompareTag("RealPlayer"))
            { 
                //The winner gets the items
                if (GetComponent<FightBehavior>().Opponent.GetComponent<HumanComponent>().Damage > _agentComponent.Damage)
                { //human yields to agent
                    GetComponent<FightBehavior>().Opponent.GetComponent<HumanShoppingBehavior>().YieldObjects(this.gameObject);
                }
                else
                { //agent yields to human
                    YieldObjects(GetComponent<FightBehavior>().Opponent);
                }

            }
        }
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
		/*List<Transform> children = new List<Transform>();
		int childCnt = CurrentObjs.transform.childCount;

		for(int i = 0; i < childCnt; i++)
			children.Add(CurrentObjs.transform.GetChild(i));

		foreach(Transform c in children) {
			if(opponent.CompareTag("RealPlayer")) 
			{
				c.parent = opponent.GetComponent<HumanShoppingBehavior>().CurrentObjs.transform;
				opponent.GetComponent<HumanShoppingBehavior>().SortObjects();
			}
			
            else 
			{
				c.parent = opponent.GetComponent<ShopperBehavior>().CurrentObjs.transform;
				opponent.GetComponent<ShopperBehavior>().SortObjects();
			}

		
		}*/
		if(opponent.CompareTag("RealPlayer"))
		{
			opponent.GetComponent<HumanShoppingBehavior>().AddCollectedItemCount(_acquiredObjCnt);
			
		}
		else
		{
			opponent.GetComponent<ShopperBehavior>().AddAquiredObjs(_acquiredObjCnt);
		}
        _acquiredObjCnt = 0;
        CurrentObjs.SetActive(true);
	}


	void PickedObject()
	{
		_agentComponent.FinishedWaiting = true;
	}

	void UpdateState()
	{
		//float minDist = 100000f;
		//_totalObjCnt = 0;

		switch (State)
		{
			case (int)ShoppingState.GoingToObject:
			{
					//isPickingUpObj = false;
					IsShopperCrowdedComponent.SetActive(true); //Enables collider in front of shopper to detect if somone is infront of them
                    _totalObjCnt = _ipadCountData.Isle[_shelfOrder[_shelfInd]]._total;

                    if (_totalObjCnt <= 0)
                    { //all objects in my shelf are consumed
                        ++_shelfInd;
                        // if ( _affectComponent.Ekman[(int)EkmanType.Afraid] > 0.5) {
                        if (_shelfInd > 10)
                        { //all objects in the store are consumed
                            _allConsumed = true;

                            if (_acquiredObjCnt > 0)
                            {//if I bought something
                             //Let them all pay
                                IsShopperCrowdedComponent.SetActive(false);
                                FindClosestIpad.SetActive(false);
                                State = (int)ShoppingState.GoingToLine;
                                //if (_affectComponent.Emotion[(int)EType.Reproach] < 0.5f)
                                //	State = (int)ShoppingState.GoingToLine;
                                //else
                                //	State = (int)ShoppingState.Exiting; //go out without paying                          
                            }
                            else
                            { // I have nothing to buy
                                IsShopperCrowdedComponent.SetActive(false);
                                FindClosestIpad.SetActive(false);
                                State = (int)ShoppingState.Exiting;
                            }
                        }
                        else
                        {
                            IsShopperCrowdedComponent.SetActive(false);
                            FindClosestIpad.SetActive(false);
                            State = (int)ShoppingState.ShelfChanging;
                        }
                    }

                    if (_desiredObj == null) { 

						if(sideOfIsle != ShelfSide.None) //switch sides if their side of isle is clear
						{
                            if (sideOfIsle == ShelfSide.Left && _ipadCountData.Isle[_shelfOrder[_shelfInd]]._left == 0)
                            {
                                switchSides();
                            }
                            else if (sideOfIsle == ShelfSide.Right && _ipadCountData.Isle[_shelfOrder[_shelfInd]]._right == 0)
                            {
                                switchSides();
                            }
                        }
						
						EnableFindClosestIpad(); //Function that sets desiredObj
					}
					else// (_desiredObj != null)
					{
						if( sideOfIsle == ShelfSide.None) { sideOfIsle = _desiredObj.GetComponent<ObjComponent>().sideOfIsle; }

						/* If the shopper is less than 3 units away from target ipad, if other shoppers are targetting this ipad,
						 * if this shopper isnt the closest, it will find another ipad */
						if ((Vector3.Distance(this.transform.position, _desiredObj.transform.position) < 3) && 
								(_desiredObj.GetComponent<ObjComponent>().ShoppersDesiringThisObj.Count > 1) && 
									(_desiredObj.GetComponent<ObjComponent>().ClosestAgent != this))
									{
										TryOtherIpad();
									}
						/* If shopper is crowded (other shopper is in its frontwards sphere collider, and if the desired objects, 
						 * closest agent is in the collider, shopper will try a different ipad*/
						if(_desiredObj != null)
						{
                            if (IsCrowded && shopperInTheWay.ShoppersInTheWay.Contains(
                            _desiredObj.GetComponent<ObjComponent>().ClosestAgent.GetComponent<ShopperBehavior>()))
                            {
                                TryOtherIpad();
                            }
                        }
						
                        _agentComponent.SteerTo(_desiredObj.transform.position);
                        if (!transform.GetComponent<VRShopperAnimationController>().IsAnimated())
						{
                            float dist = Vector2.Distance(new Vector2(_desiredObj.transform.position.x, _desiredObj.transform.position.z), new Vector2(transform.position.x, transform.position.z));

                            if (dist < 1f)
                            {//Pick up object
								FindClosestIpad.SetActive(false);
                                State = (int)ShoppingState.PickingUpObject;
                            }
                        }
					}
			}
				break;
			case (int)ShoppingState.PickingUpObject:
				
				if(_desiredObj != null)
				{
					_agentComponent.LookAt(_desiredObj.transform.position, 2);
                    if (_desiredObj.GetComponent<ObjComponent>().Achieved == false && !transform.GetComponent<VRShopperAnimationController>().IsAnimated() && !isPickingUpObj)
                    {
                        if (!isPickingUpObj)
						{
                            isPickingUpObj = true;
                            StartCoroutine(PickUpObj());
                        }
                        /*_desiredObj.GetComponent<ObjComponent>().AchievingAgent = this.gameObject;
                        _desiredObj.GetComponent<ObjComponent>().Achieved = true;
                        _animationController.PlayPickupAnimation(_desiredObj.GetComponent<ObjComponent>().height);
                        _acquiredObjCnt++;
                        _aquiredObjUI.setAquiredObjCount(_acquiredObjCnt);*/

                    }
                }

                _navmeshAgent.updateRotation = true;
					//_agentComponent.StartedWaiting = false;
				if (_acquiredObjCnt >= _desiredObjCnt)
				{

					if (_affectComponent.Emotion[(int)EType.Reproach] < 0.5f)
					{
                        IsShopperCrowdedComponent.SetActive(false);

                        State = (int)ShoppingState.GoingToLine; //go to line
                    }
					else
					{
                        IsShopperCrowdedComponent.SetActive(false);
                        State = (int)ShoppingState.Exiting; //exit without paying
                    }
                      
				}
				else
					State = (int)ShoppingState.GoingToObject; //go to another object                        

				break;
			case (int)ShoppingState.GoingToLine:
                if (_acquiredObjCnt == 0)
                {
                    State = (int)ShoppingState.Exiting;
                    break;
                }

                if (!isRegistered)
				{
					_LineHandler.RegisterWithLineHandler(this);
					isRegistered = true;
				}

				LineDestination = _LineHandler.RequestLinePosition(this);

                _agentComponent.SteerTo(LineDestination);

				if(_LineHandler.PayingShopper == this)
				{
					State = (int)ShoppingState.Paying;
				}

				else if(_LineHandler.ShopperAtFrontOfLine == this)
				{
                    State = (int)ShoppingState.WaitingInLine;
                }

				else if(Vector3.Distance(transform.position, LineDestination) < 0.2f)
				{
                    State = (int)ShoppingState.WaitingInLine;
                }

                break;

            case (int)ShoppingState.WaitingInLine:

				_agentComponent.LookAt(_LineHandler.PayingPosition.transform.position, 2);
                _agentComponent.SteerTo(LineDestination);

                if (_LineHandler.PayingShopper == this)
                {
                    State = (int)ShoppingState.Paying;
                }

                break;

			case (int)ShoppingState.Paying:

                _agentComponent.SteerTo(_LineHandler.PayingPosition.transform.position);
                _agentComponent.LookAt(paymentCollider.transform.position,2);
				if (!transform.GetComponent<VRShopperAnimationController>().IsAnimated() && transform.GetComponent<Animator>().GetBool("VRIK_IsMoving") == false)
				{
                    StartCoroutine(Pay());
                }

				break;

			case (int)ShoppingState.Exiting:

                _agentComponent.SteerTo(_exit);
				LineDestination = Vector3.zero;
				if(!hasLeft)
				{
                    if (_LineHandler.PayingShopper == null)
                    {
                        _LineHandler.DeregisterShopper(this);
						hasLeft = true;
                    }
                }
                

				//if they are outside the store
				if (Vector3.Distance(transform.position, _exit) < 20)
				{
					_appraisal.Restart(); //clear all appraisal components
					_affectComponent.ContagionMode = false;
				}
				break;

			case (int)ShoppingState.ShelfChanging:
				
				hasSwitchedSides = false;
				sideOfIsle = ShelfSide.None;

				_closestShelfPos = _shelfComp.FindClosestIsleWaypoint(transform.position, _shelfOrder[_shelfInd]);

				_agentComponent.SteerTo(_closestShelfPos);

//				Debug.DrawLine(transform.position, _closestShelfPos);

				_objs = _allObjs[_shelfOrder[_shelfInd]];


				if (Vector3.Distance(_closestShelfPos, transform.position) < 1f) //close enough ,changed from 2f to 1f
				{
					if (_isleCountData.ShopperCountInIsle[_shelfOrder[_shelfInd]] < _isleCountData.IslePerShopperMax) //If to many shoppers in isle then that isle is skipped, logical choice and prevents AI clumping as much
					{
                        State = (int)ShoppingState.GoingToObject;
                    }
					else
					{
						if (_shelfInd != numberOfIsles - 1)
							_shelfInd++;
						else State = (int)ShoppingState.GoingToLine;

                    }
                }
				break;

		}

	}

    IEnumerator PickUpObj()
    {
		yield return new WaitForSeconds(0.5f);
        _desiredObj.GetComponent<ObjComponent>().AchievingAgent = this.gameObject;
        _desiredObj.GetComponent<ObjComponent>().Achieved = true;
        _animationController.PlayPickupAnimation(_desiredObj.GetComponent<ObjComponent>().height);
        _acquiredObjCnt++;
        _aquiredObjUI.setAquiredObjCount(_acquiredObjCnt);
        yield return new WaitForSeconds(0.2f);
		isPickingUpObj = false;
    }

    IEnumerator Pay()
	{
		yield return new WaitForSeconds(1);
        _animationController.PlayPayingAnimation();
        _navmeshAgent.updateRotation = true; //change after lookat
        yield return new WaitForSeconds(1);
        State = (int)ShoppingState.Exiting;
        hasPaid = true;
    }

    private void EnableFindClosestIpad()
    {
        FindClosestIpad.SetActive(true);
		FindClosestIpad.GetComponent<FindClosestTargetCollision>().PrimeEvent();
    }

    private void TryOtherIpad()
    {
        if(!hasSwitchedSides)
		{
			switchSides();
		}
		else
		{
			if(!movingInIsle)
			{
                if (_isleCountData.ShopperCountInIsle[_shelfOrder[_shelfInd]] < 4)
                {
                    movingInIsle = true;
                    TryFarEndOfIsle();
                }
                else
                {
                    if (_shelfInd != numberOfIsles - 1)
                    {
                        _shelfInd++;
                        State = (int)ShoppingState.ShelfChanging;
                    }
                    else State = (int)ShoppingState.GoingToLine;
                }
            }
		}
    }

    private void TryFarEndOfIsle()
    {
        _closestShelfPos = _shelfComp.getOtherWaypoint(_closestShelfPos, _shelfOrder[_shelfInd]);
		StartCoroutine(DelayShopperGoingToObject(3));
    }

	IEnumerator DelayShopperGoingToObject(float seconds)
	{
        _agentComponent.SteerTo(_closestShelfPos);
        yield return new WaitForSeconds(seconds);
		movingInIsle = false;
        State = (int)ShoppingState.GoingToObject;
	}


    private void switchSides()
    {
        if(sideOfIsle == ShelfSide.Left)
		{
			sideOfIsle = ShelfSide.Right;
		}
		else
		{
            sideOfIsle = ShelfSide.Left;
        }
        hasSwitchedSides = true;
    }

    private void HideAchievedObj()
    {
		if(_desiredObj != null)
		{
            _desiredObj.GetComponent<ObjComponent>().ObjPickupSuccess();
        }
    }

    void LateUpdate()
	{
		//if (_currentObj != null) {
		//for (int i = 0; i < CurrentObjs.Count; i++) {

		//CurrentObjs.transform.position = _leftHand.position;
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
        Gizmos.color = Color.blue;
		if (LineDestination != null)
		{
            Gizmos.DrawSphere(LineDestination, 0.2f);
        }

    }

	public void CrowdedChangeDirection()
	{
		Vector3 currentDestination = _navmeshAgent.destination;
		Vector3 alternateDestination = determineAlternatePath.MoveOutOfTheWay((ShoppingState)State);
		if(alternateDestination == Vector3.zero)
		{
			//pause;
			StartCoroutine(PauseAgent(2f));
		}
		else
		{
			StartCoroutine(TravelAlternatePath(alternateDestination, currentDestination));
			//go to alternate destination
			//once complete, go to current destination
		}
		/*switch(State)
		{
			case (int)ShoppingState.GoingToObject:


				break;

            case (int)ShoppingState.GoingToLine:


                break;

            case (int)ShoppingState.ShelfChanging:


                break;
        }*/
       /* if (state == ShoppingState.ShelfChanging)
        {
            //move left, or right, or pause
            if (LeftOk && RightOk)
            {
                //Go left or right with some preference?
                int rand = UnityEngine.Random.Range(0, 1);
                if (rand == 1)
                {
                    return LeftColliderArea.transform.position;
                }
                else
                {
                    return RightColliderArea.transform.position;
                }
            }
            else if (LeftOk)
            {
                //go left
                return LeftColliderArea.transform.position;
            }
            else if (RightOk)
            {
                //go right
                return RightColliderArea.transform.position;
            }
            else
            {
                //pause
                return Vector3.zero;
            }
        }
        if (state == ShoppingState.GoingToObject)
        {
            //go to center of isle, go ahead or switch side
            if (LeftOk && RightOk)
            {
                int rand = UnityEngine.Random.Range(0, 1);
                //Go left or right with some preference? most likely rarely triggered
                if (rand == 1)
                {
                    return LeftColliderArea.transform.position;
                }
                else
                {
                    return RightColliderArea.transform.position;
                }
            }
            else if (LeftOk)
            {
                //go left
                return LeftColliderArea.transform.position;
            }
            else if (RightOk)
            {
                //go right
                return RightColliderArea.transform.position;
            }
            else
            {
                //pause
                return Vector3.zero;
            }
        }
        if (state == ShoppingState.GoingToLine)
        {
            //move left, or right, or pause
            if (LeftOk && RightOk)
            {
                //Go left or right with some preference?
                int rand = UnityEngine.Random.Range(0, 1);
                //Go left or right with some preference? most likely rarely triggered
                if (rand == 1)
                {
                    return LeftColliderArea.transform.position;
                }
                else
                {
                    return RightColliderArea.transform.position;
                }
            }
            else if (LeftOk)
            {
                //go left
                return LeftColliderArea.transform.position;
            }
            else if (RightOk)
            {
                //go right
                return RightColliderArea.transform.position;
            }
            else
            {
                //pause
                return Vector3.zero;
            }
        }
        return Vector3.zero;
    }*/
}

    IEnumerator PauseAgent(float seconds)
    {
		_navmeshAgent.isStopped = true;
		yield return new WaitForSeconds(seconds);
        _navmeshAgent.isStopped = false;
    }

    IEnumerator TravelAlternatePath(Vector3 alternateDestination, Vector3 currentDestination)
    {
		bool finishedAlternateDestination = false;
        _agentComponent.SteerTo(alternateDestination);
        while (!finishedAlternateDestination)
		{
			if(!_navmeshAgent.pathPending && !_navmeshAgent.hasPath)
			{
				finishedAlternateDestination = true;
				_agentComponent.SteerTo(currentDestination);
			}
            yield return null;
        }
		yield return 0;
    }

    public void GetClosestObj()
	{
		Transform ipad = gameObject.GetComponentInChildren<FindClosestTargetCollision>().ClosestTarget.transform;

        if (_desiredObj == null)
		{
			if((sideOfIsle == ipad.GetComponent<ObjComponent>().sideOfIsle || sideOfIsle == ShelfSide.None) && 
				ipad.parent.GetComponent<IsleComponent>().IsleIndex == _shelfOrder[_shelfInd])
			{
                _desiredObj = gameObject.GetComponentInChildren<FindClosestTargetCollision>().ClosestTarget.transform;
                _desiredObj.GetComponent<ObjComponent>().addShopperToDesiredObjList(this);
            }
                
        } 
        else if (sideOfIsle == ipad.GetComponent<ObjComponent>().sideOfIsle && 
			ipad.parent.GetComponent<IsleComponent>().IsleIndex == _shelfOrder[_shelfInd])
        {
            if (Vector3.Distance(transform.position, _desiredObj.transform.position) >
                Vector3.Distance(transform.position, gameObject.GetComponentInChildren<FindClosestTargetCollision>().ClosestTarget.transform.position))
            {
                _desiredObj = gameObject.GetComponentInChildren<FindClosestTargetCollision>().ClosestTarget.transform;
                _desiredObj.GetComponent<ObjComponent>().addShopperToDesiredObjList(this);
            }
        }
				
        FindClosestIpad.SetActive(false);
	}

    private void GetNextClosestIpad()
    {
        int counter = 0;
        int counterMax = 4;
		Transform original = _desiredObj;
		Transform ipad;
		EnableFindClosestIpad();
        if (gameObject.GetComponentInChildren<FindClosestTargetCollision>().ClosestTarget.transform.parent.GetComponent<IsleComponent>().IsleIndex == _shelfOrder[_shelfInd])
        {
			
            for (int i = counter; i < counterMax; i++)
            {
                ipad = gameObject.GetComponentInChildren<FindClosestTargetCollision>().ClosestTarget.transform;
                if (_desiredObj != ipad)
                {
					_desiredObj = ipad;
                    ipad.GetComponent<ObjComponent>().addShopperToDesiredObjList(this);
                }

                else if(_desiredObj != original)
                {
                    if (Vector3.Distance(transform.position, _desiredObj.transform.position) >
                        Vector3.Distance(transform.position, gameObject.GetComponentInChildren<FindClosestTargetCollision>().ClosestTarget.transform.position))
                    {
                        _desiredObj = gameObject.GetComponentInChildren<FindClosestTargetCollision>().ClosestTarget.transform;
                        _desiredObj.GetComponent<ObjComponent>().addShopperToDesiredObjList(this);
                    }
                }
                counter++;
                //if (counter == 4 && _desiredObj.GetComponent<ObjComponent>().ClosestAgent != this) counter = 0;
            }

            FindClosestIpad.SetActive(false);
        }
    }

    public void ResetDesiredObj()
    {
		_desiredObj = null;
    }

    public void setIsCrowded(bool v)
    {
		IsCrowded = v;
    }

    public void AddAquiredObjs(int collectedItemCnt)
    {
		_acquiredObjCnt += collectedItemCnt;
    }

    public void MoveUpInLine()
    {
        LineDestination = _LineHandler.RequestLinePosition(this);
        _agentComponent.SteerTo(LineDestination);
    }
}
