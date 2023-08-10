using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
///Navmeshagent stops during fight, so we can use the whole body fight animations
/// </summary>

public class FightBehavior : MonoBehaviour
{
	Appraisal _appraisal;
	AgentComponent _agentComponent; // itself
	GeneralStateComponent _opponentComponent; // opponent
	public GameObject Opponent;
	public float BeginTime;
	public float EndTime;
	public List<GameObject> Watchers = new List<GameObject>();
	public bool IsOver { get; set; }
	//private AnimationSelector _animationSelector;
	private VRShopperAnimationController _shopperAnimationController; //FOR VR
	UnityEngine.AI.NavMeshAgent _navMeshAgent;
	


	public void Init(GameObject o)
	{
		_appraisal = GetComponent<Appraisal>();
		_agentComponent = GetComponent<AgentComponent>();
		BeginTime = Time.time;
		InitAppraisalStatus();
		Opponent = o;
		_opponentComponent = Opponent.GetComponent<GeneralStateComponent>();
		//_animationSelector = GetComponent<AnimationSelector>();
		//_animationSelector.SelectAction("");
		_navMeshAgent = GetComponent<NavMeshAgent>();
		_navMeshAgent.updateRotation = false; //rotation is updated according to opponent's direction

        //Added by Nathan Brilmayer FOR VR
        _shopperAnimationController = GetComponent<VRShopperAnimationController>();
		_shopperAnimationController.EnterFightState();
    }



	public FightBehavior()
	{
		IsOver = false;
	}

	void Start()
	{
		_navMeshAgent = GetComponent<NavMeshAgent>();
	}

	void Update()
	{
		if (!_opponentComponent.IsFighting() || _agentComponent.IsWounded() || _opponentComponent.IsWounded() || _agentComponent.IsFallen || _opponentComponent.HasFallen())
		{
			UpdateAppraisalStatus();
			EndTime = Time.time;
			FinishFight();
			/*if (_agentComponent.IsDead() || _agentComponent.IsFallen)
				_animationSelector.SelectAction("WRITHING");*/
		}
		else
		{
			if(!Opponent.CompareTag("RealPlayer")) { //player will add its own damage				
				_agentComponent.AddDamage( 0.05f); // get damage - human damage is higher
			}

			Vector3 targetPos = Opponent.transform.position + Opponent.transform.forward;
			_agentComponent.SteerTo(targetPos);
			//if (Vector3.Distance(transform.position, targetPos) > 1)
			//{
			//	//_navMeshAgent.updatePosition = true;
			//	//_navMeshAgent.updateRotation = true;
			//	_agentComponent.SteerTo(targetPos);
			//	Debug.DrawLine(Opponent.transform.position, targetPos);

			//}
			//else
			//{
			//	//fighting
			//	_navMeshAgent.updatePosition = false;
			//	_navMeshAgent.updateRotation = false;
			//}
			_agentComponent.LookAt(Opponent.transform.position, 0.01f);
		}
	}

	void InitAppraisalStatus()
	{

		//General distress
		_appraisal.AddGoal("fight", 0.2f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectIrrelevant);

		//Fear of losing
		_appraisal.AddGoal("fight", 0.1f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);

		//Hope to win
		_appraisal.AddGoal("fight", 0.1f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);

		//about opponent
		_appraisal.AddStandard(0.8f, AppDef.Disapproving, AppDef.FocusingOnOther, Opponent);

	}

