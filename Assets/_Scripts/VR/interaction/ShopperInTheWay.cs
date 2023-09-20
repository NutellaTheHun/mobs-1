using System.Collections.Generic;
using UnityEngine;

public class ShopperInTheWay : MonoBehaviour
{
    ShopperBehavior ParentShopperComp;
    ShopperBehavior otherShopper;
    ShopperBehavior leavingShopper;
    public List<ShopperBehavior> ShoppersInTheWay;
    // Start is called before the first frame update
    void Start()
    {
        ParentShopperComp = GetComponentInParent<ShopperBehavior>();
        ShoppersInTheWay = new List<ShopperBehavior>();
    }

    // Update is called once per frame
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
            }
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


