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
    [SerializeField] UnityEvent<UnityEngine.Object> FireEvent;

    private Transform previousDesiredObj;
    public SphereCollider Collider;
    public int pulses = 3;
    public int currentPulses;
    private float ActiveRadius;
    public GameObject ClosestTarget;
    private bool EventFired = false;
    private int shelfIndex;
    // Start is called before the first frame update
    void Start()
    {
        Collider = GetComponent<SphereCollider>();
        currentPulses = pulses;
        ActiveRadius = MinimumRange;
        Collider.radius = ActiveRadius;
      //  shelfIndex = GetComponentInParent<ShopperBehavior>().getCurrentIsleIndex();//must replace with currentIndex
    }

    // Update is called once per frame
    void Update()
    {
        ActiveRadius += Time.deltaTime * Frequency;
        Collider.radius = ActiveRadius;
        if (ActiveRadius >= MaximumRange)
        {
            ActiveRadius = MinimumRange;
            //currentPulses -= 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TargettedTag) && 
            other.GetComponentInParent<IsleComponent>().IsleIndex == shelfIndex)//must replace with currentIndex
        {
            /*if (other.GetComponent<ObjComponent>()._desiringShopperIsClose == false  && notPreviousIpad(other))
            {
                if (ClosestTarget == null)
                {
                    ClosestTarget = other.gameObject;
                    this.GetComponentInParent<AgentComponent>().SteerTo(ClosestTarget.transform.position);
                }
                else if (Vector3.Distance(other.gameObject.transform.position, transform.position) <
                    Vector3.Distance(ClosestTarget.transform.position, transform.position))
                {
                    ClosestTarget = other.gameObject;
                }

                if (FireEvent != null && currentPulses < 1)
                {
                    currentPulses = pulses;
                    if (!EventFired)
                    {
                        FireEvent.Invoke();
                        EventFired = true;

                    }
                }
            }*/
            ClosestTarget = other.gameObject;
            if (FireEvent != null/* && currentPulses < 1*/)
            {
                //currentPulses = pulses;
                if (!EventFired)
                {
                    FireEvent.Invoke(other.gameObject);
                    EventFired = true;
                }
            }
        }
           
    }

    internal void PrimeEvent()
    {
        EventFired = false;
        //shelfIndex = GetComponentInParent<ShopperBehavior>().getCurrentIsleIndex();
        ClosestTarget = null;
    }

    public void SetPreviousIpad(Transform ipad)
    {
        previousDesiredObj = ipad;
    }

    private bool notPreviousIpad(Collider other)
    {
       if(previousDesiredObj == null) { return true; }
       if(other.transform != previousDesiredObj) { return true; }
       return false;
    }
}

