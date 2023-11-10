using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInitialIsleChoice : MonoBehaviour
{
    ShopperBehavior sb;
    bool active = true;
    int shoppersSequence;
    public int door1count = 0;
    public int door2count = 0;
    // Start is called before the first frame update
    void Start()
    {
      //  sb = GetComponentInParent<ShopperBehavior>();
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AnimationMovingState"))
        {
            if (active)
            {
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
                active = false;
            }
        }
    }
    */
}
