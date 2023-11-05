using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInitialIsleChoice : MonoBehaviour
{
    ShopperBehavior sb;
    bool active = true;
    // Start is called before the first frame update
    void Start()
    {
        sb = GetComponentInParent<ShopperBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AnimationMovingState"))
        {
            if (active)
            {
                sb.ChooseInitialIsle();
                active = false;
            }
        }
    }
}
