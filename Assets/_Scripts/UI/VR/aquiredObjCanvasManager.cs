using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class aquiredObjCanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject AIprefab;
    [SerializeField] private GameObject Playerprefab;
    //List<shopperLabelLinker> counterLabels = new List<shopperLabelLinker>();
    [SerializeField] Canvas worldCanvas;

    public void initializeShopperCounterUI(ShopperBehavior target)
    {
        GameObject AquiredObjCounterUI = Instantiate(AIprefab, new Vector3(0,0,0), Quaternion.identity);
        aquiredObjUI aou = AquiredObjCounterUI.GetComponent<aquiredObjUI>();
        TextMeshProUGUI tmp = AquiredObjCounterUI.GetComponent<TextMeshProUGUI>();
        aou.setTarget(target.gameObject);
        aou.setTextComp(tmp);
        target.setAquiredUI(aou);
        AquiredObjCounterUI.transform.SetParent(worldCanvas.transform);
    }

    public void initializePlayerShopperCounterUI(InteractableIsTag target)
    {
        GameObject PlayerHandAquiredObjCounterUI = Instantiate(Playerprefab, new Vector3(0, 0, 0), Quaternion.identity);
        playerAquiredObjUI paou = PlayerHandAquiredObjCounterUI.GetComponent<playerAquiredObjUI>();
        TextMeshProUGUI tmp = PlayerHandAquiredObjCounterUI.GetComponent<TextMeshProUGUI>();
        paou.setTarget(target.gameObject);
        paou.setTextComp(tmp);
        //paou.setValues(-0.1f, 0.3f, 2.5f);
        target.setPlayerAquiredUI(paou);

        PlayerHandAquiredObjCounterUI.transform.SetParent(worldCanvas.transform);
    }
}
