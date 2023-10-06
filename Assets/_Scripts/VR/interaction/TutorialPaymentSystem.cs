using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class TutorialPaymentSystem : MonoBehaviour
{

    [SerializeField] HumanShoppingBehavior hsb;

    //GameObject parent;
    //BoxCollider c;
    //MeshRenderer mr;
    //Volume vol;

    void Start()
    {
        initializePaymentSystem();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHands"))
        {
            StartCoroutine(Wait(0.5f));
            hsb.setHasPaid(true);
        }
    }
   /* private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RealPlayer"))
        {
            hsb.setIsPaying(false);
        }
    }*/

    public IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    public void enablePaymentSystem()
    {
        //c.enabled = true;
        //mr.enabled = true;
        //vol.enabled = true;
    }

    private void initializePaymentSystem()
    {
        //parent = transform.parent.gameObject;
       // c = gameObject.GetComponentInChildren<BoxCollider>();
       //mr = parent.GetComponentInChildren<MeshRenderer>();
        //vol = parent.GetComponentInChildren<Volume>();

       // c.enabled = false;
       // mr.enabled = false;
       // vol.enabled = false;
    }

    public void playerPayingEvent()
    {
        StartCoroutine(Wait(0.5f));
        hsb.setIsPaying(true);
    }
}
