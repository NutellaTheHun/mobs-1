using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using static ShopperBehavior;

public class ObjComponent : MonoBehaviour {
    [SerializeField]
    private bool _achieved = false;
    public bool _desiringShopperIsClose = false;
    //Mesh Renderer
    MeshRenderer _meshRenderer;
    //XR Grab Interactable
    XRGrabInteractable _interactable;
    //RigidBody
    Rigidbody _rigidbody;
    //Box Collider
    BoxCollider _boxCollider;
    IsleComponent _isleComponent;
    public bool Achieved {
        get { return _achieved; }
        set { _achieved = value; }
    }

    public int Density {
        get { return _collidingAgents.Count; }
    }

    private ShopperBehavior ClosestAgent;
    public GameObject AchievingAgent;
    private List<ShopperBehavior> ShoppersDesiringThisObj;
    public ShopperBehavior targetAgent;
    private int targetShoppersAvoidancePriority;
    public bool isDesired;
    private ArrayList _collidingAgents;
    private float minDist = 10000f;
    public int isleIndex;
    public IsleDataSO isleDataSO;
    //SphereCollider
    SphereCollider _sphereCollider;
    SphereCollider parentSphereCollider;

    //FOR VR, reflects if on high, middle, or low shelves, used for calling correct grabbing animations in VRShopperAnimationControllers
    public Height height;

    UnityEvent onIpadConsumed;
    public enum Height
    { 
        None,
        High,
        Mid,
        Low
    }
    public ShelfSide sideOfIsle;
    public enum ShelfSide
    {
        None,
        Left,
        Right
    }


    private void Start() {
        if (onIpadConsumed == null)
            onIpadConsumed = new UnityEvent();

        _collidingAgents = new ArrayList();
        _meshRenderer = GetComponentInParent<MeshRenderer>();
        _interactable = GetComponentInParent<XRGrabInteractable>();
        _rigidbody = GetComponentInParent<Rigidbody>();
        _boxCollider = GetComponentInParent<BoxCollider>();
        if(sideOfIsle != ShelfSide.None)
        {
            _isleComponent = GetComponentInParent<IsleComponent>();
        }
        parentSphereCollider = GetComponentInParent<SphereCollider>();
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
       /* if (Achieved)
            GetComponentInParent<Collider>().enabled = false;
        else
            GetComponentInParent<Collider>().enabled = true;
       */
        /*
        if(ClosestAgent != null)
        {
            if(Vector3.Distance(ClosestAgent.transform.position, transform.position) < 1)
            {
                _desiringShopperIsClose = true;
            }
        }
        */
    }

    private void SetClosestShopper()
    {
        ShopperBehavior closest = null;
        float distance = 0;
        float tempDist;
        foreach(ShopperBehavior sp in ShoppersDesiringThisObj)
        {
            if (distance == 0)
            {
                closest = sp;
                _desiringShopperIsClose = true;
                distance = Vector3.Distance(transform.position, sp.transform.position);
                return;
            }
            else
            {
                tempDist = Vector3.Distance(transform.position, sp.transform.position);
                if(tempDist < 1.5f)
                {
                    closest = sp;
                    _desiringShopperIsClose = true;
                    //distance = tempDist;
                    return;
                }
            }
               
        }
        ClosestAgent = closest;
        RefocusOtherShoppers();
    }

