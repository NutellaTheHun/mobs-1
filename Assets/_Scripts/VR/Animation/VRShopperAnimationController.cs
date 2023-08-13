using System;
using System.Collections;
using UnityEngine;

//influence for programmatic animation controller
//https://www.youtube.com/watch?v=nBkiSJ5z-hE
public class VRShopperAnimationController : MonoBehaviour
{
    private Animator _animator;
    private GlobalAnimationManager _animationManager;

    public State _state;
    private bool isAnimated = false;
    private bool noFightSequence = true;

    [Range(0, 100)]
    [SerializeField] private int handsAnimationFrequency;
    [Range(0, 100)]
    [SerializeField] private int waveAnimationFrequency;
    [Range(0, 100)]
    [SerializeField] private int headSetAnimationFrequency;
    [Range(0, 100)]
    [SerializeField] private int quickAnimationFrequency;
    [Range(0, 100)]
    [SerializeField] private int beginningAnimationFrequency;

    private int totalIdleHandsAnimations;
    private int totalFightingIdleAnimations;
    private int totalLeftPunchAnimations;
    private int totalRightPunchAnimations;
    private int totalPayAnimations;
    private int totalLowGrabAnimations;
    private int totalMidGrabAnimations;
    private int totalHighGrabAnimations;

    private const string layer = "UpperBody.";
    private string[] punchSequence;

    //structure of the types of animations to play, index is appended to string to be the literal animation clip,such as quick_hands1, or quirk_wave2
    private const string quirk_hands = "quirk_hands";
    private const string quirk_headset = "quirk_headset";
    private const string quirk_wave = "quirk_wave";
    private const string quirk_quick = "quirk_quick";
    private const string beginning = "beginning";
    private const string fighting_idle = "fighting_idle"; //probably dont need idle handled here
    private const string punch_left = "punch_left";
    private const string punch_right = "punch_right";
    private const string pay = "pay";
    private const string grab_high = "grab_high";
    private const string grab_mid = "grab_mid";
    private const string grab_low = "grab_low";
    private const string indle_hands = "Idle_down";

    public enum State
    {
        beginning,
        moving,
        fighting
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _animationManager = GameObject.Find("AnimationManager").GetComponent<GlobalAnimationManager>();
        _state = State.beginning;
        InitializeTotalAnimationTypeCountInformation();
        PlayIdleAnimation();
    }

    private void Update()
    {
        FightCheck();
    }

    //-----------Interaction Functions-----------------
    //---Fighting

    //Fighting animation system overall generates a sequence of punches with varying amounts and animations, then pauses for a varied time before punching again.
    private void FightCheck()
    {
        if (_state == State.fighting)
        {
            if (noFightSequence)
            {
                punchSequence = GeneratePunchSequence();
                StartCoroutine(PlayPunchSequence(punchSequence));
            }
        }
    }

    private string[] GeneratePunchSequence()
    {
        noFightSequence = false;

        //Generate how many punches to deliver, (originally between 1 and 3 punches in one sequence)
        int randomCombo = UnityEngine.Random.Range(1, totalLeftPunchAnimations + 1);

        //Generate random index numbers to append to animation types to create an animation clip. example "punch_left" + "0"
        int[] punchAnimationIndex = new int[randomCombo];
        for (int i = 0; i < punchAnimationIndex.Length; i++)
        {
            int index = UnityEngine.Random.Range(0, totalLeftPunchAnimations);
            punchAnimationIndex[i] = index;
        }

        //Randomly choose first punch to be from left or right hand, punch sequences always alternate between hands.
        string[] punchAnimationSequence;
        int randomPunch = UnityEngine.Random.Range(0, 2);
        if (randomPunch == 0)
        {
            punchAnimationSequence = CompletePunchSequence(punch_left, punchAnimationIndex);
        }
        else
        {
            punchAnimationSequence = CompletePunchSequence(punch_right, punchAnimationIndex);
        }

        return punchAnimationSequence;
    }

