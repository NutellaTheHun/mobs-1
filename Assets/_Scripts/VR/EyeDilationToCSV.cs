using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//reference https://www.youtube.com/watch?v=sU_Y2g1Nidk
public class EyeDilationToCSV : MonoBehaviour
{
    string fileName = "";
    public List<EyeDilationData> list = new List<EyeDilationData>();
    public ViveSR.anipal.Eye.SingleEyeData eye;
    [System.Serializable]
    public class EyeDilationData
    {
        public float eyeDilationMM;
        public float time;
        public EyeDilationData(float timeVal, float eyeDilationVal)
        {
            eyeDilationMM = eyeDilationVal;
            time = timeVal;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        fileName = Application.dataPath + "/dilation.csv";
    }

    void OnApplicationQuit()
    {
        toCSV();
    }

        // Update is called once per frame
        void Update()
    {
        list.Add(new EyeDilationData(Time.time, eye.pupil_diameter_mm));
    }

    public void toCSV()
    {
        if(list.Count > 0)
        {
            TextWriter tw = new StreamWriter(fileName, false);
            tw.WriteLine("deltaTime, eyeDilationMM");
            tw.Close();

            tw = new StreamWriter(fileName, true);
            for(int i = 0; i < list.Count; i++) 
            {
                tw.WriteLine(list[i].time + "," + list[i].eyeDilationMM);
            }
            tw.Close();
        }
    }
}
