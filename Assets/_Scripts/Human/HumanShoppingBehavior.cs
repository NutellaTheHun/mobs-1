using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;


public class UserStats {
    public float TimeSpent = 0f;
    public int FightCnt = 0;
    public int PunchCnt = 0;
    public float AvgSpeed = 0f;
    public float TotalDistance = 0f; //Total distance traversed in the store
    public int CollectedItemCnt = 0; //Items collected from the shelves
    public int TotalItemCnt = 0; //Total item cnt

    public UserStats() {
        TimeSpent = Time.time;
    }

}

public class HumanShoppingBehavior : MonoBehaviour {
    public GameObject CurrentObjs;
    public Transform _rightHand, _leftHand;

    public UserStats Stats;

   
    public Text IpadCntText;


    Animator _animator;

    HumanComponent _humanComponent;
    List<Collider> _ipadColliders;

    [SerializeField]
    string _currSceneName;

    string _missionMsg = "";

    Transform _desiredObj;
    int _missionItemCnt = 10; //number of iPads to collect
                              // Start is called before the first frame update

    bool _hasPaid, _hasCompletedShopping;


    Vector3 _prevPosition;
    
    int _fixedUpdateCnt = 0;


    //Author Nathan Brilmayer, for VR to check if proper item is grabbed
    bool desiredObjInHand = false;
    bool isPaying = false;
    PaymentSystem _paymentSystem;

    [DllImport("__Internal")]
    private static extern void SendUserStatsToPage(float timeSpent, int fightCnt, int punchCnt, float avgSpeed, float totalDist, int collectedItemCnt, int totalItemCnt, int crowdPersonality);
    [DllImport("__Internal")]
    private static extern void SendMissionMessageToPage(string message);


