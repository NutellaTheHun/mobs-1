using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;


public class ControllerEditor : MonoBehaviour {

    [MenuItem("Controllers/Copy")]
    static void CreateController() {
        // Creates the controller

       

        const string assetPath = "Assets/Resources/ControllerAgentN1.controller";

        foreach(string c in Globals.CharNames) {

            string copyPath = "Assets/Resources/Controller" + c + ".controller";
            if(AssetDatabase.CopyAsset(assetPath, copyPath)) {
                Debug.Log("success");
                //Assign to their corresponding characters



                string prefabPath = "Assets/Resources/" + c + ".prefab";
                GameObject agent = PrefabUtility.LoadPrefabContents(prefabPath);

//                GameObject controller = PrefabUtility.LoadPrefabContents(copyPath);//


                RuntimeAnimatorController controller = Resources.Load("Controller" + c ) as RuntimeAnimatorController;
                Debug.Log("Controller" + c +".controller");
                Debug.Log(controller);

                agent.GetComponent<Animator>().runtimeAnimatorController = controller;

                PrefabUtility.SaveAsPrefabAsset(agent, prefabPath);

            }

            
        }

        
    }
}