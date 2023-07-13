using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class LineHandler : MonoBehaviour
{

	public Vector3 LineEnd;
	Vector3 _originalPos;

	public List<GameObject> AgentsInLine;
	float _agentSpace = 1f;

	void Start()
	{
		_originalPos = GameObject.Find("counter").transform.position;
		LineEnd = _originalPos;
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


}