    void SummarizeStats() {
        Stats.TimeSpent = Time.time - Stats.TimeSpent; //Timespent is initiated to the beginning time
        Stats.TotalItemCnt = CurrentObjs.transform.childCount;

        Stats.AvgSpeed /= _fixedUpdateCnt;
        Stats.FightCnt = GetComponent<HumanComponent>().FightCnt;

        //Debug.Log(Stats.TimeSpent);
        //Debug.Log(Stats.CollectedItemCnt);
        //Debug.Log(Stats.AvgSpeed);
        //Debug.Log(Stats.FightCnt);

#if !UNITY_EDITOR && UNITY_WEBGL
        SendUserStatsToPage(Stats.TimeSpent, Stats.FightCnt, Stats.PunchCnt, Stats.AvgSpeed,  Stats.TotalDistance, Stats.CollectedItemCnt, Stats.TotalItemCnt, UserInfo.PersonalityDistribution );
#endif

    }
    void Start() {
        Stats = new UserStats();
        _prevPosition = transform.position;

        CurrentObjs = new GameObject("AchievedObjects");
        CurrentObjs.transform.parent = this.transform;
        //_animator = transform.Find("FpsArm2").GetComponent<Animator>();
        _animator = GetComponent<Animator>();


        _currSceneName = SceneManager.GetActiveScene().name;

        _hasCompletedShopping = false;
        _hasPaid = false;

    
        if(_currSceneName == "Warmup") {
            _missionMsg = "Walk around the store and grab " + _missionItemCnt + " iPads.\n Pay at the counter and exit the store when finished.";
#if !UNITY_EDITOR && UNITY_WEBGL
            SendMissionMessageToPage(_missionMsg);
#endif
        }

        else { //Actual shopping scenario
            _missionMsg = "Walk around the store and grab as many iPads as you can. \n Pay at the counter and exit the store when finished.";

#if !UNITY_EDITOR && UNITY_WEBGL
            SendMissionMessageToPage(_missionMsg);
#endif

            //UserInfo.PersonalityDistribution = 1;
            if(UserInfo.PersonalityDistribution == 1) {  //Hostile

                float[] persMean = { 0f, -1f, 1f, -1f, 1f };
                float[] persStd = { 0f, 0f, 0f, 0f, 0f };
                UpdatePersonalityAndBehavior(persMean, persStd);
            }
        }

        _ipadColliders = new List<Collider>();
        _humanComponent = GetComponent<HumanComponent>();

        _rightHand = transform.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
        _leftHand = transform.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.LeftHand);
        _paymentSystem = GameObject.Find("PaymentCollider").GetComponentInChildren<PaymentSystem>();
    }

    private void FixedUpdate() {
        Stats.TotalDistance += (transform.position - _prevPosition).magnitude;
        Stats.AvgSpeed += (transform.position - _prevPosition).magnitude / Time.deltaTime;
        _prevPosition = transform.position;
        _fixedUpdateCnt++;

    }

    public void UpdatePersonalityAndBehavior(float[] persMean, float[] persStd) {
        AgentComponent[] agentComponents = GameObject.Find("CrowdGroup1").GetComponentsInChildren<AgentComponent>();

        for(int i = 0; i < agentComponents.Length; i++) {

            if(agentComponents[i] != null) {
                agentComponents[i].GetComponent<AffectComponent>().UpdatePersonality(persMean, persStd);
                //Update steering behaviors according to Personality parameters
                agentComponents[i].GetComponent<PersonalityMapper>().PersonalityToSteering();
            }
        }
    }
    IEnumerator DisplayEndOfWarmupScene() {
        {
    
            _missionMsg = "Warmup complete. \nYou are now ready to join the crowd.";
#if !UNITY_EDITOR && UNITY_WEBGL
            SendMissionMessageToPage(_missionMsg);
#endif
            yield return new WaitForSeconds(3);

            SceneManager.LoadScene("Sales");
        }

    }
    IEnumerator DisplayEndOfShoppingScene() {
    
        _missionMsg = "Shopping complete. \nPlease answer the questions in the following scene. ";
        SummarizeStats();
#if !UNITY_EDITOR && UNITY_WEBGL
        SendMissionMessageToPage(_missionMsg);
#endif
        yield return new WaitForSeconds(2);

        SceneManager.LoadScene("UserPostStudySurvey");
        
    }


    private void OnTriggerEnter(Collider collider) {
        //Assign the closest ipad
        if(collider.CompareTag("Ipad")) {
            _ipadColliders.Add(collider);
        }
   
        else if(collider.gameObject.CompareTag("ExitZone")) {
            if(_hasPaid) 
                _hasCompletedShopping = true;            
        }


    }

    private void OnTriggerStay(Collider collider) {

        if(collider.CompareTag("PaymentZone")) {
            if(isPaying) //FOR VR, Is activated by PaymentSystem component
            {//Input.GetKey(KeyCode.P)

                if (_currSceneName == "Warmup" && CurrentObjs.transform.childCount >= _missionItemCnt || _currSceneName == "Sales" || _currSceneName == "SuperStore") {
                    _hasPaid = true;
                    _missionMsg = "Payment complete. \nYou may exit the store.";

                    collider.transform.Find("Dialog").gameObject.SetActive(true);
                }
                else {
                    _missionMsg = "Please collect all the " + _missionItemCnt + " iPads first.";


                }
            }
            
#if !UNITY_EDITOR && UNITY_WEBGL
            SendMissionMessageToPage(_missionMsg);
#endif

        }

    }

