using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

public class CrowdEditor : EditorWindow {
		
	static int agentCnt = 0;	//# of agents to be added
	static int totalAgentCnt = 0;
	int agentRole = 0; //audience
	string[] roleNames = {"Audience", "Shopper", "Protester", "Police", "Passenger"};
	int[] roleInds = {0,1,2,3,4,5,6,7,8, 9};

    static float _sliderRectX;
    static float _sliderRectZ;

    [MenuItem ("MOBS/Crowd")]
	 static void Init () {
        // Get existing open window or if none, make a new one:
        CrowdEditor window = (CrowdEditor)EditorWindow.GetWindow(typeof (CrowdEditor));

        agentCnt = Globals.CharNames.Length; //default value

    }

    void OnGUI() {

        
        GUILayout.Label("Create New Group", EditorStyles.largeLabel);

        agentRole = EditorGUILayout.IntPopup("Role: ", agentRole, roleNames, roleInds);
        agentRole = 1; //default role is shopper

        
        agentCnt = EditorGUILayout.IntField("Agent Count", agentCnt, GUILayout.ExpandWidth(true));

        //agentCnt = 30; //default value


        GUILayout.Label("Group region");
        EditorGUILayout.BeginHorizontal();
        _sliderRectX = EditorGUILayout.Slider(" X", _sliderRectX, 0f, 80f);
        _sliderRectZ = EditorGUILayout.Slider("Z", _sliderRectZ, 0f, 80f);
        EditorGUILayout.EndHorizontal();

        //Default region is a 2x2 area
        _sliderRectX = 2;
        _sliderRectZ = 2;

        //if (GUILayout.Button("Update region", GUILayout.ExpandWidth(false)))
        //    _groupBuilder.UpdateRegion(_sliderRectX, _sliderRectZ);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Add Group", GUILayout.ExpandWidth(true))) {

            int groupId = ComputeNewGroupId();
            GameObject go = new GameObject("CrowdGroup" + (groupId));
            GameObject crowd = GameObject.Find("Crowd");
            if (crowd == null) {
                crowd = new GameObject("Crowd");
                crowd.AddComponent<CrowdManager>();
            }

            go.transform.parent = GameObject.Find("Crowd").transform;
            //	if(selected == true) //Add to the selected location on screen
            //		go.transform.position = selectedPoint;
            //	else //add to the parent gameobject's location
            go.transform.position = go.transform.parent.transform.position;
            go.AddComponent<GroupBuilder>();

            

            go.GetComponent<GroupBuilder>().Init(agentRole, agentCnt, groupId, totalAgentCnt);

            totalAgentCnt += agentCnt;

            go.GetComponent<GroupBuilder>().UpdateRegion(_sliderRectX, _sliderRectZ);

        }
        GUILayout.FlexibleSpace();


//        if (GUILayout.Button("Update Animation Clips", GUILayout.ExpandWidth(false))) {

//            GameObject crowd = GameObject.Find("Crowd");
//            if (crowd == null) {
//                return;
//            }

//            List<AnimationClip> clips = new List<AnimationClip>();
//            List<string> animationNames = new List<string>();
//            animationNames.Add("run");
//            animationNames.Add("walk");
//            animationNames.Add("idle");
//            animationNames.Add("clap");
//            animationNames.Add("fight");
//            animationNames.Add("protest");
//            animationNames.Add("yelling");
//            animationNames.Add("pick");

//            foreach (string t in animationNames) {
//// Load the animation file.
//                string path = "Assets/Revision/Animation Clips/" + t +".anim" ;
//                //Animation anim = AssetDatabase.LoadAssetAtPath(path, typeof(Animation)) as Animation;
//                //Object[] anim = Resources.LoadAll(path);//, typeof(GameObject)) as GameObject;

//                AnimationClip anim = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;

//                if (anim == null) 
//                    EditorUtility.DisplayDialog("Error!", "Could not load animation clip.\n\nAttempted path was: " + path , "Ok");                
//                else
//                    clips.Add(anim);
//            }

           
//            foreach (Transform group in crowd.transform) {
//                foreach (Transform agent in group) {                    
//                    AnimationUtility.SetAnimationClips(agent.GetComponent<Animation>(), clips.ToArray());
           
//                }
//            }
           
//        }