	void UpdateAppraisalStatus()
	{
		if (_agentComponent.IsWounded())
		{
			//Resentment to opponent
			_appraisal.AddGoal("fight", 0.5f, AppDef.Displeased, AppDef.ConsequenceForOther, Opponent, AppDef.DesirableForOther);

			//Change hope to disappointment			
			_appraisal.RemoveGoal("fight", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
			_appraisal.AddGoal("fight", 0.5f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed);

			//Change fear to fearsConfirmed
			_appraisal.RemoveGoal("fight", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
			_appraisal.AddGoal("fight", 0.5f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);


		}
		if (_opponentComponent.IsWounded())
		{ //If opponent is also wounded
		  //Gloating to opponent
			_appraisal.AddGoal("fight", 0.2f, AppDef.Pleased, AppDef.ConsequenceForOther, Opponent, AppDef.UndesirableForOther);

			//Change hope to satisfaction
			_appraisal.RemoveGoal("fight", AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
			_appraisal.AddGoal("fight", 0.5f, AppDef.Pleased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Confirmed);

			//Change fear to relief
			_appraisal.RemoveGoal("fight", AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Unconfirmed);
			_appraisal.AddGoal("fight", 0.2f, AppDef.Displeased, AppDef.ConsequenceForSelf, AppDef.ProspectRelevant, AppDef.Disconfirmed);

		}
	}

	//Update other person's goals about me if they have standards about me
	public void UpdateAppraisalStatusOfOther(GameObject other)
	{
		if (other.GetComponent<Appraisal>().DoesGoalExist("fight", this.gameObject) || other.GetComponent<Appraisal>().DoesGoalExist("fight", Opponent)) //already updated
			return;
		foreach (Standard s in other.GetComponent<Appraisal>().Standards)
		{
			if (s.subject != null && (s.subject.Equals(this.gameObject) || s.subject.Equals(transform.parent.gameObject)))
			{

				if (s.approving)
				{
					if (_agentComponent.IsWounded())
					{
						//Pity for this agent
						//if goal does not already exist                        
						other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Displeased, AppDef.ConsequenceForOther, this.gameObject, AppDef.UndesirableForOther);
						//Resentment to the opponent
						other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Displeased, AppDef.ConsequenceForOther, Opponent, AppDef.DesirableForOther);
					}
					if (_opponentComponent.IsWounded())
					{
						//Happy-for  this agent
						other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Pleased, AppDef.ConsequenceForOther, this.gameObject, AppDef.DesirableForOther);
						//Gloating to the opponent
						other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Pleased, AppDef.ConsequenceForOther, Opponent, AppDef.UndesirableForOther);
					}
				}
				else
				{//disapproving
					if (_agentComponent.IsWounded())
					{
						//Gloating for this agent
						other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Pleased, AppDef.ConsequenceForOther, this.gameObject, AppDef.UndesirableForOther);
						//Happy-for  the opponent
						other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Pleased, AppDef.ConsequenceForOther, Opponent, AppDef.DesirableForOther);
					}
					if (_opponentComponent.IsWounded())
					{
						//Resentment for this agent
						other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Displeased, AppDef.ConsequenceForOther, this.gameObject, AppDef.DesirableForOther);
						//Pity for the opponent
						other.GetComponent<Appraisal>().AddGoal("fight", 0.2f, AppDef.Displeased, AppDef.ConsequenceForOther, Opponent, AppDef.UndesirableForOther);
					}
				}
			}
		}

	}

	public void FinishFight()
	{
		if (!_agentComponent.IsFallen)
		{
			//_navMeshAgent.Resume();
			_navMeshAgent.isStopped = false;
			_navMeshAgent.updatePosition = true; //rotation is updated by navmeshagent
			_navMeshAgent.updateRotation = true; //rotation is updated by navmeshagent
		}

		foreach (GameObject c in Watchers)
		{
			UpdateAppraisalStatusOfOther(c);
			c.GetComponent<AgentComponent>().IsWatchingFight = false;
			//c.GetComponent<UnityEngine.AI.NavMeshAgent>().Resume();
			c.transform.GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = false;

		}

		//TODO: Organize this part to be more modular
		// send yield message
		if(_agentComponent.IsShopper()) {
			if(Opponent.CompareTag("Player")) {

				//The winner gets the items
				if(Opponent.GetComponent<AgentComponent>().Damage > _agentComponent.Damage) { //opponent yields to me
					Opponent.GetComponent<ShopperBehavior>().YieldObjects(this.gameObject);
				}
				else { //agent yield to opponent
					GetComponent<ShopperBehavior>().YieldObjects(Opponent);
				}

			}
			else if(Opponent.CompareTag("RealPlayer")) {

				//The winner gets the items
				if(Opponent.GetComponent<HumanComponent>().Damage > _agentComponent.Damage) { //opponent yields to me
					Opponent.GetComponent<HumanShoppingBehavior>().YieldObjects(this.gameObject);
				}
				else { //agent yield to opponent
					GetComponent<ShopperBehavior>().YieldObjects(Opponent);
				}

			}
		}

		//FOR VR
		_shopperAnimationController.EnterMovingState();

        _agentComponent.TimeLastFight = Time.time;
		DestroyImmediate(this);
	}
}
