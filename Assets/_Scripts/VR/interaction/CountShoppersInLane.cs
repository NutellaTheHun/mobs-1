using System.Collections.Generic;
using UnityEngine;

public class CountShoppersInLane : MonoBehaviour
{
    public IsleDataSO isleDataSO;
    public enum LaneType
    {
        front,
        back,
    }
    public LaneType lane;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LinePositionCollision")) //Was using "PlayerTag" but some component initialized a sphere with radius 4 of that tag so using an already implemented collider
        {
            isleDataSO.RegisterShopper(other.GetComponentInParent<ShopperBehavior>(), lane);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LinePositionCollision"))
        {
            isleDataSO.DeRegisterShopper(other.GetComponentInParent<ShopperBehavior>(), lane);
        }
    }
}
