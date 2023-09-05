using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationData", menuName = "ScriptableObject/AnimationData")]
public class AnimationData : ScriptableObject
{
    public int totalIdleHandsAnimations;
    public int totalFightingIdleAnimations;
    public int totalLeftPunchAnimations;
    public int totalRightPunchAnimations;
    public int totalPayAnimations;
    public int totalLowGrabAnimations;
    public int totalMidGrabAnimations;
    public int totalHighGrabAnimations;
    public int totalHandAnimations;
    public int totalHeadsetAnimations;
    public int totalWaveAnimations;
    public int totalQuickAnimations;
    public int totalBeginningAnimations;

    public string quirkHands = "quirk_hands";
    public string quirkHeadset = "quirk_headset";
    public string quirkWave = "quirk_wave";
    public string quirkQuick = "quirk_quick";
    public string beginning = "beginning";
    public string fightingIdle = "fighting_idle";
    public string punchLeft = "punch_left";
    public string punchRight = "punch_right";
    public string pay = "pay";
    public string grabHigh = "grab_high";
    public string grabMid = "grab_mid";
    public string grabLow = "grab_low";
    public string indleHands = "Idle_down";

    public string layer = "UpperBody.";

}
