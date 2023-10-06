using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPathClear : MonoBehaviour
{
    DetermineAlternatePath parent;
    [SerializeField] private State position;
    private List<GameObject> objectsInTheWay = new List<GameObject>();
    enum State
    {
        Left,
        Right
    }

    void Start()
    {
        parent = GetComponentInParent<DetermineAlternatePath>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    { 
        if(CompareTag("Player") || CompareTag("RealPlayer") || CompareTag("PhysicalObstruction"))
        {
            switch (position)
            {
                case State.Left:
                    parent.LeftOk = false;
                    if(!objectsInTheWay.Contains(other.gameObject)) objectsInTheWay.Add(other.gameObject);
                    break;
                case State.Right:
                    parent.RightOk = false;
                    if (!objectsInTheWay.Contains(other.gameObject)) objectsInTheWay.Add(other.gameObject);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (objectsInTheWay.Contains(other.gameObject)) objectsInTheWay.Remove(other.gameObject);
        if(objectsInTheWay.Count == 0)
        switch (position)
        {
        case State.Left:
            parent.LeftOk = true;
            break;
        case State.Right:
            parent.RightOk = true;
            break;
        default:
            break;
        }
    }
}


