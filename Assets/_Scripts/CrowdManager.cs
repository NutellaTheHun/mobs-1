using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;
using System;

public class CrowdManager : MonoBehaviour {
    //[DllImport("__Internal")]
    //private static extern void Hello();

    //[DllImport("__Internal")]
    //private static extern void HelloString(string str);

    //[DllImport("__Internal")]
    //private static extern void PrintFloatArray(float[] array, int size);

    //[DllImport("__Internal")]
    //private static extern int AddNumbers(int x, int y);

    //[DllImport("__Internal")]
    //private static extern string StringReturnValueFunction();

    //[DllImport("__Internal")]
    //private static extern void BindWebGLTexture(int texture);


    public void Restart() {
      
        AgentComponent[] agentComponents = FindObjectsOfType(typeof(AgentComponent)) as AgentComponent[];
        foreach(AgentComponent a in agentComponents)
            a.Restart();
    
        AffectComponent[] affectComponents = FindObjectsOfType(typeof(AffectComponent)) as AffectComponent[];
        foreach (AffectComponent a in affectComponents)
            a.Restart();


        Appraisal[] appraisalComponents = FindObjectsOfType(typeof(Appraisal)) as Appraisal[];
        foreach (Appraisal a in appraisalComponents)
            a.Restart();
				

        ShopperBehavior[] shopperComponents = FindObjectsOfType(typeof(ShopperBehavior)) as ShopperBehavior[];
        foreach (ShopperBehavior s in shopperComponents)
            s.Restart();

  

        AudienceBehavior[] audienceComponents = FindObjectsOfType(typeof(AudienceBehavior)) as AudienceBehavior[];
        foreach (AudienceBehavior a in audienceComponents)
            a.Restart();

        AnimationSelector[] animationComponents = FindObjectsOfType(typeof(AnimationSelector)) as AnimationSelector[];
        foreach (AnimationSelector a in animationComponents)
            a.Restart();



    }


     void Start() {
        //Hello();

        //HelloString("This is a string.");

        //float[] myArray = new float[10];
        //PrintFloatArray(myArray, myArray.Length);

        //int result = AddNumbers(5, 7);
        //Debug.Log(result);

        //Debug.Log(StringReturnValueFunction());

        //var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
        //BindWebGLTexture(texture.GetNativeTextureID());
    }


}
