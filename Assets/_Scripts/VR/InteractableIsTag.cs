using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractableIsTag : MonoBehaviour
{
    [SerializeField] XRDirectInteractor interactor;
    [SerializeField] string targetTag;
    [SerializeField] HumanShoppingBehavior humanShopBehv;
    private playerAquiredObjUI _playerAquiredObjUI;
    private aquiredObjCanvasManager _playerAquiredObjCanvasManager;
    void Start()
    {
        interactor = GetComponent<XRDirectInteractor>();
        _playerAquiredObjCanvasManager = GetComponent<aquiredObjCanvasManager>();
        _playerAquiredObjCanvasManager.initializePlayerShopperCounterUI(this);
    }

    void Update()
    {
        if(interactor.hasSelection) { checkTag(); }
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
                //Debug.Log(c.transform.gameObject.tag);
                humanShopBehv.DesiredObjectPickedUp();
                _playerAquiredObjUI.handUILabelActivation(humanShopBehv.getCollectedCount());
            }
            else { humanShopBehv.DesiredObjectDropped();}
        }
    }


}
