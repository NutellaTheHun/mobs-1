
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class UserPersonalitySurveyGUI : MonoBehaviour {

	// static string[] _tipi = {" Extraverted, enthusiastic", "Critical, quarrelsome", "Dependable, self-disciplined", "Anxious, easily upset", "Open to new experiences, complex", "Reserved, quiet", "Sympathetic, warm", "Disorganized, careless", "Calm, emotionally stable", "Conventional, uncreative" }; 
	static string[] _scale = {"Disagree strongly", "Disagree moderately", "Disagree slightly", "Neither disagree nor agree", "Agree slightly", "Agree moderately", "Agree strongly" };


	public GameObject[] QInput = new GameObject[10];

	[DllImport("__Internal")]
	private static extern void SendSurveyToPage(string surveyType, float[] responses, int size);

	private void Start() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void UpdateTIPI(int qNumber) {
		Toggle t = QInput[qNumber].transform.Find("ToggleGroup").GetComponent<ToggleGroup>().GetFirstActiveToggle();
		if(t == null)
			return;
		for(int i = 0; i < _scale.Length; i++) {
			if(t.name == _scale[i]) {
				UserInfo.TIPI[qNumber] = (float)i;
				Debug.Log(t.name + " "  + _scale[i] + " " + qNumber+ "" +  i);
				
			}

		}
	}
	public void Submit() {
#if !UNITY_EDITOR && UNITY_WEBGL
		SendSurveyToPage("personality", UserInfo.TIPI, UserInfo.TIPI.Length);
#endif

        SceneManager.LoadScene("UserPreStudySurvey");
    }



}

	//	void OnGUI() {



	//        style.fontSize = 16;
	//        style.normal.textColor = Color.Lerp(Color.red, Color.yellow, 0.2f);

	//		GUI.skin = GridSkin;

	//		GUILayout.BeginArea(new Rect(20, 20, 1000, Screen.height-40));

	//		GUILayout.Label("Please select how much you see yourself as:\n\n", style);

	//		style.fontSize = 13;
	//		style.normal.textColor = Color.white;
	//		for (int i = 0; i < _tipi.Length; i++){



	//			//GUI.color = Color.white;

	//			//GUI.backgroundColor = Color.white;


	//			GUI.Label(new Rect(0, _qHeight+ 20 + i * _qHeight, 100, _qHeight), "" + (i+1).ToString()+ ". "+ _tipi[i], style);



	//			UserInfo.TIPI[i] = GUI.SelectionGrid(new Rect(210, _qHeight + 10 + i * _qHeight, 1000, _qHeight), i, _scale, 7, GridSkin.GetStyle("button"));

	//			//GUILayout.Label("\n");			
	//		}



	//		GUILayout.EndArea();

	//		if (GUI.Button(new Rect(Screen.width / 2f, Screen.height-40, 100, 30), "Submit"))
	//		{
	//#if !UNITY_EDITOR && UNITY_WEBGL
	//			SendSurveyToPage("personality", UserInfo.TIPI, UserInfo.TIPI.Length);
	//#endif

	//			SceneManager.LoadScene("UserPreStudySurvey");
	//		}


	//}

