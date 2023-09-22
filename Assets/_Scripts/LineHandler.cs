using UnityEngine;
using System.Collections.Generic;
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
	public Queue<ShopperBehavior> ShoppersWaitingInLine = new Queue<ShopperBehavior>();
	public bool shoppersWaitingToPay = false;

	public Vector3 LineEnd;
	Vector3 _originalPos;

	public List<GameObject> AgentsInLine;
	float _agentSpace = 1f;
    public float LinePositionStepSize;

    void Start()
	{
		PayingCollider = PayingPosition.GetComponent<GetShopperInPosition>();
		FrontOfLineCollider = FrontOfLinePosition.GetComponent<GetShopperInPosition>();
		//_originalPos = GameObject.Find("counter").transform.position;
		//LineEnd = _originalPos;
		linesize = 0;
	}

	private void Update()
	{

	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(nextPosition, 0.2f);

	}

    public Vector3 RequestLinePosition(ShopperBehavior sb)
    {
        /*if(linesize < 3)
		{
            return nextPosition;
        }
		else
		{
			return CalculateLinePosition(sb);
		}*/
        return CalculateLinePosition(sb);
    }

	private int CompareDistance(ShopperBehavior shopperA, ShopperBehavior shopperB)
	{
		float a = Vector3.Distance(shopperA.transform.position, nextPosition);
		float b = Vector3.Distance(shopperB.transform.position, nextPosition);
		return a.CompareTo(b);
	}

    public void RegisterWithLineHandler(ShopperBehavior shopperBehavior)
    {
		LineList.Add(shopperBehavior);
		linesize++;
		LineList.Sort(CompareDistance);
        UpdateLinePosition();
    }

    private void UpdateLinePosition()
    {
        if (linesize == 1)
        {
            nextPosition = PayingPosition.transform.position;
        }
        else
        {
            nextPosition = FrontOfLinePosition.transform.position;
        }
    }

	private Vector3 CalculateLinePosition(ShopperBehavior sb)
	{
		int index = LineList.FindIndex(x => x.Equals(sb));
		if(index == 0) 
		{ 
			return PayingPosition.transform.position; 
		}
		if(index == 1) 
		{ 
			nextPosition = FrontOfLinePosition.transform.position;
		}
		return new Vector3(
			(FrontOfLinePosition.transform.position.x + (LinePositionStepSize * index-1)), 
			FrontOfLinePosition.transform.position.y, 
			FrontOfLinePosition.transform.position.z
			);
    }

    public void DeregisterShopper(ShopperBehavior shopperBehavior)
    {
        LineList.Remove(shopperBehavior);
        linesize--;
        LineList.Sort(CompareDistance);
        UpdateLinePosition();
		UpdateShopperPositions();
    }

    private void UpdateShopperPositions()
    {
        foreach(ShopperBehavior sb in LineList)
		{
			sb.MoveUpInLine();
		}
    }
}
