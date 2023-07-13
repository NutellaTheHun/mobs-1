using UnityEditor;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(Appraisal))]

public class AppraisalEditor : Editor {
	
	Appraisal appraisal;
	//Goals
	bool pleased = false;
	bool prospectRel = false;
	bool conseqForSelf = false;
	bool desirable = false;	
	bool checkAllGoals = false;
	//Standards
	bool approving = false;
	bool focOnSelf = false;	
	bool checkAllStandards = false;
	//Attitudes
	bool liking = false;
	bool checkAllAttitudes = false;

    float weight = 0.5f; //default

	void OnEnable()
    {	
		appraisal = target as Appraisal;
				
		
	}
	 
	public override void OnInspectorGUI () {
		
		
		//Add new goals
		Goal gl = new Goal();
		
		GUILayout.Label("Goals", EditorStyles.boldLabel);
		
		
		EditorGUILayout.BeginHorizontal();	
		pleased = EditorGUILayout.Toggle("Pleased", pleased);
		gl.pleased = pleased;
		conseqForSelf = EditorGUILayout.Toggle("CsqForSelf", conseqForSelf,  GUILayout.ExpandWidth(false));
		gl.consequenceForSelf = conseqForSelf;
		if(conseqForSelf == false) {
            desirable = EditorGUILayout.Toggle("DsrblForOther", desirable, GUILayout.ExpandWidth(false));
			gl.desirableForOther = desirable;
			
		}
		else {
            prospectRel = EditorGUILayout.Toggle("PrspctRel", prospectRel, GUILayout.ExpandWidth(false));
			gl.prospectRelevant = prospectRel;			
			gl.confirmed = AppDef.Unconfirmed;
			
		}
				
		EditorGUILayout.EndHorizontal();

        weight = EditorGUILayout.Slider("weight", weight, 0f, 1f, GUILayout.ExpandWidth(false));
		gl.weight = weight;
		
		
		if(GUILayout.Button("Add Goal", GUILayout.ExpandWidth(false)))
			appraisal.Goals.Add(gl);
		
				
		if(appraisal.Goals.Count  > 0) {
            EditorGUILayout.Separator();
            GUI.contentColor = Color.yellow;                        
            GUILayout.Label("Current goals", EditorStyles.boldLabel);
            checkAllGoals = EditorGUILayout.Toggle("Check all", checkAllGoals);            
        }
			
		foreach(Goal g in appraisal.Goals) {
			EditorGUILayout.BeginHorizontal();			
		
			g.selected= EditorGUILayout.Toggle(g.selected, GUILayout.ExpandWidth(false));
			
			
			if(g.pleased)
				GUILayout.Label  ("Pleased");
			else
				GUILayout.Label  ("Displeased");
						
			if(g.consequenceForSelf) {

                GUILayout.Label("CsqForSelf");
				if(g.prospectRelevant) {
				
					GUILayout.Label ("PrspctRel");
					if(g.confirmed == AppDef.Confirmed)
						GUILayout.Label ("Confirmed");
					else if(g.confirmed == AppDef.Disconfirmed)
						GUILayout.Label ("Disconfirmed");
					else
						GUILayout.Label ("Unconfirmed");
			
				}
				else
					GUILayout.Label ("PrspctIrrel");
			}
			else {
			
				GUILayout.Label ("CsqForOther");
				if(g.desirableForOther)
					GUILayout.Label ("Desirable");
				else
					GUILayout.Label ("Unesirable");
			}
			
			
			GUILayout.Label ("weight: " + g.weight);
			
			EditorGUILayout.EndHorizontal ();
			
			
			
			
		}
		
		if(checkAllGoals)
			foreach(Goal g in appraisal.Goals)
				g.selected = true;
			
		
		
		if(appraisal.Goals.Count > 0) {
		
			EditorGUILayout.Separator();
			
			if(GUILayout.Button("RemoveChecked", GUILayout.ExpandWidth(false))) {		
				int i = 0;		
				while(i < appraisal.Goals.Count) {
					if(appraisal.Goals[i].selected){
						appraisal.Goals.Remove(appraisal.Goals[i]);					
					}
					else
						i++;
						
				}
			
			}
		}
       
        
		EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        GUI.contentColor = Color.white;

		GUILayout.Label("Standards", EditorStyles.boldLabel);
		
		Standard st = new Standard();
		
		EditorGUILayout.BeginHorizontal();	
		approving = EditorGUILayout.Toggle("Approving", approving);
		st.approving = approving;
        focOnSelf = EditorGUILayout.Toggle("Focusing on Self", focOnSelf, GUILayout.ExpandWidth(false));
		st.focusingOnSelf = focOnSelf;
				
		EditorGUILayout.EndHorizontal();

        weight = EditorGUILayout.Slider("weight", weight, 0f, 1f, GUILayout.ExpandWidth(false));
		st.weight = weight;
		
		
		if(GUILayout.Button("Add Standard", GUILayout.ExpandWidth(false)))
			appraisal.Standards.Add(st);
		
				
		if(appraisal.Standards.Count  > 0) {
			EditorGUILayout.Separator();
            GUI.contentColor = Color.yellow;
            GUILayout.Label("Current standards", EditorStyles.boldLabel);
            checkAllStandards = EditorGUILayout.Toggle("Check all", checkAllStandards);            
         }

		foreach(Standard s in appraisal.Standards) {
			
			EditorGUILayout.BeginHorizontal();
			
			s.selected= EditorGUILayout.Toggle(s.selected, GUILayout.ExpandWidth(false));
			
						
			if(s.approving)
				GUILayout.Label ("Approving");
			else
				GUILayout.Label ("Disapproving");
			
			if(s.focusingOnSelf)
				GUILayout.Label ("FocusingOnSelf");
			else
				GUILayout.Label ("FocusingOnOther");
						
			
			GUILayout.Label ("weight: " + s.weight);
			EditorGUILayout.EndHorizontal ();
		}
		
		if(checkAllStandards)
			foreach(Standard s in appraisal.Standards)
				s.selected = true;
			
		if(appraisal.Standards.Count > 0) {
		
			EditorGUILayout.Separator();
			
			if(GUILayout.Button("RemoveChecked", GUILayout.ExpandWidth(false))) {		
				int i = 0;		
				while(i < appraisal.Standards.Count) {
					if(appraisal.Standards[i].selected){
						appraisal.Standards.Remove(appraisal.Standards[i]);					
					}
					else
						i++;
						
				}
			
			}
		}

        EditorGUILayout.Separator();
		EditorGUILayout.Separator();
        GUI.contentColor = Color.white;
		
        GUILayout.Label("Attitudes", EditorStyles.boldLabel);
		
		Attitude at = new Attitude();
		
		EditorGUILayout.BeginHorizontal();	
		liking = EditorGUILayout.Toggle("Liking", liking);
		at.liking = liking;
				
		EditorGUILayout.EndHorizontal();

        weight = EditorGUILayout.Slider("weight", weight, 0f, 1f, GUILayout.ExpandWidth(false));
		at.weight = weight;
		
		
		if(GUILayout.Button("Add Attitude", GUILayout.ExpandWidth(false)))
			appraisal.Attitudes.Add(at);


        EditorGUILayout.Separator();
        if(appraisal.Attitudes.Count  > 0) {
			EditorGUILayout.Separator();
            GUI.contentColor = Color.yellow;
            GUILayout.Label("Current attitudes", EditorStyles.boldLabel);
            checkAllAttitudes = EditorGUILayout.Toggle("Check all", checkAllAttitudes);            
        }
       
		foreach(Attitude a in appraisal.Attitudes) {
			EditorGUILayout.BeginHorizontal();			
			a.selected= EditorGUILayout.Toggle(a.selected, GUILayout.ExpandWidth(false));
			
			
			if(a.liking)
				GUILayout.Label ("Liking");
			else
				GUILayout.Label ("Disliking");
						
			GUILayout.Label ("weight: " + a.weight);
			
			EditorGUILayout.EndHorizontal ();
		}
	
		if(checkAllAttitudes)
			foreach(Attitude a in appraisal.Attitudes)
				a.selected = true;

        GUI.contentColor = Color.white;
		
		if(appraisal.Attitudes.Count > 0) {
		
			EditorGUILayout.Separator();
			
			if(GUILayout.Button("RemoveChecked", GUILayout.ExpandWidth(false))) {		
				int i = 0;		
				while(i < appraisal.Attitudes.Count) {
					if(appraisal.Attitudes[i].selected){
						appraisal.Attitudes.Remove(appraisal.Attitudes[i]);					
					}
					else
						i++;
						
				}
			
			}
		}
		
		 EditorUtility.SetDirty (target);
	}
}
