using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class AudioManagerScript : MonoBehaviour
{

    [Header("---------- Audio Source ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;



    [Header("---------- Audio clip ----------")]
    [SerializeField] public AudioClip clickSound;
    [SerializeField] public AudioClip BoonScreenOpen;
    [SerializeField] public AudioClip BackgroundMusic;
    [SerializeField] public AudioClip ResourceCreate;
    [SerializeField] public AudioClip ResourceGain;
    [SerializeField] public AudioClip Build;



    public static AudioManagerScript instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        
        musicSource.clip = BackgroundMusic;
        musicSource.Play();

    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
