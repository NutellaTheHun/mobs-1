using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjComponent : MonoBehaviour {
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
    public bool Achieved {
        get { return _achieved; }
        set { _achieved = value; }
    }

    public int Density {
        get { return _collidingAgents.Count; }
    }

    public GameObject ClosestAgent;
    public GameObject AchievingAgent;


    public ArrayList _collidingAgents;
    private float minDist = 10000f;


    private void Start() {
        _collidingAgents = new ArrayList();
        _meshRenderer = GetComponent<MeshRenderer>();
        _interactable = GetComponent<XRGrabInteractable>();
        _rigidbody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (Achieved)
            GetComponent<Collider>().enabled = false;
        else
            GetComponent<Collider>().enabled = true;
    }


/// Finds the agents around me
    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("RealPlayer")) {
          
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
        }

    }


    void OnTriggerExit(Collider collider) {
        if (collider.gameObject.CompareTag("Player")|| collider.gameObject.CompareTag("RealPlayer")) {
            _collidingAgents.Remove(collider.gameObject);
            UpdateClosestAgent();
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
                ClosestAgent = a;
            }
        }
    }

     public void ObjPickupSuccess()
    {
        _meshRenderer.enabled = false;
        _interactable.enabled = false;
        _rigidbody.isKinematic = true;
        _rigidbody.detectCollisions = false;
        _boxCollider.enabled = false;
    }

}