    //Alternates between hands and appends animation type and index as string to array which holds animation clips for the animation controller to play.
    private string[] CompletePunchSequence(string punchType, int[] punchAnimationIndex)
    {
        bool isLeft = false;
        int size = punchAnimationIndex.Length;
        string[] completePunchSequence = new string[size];
        if (punchType == punch_left) { isLeft = true; }

        for (int i = 0; i < size; i++)
        {
            if (isLeft)
            {
                completePunchSequence[i] = punch_left + punchAnimationIndex[i].ToString();
                isLeft = false;
            }
            else
            {
                completePunchSequence[i] = punch_right + punchAnimationIndex[i].ToString();
                isLeft = true;
            }
        }
        return completePunchSequence;
    }

    //Coroutine to pace the animation clips to fully play, then starts fight transition which is a pause for a random amount of time before punching again.
    IEnumerator PlayPunchSequence(string[] punchSequence)
    {
        for (int i = 0; i < punchSequence.Length; i++)
        {
            _animator.CrossFade(layer + punchSequence[i], 0.1f);
            yield return new WaitForSeconds(_animationManager.GetAnimationClipDuration(punchSequence[i]));
        }
        StartCoroutine(EnterFightTransition());
    }
    IEnumerator EnterFightTransition()
    {
        PlayIdleAnimation();
        float rng = UnityEngine.Random.Range(0.2f, 1f);
        yield return new WaitForSeconds(rng);
        noFightSequence = true;
    }
    //-----------Grabbing Functionality
    public void PlayPickupAnimation(ObjComponent.Height height)
    {
        PlayChosenInteractionAnimation(
            GetRandomGrabAnimation(height)
            );
    }
    public void PlayPayingAnimation()
    {
        PlayChosenInteractionAnimation(GetRandomPayingAnimation());
    }

    private AnimationChoice GetRandomPayingAnimation()
    {
        AnimationChoice ac = null;
        int rng;
        rng = UnityEngine.Random.Range(0, totalPayAnimations);
        ac = new AnimationChoice(pay, rng);
        return ac;
    }

    private AnimationChoice GetRandomGrabAnimation(ObjComponent.Height height)
    {
        AnimationChoice ac = null;
        int rng;
        switch (height)
        {
            case ObjComponent.Height.High:
                rng = UnityEngine.Random.Range(0, totalHighGrabAnimations);
                ac = new AnimationChoice(grab_high, rng);
                break;

            case ObjComponent.Height.Mid:
                rng = UnityEngine.Random.Range(0, totalMidGrabAnimations);
                ac = new AnimationChoice(grab_mid, rng);
                break;

            case ObjComponent.Height.Low:
                rng = UnityEngine.Random.Range(0, totalLowGrabAnimations);
                ac = new AnimationChoice(grab_low, rng);
                break;

            default:
                Debug.Log("No height state from desiredObj");
                break;
        }
        return ac;
    }

    private void PlayChosenInteractionAnimation(AnimationChoice ac)
    {
        if (!isAnimated)
        {
            if (ac == null){ Debug.Log("AnimationChoice is null"); return; }
            isAnimated = true;

            string animationToPlay = ac.ConvertToString();
            _animator.Play(layer + animationToPlay);

            Invoke("AnimationComplete", _animationManager.GetAnimationClipDuration(animationToPlay)); //Ensures Animation will complete, equal to HasExitTime bool for transitions
        }
    }

    //--------------- General Animation Functionality
    private void PlayIdleAnimation()
    {
        int chance = 0;
        switch (_state)
        {
            case State.moving:
                chance = UnityEngine.Random.Range(0, totalIdleHandsAnimations);
                _animator.Play(layer + indle_hands + chance.ToString());
                break;

            case State.beginning:
                chance = UnityEngine.Random.Range(0, totalIdleHandsAnimations);
                _animator.Play(layer + indle_hands + chance.ToString());
                break;

            case State.fighting:
                chance = UnityEngine.Random.Range(0, totalFightingIdleAnimations);
                _animator.Play(layer + fighting_idle + chance.ToString());
                break;

            default:
                Debug.Log("No state PlayIdleAnimation");
                break;
        }
    }

