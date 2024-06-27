using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixer : MonoBehaviour
{

    [SerializeField] private UnityEngine.Audio.AudioMixer audioMixer;




    private void Start()
    {
        //default volume
        SetMusicVolumeSLIDER(0.5f);
        SetSoundFXVolumeSLIDER(0.2f);

    }

    public void SetMasterVolumeSLIDER(float level)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
    }

    public void SetSoundFXVolumeSLIDER(float level)
    {
        audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(level) * 20f);
    }
    public void SetMusicVolumeSLIDER(float level)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
    }


}