        EditorGUILayout.BeginHorizontal();	
        if (GUILayout.Button("Enable Animation and Rendering", GUILayout.ExpandWidth(false))) {
            GameObject crowd = GameObject.Find("Crowd");
            if (crowd == null) {
                return;
            }

            foreach (Transform group in crowd.transform) {
                foreach (Transform agent in group) {
                    if (agent.GetComponentInChildren<AnimationSelector>())
                        agent.GetComponent<AnimationSelector>().enabled = true;
                    else
                        Debug.Log("AnimationSelector component not found!");

                    
                    if (agent.GetComponentInChildren<Animator>())
                        agent.GetComponent<Animator>().enabled = true;

                    foreach (Transform child in agent) {
                        foreach (SkinnedMeshRenderer sm in child.GetComponentsInChildren<SkinnedMeshRenderer>())
                            sm.enabled = true;

                        foreach (MeshRenderer mr in child.GetComponentsInChildren<MeshRenderer>())
                            mr.enabled = true;
                    }
                }
            }
        }
        if (GUILayout.Button("Disable Animation and Rendering", GUILayout.ExpandWidth(false))) {
            GameObject crowd = GameObject.Find("Crowd");
            if (crowd == null) {
                return;
            }

            foreach (Transform group in crowd.transform) {
                foreach (Transform agent in group) {
                    if (agent.GetComponentInChildren<AnimationSelector>())
                        agent.GetComponent<AnimationSelector>().enabled = false;
                    else
                        Debug.Log("AnimationSelector component not found!");
                    

                    if (agent.GetComponentInChildren<Animator>())
                        agent.GetComponent<Animator>().enabled = false;

                    foreach(Transform child in agent)
                        if(child.name.Equals("GUI Text"))
                            child.gameObject.SetActive(false);
                        

                   
                    foreach (Transform child in agent) {
                        foreach (SkinnedMeshRenderer sm in child.GetComponentsInChildren<SkinnedMeshRenderer>())
                            sm.enabled = false;

                        foreach (MeshRenderer mr in child.GetComponentsInChildren<MeshRenderer>())
                            mr.enabled = false;
                    }
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        //if (GUILayout.Button("Assign random personality to crowd", GUILayout.ExpandWidth(false))) {
        //    GameObject crowd = GameObject.Find("Crowd");
        //    float[] persMean = { 0f, 0f, 0f, 0f, 0f };
        //    float[] persStd = { 0.35f, 0.35f, 0.35f, 0.35f, 0.35f };

        //    foreach (Transform group in crowd.transform) {
        //        group.gameObject.GetComponent<GroupBuilder>().AssignAgents();
        //        group.gameObject.GetComponent<GroupBuilder>().UpdatePersonalityAndBehavior(persMean, persStd);
        //    }

        //}
        
        
        
        //As shortcuts to settings
        GUILayout.FlexibleSpace();
        string[] names = QualitySettings.names;
        GUILayout.BeginVertical();
        int i = 0;
        while (i < names.Length) {
            if (GUILayout.Button(names[i]))
                QualitySettings.SetQualityLevel(i, true);

            i++;
        }
        GUILayout.EndVertical();

        Time.timeScale = EditorGUILayout.FloatField("Time Scale", Time.timeScale, GUILayout.ExpandWidth(true));

        

  
      	
    }

    int ComputeNewGroupId() {
        int id;
        int maxId = 0;

        foreach (GameObject g in UnityEngine.Object.FindObjectsOfType(typeof(GameObject))) {
            if (g.name.Contains("CrowdGroup") && g.transform.parent.name.Equals("Crowd")) {                
                id = Int32.Parse(g.name.Substring(10));                
                if (id > maxId)
                    maxId = id;
            }
        }
        return maxId + 1;       
    }
	
	
}
