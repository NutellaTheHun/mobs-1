using System;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class UserPADStudySurveyGUI : MonoBehaviour
{

	static string[] _padScalePos = { "Happy", "Pleased", "Satisfied", "Contented", "Hopeful", "Amused", "Stimulated", "Excited", "Frenzied", "Jittery", "Wide awake", "Aroused", "Controlling", "Influential", "Important","Dominant", "Autonomous" };
	static string[] _padScaleNeg = { "Unhappy", "Annoyed", "Unsatisfied", "Melancholic", "Despairing", "Bored", "Relaxed", "Calm", "Sluggish", "Dull", "Sleepy", "Unaroused", "Controlled", "Influenced",  "Awed", "Submissive", "Guided" };

	public GameObject[] QInput = new GameObject[17];
	float[] _padAnswers = new float[17];

	//float _qHeight = 28;

	public bool isPreStudy;

	[DllImport("__Internal")]
	private static extern void SendSurveyToPage(string surveyType, float[] responses, int size, int crowdPrsonality);

	[DllImport("__Internal")]
	private static extern void SendCompletedToPage();

	private void Start() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

	}


	public void UpdatePAD(int qNumber) {		
		_padAnswers[qNumber] = (float)QInput[qNumber].transform.Find("Slider").GetComponent<Slider>().value;


		//Debug.Log(qNumber + " " + _padAnswers[qNumber] + " "+ _padScalePos[qNumber] + " " + _padScaleNeg[qNumber]);

	}

	public void Submit() {
		if(isPreStudy) {
#if !UNITY_EDITOR && UNITY_WEBGL

		SendSurveyToPage("preStudy", _padAnswers, _padAnswers.Length, UserInfo.PersonalityDistribution);
#endif

			SceneManager.LoadScene("Warmup");
		}

        else {
#if !UNITY_EDITOR && UNITY_WEBGL

		SendSurveyToPage("postStudy", _padAnswers, _padAnswers.Length, UserInfo.PersonalityDistribution);
		SendCompletedToPage();
#endif

			SceneManager.LoadScene("End");

		}
    }




}
