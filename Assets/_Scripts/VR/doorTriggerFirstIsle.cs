using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorTriggerFirstIsle : MonoBehaviour
{
    public int door1count = 0;
    public int door2count = 0;
    int shoppersSequence;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LinePositionCollision"))
        {
            ShopperBehavior sb = other.GetComponentInParent<ShopperBehavior>();
            if (sb.door1Set)
            {
                door1count++;
                shoppersSequence = door1count;
            }
            else
            {
                door2count++;
                shoppersSequence = door2count;
            }
            sb.ChooseInitialIsle(shoppersSequence);
        }
    }
}