private void OnTriggerExit(Collider collider) {

        if(collider.CompareTag("PaymentZone"))
            collider.transform.Find("Dialog").gameObject.SetActive(false);

        //Assign the closest ipad
        if(_ipadColliders.Contains(collider)) {
            _ipadColliders.Remove(collider);
        }
        
        
    }



    // Update is called once per frame
    void Update() {

        //Boss key
        if(Input.GetKey(KeyCode.B) && Input.GetKey(KeyCode.Keypad6) && Input.GetKey(KeyCode.LeftControl)) {
            SceneManager.LoadScene("UserPostStudySurvey");
        }

        IpadCntText.text = CurrentObjs.transform.childCount.ToString();

        if(_hasCompletedShopping){
            if(_currSceneName == "Sales") 
                StartCoroutine(DisplayEndOfShoppingScene());
            else
                StartCoroutine(DisplayEndOfWarmupScene());
        }

        if(_humanComponent.IsFighting())
            CurrentObjs.SetActive(false); //don't show items when fighting
        else
            CurrentObjs.SetActive(true); //don't show items when fighting
    }

    public int getCollectedCount()
    {
        return Stats.CollectedItemCnt;
    }

    //Arrange positions of object
    public void SortObjects() {
        for(int i = 0; i < CurrentObjs.transform.childCount; i++)
            CurrentObjs.transform.GetChild(i).transform.position = CurrentObjs.transform.position - 0.05f * Vector3.up + i * 0.05f * Vector3.up;

    }


    //Give all your objects to your opponent who is always an agent
    public void YieldObjects(GameObject opponent) {

        List<Transform> children = new List<Transform>();
        int childCnt = CurrentObjs.transform.childCount;
        for(int i = 0; i < childCnt; i++) 
            children.Add(CurrentObjs.transform.GetChild(i));

        foreach(Transform c in children) {
            c.parent = opponent.GetComponent<ShopperBehavior>().CurrentObjs.transform;        
        }

        opponent.GetComponent<ShopperBehavior>().SortObjects();
        CurrentObjs.SetActive(true);

        Debug.Log("yielded objects");
    }

    private void OnGUI() {
        GUIStyle style = new GUIStyle();

        if(!_humanComponent.IsFighting()) {
            GUILayout.BeginArea(new Rect(x: 200, y: 10, width: 600, height: 300));

            //style.normal.textColor = Color.Lerp(Color.red, Color.blue, 0.3f);
            style.normal.textColor = Color.black;
            style.fontSize = 20;

            GUILayout.Label(_missionMsg, style);
            GUILayout.EndArea();
        }


    }


    //Not working properly for human
    void OnAnimatorIK() {
        if(_animator.GetCurrentAnimatorStateInfo(2).IsName("pickingUp")) {
            if(_desiredObj) {
                float reach = _animator.GetFloat("RightHandReach");  //param is a curve set in pickingUp animation


                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, reach);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, reach);
                _animator.SetIKPosition(AvatarIKGoal.RightHand, _desiredObj.position);
                _animator.SetIKRotation(AvatarIKGoal.RightHand, _desiredObj.rotation);
            }

        }

    }

    //Called as an animation event when the picking up animation ends
    void PickedObject() {
        _humanComponent.FinishedWaiting = true;

    }

    private void LateUpdate() {
        CurrentObjs.transform.position = _leftHand.position;
    }

    //Author Nathan Brilmayer, Methods for VR stuff
    public void DesiredObjectPickedUp(GameObject desiredObj)
    {
        ObjComponent oc = desiredObj.GetComponent<ObjComponent>();
        oc.AchievingAgent = this.gameObject;
        oc.Achieved = true;

        Stats.CollectedItemCnt++;
        if (Stats.CollectedItemCnt == 1) 
        {
            _paymentSystem.enablePaymentSystem(); 
        }
        StartCoroutine(ObjPickupSuccessDelayed(desiredObj, 0.5f));

        _ipadColliders.Remove(desiredObj.GetComponent<Collider>());

        //desiredObjInHand = true;
    }
    public void DesiredObjectDropped()
    {
        //desiredObjInHand = false;
    }

    IEnumerator ObjPickupSuccessDelayed(GameObject desiredObj, float time)
    {
        yield return new WaitForSeconds(time);
        desiredObj.GetComponent<ObjComponent>().ObjPickupSuccess();
    }

    public void setIsPaying(bool val)
    {
        isPaying = val;
    }

    //**************************************
}
