using System;
using System.Collections;
using UnityEngine;
using static RootMotion.FinalIK.IKSolverVR;

//influence for programmatic animation controller
//https://www.youtube.com/watch?v=nBkiSJ5z-hE
public class VRShopperAnimationController : MonoBehaviour
{
    private AIManager _AIManager;
    private Animator _Animator;
    private FootStep _Footstep;
    [SerializeField] private AnimationData _AnimationData;

    public State _state;
    public bool isAnimated = false;
    private bool noFightSequence = true;
    public bool IsInIsle = false; //no quirks are played in isle as people are focused on grabbing items

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

    private string[] punchSequence;

    public enum State
    {
        beginning,
        moving,
        fighting
    }

    void Start()
    {
        _Animator = GetComponentInChildren<Animator>();
        _AIManager = GameObject.Find("AIManager").GetComponent<AIManager>();
        _Footstep = GetComponentInChildren<FootStep>();
        //_AIManager.RegisterAnimationController(this);
        _state = State.beginning;
        PlayIdleAnimation();
    }

    private void Update()
    {
        FightCheck();
        _Animator.SetFloat("VRIK_Vertical", transform.forward.magnitude);
        //_Animator.SetFloat("VRIK_Horizontal", transform.right.magnitude);
    }

    //Interaction Functions
    //Fighting
    #region

    //The fight animation system generates avarying sequence of punches with different animations, then pauses for a varied time before punching again.
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
        int randomCombo = UnityEngine.Random.Range(1, _AnimationData.totalLeftPunchAnimations + 1);

        //Generate random index numbers to append to animation types to create an animation clip. example "punch_left" + "0"
        int[] punchAnimationIndex = new int[randomCombo];
        for (int i = 0; i < punchAnimationIndex.Length; i++)
        {
            int index = UnityEngine.Random.Range(0, _AnimationData.totalLeftPunchAnimations);
            punchAnimationIndex[i] = index;
        }

        //Randomly choose first punch to be from left or right hand, punch sequences always alternate between hands.
        string[] punchAnimationSequence;
        int randomPunch = UnityEngine.Random.Range(0, 2);
        if (randomPunch == 0)
        {
            punchAnimationSequence = CompletePunchSequence(_AnimationData.punchLeft, punchAnimationIndex);
        }
        else
        {
            punchAnimationSequence = CompletePunchSequence(_AnimationData.punchRight, punchAnimationIndex);
        }

