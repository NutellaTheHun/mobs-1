using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AffectComponent))]
public class AffectEditor : Editor{
	
	AffectComponent _affect;
	
	void OnEnable(){
		_affect = target as AffectComponent;
	    

	}
    
	
	public override void OnInspectorGUI () {
		
		
		//Personality is editable
		GUILayout.Label("Personality", EditorStyles.boldLabel);
        float []prevOCEAN = new float[5];
        for(int i = 0; i < 5; i++) 
            prevOCEAN[i] = _affect.Personality[i];
        
		_affect.Personality[(int)OCEAN.O] = EditorGUILayout.Slider ("Openness", _affect.Personality[(int)OCEAN.O], -1f, 1f);
		_affect.Personality[(int)OCEAN.C] = EditorGUILayout.Slider ("Conscientiousness", _affect.Personality[(int)OCEAN.C], -1f, 1f);
		_affect.Personality[(int)OCEAN.E] = EditorGUILayout.Slider ("Extroversion", _affect.Personality[(int)OCEAN.E], -1f, 1f);
		_affect.Personality[(int)OCEAN.A] = EditorGUILayout.Slider ("Agreeableness", _affect.Personality[(int)OCEAN.A], -1f, 1f);
		_affect.Personality[(int)OCEAN.N] = EditorGUILayout.Slider ("Neuroticism", _affect.Personality[(int)OCEAN.N], -1f, 1f);

        for (int i = 0; i < 5; i++) {
            if (prevOCEAN[i] != _affect.Personality[i]) {
                _affect.UpdatePersonalityDependents();
                  break;
              }
        }
		EditorGUILayout.Space ();

        //Mood is not editable
        GUILayout.Label("Mood", EditorStyles.boldLabel);

        EditorGUILayout.FloatField("Pleasure", _affect.Mood[(int)PADType.P], GUILayout.ExpandWidth(true));
        EditorGUILayout.FloatField("Arousal", _affect.Mood[(int)PADType.A], GUILayout.ExpandWidth(true));
        EditorGUILayout.FloatField("Dominance", _affect.Mood[(int)PADType.D], GUILayout.ExpandWidth(true));
        EditorGUILayout.FloatField("Magnitude", _affect.Mood.magnitude, GUILayout.ExpandWidth(true));

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Current mood: " + _affect.GetMoodName(_affect.GetCurrMoodOctant()), EditorStyles.largeLabel);
        GUILayout.Label("Default mood: " + _affect.GetMoodName(_affect.GetDefaultMoodOctant()), EditorStyles.largeLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.Label("Emotion Center: " + _affect.EmotionCenter.x.Truncate(3) + " " + _affect.EmotionCenter.y.Truncate(3) + " " + _affect.EmotionCenter.z.Truncate(3), EditorStyles.largeLabel);
		//Emotion is not editable
		GUILayout.Label("Emotion", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Expressibility Threshold", _affect.ExpressibilityThreshold, GUILayout.ExpandWidth(false));
        EditorGUILayout.FloatField("Dose Threshold" , _affect.LambdaE[(int)EType.Anger].DoseThreshold, GUILayout.ExpandWidth(false));
        EditorGUILayout.EndHorizontal();


        GUILayout.Label("\t\t\t\t\t\t\tEmotion\t\tContagion\tEvent Weight", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Admiration", _affect.Emotion[(int)EType.Admiration].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Admiration].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Admiration].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Anger", _affect.Emotion[(int)EType.Anger].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Anger].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Anger].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Disappointment", _affect.Emotion[(int)EType.Disappointment].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Disappointment].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Disappointment].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Distress", _affect.Emotion[(int)EType.Distress].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Distress].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Distress].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Fear", _affect.Emotion[(int)EType.Fear].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Fear].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Fear].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("FearsConfirmed", _affect.Emotion[(int)EType.FearsConfirmed].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.FearsConfirmed].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.FearsConfirmed].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Gloating", _affect.Emotion[(int)EType.Gloating].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Gloating].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Gloating].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Gratification", _affect.Emotion[(int)EType.Gratification].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Gratification].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Gratification].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Gratitude", _affect.Emotion[(int)EType.Gratitude].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Gratitude].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Gratitude].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("HappyFor", _affect.Emotion[(int)EType.HappyFor].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.HappyFor].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.HappyFor].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Hate", _affect.Emotion[(int)EType.Hate].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Hate].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Hate].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Hope", _affect.Emotion[(int)EType.Hope].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Hope].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Hope].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Joy", _affect.Emotion[(int)EType.Joy].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Joy].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Joy].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Love", _affect.Emotion[(int)EType.Love].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Love].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Love].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Pity", _affect.Emotion[(int)EType.Pity].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Pity].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Pity].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Pride", _affect.Emotion[(int)EType.Pride].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Pride].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Pride].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Relief", _affect.Emotion[(int)EType.Relief].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Relief].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Relief].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Remorse", _affect.Emotion[(int)EType.Remorse].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Remorse].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Remorse].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Reproach", _affect.Emotion[(int)EType.Reproach].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Reproach].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Reproach].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Resentment", _affect.Emotion[(int)EType.Resentment].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Resentment].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Resentment].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Satisfaction", _affect.Emotion[(int)EType.Satisfaction].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Satisfaction].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Satisfaction].Truncate(2));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.FloatField("Shame", _affect.Emotion[(int)EType.Shame].Truncate(2), GUILayout.ExpandWidth(false));
        GUILayout.Label(" " + _affect.LambdaE[(int)EType.Shame].Dose.Truncate(2));
        GUILayout.Label(" " + _affect.EmotionWeight[(int)EType.Shame].Truncate(2));
		EditorGUILayout.EndHorizontal();



        EditorGUILayout.Space();


        GUILayout.Label("Ekman", EditorStyles.boldLabel);

        EditorGUILayout.FloatField("Happy", _affect.Ekman[(int)EkmanType.Happy].Truncate(2), GUILayout.ExpandWidth(false));
        EditorGUILayout.FloatField("Sad", _affect.Ekman[(int)EkmanType.Sad].Truncate(2), GUILayout.ExpandWidth(false));
        EditorGUILayout.FloatField("Angry", _affect.Ekman[(int)EkmanType.Angry].Truncate(2), GUILayout.ExpandWidth(false));
        EditorGUILayout.FloatField("Afraid", _affect.Ekman[(int)EkmanType.Afraid].Truncate(2), GUILayout.ExpandWidth(false));



		//GUILayout.Label("Contagion", EditorStyles.boldLabel);
		
		//EditorGUILayout.BeginHorizontal();
		//EditorGUILayout.EndHorizontal();
	}
}
