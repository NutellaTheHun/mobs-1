using UnityEngine;
using System.Collections;



public class GroupBuilder : MonoBehaviour {

 
    private GameObject[] _agents;
  //  string[] _charNames = {

		////"Adam","Alex", "Alexa","Andromeda", "Brian", "Bryce","Carl","David",  "Elizabeth", "Frank", "James", "Joan", "Jody", "Joe",  "Josh", "Kate",  "Leonard", "Lewis", "Liam","Louise",  "Malcolm","Martha","Megan","Mia", "Michael",   "Regina", "Remy", "Roth", "Shae", "Shannon","Sophie",  "Stefani",  "Suzie", "Victoria",
                 
  //      "Andromeda", "Alexa",  "Liam",  "Malcolm",  "McPerson1", "McPerson2",   "Regina", "Remy",  "Shae",   "Stefani", "AgentN1", "Pass1N", "Pass2N", "Pass3N", "Pass4N", "Pass5N", "Pass6N", "Pass7N", "Pass8N", "Pass9N"
  //          				//the ones with blendshapes 
  //  };

    private int _agentCnt = 0;


    enum RoleName {
        Audience,
        Shopper,
        Protester,
        Police

    }

    public void SetAgentCnt() {
        _agentCnt = Globals.CharNames.Length; //number of agents in the group
    }
   


    public void AssignAgents() {
        AgentComponent[] agentComponents = GetComponentsInChildren<AgentComponent>();
        _agentCnt = agentComponents.Length;
        _agents = new GameObject[_agentCnt];

        for(int i = 0; i < _agentCnt; i++) {
            _agents[i] = agentComponents[i].gameObject;
        }
    }

    public void Init(int role, int cnt, int groupId, int totalAgentCnt) {
        int i;
        this._agentCnt = cnt;
        _agents = new GameObject[_agentCnt];


        for(i = 0; i < _agentCnt; i++) {


            _agents[i] = (GameObject)Instantiate(UnityEngine.Resources.Load(Globals.CharNames[i % Globals.CharNames.Length]), transform.position, transform.rotation);

            _agents[i].AddComponent<AgentComponent>();
            _agents[i].AddComponent<AnimationSelector>();
            _agents[i].AddComponent<Appraisal>();
            _agents[i].AddComponent<AffectComponent>();
            _agents[i].AddComponent<PersonalityMapper>();            
            _agents[i].AddComponent<FaceAnimator>();


            //_agents[i].AddComponent<Object2Record>();
            //_agents[i].GetComponent<Object2Record>().enabled = false;


            _agents[i].GetComponent<AgentComponent>().GroupId = groupId;
            _agents[i].GetComponent<AgentComponent>().Id = totalAgentCnt + i;



            _agents[i].tag = "Player";



            _agents[i].transform.parent = GameObject.Find("CrowdGroup" + groupId).transform;


            switch(role) {


                case (int)RoleName.Shopper:
                    _agents[i].AddComponent<ShopperBehavior>();
                    break;


                case (int)RoleName.Audience:
                    _agents[i].AddComponent<AudienceBehavior>();
                    break;
            }

        }

       

    }

    public void UpdatePersonalityAndBehavior(float[] persMean, float[] persStd) {

        for(int i = 0; i < _agents.Length; i++) {

            if(_agents[i] != null) {
                _agents[i].GetComponent<AffectComponent>().UpdatePersonality(persMean, persStd);
                //Update steering behaviors according to Personality parameters
                _agents[i].GetComponent<PersonalityMapper>().PersonalityToSteering();
            }
        }
    }

    public void UpdateRegion(float rectX, float rectZ) {
        for(int i = 0; i < _agentCnt; i++) {
            //agents[i].transform.Translate(transform.position - agents[i].transform.position);
            _agents[i].transform.position = transform.position + new Vector3(MathDefs.GetRandomNumber(-rectX, rectX), 0, MathDefs.GetRandomNumber(-rectZ, rectZ));
        }
    }



}
