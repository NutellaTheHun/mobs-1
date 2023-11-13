using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ViveSR.anipal.Eye;
using System.Runtime.InteropServices;

//reference https://www.youtube.com/watch?v=sU_Y2g1Nidk
public class EyeDilationToCSV : MonoBehaviour
{
    string fileName = "";
    public List<EyeDilationData> list = new List<EyeDilationData>();
    private float openness;
    public ViveSR.anipal.Eye.SingleEyeData eye;
    VerboseData data;
    private static EyeData eyeData = new EyeData();
    private bool eye_callback_registered = false;
    bool isLeftEyeActive = false;
    bool isRightEyeAcitve = false;
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
        if (!SRanipal_Eye_Framework.Instance.EnableEye)
        {
            enabled = false;
            return;
        }
    }

    void OnApplicationQuit()
    {
        toCSV();
    }
    //list.Add(new EyeDilationData(Time.time, eye.pupil_diameter_mm));
        // Update is called once per frame
    void Update()
    {
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                      SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

        if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
        {
            SRanipal_Eye.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
            eye_callback_registered = true;
        }
        else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
        {
            SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }
        else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false)
            SRanipal_Eye_API.GetEyeData(ref eyeData);

        if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING)
        {
            isLeftEyeActive = eyeData.verbose_data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY);
            isRightEyeAcitve = eyeData.verbose_data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY);
        }
        if (isLeftEyeActive || isRightEyeAcitve)
        {
            if (eye_callback_registered == true)
                if(SRanipal_Eye.GetVerboseData(out data))
                {

                    list.Add(new EyeDilationData(Time.time, data.left.pupil_diameter_mm));
                }
        }
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

    private static void EyeCallback(ref EyeData eye_data)
    {
        eyeData = eye_data;
    }
}
