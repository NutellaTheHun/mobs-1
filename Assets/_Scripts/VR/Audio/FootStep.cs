using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStep : MonoBehaviour
{
    AudioSource m_AudioSource;
    [SerializeField] private AudioClip HardFootStep;
    [SerializeField] private AudioClip SoftFootStep;
    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.clip = HardFootStep;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayFootStep()
    {
        m_AudioSource.Play();
    }
}
