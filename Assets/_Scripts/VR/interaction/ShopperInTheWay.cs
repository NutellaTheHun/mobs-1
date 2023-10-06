using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShopperInTheWay : MonoBehaviour
{
    ShopperBehavior ParentShopperComp;
    ShopperBehavior otherShopper;
    ShopperBehavior leavingShopper;
    public List<ShopperBehavior> ShoppersInTheWay;
    [SerializeField] UnityEvent FireEvent;
    public float crowdedTolerance = 2f;
   
    void Start()
    {
        ParentShopperComp = GetComponentInParent<ShopperBehavior>();
        ShoppersInTheWay = new List<ShopperBehavior>();
    }

    
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("LinePositionCollision"))
        {
            otherShopper = other.GetComponentInParent<ShopperBehavior>();
            if (!ShoppersInTheWay.Contains(otherShopper) /*&& other != GetComponentInParent<ShopperBehavior>()*/)
            {
                ShoppersInTheWay.Add(otherShopper);
                ParentShopperComp.setIsCrowded(true);
                StartCoroutine(CrowdedToLongCheck(crowdedTolerance, otherShopper));
            }
        }
    }

    IEnumerator CrowdedToLongCheck(float crowdedTolerance, ShopperBehavior otherShopper)
    {
        yield return new WaitForSeconds(crowdedTolerance);
        if(ShoppersInTheWay.Contains(otherShopper))
        {
            FireEvent.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        leavingShopper = other.GetComponentInParent<ShopperBehavior>();
        if (ShoppersInTheWay.Contains(leavingShopper))
        {
            ShoppersInTheWay.Remove(leavingShopper);
            CheckIfNotCrowded();
        }
    }

    private void CheckIfNotCrowded()
    {
        if(ShoppersInTheWay.Count == 0)
        {
            ParentShopperComp.setIsCrowded(false);
        }
    }
}


