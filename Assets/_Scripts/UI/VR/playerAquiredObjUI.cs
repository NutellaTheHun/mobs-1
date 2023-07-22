using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Utility;

public class playerAquiredObjUI : MonoBehaviour
{
    TextMeshProUGUI text;

    Transform mainCam;

    GameObject target;

    Vector3 offset;

    Color32 startColor = new Color32(255, 255, 255, 255);
    Color32 endColor = new Color32(115, 0, 0, 255);

    //whole thing should be ~.75s
    [SerializeField] float handOffset;
    [SerializeField] float travelDistance;
    [SerializeField] float travelDuration;
    //[SerializeField] float fadeRate;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        mainCam = Camera.main.transform;

    }

    void Update()
    {
        offset = new Vector3(0, handOffset, 0);
        transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
        transform.position = target.transform.position - offset;
    }

    public void handUILabelActivation(int newTotal)
    {
        text.text = "+" + newTotal.ToString();

        //AI goal to collect 20 Ipads, not realistic anyone will get 20 so 10 is "max" for colorization of quantity
        text.color = Color32.Lerp(startColor, endColor, (newTotal / 10f));
        text.transform.position = Vector3.Lerp(offset, offset + new Vector3(0, travelDistance,0), Time.time / travelDuration);
        StartCoroutine(fade());
        resetPosition();
    }

    public void setTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public void setTextComp(TextMeshProUGUI newText)
    {
        text = newText;
    }

    IEnumerator fade()
    {
        Color32 c = text.color;
        for(float a = 255f; a >= 0; a -= 7.5f) //7.5f is rough estimate for ~.75s fade
        {
            text.alpha = a;
            yield return null;
        }
    }

    public void setValues(float offset, float distance, float duration)
    {
        handOffset = offset;
        travelDistance = distance;
        travelDuration = duration;
    }

    private void resetPosition()
    {
        text.transform.position = offset;
        text.alpha = 255f;
    }
}