    private void PlayChosenQuirkAnimation(AnimationChoice ac)
    {
        if (!isAnimated)
        {
            string animationToPlay = ac.ConvertToString();
            isAnimated = true;
            _animator.Play(layer + animationToPlay);
            _animationManager.UpdateQuirkCount(ac);
            Invoke("AnimationComplete", _animationManager.GetAnimationClipDuration(animationToPlay)); //Ensures Animation will complete, equal to HasExitTime bool for transitions
        }
    }
    void AnimationComplete()
    {
        isAnimated = false;
        PlayIdleAnimation();
    }

    //----------------------------------

    //------------------Quirk Related Functions

    //ChooseQuirk() is called in GlobalAnimationManager, manager selects controller to perform quirk animation,
    //manager gives an array with 1 of each quirk subtype,
    //controller chooses from array with defined probabilities
    public void ChooseQuirk(AnimationChoice[] quirks)
    {
        AnimationChoice _animationToPlay = null;

        if (_state == State.beginning)
        {
            //beginning animation
            _animationToPlay = unpackChoice(beginning, quirks);
        }

        if (_state == State.moving)
        {
            int total_p = handsAnimationFrequency + waveAnimationFrequency + headSetAnimationFrequency + quickAnimationFrequency;
            int chance = UnityEngine.Random.Range(0, total_p);

            if (chance < handsAnimationFrequency)
            {
                //hands
                _animationToPlay = unpackChoice(quirk_hands, quirks);
            }

            else if (chance < handsAnimationFrequency + headSetAnimationFrequency)
            {
                //headset
                _animationToPlay = unpackChoice(quirk_wave, quirks);
            }

            else if (chance < handsAnimationFrequency + headSetAnimationFrequency + waveAnimationFrequency)
            {
                //wave
                _animationToPlay = unpackChoice(quirk_headset, quirks);
            }

            else
            {
                //quick quirk
                _animationToPlay = unpackChoice(quirk_quick, quirks);
            }
        }
        PlayChosenQuirkAnimation(_animationToPlay);
    }

    
    //Gets specific animation type from Quirk array in ChooseQuirk(), wanted to make the order of the array not matter.
    private AnimationChoice unpackChoice(string quirkType, AnimationChoice[] choices)
    {
        AnimationChoice choice;
        for (int i = 0; i < choices.Length; i++)
        {
            if (choices[i].animationType == quirkType)
            {
                choice = choices[i];
                return choice;
            }
        }
        throw new ArgumentException("quirk not found in unpackChoice function");
    }

    //----------------------------------------

    //------------State and Boolean Checks, utilities
    public void EnterFightState()
    {
        _state = State.fighting;
        PlayIdleAnimation();
    }

    public void EnterMovingState()
    {
        _state = State.moving;
        PlayIdleAnimation();
    }

    public void UpdateAnimationState(State state)
    {
        _state = state;
    }

    public bool isMoving()
    {
        if (_state == State.moving) { return true; }
        return false;
    }

    public bool isFighting()
    {
        if (_state == State.fighting) { return true; }
        return false;
    }

    //Trigger is in Door entrances, shoppers start in beginning state.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AnimationMovingState"))
        {
            EnterMovingState();
        }
    }

    private void InitializeTotalAnimationTypeCountInformation()
    {
        int[] input = _animationManager.InitializeShopperInteractionAnimationCounts();

        totalIdleHandsAnimations = input[0];
        totalFightingIdleAnimations = input[1];
        totalLeftPunchAnimations = input[2];
        totalRightPunchAnimations = input[3];
        totalPayAnimations = input[4];
        totalLowGrabAnimations = input[5];
        totalMidGrabAnimations = input[6];
        totalHighGrabAnimations = input[7];
    }

    
}
