using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public enum OCEAN {
    O, //Openness,
    C, //Conscientiousness,
    E, //Extroversion,
    A, //Agreeableness,
    N  //Neuroticism

}

public enum PADType {
    P, //Pleasure
    A, //Arousal
    D  //Dominance
}
public enum EkmanType {
    Happy = 0,
    Sad = 1,
    Angry = 2,
    Afraid = 3,
    Neutral = 4,
    Disgust = 5,
    Shock = 6
}

public enum EmotionRange {
    Low,
    Moderate,
    High
};

public enum EType {
    Admiration, //0
    Anger, //1
    Disappointment, //2
    Distress, //3
    Fear, //4
    FearsConfirmed, //5
    Gloating, //6
    Gratification, //7
    Gratitude, //8
    HappyFor, //9
    Hate, //10
    Hope, //11
    Joy, //12
    Love, //13
    Pity, //14
    Pride, //15
    Relief, //16
    Remorse, //17
    Reproach, //18
    Resentment, //19
    Satisfaction, //20
    Shame //21
}


public enum MType {
    Exuberant, //Extroverted, outgoing, happy, sociable
    Dependent, //Attached to people, needy of others and their help, interpersonally positive and sociable
    Relaxed, //Comfortable, secure, confident, resilient to stress 
    Docile, //Pleasant, unemotional, and submissive; likeable; conforming
    Bored, //Sad, lonely, socially withdrawn, physically inactive
    Disdainful, //Contemptuous of others, loner, withdrawn and calculating, sometimes anti-social 
    Anxious, //Worried, nervous, insecure, tense, unhappy, illness prone 
    Hostile   //Angry, emotional in negative ways, possibly violent
}


public struct IndexValuePair {
    public int Ind;
    public float Value;
}
public class AffectComponent : MonoBehaviour {


