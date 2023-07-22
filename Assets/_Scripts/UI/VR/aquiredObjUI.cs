
using TMPro;
using UnityEngine;

//Video followed for intial code: https://www.youtube.com/watch?v=ysg9oaZEgwc
public class aquiredObjUI : MonoBehaviour
{
    TextMeshProUGUI text;

    Transform mainCam;
    GameObject target;

    Vector3 offset;

    Color32 startColor = new Color32(255, 255, 255, 255);
    Color32 endColor = new Color32(115, 0, 0, 255);

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        mainCam = Camera.main.transform;
        offset = new Vector3(0, -2.5f, 0);
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
        transform.position = target.transform.position - offset;
    }

    public void setAquiredObjCount(int newTotal)
    {
        text.text = newTotal.ToString();

        //AI goal to collect 20 Ipads, not realistic anyone will get 20 so 10 is "max" for colorization of quantity
        text.color = Color32.Lerp(startColor, endColor, (newTotal / 10f));
    }

    public void setTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    public void setTextComp(TextMeshProUGUI newText)
    {
        text = newText;
    }

}
