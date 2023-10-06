using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class TutorialObjComponent : MonoBehaviour {
    [SerializeField]
    private bool _achieved = false;

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

    public ShopperBehavior ClosestAgent;
    public GameObject AchievingAgent;
    public List<ShopperBehavior> ShoppersDesiringThisObj;

    public ArrayList _collidingAgents;
    private float minDist = 10000f;

    //FOR VR, reflects if on high, middle, or low shelves, used for calling correct grabbing animations in VRShopperAnimationControllers
    public Height height;
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
        _collidingAgents = new ArrayList();
        _meshRenderer = GetComponent<MeshRenderer>();
        _interactable = GetComponent<XRGrabInteractable>();
        _rigidbody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        if(sideOfIsle != ShelfSide.None)
        {
            _isleComponent = GetComponentInParent<IsleComponent>();
        }
    }

    private void Update()
    {
        if (Achieved)
            GetComponent<Collider>().enabled = false;
        else
            GetComponent<Collider>().enabled = true;

        if(ShoppersDesiringThisObj.Count > 1) { SetClosestShopper(); }
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
                distance = Vector3.Distance(transform.position, sp.transform.position);
            }
            else
            {
                tempDist = Vector3.Distance(transform.position, sp.transform.position);
                if(tempDist < Vector3.Distance(transform.position, closest.transform.position))
                {
                    closest = sp;
                    distance = tempDist;
                }
            }
               
        }
        ClosestAgent = closest;
    }


    /// Finds the agents around me
    void OnTriggerEnter(Collider collider) {
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
        //_isleComponent.UpdateIsleCount(sideOfIsle);
        AchievingAgent.GetComponent<ShopperBehavior>().ResetDesiredObj();
        Destroy(this.gameObject);
    }

    public void addShopperToDesiredObjList(ShopperBehavior shopperBehavior)
    {
        if (!ShoppersDesiringThisObj.Contains(shopperBehavior))
        {
            ShoppersDesiringThisObj.Add(shopperBehavior);
            if (ShoppersDesiringThisObj.Count == 1) ClosestAgent = shopperBehavior;
        }
    }
}