    private Vector3[] PAD = new Vector3[22];
    public Vector3 DefaultMood = Vector3.zero;
    public Vector3 EmotionCenter;
    public Vector3 Mood = Vector3.zero;
    public float[] Personality = new[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
    public float[] Emotion = new[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };    
    public float[] Ekman = new[] {0f, 0f, 0f, 0f,0f};
    public int DominantEkmanEmotion;
        
    public string[] EkmanStr = new [] {"happy", "sad", "angry", "afraid", "neutral"};
    [SerializeField]
    public Contagion[] LambdaE = new Contagion[22];
    
    private Vector3[] _pad = new Vector3[22];
    //private GUIHandler _guiHandler;

    public bool ContagionMode;

    public float[] EmotionWeight = new float[22];
    public float DecayCoef; //emotion decay coefficient


    private double[][] _weightCoeffs = new double[][]{ 
        new double[]{-0.005,	-0.034,	-0.015,	0.449,	-0.076},
        new double[] {0.151,	0.0425,	0.0429,	-0.2039,	-0.4332},
        new double[] {-0.085,	-0.068,	-0.303,	-0.019,	-0.114},
        new double[] {-0.155,	-0.085,	-0.384,	-0.136,	0.038},
        new double[] {-0.0175,	-0.0731,	-0.3924,	-0.06,	-0.4636},
        new double[] {-0.22,	-0.119,	-0.525,	-0.161,	0.076},
        new double[] {-0.07,	-0.017,	0.003,	0.119,	0.228},
        new double[] {0.175,	0.068,	0.366,	0.376,	-0.171},
        new double[] {-0.045,	-0.051,	-0.096,	0.392,	-0.038},
        new double[] {0.08,	0.034,	0.204,	0.232,	-0.038},
        new double[] {0.165,	0.051,	0.054,	-0.27,	-0.456},
        new double[] {0.005,	-0.017,	-0.018,	0.21,	-0.076},
        new double[] {0.055,	0.017,	0.144,	0.264,	-0.038},
        new double[] {0.065,	0.034,	0.183,	0.143,	0},
        new double[] {-0.155,	-0.085,	-0.384,	-0.136,	0.038},
        new double[] {0.12,	0.051,	0.264,	0.23,	-0.095},
        new double[] {0.055,	0.068,	0.282,	-0.1,	0.209},
        new double[] {-0.135,	-0.102,	-0.423,	0.045,	-0.114},
        new double[] {0.085,	0.068,	0.177,	-0.335,	0},
        new double[] {-0.095,	-0.034,	-0.162,	-0.144,	0.133},
        new double[] {0.07,	0.068,	0.303,	-0.011,	0.171},
        new double[] {-0.135,	-0.102,	-0.423,	0.045,	-0.114}};
    


    
    private float _expressibilityThreshold = 0.5f; //how expressive a person is -- depends on extroversion
    public float ExpressibilityThreshold {
        get {
            return _expressibilityThreshold;
        }  
        set {
            _expressibilityThreshold = value;
        }
    }




    public void Awake() {
        //Alloc memory for contagion
        LambdaE = new Contagion[22];
        for (int i = 0; i < LambdaE.Length; i++)
            LambdaE[i] = new Contagion();

        EmotionWeight = new float[22];

        InitEmotionContagion();
        UpdatePersonalityDependents();

        

        if (GetComponent<PersonalityMapper>() != null)
            GetComponent<PersonalityMapper>().PersonalityToSteering();

        InitPad();


        ContagionMode = true;
        //_guiHandler = FindObjectOfType(typeof(GUIHandler)) as GUIHandler;

        //ContagionMode = _guiHandler.ContagionMode;

    }

    public void InitPad() {

        PAD[(int)EType.Admiration].x = 0.5f; PAD[(int)EType.Admiration].y = 0.3f; PAD[(int)EType.Admiration].z = -0.2f;
        PAD[(int)EType.Anger].x = -0.51f; PAD[(int)EType.Anger].y = 0.59f; PAD[(int)EType.Anger].z = 0.25f;
        PAD[(int)EType.Disappointment].x = -0.3f; PAD[(int)EType.Disappointment].y = 0.1f; PAD[(int)EType.Disappointment].z = -0.4f;
        PAD[(int)EType.Distress].x = -0.4f; PAD[(int)EType.Distress].y = 0.2f; PAD[(int)EType.Distress].z = -0.5f;
        PAD[(int)EType.Fear].x = -0.64f; PAD[(int)EType.Fear].y = 0.6f; PAD[(int)EType.Fear].z = -0.43f;
        PAD[(int)EType.FearsConfirmed].x = -0.5f; PAD[(int)EType.FearsConfirmed].y = -0.3f; PAD[(int)EType.FearsConfirmed].z = -0.7f;
        PAD[(int)EType.Gloating].x = 0.3f; PAD[(int)EType.Gloating].y = -0.3f; PAD[(int)EType.Gloating].z = -0.1f;
        PAD[(int)EType.Gratification].x = 0.6f; PAD[(int)EType.Gratification].y = 0.5f; PAD[(int)EType.Gratification].z = 0.4f;
        PAD[(int)EType.Gratitude].x = 0.4f; PAD[(int)EType.Gratitude].y = 0.2f; PAD[(int)EType.Gratitude].z = -0.3f;
        PAD[(int)EType.HappyFor].x = 0.4f; PAD[(int)EType.HappyFor].y = 0.2f; PAD[(int)EType.HappyFor].z = 0.2f;
        PAD[(int)EType.Hate].x = -0.6f; PAD[(int)EType.Hate].y = 0.6f; PAD[(int)EType.Hate].z = 0.3f;
        PAD[(int)EType.Hope].x = 0.2f; PAD[(int)EType.Hope].y = 0.2f; PAD[(int)EType.Hope].z = -0.1f;
        PAD[(int)EType.Joy].x = 0.4f; PAD[(int)EType.Joy].y = 0.2f; PAD[(int)EType.Joy].z = 0.1f;
        PAD[(int)EType.Love].x = 0.3f; PAD[(int)EType.Love].y = 0.1f; PAD[(int)EType.Love].z = 0.2f;
        PAD[(int)EType.Pity].x = -0.4f; PAD[(int)EType.Pity].y = -0.2f; PAD[(int)EType.Pity].z = -0.5f;
        PAD[(int)EType.Pride].x = 0.4f; PAD[(int)EType.Pride].y = 0.3f; PAD[(int)EType.Pride].z = 0.3f;
        PAD[(int)EType.Relief].x = 0.2f; PAD[(int)EType.Relief].y = -0.3f; PAD[(int)EType.Relief].z = 0.4f;
        PAD[(int)EType.Remorse].x = -0.3f; PAD[(int)EType.Remorse].y = 0.1f; PAD[(int)EType.Remorse].z = -0.6f;
        PAD[(int)EType.Reproach].x = -0.3f; PAD[(int)EType.Reproach].y = 0.1f; PAD[(int)EType.Reproach].z = 0.4f;
        PAD[(int)EType.Resentment].x = -0.2f; PAD[(int)EType.Resentment].y = -0.3f; PAD[(int)EType.Resentment].z = -0.2f;
        PAD[(int)EType.Satisfaction].x = 0.3f; PAD[(int)EType.Satisfaction].y = -0.2f; PAD[(int)EType.Satisfaction].z = 0.4f;
        PAD[(int)EType.Shame].x = -0.3f; PAD[(int)EType.Shame].y = 0.1f; PAD[(int)EType.Shame].z = -0.6f;

    }

    private void InitDefaultMood() {

        DefaultMood.x = 0.21f * Personality[(int)OCEAN.E] + 0.59f * Personality[(int)OCEAN.A] + 0.19f * Personality[(int)OCEAN.N];
        DefaultMood.y = 0.15f * Personality[(int)OCEAN.O] + 0.3f * Personality[(int)OCEAN.A] + 0.57f * Personality[(int)OCEAN.N];
        DefaultMood.z = 0.25f * Personality[(int)OCEAN.O] + 0.17f * Personality[(int)OCEAN.C] + 0.6f * Personality[(int)OCEAN.E] - 0.32f * Personality[(int)OCEAN.A];

        Mood = DefaultMood;
    }

    public void ComputeMood() {

        int i;
        
        //const float pullPushSpeed = 1f;
        //const float decaySpeed = 0.001f;
        //Vector3 m2ec, ec2m, m2dm, ec2dm;
        Vector3  m2dm;

        int activeEmotionCnt = 0;
        EmotionCenter = DefaultMood;
        for (i = 0; i < PAD.Length; i++) {

            if (Emotion[i] > 0) {

                EmotionCenter += PAD[i] * Emotion[i];
                activeEmotionCnt++;

            }
        }

        

        m2dm = DefaultMood - Mood;
        m2dm.Normalize();

        /*
        if (activeEmotionCnt > 0) {

            //emotionCenter = emotionCenter / activeEmotionCnt;
            emotionCenter = emotionCenter / MathDefs.Length(Emotion);

            m2ec = emotionCenter - Mood;
            ec2m = Mood - emotionCenter;
            ec2dm = DefaultMood - emotionCenter;

            m2ec.Normalize();

            if (Vector3.Dot(m2ec, m2dm) > 0.0f && Vector3.Dot(ec2m, ec2dm) < 0.0f)
                Mood = -pullPushSpeed * m2ec;
            else
                Mood = pullPushSpeed * m2ec;

            

        }
        */


       // Mood += pullPushSpeed*(EmotionCenter - Mood) * Time.deltaTime +  decaySpeed*(DefaultMood - Mood) * Time.deltaTime;


        Mood = EmotionCenter;

        //now decay Mood to the default Mood		
     //   Mood = Mood - decaySpeed * m2dm * Time.deltaTime;


        //constrain to region -1 to 1

        if (Mood.x > 1) Mood.x = 1;
        else if (Mood.x < -1) Mood.x = -1;

        if (Mood.y > 1) Mood.y = 1;
        else if (Mood.y < -1) Mood.y = -1;

        if (Mood.z > 1) Mood.z = 1;
        else if (Mood.z < -1) Mood.z = -1;

    }
    public int GetMoodOctant(Vector3 md) {
        if (md[(int)PADType.P] >= 0 && md[(int)PADType.A] <= 0 && md[(int)PADType.D] >= 0)
            return (int)MType.Relaxed;
        else if (md[(int)PADType.P] < 0 && md[(int)PADType.A] > 0 && md[(int)PADType.D] < 0)
            return (int)MType.Anxious;
        else if (md[(int)PADType.P] > 0 && md[(int)PADType.A] > 0 && md[(int)PADType.D] > 0)
            return (int)MType.Exuberant;
        else if (md[(int)PADType.P] <= 0 && md[(int)PADType.A] <= 0 && md[(int)PADType.D] <= 0)
            return (int)MType.Bored;
        else if (md[(int)PADType.P] > 0 && md[(int)PADType.A] > 0 && md[(int)PADType.D] <= 0)
            return (int)MType.Dependent;
        else if (md[(int)PADType.P] <= 0 && md[(int)PADType.A] <= 0 && md[(int)PADType.D] > 0)
            return (int)MType.Disdainful;
        else if (md[(int)PADType.P] > 0 && md[(int)PADType.A] <= 0 && md[(int)PADType.D] <= 0)
            return (int)MType.Docile;
        else if (md[(int)PADType.P] <= 0 && md[(int)PADType.A] > 0 && md[(int)PADType.D] > 0)
            return (int)MType.Hostile;
        else
            return -1;
    }

    public bool IsMood(int moodType) {
        if (GetCurrMoodOctant() == moodType)
            return true;
            return false;
    }
    public int GetCurrMoodOctant() {
        return GetMoodOctant(Mood);
    }

    public int GetDefaultMoodOctant() {
        return GetMoodOctant(DefaultMood);
    }

    public string GetMoodName(int type) {
        switch (type) {
            case (int)MType.Relaxed:
                return "Relaxed";
            case (int)MType.Anxious:
                return "Anxious";
            case (int)MType.Exuberant:
                return "Exuberant";
            case (int)MType.Bored:
                return "Bored";
            case (int)MType.Dependent:
                return "Dependent";
            case (int)MType.Disdainful:
                return "Disdainful";
            case (int)MType.Docile:
                return "Docile";
            case (int)MType.Hostile:
                return "Hostile";
            default:
                return "Neutral";
        }
    }
    public void Restart() {
        //Update Emotion  (Personality updated by user)
        for (int i = 0; i < Emotion.Length; i++)
            Emotion[i] = 0f;

        for (int i = 0; i < LambdaE.Length; i++) {
            //LambdaE[i] = new Contagion();            
            LambdaE[i].Restart();
        }


    }
    
    public int GetExpressionInd() {
        int moodOctant = GetCurrMoodOctant();
        if (moodOctant == (int)MType.Exuberant || moodOctant == (int)MType.Docile || moodOctant == (int)MType.Dependent || moodOctant == (int)MType.Relaxed)
            return (int)EkmanType.Happy;
        if (moodOctant == (int)MType.Disdainful || moodOctant == (int)MType.Bored) //sad
            return (int)EkmanType.Sad;
        if (moodOctant == (int)MType.Anxious)
            return (int)EkmanType.Afraid;
        if (moodOctant == (int)MType.Hostile)
            return (int)EkmanType.Angry;
        
        return (int)EkmanType.Neutral;

    }
    
    public float GetExpressionValue() {
        return Mathf.Clamp(Mood.magnitude, 0, 1f);
    }

    public EmotionRange GetExpressionRange() {
        float val = GetExpressionValue();
        if (val <= 0.3f)
            return EmotionRange.Low;
        else if (val <= 0.70f)
            return EmotionRange.Moderate;
        else
            return EmotionRange.High;

    }

    /*
    public int GetExpressionInd() {
        return FindDominantEkmanEmotion();
    }
    public float GetExpressionValue() {
      return FindDominantEkmanEmotionValue();
    }
    */
    
    public Color GetPADColor(int dimension) {
        float value;
        if(dimension == (int)PADType.P) {
            value = Mood.x;
            if (value < 0)
                return Color.Lerp(Color.white, Color.green, Mathf.Abs(value));
            return Color.Lerp(Color.white, Color.red, Mathf.Abs(value));
        }
        if (dimension == (int)PADType.A) {
            value = Mood.y;
            if (value < 0)
                return Color.Lerp(Color.white, Color.blue, Mathf.Abs(value));
            return Color.Lerp(Color.white, new Color(1,0.5f,0), Mathf.Abs(value));
        }
        if (dimension == (int)PADType.D) {
            value = Mood.z;
            if (value < 0)
                return Color.Lerp(Color.white, Color.yellow, Mathf.Abs(value));
            return Color.Lerp(Color.white, new Color(0.5f,0,1f), Mathf.Abs(value));
        }

        return Color.white;
    }

    
    //Ekman Emotion's color
     public Color GetExpressionColor() {
         int moodOctant = GetCurrMoodOctant();
        Color c = new Color(1,1,1);

         //return new Color((Mood.x + 1)/2f, (Mood.y + 1)/2f, (Mood.z + 1)/2f);


         
        if(moodOctant == (int)MType.Dependent)
            c = new Color(1, 1, 1);
        else if(moodOctant == (int)MType.Docile)
            c = new Color(0, 1, 1);
        else if (moodOctant == (int)MType.Relaxed)
            c = new Color(1, 0, 1);
        
            
        else if (moodOctant == (int)MType.Exuberant)
            c = new Color(1, 1, 0);
        else if (moodOctant == (int)MType.Bored)
            c = new Color(0, 0, 1);
        else if(moodOctant == (int)MType.Disdainful)
            c = new Color(0, 0, 0);
        else if (moodOctant == (int)MType.Anxious)
            c = new Color(0, 1, 0);
        else if (moodOctant == (int)MType.Hostile)
            c = new Color(1, 0, 0);
        else
            c = new Color(1, 1, 1);

         return Color.Lerp(new Color(0.5f, 0.5f, 0.5f), c, Mood.magnitude);


       

    }


    public void UpdatePersonality(float[] persMean, float[] persStd) {

        for (int i = 0; i < Personality.Length; i++) {
            ComputePersonality(i, persMean[i], persStd[i]);
        }

        UpdatePersonalityDependents();
    }

    public void UpdatePersonalityDependents() {
        InitEmotionContagion(); //Depends on Personality
        _expressibilityThreshold = -0.5f * Personality[(int)OCEAN.E] + 0.5f; //0.25ti
        DecayCoef = (Personality[(int)OCEAN.N] + 3f); //between 0.02 and 0.04 units per second

        ComputeEventWeights(1f);
        InitDefaultMood();
    }


    private void ComputeEventWeights(float speed) {
        
        float []negWeights = new float[Emotion.Length]; //including negative values
        EmotionWeight = new float[Emotion.Length];
        /*for (int i = 0; i < Emotion.Length; i++) {
            negWeights[i] = 0;
            EmotionWeight[i] = 0;
            for (int j = 0; j < Personality.Length; j++)
                negWeights[i] += speed * (float)_weightCoeffs[i][j] * Personality[j];

       //     if (EmotionWeight[i] < 0) EmotionWeight[i] = 0;
        }

        //Compute reverse emotion weights        
         for (int i = 0; i < EmotionWeight.Length; i++) {
             
             if (negWeights[i] >= 0)
                 EmotionWeight[i] = negWeights[i];
             else {
                 
                 EmotionWeight[OppositeEmotionInd(i)] -= negWeights[i];
                 
             }
         }
        */
         for (int i = 0; i < EmotionWeight.Length; i++)              
                 EmotionWeight[i] = 1f;
       
    }
    private int OppositeEmotionInd(int ind) {
        switch (ind) {
            case (int)EType.Admiration:
                return (int)EType.Reproach;
            case (int)EType.Anger:
                return (int)EType.Gratitude;
            case (int)EType.Disappointment:
                return (int)EType.Satisfaction;
            case (int)EType.Distress:
                return (int)EType.Joy;
            case (int)EType.Fear:
                return (int)EType.Hope;
            case (int)EType.FearsConfirmed:
                return (int)EType.Relief;
            case (int)EType.Gloating:
                return (int)EType.Pity;
            case (int)EType.Gratification:
                return (int)EType.Remorse;
            case (int)EType.Gratitude:
                return (int)EType.Anger;
            case (int)EType.HappyFor:
                return (int)EType.Resentment;
            case (int)EType.Hate:
                return (int)EType.Love;
            case (int)EType.Hope:
                return (int)EType.Fear;
            case (int)EType.Joy:
                return (int)EType.Distress;
            case (int)EType.Love:
                return (int)EType.Hate;
            case (int)EType.Pity:
                return (int)EType.Gloating;
            case (int)EType.Pride:
                return (int)EType.Shame;
            case (int)EType.Relief:
                return (int)EType.FearsConfirmed;
            case (int)EType.Remorse:
                return (int)EType.Gratification;
            case (int)EType.Reproach:
                return (int)EType.Admiration;
            case (int)EType.Resentment:
                return (int)EType.HappyFor;
            case (int)EType.Satisfaction:
                return (int)EType.Disappointment;
            case (int)EType.Shame:
                return (int)EType.Pride;
            default:
                Debug.LogError("Incorrect emotion index.");
                return -1;
        }

    }
        
    public bool CanExpress(int emotionInd) {
        return Emotion[emotionInd] >= _expressibilityThreshold;
    }



    
    private void InitEmotionContagion() {

        //empathy  in the range [0 1]
        //float empathy = (1f + Personality[(int)OCEAN.O] * 0.34f + Personality[(int)OCEAN.C] * 0.17f + Personality[(int)OCEAN.E] * 0.13f + Personality[(int)OCEAN.A] * 0.33f + Personality[(int)OCEAN.N] * 0.03f) / 2f;
        //empathy  in the range [-1 1]
        float empathy =(Personality[(int)OCEAN.O] * 0.34f + Personality[(int)OCEAN.C] * 0.17f + Personality[(int)OCEAN.E] * 0.13f + Personality[(int)OCEAN.A] * 0.3f + Personality[(int)OCEAN.N] * 0.02f) /0.96f;
        if (LambdaE == null) {
            return;
        }

        //dose threshold in the range [0 1], inversely correlated with empathy
        for (int i = 0; i < LambdaE.Length; i++) {
           // LambdaE[i].DoseThreshold = 1.0f - empathy;
            LambdaE[i].DoseThreshold = 0.5f - 0.5f*empathy;
        }
        

    }

    /// <summary>
    ///Emotions are contracted depending on other agents' distance and orientation 
    /// </summary>
    /// <param name="inds">
    /// Gives the indices of the affected emotions
    /// </param>
    private void ComputeEmotionContagion(List<IndexValuePair> indVal, float coef) {

        //Add new doses of Emotion
        for (int i = 0; i < indVal.Count; i++) {
            LambdaE[indVal[i].Ind].AddDose(indVal[i].Value, coef);            
        }
       
        //Decay doses of all contracted emotions
        //We need to call separately because we may not call adddose when agent is immune, but we need to decay doses
    //    for (int i = 0; i < LambdaE.Length; i++) {
     //       LambdaE[i].DecayDose();
     //   }
         
    }

    public void ComputePersonality(int ind, float mean, float std) {

        Personality[ind] = MathDefs.GaussianDist(mean, std);
        if (Personality[ind] < -1f)
            Personality[ind] = -1f;
        else if (Personality[ind] > 1f)
            Personality[ind] = 1f;

    }
    
    //Emotion is between 0 and 1
    public void ComputeEmotion(float[] eventFactor, Dictionary<int, float> indVal) {


        int i;
        
        float timeCoef = 0.1f;



        //ComputeEmotionContagion(indVal, (LambdaE[indVal[i].Ind].K * contractedEmotionCnt[indVal[i].Ind])); //Updates lambda		

        //Emotion contagion
        //Add new doses of Emotion
        /*
        int[] contractedEmotionCnt = new int[Emotion.Length]; // counts the number of agents that contracted the emotion i
        for (i = 0; i < contractedEmotionCnt.Length; i++)
            contractedEmotionCnt[i] = 0;
        for (i = 0; i < indVal.Count; i++) 
            contractedEmotionCnt[indVal[i].Ind]++;
        */

        if (ContagionMode) {
            for (i = 0; i < Emotion.Length; i++) {
                if (indVal.ContainsKey(i))
                    LambdaE[i].AddDose(indVal[i], LambdaE[i].K);
                else
                    LambdaE[i].AddDose(0, LambdaE[i].K);
            }

        }
        //  for (i = 0; i < indVal.Count; i++) {
      //      LambdaE[indVal[i].Ind].AddDose(indVal[i].Value, (LambdaE[indVal[i].Ind].K * contractedEmotionCnt[indVal[i].Ind]));//normalize doses to  to [0 1] range
            
      //  }
        //We need to decay doses otherwise expressibility will be meaningless
         

        //Decay doses of all contracted emotions
        //We need to call separately because we may not call adddose when agent is immune, but we need to decay doses
        //    for (i = 0; i < LambdaE.Length; i++) 
          //     LambdaE[i].DecayDose();
        //   }



        for (i = 0; i < Emotion.Length; i++) {
            Emotion[i] += eventFactor[i] * timeCoef * Time.deltaTime; //eventfactor between 0 1, so reaches max within 0 to 10 seconds

            if (LambdaE[i].Status == InfectionStatus.Infected) //only infected individuals' emotions are affected by their surroundings
                Emotion[i] += LambdaE[i].Dose * timeCoef * Time.deltaTime; //dose decays in time  -- 10 seconds
            //Dose is between [0 10n] n being the number of agents, so we normalize it to [0 1] range

        }

        
        //Clamp emotions to [0 1] interval
        for (i = 0; i < Emotion.Length; i++) {
            if (Emotion[i] > 1f)
                Emotion[i] = 1f;
        }
        //Decay Emotion
        for (i = 0; i < Emotion.Length; i++) {
            // Emotion[i] -=   Emotion[i] * beta * Time.deltaTime;  //causes smaller emotions to decay slower
            Emotion[i] -= Emotion[i] * DecayCoef * timeCoef * 0.01f * Time.deltaTime;  //emotions reach 0 within 25 to 50 seconds
            
            if (Emotion[i] < 0.00001f)
                Emotion[i] = 0f;
        }
        

        //UpdateEkmanEmotion();

        //DominantEkmanEmotion = FindDominantEkmanEmotion();


    }

 

    //Assign maximum emotion for each category
    private void UpdateEkmanEmotion() {
        float[] happy = new float[11];
        float[] sad = new float[6];
        float[] angry = new float[3];
        float[] afraid = new float[2];


        int i = 0;
        happy[i++] = Emotion[(int)EType.HappyFor];
        happy[i++] = Emotion[(int)EType.Gloating];
        happy[i++] = Emotion[(int)EType.Joy];
        happy[i++] = Emotion[(int)EType.Pride];
        happy[i++] = Emotion[(int)EType.Admiration];
        happy[i++] = Emotion[(int)EType.Love];
        happy[i++] = Emotion[(int)EType.Satisfaction];
        happy[i++] = Emotion[(int)EType.Relief];
        happy[i++] = Emotion[(int)EType.Gratification];
        happy[i++] = Emotion[(int)EType.Gratitude];
        happy[i++] = Emotion[(int)EType.Hope];
        Ekman[(int)EkmanType.Happy] = happy.Max();

        i = 0;
        sad[i++] = Emotion[(int)EType.Resentment];
        sad[i++] = Emotion[(int)EType.Pity];
        sad[i++] = Emotion[(int)EType.Shame];
        sad[i++] = Emotion[(int)EType.Remorse];
        sad[i++] = Emotion[(int)EType.Disappointment];
        sad[i++] = Emotion[(int)EType.Distress];
        Ekman[(int)EkmanType.Sad] = sad.Max();

        i = 0;
        angry[i++] = Emotion[(int)EType.Anger];
        angry[i++] = Emotion[(int)EType.Hate];
        angry[i++] = Emotion[(int)EType.Reproach];
        Ekman[(int)EkmanType.Angry] = angry.Max();

        i = 0;
        afraid[i++] = Emotion[(int)EType.Fear];
        afraid[i++] = Emotion[(int)EType.FearsConfirmed];
        Ekman[(int)EkmanType.Afraid] = afraid.Max();



    }

    /*
    private int FindDominantEkmanEmotion() {
        int maxInd = 0;
        float maxEm = Ekman[0];
        for (int i = 1; i < Ekman.Length; i++) {         
            if (Ekman[i] >= maxEm) {
                maxEm = Ekman[i];
                maxInd = i;
            }
        }
        return maxInd;        
    }

    
    public float  FindDominantEkmanEmotionValue() { //same for both ekman and occ emotions
        return Ekman[FindDominantEkmanEmotion()];
    }
 

    */


}
