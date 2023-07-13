using System.Collections;
using System.Collections.Generic;
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
			//	_audioSource.PlayOneShot(_getHitSound);


			if(humanComponent.getIsPunch())
            {/*Input.GetKey(KeyCode.F) */

                GetComponent<HumanShoppingBehavior>().Stats.PunchCnt++; //Nathan comment, spamming f will add punch count but if statement below actually faciliates punch, higher count than actuall punches possible

				if(Time.time - _lastPunchTime >= 0.2f) { //Don't allow punching continuously
					_lastPunchTime = Time.time;
		
					//_humanAnimationSelector.SelectAction("PUNCH");
					_opponentAnimationSelector.SelectAction("RECEIVEPUNCH");

					opponentComponent.AddDamage(0.5f);
				}
			}
						

		}
	}

	


	public void FinishFight()
	{
		humanComponent.TimeLastFight = Time.time;
		opponentComponent.TimeLastFight = Time.time;
		_agentHealthbar.SetActive(false);		
		DestroyImmediate(this);
	}
}
