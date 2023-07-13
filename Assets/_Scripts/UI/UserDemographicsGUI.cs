using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;


public static class UserInfo {

    public static string UserId = "";
    public static string Gender = "Male";
    public static int Age = 0;
    public static string Nationality = "White";

    public static int Familiarity = 0;


    public static float[] TIPI = new float[10];
    public static int PersonalityDistribution; //0: docile, 1: hostile

}


public class UserDemographicsGUI : MonoBehaviour {


    string[] _genderStr = { "Male", "Female", "Other" };
    string[] _nationalityStr = { "White", "Black", "Asian", "Hispanic / Latinx", "Other" };

    

    public GameObject AgeInput;
    public GameObject GenderInput;
    public GameObject NationalityInput;
    public GameObject FamiliarityInput;


    [DllImport("__Internal")]
    private static extern void SendDemographicsToPage(string gender, int age, string nationality, int familiarity);

    private void Awake() {
     
        UserInfo.PersonalityDistribution = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }


    public void SetAge() {
        int.TryParse(AgeInput.GetComponent<TMP_InputField>().text, out UserInfo.Age);

    }

    public void SetGender() {
        UserInfo.Gender = _genderStr[GenderInput.GetComponent<TMP_Dropdown>().value];

    }

    public void SetNationality() {
        UserInfo.Nationality = _nationalityStr[NationalityInput.GetComponent<TMP_Dropdown>().value];
    }

    public void SetFamiliarity() {
        
        UserInfo.Familiarity = (int)FamiliarityInput.transform.Find("Slider").GetComponent<Slider>().value;
    
    }
    public void Submit() {
#if !UNITY_EDITOR && UNITY_WEBGL
            SendDemographicsToPage(UserInfo.Gender, UserInfo.Age, UserInfo.Nationality, UserInfo.Familiarity);
#endif

        SceneManager.LoadScene("UserPersonalitySurvey");

    }
   

}
