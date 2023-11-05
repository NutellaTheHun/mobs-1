using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStep : MonoBehaviour
{
    AudioSource m_AudioSource;
    [SerializeField] private List<AudioClip> FootSteps;
    
    public float minPitch;
    public float maxPitch;
    public float minVolume;
    public float maxVolume;

    private bool isPlaying = false;
    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayFootStep()
    {
       if(!isPlaying)
       {
            isPlaying = true;
            m_AudioSource.clip = FootSteps[Random.Range(0, 4)];
            m_AudioSource.pitch = Random.Range(minPitch, maxPitch);
            m_AudioSource.volume = Random.Range(minVolume, maxVolume);
            StartCoroutine(PlayFootStepSound()); 
        }
       
    }

    IEnumerator PlayFootStepSound()
    {
        m_AudioSource.Play();
        yield return new WaitForSeconds(0.5f);
        isPlaying = false;
    }
}
