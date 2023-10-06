using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntroductionSphere : MonoBehaviour
{
    private AIManager AIManager;

    private GameObject PaymentCollider;

    private GameObject LeftIpad;
    private GameObject RightIpad;

    private GameObject IpadInstruction;
    private GameObject PaymentInstruction;
    private GameObject ExitInstruction;
    List<GameObject> Ipads = new List<GameObject>();
    List<GameObject> TextList = new List<GameObject>();

    private GameObject Sphere;
    private MeshRenderer renderer;
    private Material SphereMaterial;

    HumanShoppingBehavior User;
    private InstructionState State;
    private bool finishing;

    enum InstructionState
    { 
        PickupIpads,
        Pay,
        WaitingForUsers,
    }


    void Start()
    {
        User = GameObject.Find("XR Origin").GetComponentInChildren<HumanShoppingBehavior>();

        AIManager = GameObject.Find("AIManager").GetComponent<AIManager>();

        PaymentCollider = GameObject.Find("TutorialPaymentCollider");

        LeftIpad = GameObject.Find("LeftIpad");
        RightIpad = GameObject.Find("RightIpad");

        IpadInstruction = GameObject.Find("Ipad Instruction");
        PaymentInstruction = GameObject.Find("Payment Instruction");
        ExitInstruction = GameObject.Find("Exit Instruction");

        Sphere = GameObject.Find("Sphere");
        renderer = Sphere.GetComponent<MeshRenderer>();
        SphereMaterial = renderer.material;

        TextList.Add(IpadInstruction);
        TextList.Add(PaymentInstruction);
        TextList.Add(ExitInstruction);

        Ipads.Add(LeftIpad);
        Ipads.Add(RightIpad);

        StartState();
    }

    private void StartState()
    {
        UpdateState(State = InstructionState.PickupIpads);
    }

    // Update is called once per frame
    void Update()
    {
        switch(State)
        {
            case InstructionState.PickupIpads:
                if (User.Stats.CollectedItemCnt == 2)
                {
                    UpdateState(State = InstructionState.Pay);
                }
                break;
            case InstructionState.Pay:
                if(User.getHasPaid())
                {
                    UpdateState(State = InstructionState.WaitingForUsers);
                }
                break;
            case InstructionState.WaitingForUsers:

                break;
            default:
                Debug.Log("default update state IntroSphere\n");
                break;
        }
       
    }

    private void SetInstructionalText(GameObject InstructionalText)
    {
        foreach(GameObject textObject in TextList)
        {
            if(textObject == InstructionalText)
            {
                textObject.GetComponent<TextMeshPro>().enabled = true;
            }
            else
            {
                textObject.GetComponent<TextMeshPro>().enabled = false;
            }
        }
    }

    private void UpdateState(InstructionState state)
    {
        switch(state)
        {
            case InstructionState.PickupIpads:

                SetInstructionalText(IpadInstruction);
                PaymentCollider.SetActive(false);

                break;
            case InstructionState.Pay:

                SetInstructionalText(PaymentInstruction);
                PaymentCollider.SetActive(true);

                break;
            case InstructionState.WaitingForUsers:

                SetInstructionalText(ExitInstruction);
                PaymentInstruction.SetActive(false);
                while(!finishing)
                {
                    finishing = true;
                    StartCoroutine(WaitingForUsers());
                }
                

                break;
            default:
                Debug.Log("default State UpdateState Introduction Sphere\n");
                break; 
        }
    }

    IEnumerator WaitingForUsers()
    {
        User.ResetShoppingStats();
        User.ResetPaymentSystem();
        yield return new WaitForSeconds(4);

        Vector3 ActiveRadius = Sphere.transform.localScale;
        float modifier = 0;
        float frequency = 1f;
        while (ActiveRadius.x < 174)
        {
            modifier += Time.deltaTime * frequency;
            ActiveRadius = new Vector3(ActiveRadius.x + modifier, ActiveRadius.y + modifier, ActiveRadius.z + modifier);
            Sphere.transform.localScale = ActiveRadius;
            yield return null;
        }

        AIManager.MoveShoppersIntoScene();
        Destroy(this.gameObject);
    }
}
