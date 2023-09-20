using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FindClosestTargetCollision : MonoBehaviour
{
    [SerializeField] string TargettedTag;
    [SerializeField] float MinimumRange;
    [SerializeField] float MaximumRange;
    [SerializeField] float Frequency;
    [SerializeField] UnityEvent FireEvent;
    public SphereCollider Collider;
    private float ActiveRadius;
    public GameObject ClosestTarget;
    private bool EventFired = false;
    // Start is called before the first frame update
    void Start()
    {
        Collider = GetComponent<SphereCollider>();
        ActiveRadius = MinimumRange;
        Collider.radius = ActiveRadius;
    }

    // Update is called once per frame
    void Update()
    {
        ActiveRadius += Time.deltaTime * Frequency;
        Collider.radius = ActiveRadius;
        if (ActiveRadius >= MaximumRange) ActiveRadius = MinimumRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(TargettedTag))
        {
            if (FireEvent != null)
            {
                ClosestTarget = other.gameObject; // potential problems if colliders are children of gameobject or collider on correct gameobject of prefab
                if(!EventFired)
                {
                    FireEvent.Invoke();
                    EventFired = true;
                }
                
            }    
        }
        /*else
        {
            Debug.Log("Collision at: " + Vector3.Distance(other.transform.position, transform.position));
        }*/
    }

    internal void PrimeEvent()
    {
        EventFired = false;
    }
}

