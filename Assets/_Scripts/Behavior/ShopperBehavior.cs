//#define WAITING_AT_ENTRANCE
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjComponent;
using Random = UnityEngine.Random;

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
	private const int NUMOFISLES = 11;

	private Transform _rightHand, _leftHand;


	private bool _finishedWaitingAtEntrance;

	//[SerializeField]
	private Vector3 _closestShelfPos;
	//[SerializeField]
	private int _shelfInd;
	//[SerializeField]
	int[] _shelfOrder; //shelf visiting order
	private AnimationSelector _animationSelector;
	private GUIHandler _guiHandler;

	//Variables made by Nathan Brilmayer
	private const int numberOfIsles = 11; //hard coded for shopper world
	private const int totalShoppers = 21; //hard coded for shopper world

	//Variables for LineHandling and Paying_isleCountData
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
	//[SerializeField] private IsleCountData _isleCountData;
	//[SerializeField] private IpadCountData _ipadCountData;
	private bool IsCrowded = false;
	ShelfSide sideOfIsle = ShelfSide.None;
    AIManager aiManager;

	[SerializeField]
	private int _state;
	private bool hasSwitchedSides = false;
	private bool isRegistered = false; //for registering with linehandler in State.GoingToLine
	private Vector3 LineDestination;
	private bool hasLeft = false;
	public bool isPickingUpObj = false;
	private bool movingInIsle = false;
    private bool door1Set = false;



    public int currentIsleIndex;
    public int nextIsleIndex;
	private int islesChecked = 0;
	IsleVolumeManager isleManager;
	private Vector3 IsleDestination;
    private int currentSwitchingIsle;
    private Vector3 secondaryIsleDestination;
	public bool switchingLaneSides = false;
    public int islesTravelledSignature = 0;
    private bool destinationSet;
    public Transform targetObject;
    private const int ALLISLESDONE = 2047;
    public int assignedAvoidancePriority;
    public IsleDataSO isleDataSO;
    private bool attemptItemPickup;
    public bool isWaitingOnAnimation;
    private int isMoving;

    public bool ShopperStatusWatch { get; private set; }

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
        _agentComponent = GetComponent<AgentComponent>();
        initializeFirstShelf();
    }
    // Use this for initialization
    void Start()
	{
        aiManager = GameObject.Find("AIManager").GetComponent<AIManager>();
        //assignedAvoidancePriority = _navmeshAgent.avoidancePriority;
        //nextIsleIndex = getRandomFirstIsleTarget();
        _agentComponent.SteerTo(ChooseEntrance());
        IsleDestination = _shelfComp.FindClosestIsleWaypoint(transform.position, nextIsleIndex);
		isleManager = GameObject.Find("IsleShopperCountVolumes").GetComponent<IsleVolumeManager>();

        _LineHandler = GameObject.Find("counter").GetComponent<LineHandler>();
		paymentCollider = GameObject.Find("PaymentCollider");
		IsShopperCrowdedComponent = transform.Find("IsShopperCrowdedComponent").gameObject;

        _appraisal = GetComponent<Appraisal>();
		
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
        assignedAvoidancePriority = _navmeshAgent.avoidancePriority;
    }

    private Vector3 ChooseEntrance()
    {
        Vector3 door1 = GameObject.Find("DoorTarget1").transform.position;
        Vector3 door2 = GameObject.Find("DoorTarget2").transform.position;

        float door1Dist = Vector3.Distance(transform.position, door1);
        float door2Dist = Vector3.Distance(transform.position, door2);
        if(door1Dist < door2Dist)
        {
            door1Set = true;
            return door1;
        }
        else
        {
            return door2;
        }
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
		/*else
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
        }*/
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
        _aquiredObjUI.setAquiredObjCount(_acquiredObjCnt);
        CurrentObjs.SetActive(true);
	}

	void PickedObject()
	{
		_agentComponent.FinishedWaiting = true;
	}

	void UpdateState()
	{
		switch (State)
		{
			case (int)ShoppingState.GoingToObject:
			{
                    IsShopperCrowdedComponent.SetActive(true);
                    if (_acquiredObjCnt == _desiredObjCnt)
                    {
                        PayOrLeave();
                        return;
                    }
                    _totalObjCnt = isleDataSO.ipadCount[currentIsleIndex];

                    if (_totalObjCnt <= 0)
					{
                        //IsShopperCrowdedComponent.SetActive(false);
                        //FindClosestIpad.SetActive(false);
                        UpdateIsleTravelFlags(currentIsleIndex);

                        if (islesTravelledSignature == ALLISLESDONE)
						{
                            
                            PayOrLeave();
							return;
						}
						else
						{
                            GetNextIsle();
							return;
                        }
                    }
                    if(targetObject != null)
                    {
                        float targetDist = Vector2.Distance(new Vector2(targetObject.transform.position.x, targetObject.transform.position.z), new Vector2(transform.position.x, transform.position.z));
                        if (targetDist < 1f)
                        {
                            _desiredObj = targetObject;
                            _desiredObj.GetComponentInChildren<ObjComponent>().addShopperListener(this);
                            _agentComponent.LookAtTargetSmooth(_desiredObj, 2);
                            State = (int)ShoppingState.PickingUpObject;
                        }
                    }
					
					if(_desiredObj != null & !isPickingUpObj)
					{
                        _agentComponent.SteerTo(_desiredObj.position);
                        _desiredObj.GetComponentInChildren<ObjComponent>().addShopperListener(this);
                        _agentComponent.LookAtTargetSmooth(_desiredObj, 2);
                        float dist = Vector2.Distance(new Vector2(_desiredObj.transform.position.x, _desiredObj.transform.position.z), new Vector2(transform.position.x, transform.position.z));
                        if (dist < 1f)
                        {
                            State = (int)ShoppingState.PickingUpObject;
                        }
                    }
					else if(targetObject != null)
					{
                        if (targetObject.GetComponentInChildren<ObjComponent>().isDesired)
                        {
                            targetObject = GetClosestIpad(transform, currentIsleIndex);
                        }
                        else
                        {
                            _agentComponent.SteerTo(targetObject.position);
                        }
                    }
					else
					{
                        targetObject = GetClosestIpad(transform, currentIsleIndex);
                        if(targetObject != null)
                        {
                            targetObject.GetComponentInChildren<ObjComponent>().addShopperListener(this);
                            _agentComponent.SteerTo(targetObject.position);
                        }
                    }
                    if (targetObject != null) 
                    {
                        if (targetObject.GetComponentInChildren<ObjComponent>().Achieved == true)
                        {
                            targetObject = null;
                        }
                    }

                    if (_desiredObj != null)
                    {
                        if (_desiredObj.GetComponentInChildren<ObjComponent>().Achieved == true & _desiredObj.GetComponentInChildren<ObjComponent>().AchievingAgent != this)
                        {
                            _desiredObj = null;
                        }
                    }
                    
                }
				break;
			case (int)ShoppingState.PickingUpObject:
				
				if(_desiredObj != null)
				{
                    //_agentComponent.SteerTo(_desiredObj.position);
                    _agentComponent.LookAtTargetSmooth(_desiredObj, 3);
                    if (!transform.GetComponent<VRShopperAnimationController>().IsAnimated()) //one function, coroutine time delay?
                    {
                        if (isFacingTarget(_desiredObj))
                        {
                            if (!isPickingUpObj)
                            {
                                isPickingUpObj = true;
                                _desiredObj.GetComponentInChildren<ObjComponent>().AchievingAgent = gameObject;
                                _desiredObj.GetComponentInChildren<ObjComponent>().Achieved = true;
                                StartCoroutine(PickUpObj());
                            }
                        }
                    }
                    return;
                }
				else
				{
					State = (int)ShoppingState.GoingToObject;
				}

                _navmeshAgent.updateRotation = true;

				if (_acquiredObjCnt >= _desiredObjCnt)
				{ 
					if (_affectComponent.Emotion[(int)EType.Reproach] < 0.5f)
					{
                        //IsShopperCrowdedComponent.SetActive(false);
                        State = (int)ShoppingState.GoingToLine; //go to line
                    }
					else
					{
                        //IsShopperCrowdedComponent.SetActive(false);
                        State = (int)ShoppingState.Exiting; //exit without paying
                    }
                      
				}
				else
					if (!isPickingUpObj)
					{
						State = (int)ShoppingState.GoingToObject; //go to another object  
					}
					                      

				break;
			case (int)ShoppingState.GoingToLine:

                if (_acquiredObjCnt == 0)
                {
                    State = (int)ShoppingState.Exiting;
                    break;
                }

                if (_LineHandler.PayingShopper == this)
                {
                    State = (int)ShoppingState.Paying;
                    return;
                }
                else if (_LineHandler.ShopperAtFrontOfLine == this)
                {
                    State = (int)ShoppingState.WaitingInLine;
                    return;
                }

                if (Vector3.Distance(transform.position, _LineHandler.LineEntrance) < 0.5f)
                {
                    if (!isRegistered)
                    {
                        _LineHandler.RegisterWithLineHandler(this); //immiedietly adds to line and adds index ???
                        isRegistered = true;
                    }

                    LineDestination = _LineHandler.RequestLinePosition(this); //then gets line position, even when far away

                    _agentComponent.SteerTo(LineDestination);

                    if (_LineHandler.PayingShopper == this)
                    {
                        State = (int)ShoppingState.Paying;
                    }

                    else if (_LineHandler.ShopperAtFrontOfLine == this)
                    {
                        State = (int)ShoppingState.WaitingInLine;
                    }

                    else if (Vector3.Distance(transform.position, LineDestination) < 0.2f)
                    {
                        State = (int)ShoppingState.WaitingInLine;
                    }
                }
                else 
                {
                    if(LineDestination == Vector3.zero)
                    {
                        _agentComponent.SteerTo(_LineHandler.LineEntrance);
                    }
                    else
                    {
                        _agentComponent.SteerTo(LineDestination);
                    }
                }
               

                break;

            case (int)ShoppingState.WaitingInLine:

				//_agentComponent.LookAt(_LineHandler.PayingPosition.transform.position, 2); //smoothLookat
                _agentComponent.LookAtTargetSmooth(_LineHandler.PayingPosition.transform, 1);
                _agentComponent.SteerTo(LineDestination);

                if (_LineHandler.PayingShopper == this)
                {
                    State = (int)ShoppingState.Paying;
                }

                break;

			case (int)ShoppingState.Paying: //avoidance priority 0?
                _navmeshAgent.avoidancePriority = 0;
               // _agentComponent.SteerTo(_LineHandler.PayingPosition.transform.position);
                //_agentComponent.LookAt(paymentCollider.transform.position,2); //smoothLookAt
                _agentComponent.LookAtTargetSmooth(paymentCollider.transform, 1f);

                if (!transform.GetComponent<VRShopperAnimationController>().IsAnimated() && transform.GetComponent<Animator>().GetBool("VRIK_IsMoving") == false 
                    && isFacingTarget(paymentCollider.transform))
				{
                    StartCoroutine(Pay());
                }

				break;

			case (int)ShoppingState.Exiting:

                _agentComponent.SteerTo(_exit);
                if (hasPaid)
                {
                    LineDestination = Vector3.zero;
                    if (!hasLeft)
                    {
                        if (_LineHandler.PayingShopper == null)
                        {
                            _LineHandler.DeregisterShopper(this);
                            hasLeft = true;
                        }
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

                if (islesTravelledSignature == ALLISLESDONE)
				{
					PayOrLeave();
				}
				else if (switchingLaneSides)
				{
                    _agentComponent.SteerTo(secondaryIsleDestination);
                    /* if (Vector3.Distance(transform.position, secondaryIsleDestination) < 3f)
                     {
                         switchingLaneSides = false;
                         secondaryIsleDestination = Vector3.zero;
                         IsleDestination = _shelfComp.FindClosestIsleWaypoint(transform.position, nextIsleIndex);
                         _agentComponent.SteerTo(IsleDestination);
                     }
                     else
                     {
                         _agentComponent.SteerTo(secondaryIsleDestination);
                     }*/

                    /* if(Vector3.Distance(transform.position, secondaryIsleDestination) < 1f)
                     {
                         secondaryIsleDestination = Vector3.zero;
                         switchingLaneSides = false;
                         GetNextIsle();
                     }*/
                }
                else
                {
                    
                    if(IsleDestination == Vector3.zero)
                    {
                        GetNextIsle();
                        _agentComponent.SteerTo(IsleDestination);
                    }
                    else
                    {
                        _agentComponent.SteerTo(IsleDestination);
                    }
                   /* if (Vector3.Distance(transform.position, IsleDestination) < 1f)
                    {
                        ShopInIsle();
                    }*/
                }
                
				break;

		}

	}

    private bool isFacingTarget(Transform desiredObj)
    {
        Vector3 target = desiredObj.position;
        target.y = 0;
        Vector3 shopper = transform.position;
        shopper.y = 0;
        float angle = 15;
        //float x = Vector3.Angle(transform.forward, desiredObj.transform.position - transform.position);
        float x = Vector3.Angle(transform.forward, target - shopper);
        if ( x < angle)
        {
            return true;
        }
        return false;
    }

    private void UpdateIsleTravelFlags(int index)
	{
        islesTravelledSignature = islesTravelledSignature | (1 << index);
	}

	private bool hasAlreadyTraversedIsle(int index)
	{
		if((islesTravelledSignature | (1 << index)) == islesTravelledSignature)
		{
			return true;
		}
		return false;
    }

	private void ShopInIsle()
	{
		//UpdateIsleTravelFlags(currentIsleIndex);
        State = (int)ShoppingState.GoingToObject;
	}
	public void switchLaneSide()
	{
        currentSwitchingIsle = currentIsleIndex;
        secondaryIsleDestination = _shelfComp.getOtherWaypoint(transform, currentIsleIndex); //if random then secondaryDestination check will fail
        _agentComponent.SteerTo(secondaryIsleDestination);
		switchingLaneSides = true;
        State = (int)ShoppingState.ShelfChanging;
    }

    //evaluation at WP on route to change sides or shop depending on ipad/shopper ratio
    //While agent is shelfchanging to nextIsleIndex, while crossing an isle intersection
    //they will evaluate whether to grab ipads here or continue
    //based on the distance of the closest ipad,compared to the next closest agent in the aisle,
    //compared to the ratio of shoppers to ipads remaining in the aisle
    //compared to the number of isles checked

    //Wp eval, go down isle or continue?
    //is shopper ratio good?
    //is ipad super close? distance to ipad vs total ipads in isle (lots of ipads and close ipad, go for it) , (few ipads, far away, dont bother)
    //scarcity would be number of isles checked vs how many ipads i have (check many isles dont have many, more desparate)
    //is there lots of shoppers in lane infront?
    public void wpEvaluation(Transform wp) //should be in shelfChanging State while performign wpEvaluation
	{
        currentIsleIndex = wp.GetComponentInParent<IsleComponent>().IsleIndex;

        if (switchingLaneSides)
        {
            if (Vector3.Distance(wp.position, secondaryIsleDestination) < 2f)
            {
                switchingLaneSides = false;
                secondaryIsleDestination = Vector3.zero;
                if (currentIsleIndex != nextIsleIndex)
                {
                    IsleDestination = _shelfComp.FindClosestIsleWaypoint(transform.position, nextIsleIndex);
                    _agentComponent.SteerTo(IsleDestination);
                    return;
                }     
            }
        }

        if (Vector3.Distance(wp.position, IsleDestination) < 2f)
		{
			destinationSet = false;
		}

        float distTolerance = 1.3f;
		float shopperRatioTolerance = 3;

        float ipadCount = isleDataSO.ipadCount[currentIsleIndex]; 
        

        if (ipadCount == 0)
		{
            UpdateIsleTravelFlags(currentIsleIndex);
            if(currentIsleIndex == nextIsleIndex)
            {
                GetNextIsle();
                return;
            }
            else
            {
                if(!destinationSet)
                {
                    IsleDestination = _shelfComp.FindClosestIsleWaypoint(transform.position, nextIsleIndex);
                    destinationSet = true;
                }
               
                _agentComponent.SteerTo(IsleDestination);
            }
            return;
        }

        int shopperIsleCount = isleDataSO.shoppersCount[currentIsleIndex];
        float ipadShopperRatio = ipadCount;

        if (shopperIsleCount != 0)
        { 
            ipadShopperRatio = ipadCount / (float)shopperIsleCount;
            Transform closestIpad = GetClosestIpad(transform, currentIsleIndex);
            Transform nextClosestShopper = GetClosestShopperToTargetObj(closestIpad, wp, currentIsleIndex);

            //remainingIsles vs checkedIsles
            //aquiredIpad vs checkedIsles

            //how shoppers go from ShelfChanging -> GoingToObj/PickingUpObj (shopping)
            if(closestIpad != null)
            {
                if ((Vector3.Distance(closestIpad.position, transform.position) * distTolerance) < //keeps throwing null reference
               Vector3.Distance(closestIpad.position, nextClosestShopper.position))
                {
                    //islesChecked++;
                    ShopInIsle();
                    return;
                }
            }
           
            if (ipadShopperRatio >= shopperRatioTolerance)
            {
                //islesChecked++;
                ShopInIsle();
                return;
            }

            int shopperLaneCount = howManyShoppersThisWay(_shelfComp.getWaypoint(nextIsleIndex));
            //int remainingIsles = nextIsleIndex - currentIsleIndex;
            if(Math.Abs(currentIsleIndex - nextIsleIndex) > 2)
            {
                if (shopperLaneCount > shopperIsleCount)
                {
                    if(currentSwitchingIsle != currentIsleIndex)
                    {
                        UpdateIsleTravelFlags(currentIsleIndex);
                        switchLaneSide();
                        return;
                    }
                    //GoToOtherSide
                    //mainly to other side?
                }
            }
          

            UpdateIsleTravelFlags(currentIsleIndex);
            if(currentIsleIndex == nextIsleIndex)
            {
                GetNextIsle();
            }
            
            return;
                    
        }
		else
		{
            ShopInIsle();
            return;
        }
        /*
        if(nextIsleIndex == currentIsleIndex)
        {
            UpdateIsleTravelFlags(currentIsleIndex);
            GetNextIsle();
        }*/
        
        //if closest to ipad by signifant margin => goToIpad, InIsleState now?
        //if myDistance * 1.3 < otherDistance, GO FOR IT, 1.3 should be variable

        //in isleShoppingState
        //if ipadShopperRatio is good, the ihgher the ratio, the higher difference in distance
        //ipadShopperRatio vs (closestIpad to nextClosestShopper)
        //keep shopping or switch isles
        //if shopperIsleCount < shopperLaneCount

        //if ipadShopper Ratio vs. (shopperLaneCount / remainingIsles)

        //how many ipads do i have
        //greedy search, (ipad/shopper ratio vs closest ipad distance) vs (shoppers in lane/distance to nextIsle)
        //overall a (distance to closest ipad) vs (shopper / ipad count ) makes sense, change isle if fail to get ipad
        //(closestIpad vs nextClosestShopper) vs (ipadShopperRatio)
        //how many people are on my way to my current destination
        //how many people are in the isle
        //how far my original distance is


        //colliding with ipad chooses who consumes it, not other way around
        //from parent, if collider with agent invoke agent to take
    }
	private int howManyShoppersThisWay(Vector3 Target)
	{
        int layermask = LayerMask.GetMask("IpadDetection");
        int count = 0;
        RaycastHit[] hits;
		Ray ray = new Ray(transform.position + new Vector3(0,1,0), transform.forward/*(Target - transform.position)*/);
        hits = Physics.RaycastAll(ray, 5f, layermask, QueryTriggerInteraction.Collide);

        Debug.DrawRay(transform.position + new Vector3(0, 1, 0),
        /*Target - transform.position*/transform.forward,
        Color.green, 3f);

        foreach (var hit in hits) {
			if (hit.transform.CompareTag("Player")) count++; 
		}
        //Debug.Log("COUNT: " + count);
		return count;
    }

	private int getRandomFirstIsleTarget()
	{
		Reseed();
        return Random.Range(0, NUMOFISLES);
    }

	private void Reseed()
	{
		Random.InitState(System.DateTime.Now.Millisecond);
    }

    private Transform GetClosestShopperToTargetObj(Transform closestIpad, Transform wp, int isleIndex)
    {
		//could equal self
		//List<ShopperBehavior> shoppers = isleManager.GetShoppersInIsle(isleIndex);
        List<ShopperBehavior> shoppers = isleDataSO.shoppers[isleIndex];

        if (shoppers.Count == 0)
		{
			return transform;
		}
        float zPos = transform.position.z;
        float minDist = 100;
        Transform closestShopper = null;
        foreach (ShopperBehavior shopper in shoppers)
        {
            float shopperDist = System.Math.Abs(shopper.transform.position.z - zPos);

            if (minDist > shopperDist)
            {
                minDist = shopperDist;
                closestShopper = shopper.transform;
            }
        }
        return closestShopper;
    }

    private Transform GetClosestIpad(Transform Shopper, int index)
    {
        Ray ray;
        List<ObjComponent> ipads = isleDataSO.ipads[index];

        //float zPos = transform.position.z;
        float minDist = 100;
        int count = 0;
        Transform closestIpad = null;
        foreach (ObjComponent ipad in ipads)
        {
            if (ipad != null)
            {
                if (!ipad.isDesired)
                {
                    count++;
                    float ipadDist = Vector3.Distance(ipad.transform.parent.position, transform.position);

                    ray = new Ray(transform.position, 
                          ipad.transform.parent.position - transform.position);

                    /*
                    Debug.DrawRay(transform.position,
                    ipad.transform.parent.position - transform.position, 
                    Color.green, 3f);
                    */

                    int layermask = LayerMask.GetMask("IpadDetection");
                    RaycastHit[] hits;
                    hits = Physics.RaycastAll(ray, ipadDist, layermask, QueryTriggerInteraction.Collide);
                    foreach(RaycastHit hit in hits)
                    {
                        if (hit.collider.transform.CompareTag("IpadConsumer"))
                        {
                            ipadDist += 5f;
                        }
                    }
                    /*
                    if (Physics.Raycast(ray, out RaycastHit hit, ipadDist, (1 << 8)))
                    {
                       if(hit.transform.CompareTag("IpadConsumer"))
                        {
                            ipadDist += 5f;
                        }
                    }*/
                    if (minDist > ipadDist)
                    {
                        minDist = ipadDist;
                        closestIpad = ipad.transform;
                    }
                }
            }
        }
        if(currentIsleIndex != 10)
        {
            if (count == 0 || minDist > 5)
            {
                UpdateIsleTravelFlags(currentIsleIndex);
                GetNextIsle();
            }
        }
        else
        {
            if(count == 0 || minDist > 10)
            {
                UpdateIsleTravelFlags(currentIsleIndex);
                GetNextIsle();
            }
        }
       

        //objComponent is child of physical ipad, which is offset for the colliders to be in the aisle
        if (closestIpad != null)
        {
            return closestIpad.parent.transform;
        }
        else
        {
            return null;
        }
    }

    //colliding with ipad chooses who consumes it, not other way around
    //from parent, if collider with agent invoke agent to take
    public void PickUpIpad(Transform targetIpad)
	{
		_desiredObj = targetIpad;
		_agentComponent.SteerTo(targetIpad.position);
		State = (int)ShoppingState.GoingToObject;
    }
    private void GetNextIsle()
    {
        float[] isleIndexes = new float[NUMOFISLES];
        float summation = 0;
        
        if (!hasAlreadyTraversedIsle(0))
        {
            isleIndexes[0] = (1f / System.Math.Abs((float)currentIsleIndex));
            summation += isleIndexes[0];
        }
        else
        {
            isleIndexes[0] = 0;
        }
        for (int i = 1; i < NUMOFISLES; i++)
		{
			if (!hasAlreadyTraversedIsle(i))
			{
				float val = (1f / System.Math.Abs((float)currentIsleIndex - (float)i));
                isleIndexes[i] = val + isleIndexes[i-1];
				summation += val;
			}
			else
			{
				isleIndexes[i] = isleIndexes[i - 1];
            }
        }

		if(summation == 0)
		{
			PayOrLeave();
            return;
		}

		Reseed();
        float p = UnityEngine.Random.Range(0, summation);
        for (int i = 0; i < NUMOFISLES; i++)
		{
			if(p <= isleIndexes[i])
			{
                destinationSet = true;
                nextIsleIndex = i;
                IsleDestination = _shelfComp.FindClosestIsleWaypoint(transform.position, nextIsleIndex);
                State = (int)ShoppingState.ShelfChanging;
				return;
			}
		}
    }

    private void PayOrLeave()
	{
		IsShopperCrowdedComponent.SetActive(false);
		FindClosestIpad.SetActive(false);
		if (_acquiredObjCnt > 0)
		{
			State = (int)ShoppingState.GoingToLine;
		}
		else
		{
			State = (int)ShoppingState.Exiting;
		}
	}
    IEnumerator PickUpObj()
    {
        _navmeshAgent.avoidancePriority = 0;
        //_agentComponent.LookAt(_desiredObj.parent.transform.position, 0.5f);
        yield return new WaitForSeconds(0.2f);
        if(_desiredObj != null)
        {
            _animationController.PlayPickupAnimation(_desiredObj.GetComponentInChildren<ObjComponent>().height);
            _acquiredObjCnt++;
        }
        //_aquiredObjUI.setAquiredObjCount(_acquiredObjCnt);
        //return new WaitForSeconds(0.1f);
		//State = (int)ShoppingState.GoingToObject;
		//isPickingUpObj = false;
    }

    IEnumerator Pay()
	{
		yield return new WaitForSeconds(1);
        _animationController.PlayPayingAnimation();
       // _navmeshAgent.updateRotation = true; //change after lookat
        yield return new WaitForSeconds(1);
        _navmeshAgent.avoidancePriority = assignedAvoidancePriority;
        hasPaid = true;
        State = (int)ShoppingState.Exiting;
        
    }

    private void EnableFindClosestIpad()
    {
        FindClosestIpad.SetActive(true);
		FindClosestIpad.GetComponent<FindClosestTargetCollision>().PrimeEvent();
    }

	public void resetDesiredObj()
	{
		//if( _desiredObj != null )
		//{
            //_desiredObj.GetComponent<ObjComponent>().removeShopperFromDesiredObjList(this);
            _desiredObj = null;
            isPickingUpObj = false;
			if (State != (int)ShoppingState.ShelfChanging)
			{
				State = (int)ShoppingState.GoingToObject;
			}
       // }
	}

    private void TryOtherIpad()
    {
		if( _desiredObj != null )
		{
            FindClosestIpad.GetComponent<FindClosestTargetCollision>().SetPreviousIpad(_desiredObj.transform);
            resetDesiredObj();
            if (!hasSwitchedSides)
            {
                switchSides();
            }
            else
            {
                if (!movingInIsle)
                {
                    if (//_isleCountData.ShopperCountInIsle[_shelfOrder[_shelfInd]]
                        isleDataSO.shoppersCount[currentIsleIndex] < 4)
                    {
                        movingInIsle = true;
                        TryFarEndOfIsle(3);
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
		else
		{
			if(_totalObjCnt < 3)
			{
                _shelfInd++;
                State = (int)ShoppingState.ShelfChanging;
			}
		}
        
    }

    private void TryFarEndOfIsle(int delaySeconds)
    {
		if(hasSwitchedSides)
		{
            _closestShelfPos = _shelfComp.FindFarthestIsleWaypoint(transform.position, _shelfOrder[_shelfInd]);
        }
		else
		{
            _closestShelfPos = _shelfComp.getOtherWaypoint(_closestShelfPos, _shelfOrder[_shelfInd]);
        }
		
        StartCoroutine(DelayShopperGoingToObject(delaySeconds));
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
            _desiredObj.GetComponentInChildren<ObjComponent>().ObjPickupSuccess();
        }
       _aquiredObjUI.setAquiredObjCount(_acquiredObjCnt);
       _navmeshAgent.avoidancePriority = assignedAvoidancePriority;
        //isPickingUpObj = false;
        //_animationController.isAnimated = false;
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
			if (_desiredObj != null && _desiredObj.GetComponentInChildren<ObjComponent>().Achieved && _desiredObj.GetComponentInChildren<ObjComponent>().AchievingAgent != this.gameObject)
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
				bool exists = _appraisal.DoesStandardExist(_desiredObj.GetComponentInChildren<ObjComponent>().AchievingAgent, AppDef.Disapproving);
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
		return (_desiredObj != null && _desiredObj.GetComponentInChildren<ObjComponent>().Achieved &&
			_desiredObj.GetComponentInChildren<ObjComponent>().AchievingAgent != null &&
			_desiredObj.GetComponentInChildren<ObjComponent>().AchievingAgent != this.gameObject);

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
		if (IsleDestination != null)
		{
            Gizmos.DrawSphere(IsleDestination, 0.2f);
        }
        Gizmos.color = Color.green;
        if (secondaryIsleDestination != null)
        {
            Gizmos.DrawSphere(secondaryIsleDestination, 0.2f);
        }

    }
	/*
	public void CrowdedChangeDirection()
	{
		//check these two spots when needed rather than always check them
		
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
			case (int)ShoppingState.GoingToObjecta:


				break;

            case (int)ShoppingState.GoingToLine:


                break;

            case (int)ShoppingState.ShelfChanginga:


                break;
        }
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
	   
	//}

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
	public void GoToClosestObj(GameObject target)
	{
		targetObject = target.transform;
        _agentComponent.SteerTo(targetObject.position);
	}

    public void GetClosestObj()
	{
		Transform ipad = gameObject.GetComponentInChildren<FindClosestTargetCollision>().ClosestTarget.transform;

		targetObject = ipad;

        FindClosestIpad.SetActive(false);
		/*
        Transform ipad = gameObject.GetComponentInChildren<FindClosestTargetCollision>().ClosestTarget.transform;

        if (_desiredObj == null)
        {
            if ((sideOfIsle == ipad.GetComponent<ObjComponent>().sideOfIsle || sideOfIsle == ShelfSide.None) && 
				ipad.parent.GetComponent<IsleComponent>().IsleIndex == _shelfOrder[_shelfInd])
            {
                _desiredObj = ipad;
                _desiredObj.GetComponent<ObjComponent>().addShopperToDesiredObjList(this);
            }

        }
        else if (sideOfIsle == ipad.GetComponent<ObjComponent>().sideOfIsle && 
			ipad.parent.GetComponent<IsleComponent>().IsleIndex == _shelfOrder[_shelfInd])
        {
            if (Vector3.Distance(transform.position, _desiredObj.transform.position) >
                Vector3.Distance(transform.position, gameObject.GetComponentInChildren<FindClosestTargetCollision>().ClosestTarget.transform.position))
            {
                _desiredObj.GetComponent<ObjComponent>().removeShopperFromDesiredObjList(this);

                _desiredObj = ipad;
                _desiredObj.GetComponent<ObjComponent>().addShopperToDesiredObjList(this);
            }
        }

        if (_desiredObj == null)
        {
            _desiredObj = ipad;
            _desiredObj.GetComponent<ObjComponent>().addShopperToDesiredObjList(this);
            sideOfIsle = ipad.GetComponent<ObjComponent>().sideOfIsle;
        }

        FindClosestIpad.SetActive(false);
*/
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

    public void setIsCrowded(bool v)
    {
		IsCrowded = v;
    }

    public void AddAquiredObjs(int collectedItemCnt)
    {
		_acquiredObjCnt += collectedItemCnt;
    }

    public void MoveUpInLine(int index)
    {
        _navmeshAgent.avoidancePriority = index;
        LineDestination = _LineHandler.RequestLinePosition(this);
        StartCoroutine(DelayedMovement(LineDestination, 0.6f));
    }

    IEnumerator DelayedMovement(Vector3 lineDestination, float v)
    {
        Reseed();
        float x = Random.Range(0f, v);
        yield return new WaitForSeconds(x);
        _agentComponent.SteerTo(LineDestination);
    }

    public int getCurrentIsleIndex()
	{
		/*
		if(_shelfInd <= 10)
		{
            return _shelfOrder[_shelfInd];
        }
		else
		{
            return _shelfOrder[10];
        }*/
		return currentIsleIndex;
    }

    public void getNewDesiredObj(Transform ipad)
    {
		//if closestAgent > 1 away from desiredObj
		resetDesiredObj();
        FindClosestIpad.GetComponent<FindClosestTargetCollision>().SetPreviousIpad(ipad);
		EnableFindClosestIpad();
    }

    public void resetTargets()
    {
		_desiredObj = null;
		targetObject = null;
    }

    public void setAssignedAvoidancePriority()
    {
        assignedAvoidancePriority = _navmeshAgent.avoidancePriority;
    }
  
    public void updateShopperTargets(ObjComponent ipad)
    {
        if(ipad == targetObject)
        {
            targetObject = null;
        }
        if(ipad == _desiredObj)
        {
            _desiredObj = null;
            isPickingUpObj = false;
        }
        if(State == (int)ShoppingState.PickingUpObject)
        {
            isPickingUpObj = false;
            State = (int)ShoppingState.GoingToObject;
        }
    }

    public void ShopperStatusCheck()
    {
        if(State == (int)ShoppingState.ShelfChanging ||
            State == (int)ShoppingState.GoingToObject ||
            State == (int)ShoppingState.PickingUpObject)
        {
            isMoving = 0;

            if (!ShopperStatusWatch)
            {
                ShopperStatusWatch = true;
                StartCoroutine(CountDownToHandle(3f));
            }
        }
    }

    public void RemoveShopperStatusCheck()
    {
        isMoving++;
    }

    IEnumerator CountDownToHandle(float time)
    {
        yield return new WaitForSeconds(time);
        if(isMoving > 0)
        {
            HandleShopperStuck();
        }
        ShopperStatusWatch = false;
    }

    private void HandleShopperStuck()
    {
        if(_desiredObj != null)
        {
            _desiredObj.GetComponentInChildren<ObjComponent>().reset();
        }
        _desiredObj = null;
        targetObject = null;
        isPickingUpObj = false;
        State = (int)ShoppingState.GoingToObject;
        GetNextIsle(); //REMOVE
    }

    public void ChooseInitialIsle()
    {
        float[] isleIndexes = new float[NUMOFISLES];
        float summation = 0;
        float val = 0;
        if (door1Set)
        {
            Reseed();
            currentIsleIndex = Random.Range(4, 7);
        }
        else
        {
            Reseed();
            currentIsleIndex = Random.Range(9, 11);
        }

        val = 1f / currentIsleIndex;
        isleIndexes[0] = val;
        summation += val;

        for (int i = 1; i < NUMOFISLES; i++)
        {
            
            if (i != currentIsleIndex)
            {
                val = (1f / System.Math.Abs((float)currentIsleIndex - (float)i));
                val = val - (aiManager.intitalIsleShopperCount[i] * (val / 3)); 
                if (val < 0) val = 0;
                isleIndexes[i] = val + isleIndexes[i - 1];
                summation += val;
            }
            else
            {
                val = 4f;
                val = val - (aiManager.intitalIsleShopperCount[i] * (val / 3));
                if (val < 0) val = 0;
                isleIndexes[i] = val + isleIndexes[i - 1];
                summation += val;
            }
        }

        Reseed();
        float p = UnityEngine.Random.Range(0, summation);
        for (int i = 0; i < NUMOFISLES; i++)
        {
            if (p <= isleIndexes[i])
            {
                destinationSet = true;
                nextIsleIndex = i;
                IsleDestination = _shelfComp.FindClosestIsleWaypoint(transform.position, nextIsleIndex);
                aiManager.intitalIsleShopperCount[i]++;
                State = (int)ShoppingState.ShelfChanging;
                return;
            }
        }

    }

}
