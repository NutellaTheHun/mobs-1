using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Velocity tracking portions from youtube channel Andrew, https://www.youtube.com/watch?v=i6lltmrE9V8

public class PunchComponent : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float punchSpeedMin;
    public enum Hand{ Left, Right };

    [SerializeField] Hand selection;

    [SerializeField] HumanComponent humanComponent;

    private bool isPunching = false;
    public AudioClip hitSound;
    AudioSource m_AudioSource;
    public float minPitch;
    public float maxPitch;
    public float minVolume;
    public float maxVolume;

    bool isFist = false;
    bool isFast = false;
    bool hasPunched = false;

    public InputActionProperty FistVelocityProperty;

    public Vector3 FistVelocity { get; private set; } = Vector3.zero;

    void Start()
    {
        m_AudioSource = GetComponentInParent<AudioSource>();
        m_AudioSource.clip = hitSound;
    }

    // Update is called once per frame
    void Update()
    {
        switch(selection)
        {
            case Hand.Left:
                if (animator.GetFloat("Left Grab") == 1) { isFist = true; /*Debug.Log("LEFT IS FIST");*/ }
                else isFist = false;
                FistVelocity = FistVelocityProperty.action.ReadValue<Vector3>();
                //Debug.Log("LEFT VELOCITY: " + FistVelocity.magnitude);
                break;
            case Hand.Right:
                if (animator.GetFloat("Right Grab") == 1) { isFist = true; /*Debug.Log("Right IS FIST");*/ }
                else isFist = false;
                FistVelocity = FistVelocityProperty.action.ReadValue<Vector3>();
                //Debug.Log("Right VELOCITY: " + FistVelocity.magnitude);
                break;

            default:
                Debug.Log("PunchComp Enum Error, went to default");
                break;
        }
     
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFist && FistVelocity.magnitude >= punchSpeedMin)
        {
            if (other.CompareTag("Player") && !hasPunched)
            {
                //Debug.Log("PUNCH");
                PunchSound();
                humanComponent.setIsPunch(true); //not needed probably
                humanComponent.PunchDamage(0.06f);
                hasPunched = true;
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (hasPunched) { hasPunched = false; }
        humanComponent.setIsPunch(false);

    }

    public void PunchSound()
    {
        if (!isPunching)
        {
            isPunching = true;
            m_AudioSource.pitch = Random.Range(minPitch, maxPitch);
            m_AudioSource.volume = Random.Range(minVolume, maxVolume);
            StartCoroutine(PlayPunchSound());

        }

    }

    IEnumerator PlayPunchSound()
    {
        m_AudioSource.Play();
        yield return new WaitForSeconds(0.2f);
        isPunching = false;
    }

}
