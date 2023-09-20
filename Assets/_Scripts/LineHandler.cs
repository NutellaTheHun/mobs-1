using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System;

public class LineHandler : MonoBehaviour
{
	//Queue<ShopperBehavior> queue = new Queue<ShopperBehavior>(); //.Enqueue adds to end of queue, .Dequeue returns and removes from beginning of queue
	[SerializeField] public GameObject PayingPosition;
	private GetShopperInPosition PayingCollider;
	[SerializeField] public GameObject FrontOfLinePosition;
    private GetShopperInPosition FrontOfLineCollider;

	public Vector3 nextPosition;
    public ShopperBehavior ShopperAtEndOfLine;
	public ShopperBehavior ShopperAtFrontOfLine;
	public ShopperBehavior PayingShopper;
	public int linesize;
	public List<ShopperBehavior> LineList = new List<ShopperBehavior>();
	public bool shoppersWaitingToPay = false;

	public Vector3 LineEnd;
	Vector3 _originalPos;

	public List<GameObject> AgentsInLine;
	float _agentSpace = 1f;

	void Start()
	{
		PayingCollider = PayingPosition.GetComponent<GetShopperInPosition>();
		FrontOfLineCollider = FrontOfLinePosition.GetComponent<GetShopperInPosition>();
        _originalPos = GameObject.Find("counter").transform.position;
		LineEnd = _originalPos;
		linesize = 0;
    }

    private void Update()
    {
		
		if(PayingShopper == null)
		{
			if(ShopperAtFrontOfLine == null)
			{
				if(linesize == 0)
				{
                    nextPosition = PayingPosition.transform.position;
                    FrontOfLineCollider.enabled = true;
                    PayingCollider.enabled = true;
                }
			}
		}
		else
		{
            FrontOfLineCollider.enabled = false;
            if (ShopperAtFrontOfLine == null)
			{
                if (linesize == 0)
                {
                    nextPosition = FrontOfLinePosition.transform.position;
                }
			}
			else
			{
                UpdateEndOfLineShopper();
                PayingCollider.enabled = false;
                nextPosition = ShopperAtEndOfLine.NextInLinePositionCollider.transform.position;
            }
		}
		/*if (ShopperAtFrontOfLine != null)
		{
			shoppersWaitingToPay = true;
		}
		else
		{
			shoppersWaitingToPay = false;
			FrontOfLineCollider.enabled = true;
        }
		if(PayingShopper == null)
		{
			PayingCollider.enabled = false;
		}

		if (ShopperAtEndOfLine == null && ShopperAtFrontOfLine != null)
		{
			ShopperAtEndOfLine = ShopperAtFrontOfLine;
		}

		if (ShopperAtEndOfLine != null)
		{
			if (ShopperAtEndOfLine.ShopperBehindInLine != null)
			{
				ShopperAtEndOfLine = ShopperAtEndOfLine.ShopperBehindInLine;
			}
		}*/
	}

    private void UpdateEndOfLineShopper()
    {
		ShopperBehavior sb = ShopperAtFrontOfLine;
    }

    void UpdateLineEnd()
	{
		if (LineEnd.x > -12) //update line in case
			LineEnd = _originalPos + new Vector3(-AgentsInLine.Count * _agentSpace, 0, 0);

	}

	public Vector3 FindLineEndBeforeAgent(GameObject agent)
	{
		_agentSpace = agent.GetComponent<NavMeshAgent>().radius; //make it correalted with agent's personal space
		Vector3 end = _originalPos + new Vector3(-(AgentsInLine.FindIndex(a => a.gameObject == agent)) * _agentSpace, 0, 0);
		if (end.x < -12)
			end.x = -12;
		return end;
	}

	public bool IsInLine(GameObject agent)
	{
		if (AgentsInLine.Count == 0)
			return false;
		return (AgentsInLine.Contains(agent));
	}

	public bool IsFirst(GameObject agent)
	{

		if (AgentsInLine[0].Equals(agent))
		{
			float dist = Vector2.Distance(new Vector2(agent.transform.position.x, agent.transform.position.z), new Vector2(_originalPos.x, _originalPos.z));
			if (dist < 1f)
				return true;
		}

		return false;
	}

	//Get in the line
	public void GetInLine(GameObject agent)
	{
		float dist = Vector2.Distance(new Vector2(agent.transform.position.x, agent.transform.position.z), new Vector2(LineEnd.x, LineEnd.z));
		if (dist < 1f)
		{
			AgentsInLine.Add(agent);
			UpdateLineEnd();
		}

	}
	//Leave line and move all agents forward
	public void GetOutLine(GameObject agent)
	{
		AgentsInLine.Remove(agent);
		//MoveForward();
		UpdateLineEnd();
	}


	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawCube(LineEnd, new Vector3(1, 1, 1));
	}

    public Vector3 GoToLine(ShopperBehavior sb)
    {
		if(nextPosition == ShopperAtEndOfLine.NextInLinePositionCollider.transform.position)
		{
			sb.isWaitingBehindSomeone = true;
		}
		else
		{
			sb.isWaitingBehindSomeone = false;
        }
		return nextPosition;
    }

    public void SetNewFrontOfLineShopper(ShopperBehavior shopperBehavior)
    {
		FrontOfLineCollider.enabled = false;
		ShopperAtFrontOfLine = shopperBehavior;
    }

    public void SetNewPayingShopper(ShopperBehavior shopperBehavior)
    {
		PayingCollider.enabled = false;
        PayingShopper = shopperBehavior;
    }
}