        return punchAnimationSequence;
    }

    //Alternates between hands and appends animation type and index as string to array which holds animation clips for the animation controller to play.
    private string[] CompletePunchSequence(string punchType, int[] punchAnimationIndex)
    {
        bool isLeft = false;
        int size = punchAnimationIndex.Length;
        string[] completePunchSequence = new string[size];
        if (punchType == _AnimationData.punchLeft) { isLeft = true; }

        for (int i = 0; i < size; i++)
        {
            if (isLeft)
            {
                completePunchSequence[i] = _AnimationData.punchLeft + punchAnimationIndex[i].ToString();
                isLeft = false;
            }
            else
            {
                completePunchSequence[i] = _AnimationData.punchRight + punchAnimationIndex[i].ToString();
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
            _Animator.CrossFade(_AnimationData.layer + punchSequence[i], 0.1f);
            yield return new WaitForSeconds(_AIManager.animationDurations[punchSequence[i]]);
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
    #endregion
    //Grabbing Functionality
    #region
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
        rng = UnityEngine.Random.Range(0, _AnimationData.totalPayAnimations);
        ac = new AnimationChoice(_AnimationData.pay, rng);
        return ac;
    }

    private AnimationChoice GetRandomGrabAnimation(ObjComponent.Height height)
    {
        AnimationChoice ac = null;
        int rng;
        switch (height)
        {
            case ObjComponent.Height.High:
                rng = UnityEngine.Random.Range(0, _AnimationData.totalHighGrabAnimations);
                ac = new AnimationChoice(_AnimationData.grabHigh, rng);
                break;

            case ObjComponent.Height.Mid:
                rng = UnityEngine.Random.Range(0, _AnimationData.totalMidGrabAnimations);
                ac = new AnimationChoice(_AnimationData.grabMid, rng);
                break;

            case ObjComponent.Height.Low:
                rng = UnityEngine.Random.Range(0, _AnimationData.totalLowGrabAnimations);
                ac = new AnimationChoice(_AnimationData.grabLow, rng);
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
            _Animator.Play(_AnimationData.layer + animationToPlay);
            //_Animator.CrossFade(_AnimationData.layer + animationToPlay, 0.3f);

            Invoke("AnimationComplete", _AIManager.animationDurations[animationToPlay]); //Ensures Animation will complete, equal to HasExitTime bool for transitions
        }
    }
    #endregion
    //General Animation Functionality
    #region
    private void PlayIdleAnimation()
    {
        int chance = 0;
        isAnimated = false;
        switch (_state)
        {
            case State.moving:
                chance = UnityEngine.Random.Range(0, _AnimationData.totalIdleHandsAnimations);
                _Animator.Play(_AnimationData.layer + _AnimationData.indleHands + chance.ToString());
                //_Animator.CrossFade(_AnimationData.layer + _AnimationData.indleHands + chance.ToString(), 0.3f);
                break;

            case State.beginning:
                chance = UnityEngine.Random.Range(0, _AnimationData.totalIdleHandsAnimations);
                _Animator.Play(_AnimationData.layer + _AnimationData.indleHands + chance.ToString());
                //_Animator.CrossFade(_AnimationData.layer + _AnimationData.indleHands + chance.ToString(), 0.3f);
                break;

            case State.fighting:
                chance = UnityEngine.Random.Range(0, _AnimationData.totalFightingIdleAnimations);
                _Animator.Play(_AnimationData.layer + _AnimationData.fightingIdle + chance.ToString());
                //_Animator.CrossFade(_AnimationData.layer + _AnimationData.fightingIdle + chance.ToString(), 0.3f);
                break;

            default:
                Debug.Log("No state PlayIdleAnimation");
                break;
        }
    }
    #endregion

    //Quirk Related Functions
    #region

    //ChooseQuirk() is called in GlobalAnimationManager, manager selects controller to perform quirk animation,
    //manager gives an array with 1 of each quirk subtype,
    //controller chooses from array with defined probabilities
    public void ChooseQuirk(AnimationChoice[] quirks)
    {
        AnimationChoice _animationToPlay = null;

        if (_state == State.beginning)
        {
            //beginning animation
            _animationToPlay = unpackChoice(_AnimationData.beginning, quirks);
        }

        if (_state == State.moving)
        {
            int total_p = handsAnimationFrequency + waveAnimationFrequency + headSetAnimationFrequency + quickAnimationFrequency;
            int chance = UnityEngine.Random.Range(0, total_p);

            if (chance < handsAnimationFrequency)
            {
                //hands
                _animationToPlay = unpackChoice(_AnimationData.quirkHands, quirks);
            }

            else if (chance < handsAnimationFrequency + headSetAnimationFrequency)
            {
                //headset
                _animationToPlay = unpackChoice(_AnimationData.quirkWave, quirks);
            }

            else if (chance < handsAnimationFrequency + headSetAnimationFrequency + waveAnimationFrequency)
            {
                //wave
                _animationToPlay = unpackChoice(_AnimationData.quirkHeadset, quirks);
            }

            else
            {
                //quick quirk
                _animationToPlay = unpackChoice(_AnimationData.quirkQuick, quirks);
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

    private void PlayChosenQuirkAnimation(AnimationChoice ac)
    {
        if (!isAnimated && !IsInIsle)
        {
            string animationToPlay = ac.ConvertToString();
            isAnimated = true;
            _Animator.Play(_AnimationData.layer + animationToPlay);
            //_Animator.CrossFade(_AnimationData.layer + animationToPlay, 0.3f);
            _AIManager.UpdateQuirkCount(ac);
            Invoke("AnimationComplete", _AIManager.animationDurations[animationToPlay]); //Ensures Animation will complete, equal to HasExitTime bool for transitions
        }
    }
    void AnimationComplete()
    {
        //isAnimated = false;
        PlayIdleAnimation();
    }

    #endregion
    //State and Boolean Checks, utilities
    #region
    public void EnterFightState()
    {
        _state = State.fighting;
        PlayIdleAnimation();
    }

    public void EnterMovingState()
    {
        _state = State.moving;
        //PlayIdleAnimation();
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

    internal void SetIsInIsle(bool v)
    {
        IsInIsle = v;
    }

    public bool IsAnimated()
    {
        return isAnimated;
    }

    public void PlayFootStepAudio()
    {
        //_Footstep.PlayFootStep();
    }
    #endregion
}
