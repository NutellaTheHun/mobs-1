using UnityEditor;
using System.Collections;
using UnityEngine;


[CustomEditor(typeof(GroupBuilder))]
public class GroupBuilderEditor:Editor{
	
	GroupBuilder _groupBuilder;
	
	static float[] _sliderMean = {0f, 0f, 0f, 0f, 0f};
	static float[] _sliderStd = {0f, 0f, 0f, 0f, 0f};

    static float _sliderRectX;
    static float _sliderRectZ;
    
	float _minMean = -1.0f;
	float _maxMean = 1.0f;
	float _minStd = 0.0f;
	float _maxStd = 1.0f;


	
	void OnEnable() {
		 _groupBuilder = target as GroupBuilder;
		_groupBuilder.SetAgentCnt();
		_groupBuilder.AssignAgents();

	}
	 
	public override void OnInspectorGUI () {
		
		GUILayout.Label ("Personality Settings", EditorStyles.largeLabel);		
		
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("Mean", EditorStyles.boldLabel);
		GUILayout.Label("StdDev", EditorStyles.boldLabel);
		EditorGUILayout.EndHorizontal ();		
	
		EditorGUILayout.BeginHorizontal();
		_sliderMean[0] = EditorGUILayout.Slider ("Openness", _sliderMean[0], _minMean, _maxMean);
		_sliderStd[0] = EditorGUILayout.Slider ("Openness", _sliderStd[0], _minStd,_maxStd);
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal();
		_sliderMean[1] = EditorGUILayout.Slider("Concscientiousness", _sliderMean[1], _minMean, _maxMean);
		_sliderStd[1] = EditorGUILayout.Slider ("Concscientiousness", _sliderStd[1], _minStd,_maxStd);		
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal();
		_sliderMean[2] = EditorGUILayout.Slider("Extroversion", _sliderMean[2], _minMean, _maxMean);
		_sliderStd[2] = EditorGUILayout.Slider("Extroversion", _sliderStd[2], _minStd,_maxStd);
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal();
		_sliderMean[3] = EditorGUILayout.Slider ("Agreeableness", _sliderMean[3], _minMean, _maxMean);
		_sliderStd[3] = EditorGUILayout.Slider ("Agreeableness", _sliderStd[3], _minStd,_maxStd);
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.BeginHorizontal();
		_sliderMean[4] = EditorGUILayout.Slider ("Neuroticism", _sliderMean[4], _minMean, _maxMean);
		_sliderStd[4] = EditorGUILayout.Slider ("Neuroticism", _sliderStd[4], _minStd,_maxStd);
		EditorGUILayout.EndHorizontal ();
		
		if(GUILayout.Button("Update Personality", GUILayout.ExpandWidth(false))) 	
			_groupBuilder.UpdatePersonalityAndBehavior(_sliderMean, _sliderStd);
        
        EditorGUILayout.Separator();
		
        if (GUILayout.Button("Assign Random Personality", GUILayout.ExpandWidth(false)))  {
            float[] persMean = { 0f, 0f, 0f, 0f, 0f };
            float[] persStd = { 0.35f, 0.35f, 0.35f, 0.35f, 0.35f };
            _groupBuilder.UpdatePersonalityAndBehavior(persMean, persStd);
        }


        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        

        EditorGUILayout.BeginHorizontal();
        _sliderRectX = EditorGUILayout.Slider(" X", _sliderRectX, 0f, 80f);
        _sliderRectZ = EditorGUILayout.Slider("Z", _sliderRectZ, 0f, 80f);        
        EditorGUILayout.EndHorizontal();
        if(GUILayout.Button("Update region", GUILayout.ExpandWidth(false)))
            _groupBuilder.UpdateRegion(_sliderRectX, _sliderRectZ);

      
		
        EditorUtility.SetDirty(target);
	}			 
	
	
}
	