using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AIManager : MonoBehaviour
{
    [Range(0, 100)]
    [SerializeField] private float GlobalAnimationfrequency; //This number is checked every frame... should be very low or use time instead?
    [Range(0, 100)]
    [SerializeField] private float frequencyThrottle; //not used yet

    [SerializeField] private AnimationData _AnimationData;
    private List<GameObject> shoppers = new List<GameObject>();
    private VRShopperAnimationController[] shopperControllers;
    private int[] shopperControllerIndex;
    public int totalShoppers;
    private Dictionary<string, QuirkStack> animationType;
    public Dictionary<string, float> animationDurations;
    private GameObject Crowd;
    private List<float[]> IsleIndexStates = new List<float[]>();
    private List<float> IsleIndexSummationStates= new List<float>();
    public int[] intitalIsleShopperCount = new int[11];
    public enum State
    {
        beginning,
        moving,
        fighting
    }
    private void Awake()
    {
        //IsleIndexStates = new List<float[]>();
    }
    void Start()
    {
        
        CalculateIsleIndexStates();
        Crowd = GameObject.Find("Crowd");
        shoppers = GameObject.FindGameObjectsWithTag("Player").ToList<GameObject>();
        InitializeManager();
        //StartCoroutine(MoveShoppersIntoPosition());
    }

    private void CalculateIsleIndexStates()
    {
        for (int i = 0; i < 11; i++)
        {
            float[] isleIndexes = new float[11];
            float summation = 0;

            if (i != 0)
            {
                isleIndexes[0] = (1f / System.Math.Abs((float)i));
            }
            else
            {
                isleIndexes[0] = 0;
            }
            summation += isleIndexes[0];

            for (int j = 1; j < 11; j++)
            {
                if (j != i)
                {
                    float val = (1f / System.Math.Abs((float)i - (float)j));
                    isleIndexes[j] = val + isleIndexes[j - 1];
                    summation += val;
                }
                else
                {
                    isleIndexes[i] = isleIndexes[i - 1];
                }
            }
            IsleIndexStates.Add(isleIndexes);
            IsleIndexSummationStates.Add(summation);
        }
    }


    IEnumerator MoveShoppersIntoPosition()
    {

        yield return new WaitForSeconds(2);
        Crowd.transform.position = new Vector3(-26.0800018f, 8.4f, 14.1999998f);
        yield return new WaitForSeconds(1);
        foreach (GameObject shopper in shoppers)
        {
            shopper.GetComponent<AgentComponent>().StaggerStart();
        }
    }

    void Update()
    {
        CheckQuirkAnimationChance();
    }

    private void CheckQuirkAnimationChance()
    {
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
        int ranNum = UnityEngine.Random.Range(0, 100);
        if (ranNum < GlobalAnimationfrequency)
        {
            int shopperIndex = Utilities.getRandomSmallestElement(shopperControllerIndex);
            if (!shopperControllers[shopperIndex].isFighting() && !shopperControllers[shopperIndex].isAnimated && !shopperControllers[shopperIndex].isInteracting)
            {
                shopperControllerIndex[shopperIndex]++;
                ProvideRandomQuirkAnimations(shopperIndex);
            }
        }
    }

    private void ProvideRandomQuirkAnimations(int ShopperIndex)
    {
        AnimationChoice[] ac = new AnimationChoice[5]
        {
             RequestAnimation(_AnimationData.quirkHands),
             RequestAnimation(_AnimationData.quirkHeadset),
             RequestAnimation(_AnimationData.quirkWave),
             RequestAnimation(_AnimationData.quirkQuick),
             RequestAnimation(_AnimationData.beginning)
        };

        shopperControllers[ShopperIndex].ChooseQuirk(ac);
    }

    public AnimationChoice RequestAnimation(string animationType) //animationChoice class in VRShopperAnimationController.cs
    {
        AnimationChoice ac = LeastFrequentAnimation(animationType);
        return ac;
    }

    private AnimationChoice LeastFrequentAnimation(string quirk)
    {
        int index = animationType[quirk].quirkPeek();
        return new AnimationChoice(quirk, index);
    }

    public void UpdateQuirkCount(AnimationChoice ac)
    {
        animationType[ac.animationType].quirkPop();
    }
    private void InitializeAnimationDictionarys()
    {
        animationType = new Dictionary<string, QuirkStack>()
        {
            { "quirk_hands", new QuirkStack(_AnimationData.totalHandAnimations) },
            { "quirk_headset", new QuirkStack(_AnimationData.totalHeadsetAnimations) },
            { "quirk_wave", new QuirkStack(_AnimationData.totalWaveAnimations) },
            { "quirk_quick", new QuirkStack(_AnimationData.totalQuickAnimations) },
            { "beginning", new QuirkStack(_AnimationData.totalBeginningAnimations) }
        };

        //Gets all clips from a Shoppers animation controller to map their durations in a dictionary, stored in AnimationData scriptable object
        AnimationClip[] clips = shopperControllers[0].GetComponentInChildren<Animator>().runtimeAnimatorController.animationClips;
        animationDurations = new Dictionary<string, float>();
        for (int i = 0; i < clips.Length; i++)
        {
            animationDurations.Add(clips[i].name, clips[i].length);
        }
    }

    /*public void RegisterAnimationController(VRShopperAnimationController shopperController)
    {
        //shopperControllers.Add(shopperController);
    }*/

    private void InitializeShopperControllers()
    {
        shopperControllers = FindObjectsOfType<VRShopperAnimationController>();
        totalShoppers = shopperControllers.Length;
        shopperControllerIndex = new int[totalShoppers];
    }

    private void InitializeManager()
    {
        InitializeShopperControllers();
        InitializeAnimationDictionarys();
        SetAvoidancePriority();
    }

    public void MoveShoppersIntoScene()
    {
        StartCoroutine(MoveShoppersIntoPosition());
    }

    private void SetAvoidancePriority()
    {
        int i = 1;
        foreach (GameObject shopper in shoppers)
        {
            shopper.GetComponent<NavMeshAgent>().avoidancePriority = i;
            //shopper.GetComponent<ShopperBehavior>().setAssignedAvoidancePriority();
            i++;
        }
    }

    public float[] getIsleIndexState(int currentIsleIndex)
    {
        return IsleIndexStates[currentIsleIndex];
    }

    public float getIsleSummationState(int currentIsleIndex)
    {
        return IsleIndexSummationStates[currentIsleIndex];
    }
}


