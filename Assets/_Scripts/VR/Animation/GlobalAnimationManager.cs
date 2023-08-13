using System.Collections.Generic;
using UnityEngine;

public class AnimationChoice
{
    public int index { get; set; }
    public string animationType { get; set; }
    public AnimationChoice(string _animationType, int _index)
    {
        animationType = _animationType;
        index = _index;
    }
    public string ConvertToString()
    {
        return animationType + index.ToString();//1 is added to index because animation files in assets are numbered starting at 1
    }
}
public class QuirkStack
{
    Stack<int> green = new Stack<int>();
    Stack<int> blue = new Stack<int>();
    bool greenIsActive = true;
    public QuirkStack(int totalCount)
    {
        for (int i = 0; i < totalCount; i++)
        {
            green.Push(i);
        }
    }
    public void quirkPop()
    {
        if (greenIsActive)
        {
            int x = green.Pop();
            blue.Push(x);
            if (green.Count == 0) { greenIsActive = false; }
        }
        else
        {
            int x = blue.Pop();
            green.Push(x);
            if (blue.Count == 0) { greenIsActive = true; }
        }
    }
    public int quirkPeek()
    {
        if (greenIsActive)
        {
            return green.Peek();
        }
        else
        {
            return blue.Peek();
        }
    }
}
public class GlobalAnimationManager : MonoBehaviour
{
    [Range(0,100)]
    [SerializeField] private float GlobalAnimationfrequency; //Tis number is checked every frame... should be very low or use time instead?
    [Range(0, 100)]
    [SerializeField] private float frequencyThrottle; //not used yet

    private int totalShoppers;

    private VRShopperAnimationController[] shopperControllers;
    private int[] shopperControllerIndex;
    private Dictionary<string, QuirkStack> animationType;
    private Dictionary<string, float> animationDurations;

    //For now must manually enter the amount of animations per type (hand, headset, wave, quick, begining, ect.)
    //Unused Variables are still passed to VRShopperController so dont remove!
    [SerializeField] private int totalHandAnimations;
    [SerializeField] private int totalHeadsetAnimations;
    [SerializeField] private int totalWaveAnimations;
    [SerializeField] private int totalQuickAnimations;
    [SerializeField] private int totalBeginningAnimatinons;
    [SerializeField] private int totalFightingIdleAnimations;
    [SerializeField] private int totalIdleHandsAnimations;
    [SerializeField] private int totalLeftPunchAnimations;
    [SerializeField] private int totalRightPunchAnimations;
    [SerializeField] private int totalPayAnimations;
    [SerializeField] private int totalLowGrabAnimations;
    [SerializeField] private int totalMidGrabAnimations;
    [SerializeField] private int totalHighGrabAnimations;

    //structure of the types of animations to play, index is appended to string to be the literal animation clip,such as quick_hands1, or quirk_wave2
    private const string quirk_hands = "quirk_hands";
    private const string quirk_headset = "quirk_headset";
    private const string quirk_wave = "quirk_wave";
    private const string quirk_quick = "quirk_quick";
    private const string beginning = "beginning";
    //private const string fighting_idle = "fighting_idle"; //probably dont need idle handled here
    //private const string punch_left = "punch_left";
    //private const string punch_right = "punch_right";
    //private const string paying = "paying";
    //private const string grab = "grab";

    public enum State
    {
        beginning,
        moving,
        fighting
    }

    void Start()
    {
        InitializeManager();
    }

    void Update()
    {
        CheckQuirkAnimationChance();
    }

    private void CheckQuirkAnimationChance()
    {
        int ranNum = Random.Range(0, 100);
        if (ranNum < GlobalAnimationfrequency)
        {
            int shopperIndex = Utilities.getRandomSmallestElement(shopperControllerIndex);
            if(!shopperControllers[shopperIndex].isFighting())
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
             RequestAnimation(quirk_hands),
             RequestAnimation(quirk_headset),
             RequestAnimation(quirk_wave),
             RequestAnimation(quirk_quick),
             RequestAnimation(beginning)
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

    public float GetAnimationClipDuration(string v)
    {
        return animationDurations[v];
    }

    public int[] InitializeShopperInteractionAnimationCounts()
    {
        return new int[] { 
            totalIdleHandsAnimations, 
            totalFightingIdleAnimations, 
            totalLeftPunchAnimations, 
            totalRightPunchAnimations, 
            totalPayAnimations, 
            totalLowGrabAnimations,  
            totalMidGrabAnimations, 
            totalHighGrabAnimations };
    }
    

    private void InitializeAnimationDictionarys()
    {
        animationType = new Dictionary<string, QuirkStack>()
        {
            { "quirk_hands", new QuirkStack(totalHandAnimations) },
            { "quirk_headset", new QuirkStack(totalHeadsetAnimations) },
            { "quirk_wave", new QuirkStack(totalWaveAnimations) },
            { "quirk_quick", new QuirkStack(totalQuickAnimations) },
            { "beginning", new QuirkStack(totalBeginningAnimatinons) }
           
        };

        //Gets all clips from a Shoppers animation controller to map their durations in a dictionary
        AnimationClip[] clips = shopperControllers[0].GetComponent<Animator>().runtimeAnimatorController.animationClips;
        animationDurations = new Dictionary<string, float>();
        for (int i = 0; i < clips.Length; i++)
        {
            animationDurations.Add(clips[i].name, clips[i].length);
        }
    }

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
    }

    
}


