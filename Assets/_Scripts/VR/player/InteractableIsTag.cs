using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractableIsTag : MonoBehaviour
{
    [SerializeField] XRDirectInteractor interactor;
    [SerializeField] string targetTag;
    [SerializeField] HumanShoppingBehavior humanShopBehv;
    private playerAquiredObjUI _playerAquiredObjUI;
    private aquiredObjCanvasManager _playerAquiredObjCanvasManager;
    private bool ipadInHand = false;
    void Start()
    {
        interactor = GetComponent<XRDirectInteractor>();
        _playerAquiredObjCanvasManager = GameObject.Find("Canvas").GetComponentInChildren<aquiredObjCanvasManager>();
        _playerAquiredObjCanvasManager.initializePlayerShopperCounterUI(this);
    }

    public void setPlayerAquiredUI(playerAquiredObjUI paou)
    {
        _playerAquiredObjUI = paou;
    }

    public void checkTag()
    {
        foreach(IXRSelectInteractable c in interactor.interactablesSelected) 
        {
            if (c.transform.gameObject.CompareTag(targetTag))
            {
                humanShopBehv.DesiredObjectPickedUp(c.transform.gameObject);
                _playerAquiredObjUI.handUILabelActivation(humanShopBehv.getCollectedCount());
                //ipadInHand = true;
                return;
            }
        }
    }
    public void onIpadRelease()
    {
        if(ipadInHand)
        {
            humanShopBehv.DesiredObjectDropped();
            //ipadInHand = false;
        }
        
    }

    public playerAquiredObjUI getPlayerAquiredUI()
    {
        return _playerAquiredObjUI;
    }
}
