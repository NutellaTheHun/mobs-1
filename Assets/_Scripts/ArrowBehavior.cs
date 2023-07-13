using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;

public class ArrowBehavior : MonoBehaviour {
    struct SEmotions {
        public float E1, E2, E3, E4;
        public float Time;

    
    }
    public GameObject FollowedAgent;
    public bool RecordSignal = false;
    private Vector3 _initScale;
    private List<SEmotions> _emotionList;
    private StreamWriter _sw;
	
    void Start() {
        _emotionList = new List<SEmotions>();

    }

    //Follow the agent and record the emotions at specific times
	// Update is called once per frame
	void FixedUpdate () {

	   
	    SEmotions em = new SEmotions();
	    em.E1 = FollowedAgent.GetComponent<AffectComponent>().Ekman[0];
        em.E2 = FollowedAgent.GetComponent<AffectComponent>().Ekman[1];
        em.E3 = FollowedAgent.GetComponent<AffectComponent>().Ekman[2];
        em.E4 = FollowedAgent.GetComponent<AffectComponent>().Ekman[3];
	    em.Time = Time.time;
        _emotionList.Add(em);
	    // this.transform.localScale = new Vector3(_initScale.x*2f, _initScale.y, _initScale.z*2f);


	}

        

    void LateUpdate() {
        this.transform.position = FollowedAgent.transform.position + Vector3.up * 2.2f;
        this.transform.rotation = FollowedAgent.transform.rotation;

        if (RecordSignal) {
            _sw = new StreamWriter("EmotionRecordings\\Emotion " + Time.time + " " + FollowedAgent.GetComponent<AgentComponent>().Id + ".txt");
            _sw.WriteLine("Time\tHappy\tSad\tAngry\tAfraid"); 
            /*foreach (SEmotions em in _emotionList) {
                _sw.WriteLine(em.Time  + "\t" + em.E1 + "\t" + em.E2 + "\t" + em.E3 + "\t" + em.E4 ); 
            }*/
            _sw.WriteLine(FollowedAgent.GetComponent<AffectComponent>().Ekman[0] + "\t" +
                          FollowedAgent.GetComponent<AffectComponent>().Ekman[1] + "\t" +
                          FollowedAgent.GetComponent<AffectComponent>().Ekman[2] + "\t" +
                          FollowedAgent.GetComponent<AffectComponent>().Ekman[3]); 
            _sw.Close();
            RecordSignal = false;
        }

    }
}
