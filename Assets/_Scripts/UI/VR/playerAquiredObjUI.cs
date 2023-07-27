using System.Collections;
using TMPro;
using UnityEngine;

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
    [Range(0,1f)]
    [SerializeField] float fadeDuration;
    private float originalOffset;
    private float time = 0;
    private bool activated = false;
    //[SerializeField] float fadeRate;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        mainCam = Camera.main.transform;
        originalOffset = handOffset;
        text.enabled = false;
    }

    void Update()
    {
        offset = new Vector3(0, handOffset, 0);
        transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
        transform.position = target.transform.position + offset;
        if(activated) 
        {
            handOffset = Mathf.Lerp(originalOffset, originalOffset + travelDistance, time / travelDuration);
            time += Time.deltaTime;
            //Debug.Log(handOffset);
        }
    }

    public void handUILabelActivation(int newTotal)
    {
        text.text = "+" + (newTotal).ToString(); //+1 because when this function is called in InteractableIsTag, HumanShoppingBehavior.cs hasn't incremented the total yet.

        //AI goal to collect 20 Ipads, not realistic anyone will get 20 so 10 is "max" for colorization of quantity
        text.color = Color32.Lerp(startColor, endColor, (newTotal / 10f));
        activated = true;
        text.enabled = true;
        StartCoroutine(fade(fadeDuration));
    }

    public void setTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public void setTextComp(TextMeshProUGUI newText)
    {
        text = newText;
    }

    IEnumerator fade(float fadeDuration)
    {
        for(float a = 1f; a >= 0; a -= fadeDuration)
        {
            text.alpha = a;
            //Debug.Log(text.alpha);
            yield return null;
        }
        resetUI();
    }

    public void setValues(float offset, float distance, float duration)
    {
        handOffset = offset;
        travelDistance = distance;
        travelDuration = duration;
    }

    private void resetUI()
    {
        handOffset = originalOffset;
        text.alpha = 1f;
        text.enabled = false;
        activated = false;
        time = 0;
    }
}
