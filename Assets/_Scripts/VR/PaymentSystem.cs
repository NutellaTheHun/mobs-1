using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using static RootMotion.Demos.Turret;

public class PaymentSystem : MonoBehaviour
{

    [SerializeField] HumanShoppingBehavior hsb;
    
    GameObject parent;
    TextMeshPro tmp;
    BoxCollider c;
    MeshRenderer mr;
    Volume vol;

    void Start()
    {
        initializePaymentSystem();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("RealPlayer"))
        {
            //play sound or UI indicator?
            StartCoroutine(Wait(0.5f));
            hsb.setIsPaying(true);
            tmp.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RealPlayer"))
        {
            hsb.setIsPaying(false);
        }
    }

    public IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    public void enablePaymentSystem()
    {
        tmp.enabled = true;
        c.enabled = true;
        mr.enabled = true;
        vol.enabled = true;
    }

    private void initializePaymentSystem()
    {
        parent = transform.parent.gameObject;
        tmp = parent.GetComponentInChildren<TextMeshPro>();
        c = gameObject.GetComponentInChildren<BoxCollider>();
        mr = parent.GetComponentInChildren<MeshRenderer>();
        vol = parent.GetComponentInChildren<Volume>();

        tmp.enabled = false;
        c.enabled = false;
        mr.enabled = false;
        vol.enabled = false;
    }
}
