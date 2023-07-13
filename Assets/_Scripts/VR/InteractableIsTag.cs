using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractableIsTag : MonoBehaviour
{
    [SerializeField] XRDirectInteractor interactor;
    [SerializeField] string targetTag;
    [SerializeField] HumanShoppingBehavior humanShopBehv;
    void Start()
    {
        interactor = GetComponent<XRDirectInteractor>();
    }

    void Update()
    {
        if(interactor.hasSelection) { checkTag(); }
    }
    
    public void checkTag()
    {
        foreach(IXRSelectInteractable c in interactor.interactablesSelected) 
        {
            if (c.transform.gameObject.CompareTag(targetTag))
            {
                //Debug.Log(c.transform.gameObject.tag);
                humanShopBehv.DesiredObjectPickedUp();
            }
            else { humanShopBehv.DesiredObjectDropped();}
        }
    }


}
