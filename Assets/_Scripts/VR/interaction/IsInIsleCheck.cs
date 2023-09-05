using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsInIsleCheck : MonoBehaviour
{
    private VRShopperAnimationController m_Controller;

    private void Start()
    {
        m_Controller = GetComponentInParent<VRShopperAnimationController>();
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Isle Volume"))
        {
            m_Controller.SetIsInIsle(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Isle Volume"))
        {
            m_Controller.SetIsInIsle(false);
        }
    }
}
