using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanComponent : MonoBehaviour, GeneralStateComponent
{
	public float Damage;
	public float TimeLastFight = 0f;
	public bool isFighting = false;
	public bool IsFightStarter = false;
	public Vector3 HandPos; //for picking up objects
	public bool StartedWaiting = false;
	public bool FinishedWaiting = false;
	public ArrayList CollidingAgents;
	public int FightCnt = 0;
	private AgentComponent [] _agentComponents;
	private AudioSource _audioSource;
	private AudioClip _hitSound;
    [SerializeField] private Transform _eyeTransform; //what am I seeing? Camera's position
	public float VisibilityAngle = 60f;

	//For VR, used for punching, Author Nathan Brilmayer
	[SerializeField] GameObject LeftHand;
    [SerializeField] GameObject RightHand;
	bool isPunch = false;

    void Start()
	{
				
		CollidingAgents = new ArrayList(); //agents that are colliding with me
		_audioSource = GetComponent<AudioSource>();
		_hitSound = Resources.Load("hit") as AudioClip;

		//_eyeTransform = transform.Find("FirstPersonCharacter").transform; //Swapped for camera transform, added manually in inspector
	}

	public void Restart()
	{
		FightCnt = 0;
		Damage = 0f;
		if (IsFighting())
		{
			DestroyImmediate(GetComponent<HumanFightBehavior>());
		}
	}
	void Update()
	{
		if(isPunch) //isPunch for VR punching, activated from PunchComponent.cs
        {/*Input.GetKey(KeyCode.F)*/


            if (TimeSinceLastFight()>5f) { //don't start a fight immediately
				foreach(GameObject c in CollidingAgents) {

					if(c.CompareTag("Player") && IsVisible(c, VisibilityAngle) && c.GetComponent<AgentComponent>().IsGoodToFight(this.gameObject, 2f)) {
						StartFight(c, true);
						c.GetComponent<AgentComponent>().StartFight(this.gameObject, false);
					}
				}
			}
		}
		if (IsFighting() == false)
		{
			if (TimeSinceLastFight() > 2) //start healing after 2 seconds
				Heal();
		}
	}

	public void PlaySound() {
		_audioSource.PlayOneShot(_hitSound);
	}



	public IEnumerator WaitAWhile(int seconds)
	{
		StartedWaiting = true;
		FinishedWaiting = false;
		yield return new WaitForSeconds(seconds);

		FinishedWaiting = true;

		//Dont't forget to set started waiting to false before state change
	}

	public bool IsFighting()
	{
		return (GetComponent<HumanFightBehavior>() != null);
	}
	public bool IsWounded()
	{
		return Damage > 1;
	}
	public bool IsPolice()
	{
		return true;
	}
	public bool HasFallen()
	{
		return false;
	}
	public bool IsVisible(GameObject other, float viewAngle)
	{
		RaycastHit hit;
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Agent");
		Debug.DrawRay(_eyeTransform.position, _eyeTransform.forward);
		if(Physics.Raycast(_eyeTransform.position, _eyeTransform.forward, out hit, layerMask)) {
			
			return true;

        }

		return false;

		//	Vector3 orientation = _eyeTransform.forward;//GetComponent<NavMeshAgent>().velocity;
		//Vector3 distVec = other.transform.position - _eyeTransform.position;
		//orientation.y = distVec.y = 0f;
		//orientation.Normalize();
		//distVec.Normalize();
		//float angle = Vector3.Angle(orientation, distVec);//Mathf.Acos(Vector3.Dot(distVec, orientation));
		//if(angle <= VisibilityAngle)
		//	return true;
		//return false;
	}

	public bool CanFight()
	{
		if (IsFighting()) //already fighting
			return false;
		return true;
	}
	public void StartFight(GameObject other, bool isStarter)
	{
		FightCnt++;
		IsFightStarter = isStarter;
		if (other == null)
		{
			Debug.LogError("Opponent is null in fight");
			return;
		}
		if (GetComponent("HumanFightBehavior") == null)
		{
			this.gameObject.AddComponent<HumanFightBehavior>();
			GetComponent<HumanFightBehavior>().Init(other);
			IsFightStarter = isStarter;
		}
	}
	public float TimeSinceLastFight()
	{
		return Time.time - TimeLastFight;
	}
	public void AddDamage(float amount)
	{
		Damage += amount * Random.Range(0, 2) * Time.deltaTime;
        //Debug.Log("Player Damage: " + Damage);
    }
	public float GetDamage() {
		return Damage;
    }
	public void Heal()
	{
		if(Damage > 0) {
			Damage -= 0.1f * Time.deltaTime;
			Mathf.Clamp(Damage, 0, 1);
		}
	}
	public bool IsShopper()
	{
		return false;
	}

	/// Finds the agents around me
	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.CompareTag("Player"))
		{
			if (!CollidingAgents.Contains(collider))
				CollidingAgents.Add(collider.gameObject);
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.CompareTag("Player"))
		{
			CollidingAgents.Remove(collider.gameObject);
		}
	}
	//Made by Nathan Brilmayer, for VR
	public bool getIsPunch() { return isPunch; }

	public void setIsPunch(bool val) { isPunch = val;}

	public void PunchDamage(float amount)
	{
		if(GetComponent<HumanFightBehavior>() != null)
		{
            GetComponent<HumanFightBehavior>().DamageOpponent(amount);
        }
	}

	//void OnControllerColliderHit() {

	//}

}
