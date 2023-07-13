using UnityEngine;

using Debug = System.Diagnostics.Debug;
using UnityEngine.SceneManagement;

public class GUIHandler : MonoBehaviour {
	
	static public Vector3 PickedPoint;
    RaycastHit _hit;
    

    public bool ContagionMode;

 
	void OnGUI () {

       
        GUIStyle style = new GUIStyle();
        GUILayout.BeginArea(new Rect(5, 10,150, 250));
        style.fontSize = 18;
        style.normal.textColor = Color.black;

	   //GUILayout.Label("Time: "+ Time.time + " s.");
        if (GUILayout.Button("Reset")) {
            GameObject.Find("Crowd").GetComponent<CrowdManager>().Restart();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
        }

	    //if (GUILayout.Button("Agent")) {
     //       AgentComponent[] agentComponents = FindObjectsOfType(typeof(AgentComponent)) as AgentComponent[];
     //       Debug.Assert(agentComponents != null, "agentComponents != null");
     //       foreach (AgentComponent a in agentComponents) {
     //           if(a.enabled)
     //               a.IndicatorAgent.SetActive(!a.IndicatorAgent.activeInHierarchy);
     //       }
        //}
        //if (GUILayout.Button("Dominant emotion")) {
        //    AgentComponent[] agentComponents = FindObjectsOfType(typeof(AgentComponent)) as AgentComponent[];
        //    Debug.Assert(agentComponents != null, "agentComponents != null");
        //    foreach (AgentComponent a in agentComponents) {
        //        if (a.enabled)
        //            a.IndicatorCircle.SetActive(!a.IndicatorCircle.activeInHierarchy);

        //    }
        //}
        //if (GUILayout.Button("All emotions")) {
        //    AgentComponent[] agentComponents = FindObjectsOfType(typeof (AgentComponent)) as AgentComponent[];
        //    Debug.Assert(agentComponents != null, "agentComponents != null");
        //    foreach (AgentComponent a in agentComponents)
        //        if (a.enabled)
        //            a.IndicatorParticle.SetActive(!a.IndicatorParticle.activeInHierarchy);
        //}
	 /*   if (GUILayout.Button("Visibility")) {
            AgentComponent[] agentComponents = FindObjectsOfType(typeof(AgentComponent)) as AgentComponent[];
            Debug.Assert(agentComponents != null, "agentComponents != null");
            foreach (AgentComponent a in agentComponents) {
                if (a.VisibilityMesh == null)
                    a.VisibilityMesh = new MeshCreator("Visibility");
                else {
                    a.VisibilityMesh.ClearMeshes();
                    a.VisibilityMesh = null;
                }
            }            
       }*/
        /*
        if (GUILayout.Button("Save")) { //save to a video
            string videoFileName;
            int videoCnt = 0;
            do {
                videoCnt++;
                videoFileName = "VIDEOS\\video" + videoCnt + ".avi";

            } while (System.IO.File.Exists(videoFileName));
            EZReplayManager.get.saveToFile(videoFileName);
        }
        if (GUILayout.Button("Load")) {//open the last video
            string videoFileName;
            int videoCnt = 0;
            do {
                videoCnt++;
                videoFileName = "VIDEOS\\video" + videoCnt + ".avi";

            } while (System.IO.File.Exists(videoFileName));
            EZReplayManager.get.loadFromFile(videoFileName); 
        }
        */

        bool changed = GUI.changed;
	    ContagionMode = GUILayout.Toggle(ContagionMode, "Contagion");
        
        if(GUI.changed && changed == false) {
              AffectComponent[] affectComponents = FindObjectsOfType(typeof(AffectComponent)) as AffectComponent[];
            Debug.Assert(affectComponents != null, "affectComponents != null");
            foreach (AffectComponent a in affectComponents)
                a.ContagionMode = ContagionMode;            
        }



        
        GUILayout.EndArea();
      
    }
        
	
    
	void Update() {


        if (Input.GetMouseButtonDown(0)) {
       
            if (Input.GetKey("e")) {
                if (Camera.main != null) {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Physics.Raycast(ray, out _hit, Mathf.Infinity);
                }

                PickedPoint = _hit.point; //gives the gameobject's position     
                Instantiate(Resources.Load("Explosion"), PickedPoint, new Quaternion());
            }


        }

	    

        
        if(Input.GetKey("r")) {

            if(! GameObject.Find("Arrow").GetComponent<ArrowBehavior>().RecordSignal)
                GameObject.Find("Arrow").GetComponent<ArrowBehavior>().RecordSignal = true;
        }

        if (Input.GetKey("1")) {
            Time.timeScale = 1;
            
        }
        else if (Input.GetKey("2")) {
            Time.timeScale = 2;
         
        }
        else if (Input.GetKey("3")) {
            Time.timeScale = 3;
         
        }
           
        else if (Input.GetKey("4")) {
            Time.timeScale = 4;
        }
        else if (Input.GetKey("5")) {
            Time.timeScale = 0.5f;
        }
        else if (Input.GetKey("6")) {
            Time.timeScale = 0.25f;
            
        }
        else if (Input.GetKey("7")) {
            Time.timeScale = 0.1f;
        }

        
	}
    void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(PickedPoint, PickedPoint+new Vector3(0,5,0));
    }
  
    
}

