using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.Oculus;
using UnityEngine;

public class HumanFightBehavior : MonoBehaviour
{
	AgentComponent opponentComponent; // opponent
	HumanComponent humanComponent;
	HumanAnimationSelector _humanAnimationSelector;
	AnimationSelector _opponentAnimationSelector;
	public GameObject Opponent;
	public float BeginTime;
	public float EndTime;



	GameObject _agentHealthbar;

	private float _lastPunchTime;
	public void Init(GameObject o)
	{

		Opponent = o;

	

		humanComponent = GetComponent<HumanComponent>();
		opponentComponent = Opponent.GetComponent<AgentComponent>();
		//_humanAnimationSelector = GetComponent<HumanAnimationSelector>();
		_opponentAnimationSelector = o.GetComponent<AnimationSelector>();





		//_agentHealthbar = GameObject.Find("Canvas").transform.Find("HealthbarAgent").gameObject;

		//_agentHealthbar.SetActive(true);
		//_agentHealthbar.transform.Find("HealthbarRed").GetComponent<HealthBar>().Agent = opponentComponent;


		BeginTime = Time.time;
		_lastPunchTime = 0f;

		//_getHitSound = Resources.Load("getHit") as AudioClip;
	}

	private void Start()
	{
		
	}
	void Update()
	{ // grab products or lost products
		if (!opponentComponent.IsFighting() || opponentComponent.IsWounded() || opponentComponent.HasFallen() )
		{
			
			EndTime = Time.time;		
			FinishFight();

		}
		else
		{
			
			if(UserInfo.PersonalityDistribution == 0)
				humanComponent.AddDamage(0.05f);
			else
				humanComponent.AddDamage(0.1f);
			//if(Random.RandomRange(0, 1) < 0.2f)


			if(humanComponent.getIsPunch())
            {/*Input.GetKey(KeyCode.F) */

                //int x = GetComponent<HumanShoppingBehavior>().Stats.PunchCnt++; //Nathan comment, spamming f will add punch count but if statement below actually faciliates punch, higher count than actuall punches possible
				//Debug.Log("PUNCH COUNT: " + x);
				if(Time.time - _lastPunchTime >= 0.2f) { //Don't allow punching continuously
					_lastPunchTime = Time.time;
		
					//_humanAnimationSelector.SelectAction("PUNCH");
					//_opponentAnimationSelector.SelectAction("RECEIVEPUNCH");

					//opponentComponent.AddDamage(0.5f);
				}
			}
						

		}
	}

	public void FinishFight()
	{
       /* //The winner gets the items
        if (humanComponent.IsWounded())
        {   //opponent yields to me
            Opponent.GetComponent<HumanShoppingBehavior>().YieldObjects(this.gameObject);
        }
        else
        {   //agent yield to opponent
            GetComponent<ShopperBehavior>().YieldObjects(Opponent);
        }*/
        humanComponent.TimeLastFight = Time.time;
		opponentComponent.TimeLastFight = Time.time;
		//_agentHealthbar.SetActive(false);		
		DestroyImmediate(this);
	}

    public void DamageOpponent(float amount)
    {
        opponentComponent.AddDamage(amount);
        GetComponent<HumanShoppingBehavior>().Stats.PunchCnt++;
        //Debug.Log("PUNCH COUNT: " + GetComponent<HumanShoppingBehavior>().Stats.PunchCnt);
    }
}