    private void RefocusOtherShoppers()
    {
       foreach(ShopperBehavior shopper in ShoppersDesiringThisObj )
        {
            if(shopper != ClosestAgent)
            {
                shopper.getNewDesiredObj(transform);
                removeShopperFromDesiredObjList(shopper);
            }
        }
    }


   
    void OnTriggerEnter(Collider collider) {
        if (!isDesired)
        {
            if (collider.gameObject.CompareTag("IpadConsumer"))
            {
                ShopperBehavior sb = collider.GetComponentInParent<ShopperBehavior>();
                //check for proper state
                if (sb.State != (int)ShopperBehavior.ShoppingState.Exiting &&
                    sb.State != (int)ShopperBehavior.ShoppingState.GoingToLine &&
                    sb.isPickingUpObj == false && 
                    sb.currentIsleIndex == _isleComponent.IsleIndex)
                {
                    if(!isDesired)
                    {
                        targetAgent = sb;
                        targetShoppersAvoidancePriority = sb.assignedAvoidancePriority;
                    }
                    else if(sb.assignedAvoidancePriority < targetShoppersAvoidancePriority)
                    {
                        targetAgent.resetDesiredObj();
                        removeShopperListener(targetAgent);
                        targetAgent = sb;
                        targetShoppersAvoidancePriority = sb.assignedAvoidancePriority;
                    }
                    isDesired = true;
                    addShopperListener(targetAgent);
                    targetAgent.PickUpIpad(transform.parent.transform);
                    //achieving agent here?
                }
            }
        }
        
       /* if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("RealPlayer")) {
          
            if (!_collidingAgents.Contains(collider)) {
                _collidingAgents.Add(collider.gameObject);
             //   if(collider.gameObject.GetComponent<ShopperBehavior>().DeactiveObjs.Contains(this.gameObject) == false) //if agent sees this object, add it to the seen list
             //       collider.gameObject.GetComponent<ShopperBehavior>().DeactiveObjs.Add(this.gameObject);

                
                Vector3 v1 = collider.gameObject.transform.position;
                Vector3 v2 = this.gameObject.transform.position;
                v1.y = v2.y = 0f;
                float dist = Vector3.Distance(v1, v2);
                if (dist < minDist) {
                    minDist = dist;
                    ClosestAgent = collider.gameObject;

                }
            }
        }*/

    }

    

    void OnTriggerExit(Collider collider) {
        /*if (collider.gameObject.CompareTag("Player")|| collider.gameObject.CompareTag("RealPlayer")) {
            _collidingAgents.Remove(collider.gameObject);
            UpdateClosestAgent();
        }*/
        if(collider.GetComponentInParent<ShopperBehavior>() == targetAgent)
        {
            isDesired = false;
            targetAgent = null;
        }
    }

    void UpdateClosestAgent() {

        minDist = 1000000f;
        foreach (GameObject a in _collidingAgents) {
            Vector3 v1 = a.transform.position;
            Vector3 v2 = this.gameObject.transform.position;
            v1.y = v2.y = 0f;
            float dist = Vector3.Distance(v1, v2);
            if (dist < minDist) {
                minDist = dist;
                //ClosestAgent = a;
            }
        }
    }

     public void ObjPickupSuccess()
     {
        AchievingAgent.GetComponent<ShopperBehavior>().resetTargets();
        isleDataSO.RemoveIpad(this, isleIndex);
        AchievingAgent.GetComponent<ShopperBehavior>().State = (int)ShoppingState.GoingToObject;
        onIpadConsumed.Invoke();
        removeIpad();
        //AchievingAgent.GetComponent<ShopperBehavior>().isPickingUpObj = false;
    }

    private void removeIpad()
    {
        _meshRenderer.enabled = false;
        _interactable.enabled = false;
        _boxCollider.enabled = false;
        _sphereCollider.enabled = false;
        parentSphereCollider.enabled = false;
    }

    public void addShopperListener(ShopperBehavior shopper)
    {
        onIpadConsumed.AddListener(delegate { shopper.updateShopperTargets(this);});
    }

    private void removeShopperListener(ShopperBehavior shopper)
    {
        onIpadConsumed.RemoveListener(delegate { shopper.updateShopperTargets(this);});
    }

    public void HumanObjPickupSuccess()
    {
        if (_isleComponent != null)
        {
            _isleComponent.UpdateIsleCount(sideOfIsle);
        }

        Destroy(transform.parent.gameObject);
    }

    public void addShopperToDesiredObjList(ShopperBehavior shopperBehavior)
    {
        if (!ShoppersDesiringThisObj.Contains(shopperBehavior))
        {
            ShoppersDesiringThisObj.Add(shopperBehavior);
            if (ShoppersDesiringThisObj.Count == 1) ClosestAgent = shopperBehavior;
            if (ShoppersDesiringThisObj.Count > 1) { 
                if(!_desiringShopperIsClose)
                    SetClosestShopper(); 
            }
        }
    }
    public void removeShopperFromDesiredObjList(ShopperBehavior shopperBehavior)
    {
        if (!ShoppersDesiringThisObj.Contains(shopperBehavior))
        {
            if(shopperBehavior == ClosestAgent)
            {
                ShoppersDesiringThisObj.Remove(shopperBehavior);
                _desiringShopperIsClose = false;
                if(ShoppersDesiringThisObj.Count > 0)
                {
                    SetClosestShopper();
                }
            }
            
        }
    }

    public void reset()
    {
        targetAgent = null;
        isDesired = false;
        Achieved = false;
        AchievingAgent = null;
    }
}
