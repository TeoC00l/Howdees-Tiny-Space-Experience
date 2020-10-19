//@Author: Teodor Tysklind / FutureGames / Teodor.Tysklind@FutureGames.nu

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-20)]

public class SoundManager : MonoBehaviour
{
    private Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();

    private GameObject soundObject;
    private Transform mainCameraTransform;

    [SerializeField] private Sound[] soundList;
    [SerializeField] private AudioMixer mixer;
    
    public static SoundManager Instance;

    [Serializable]
    public struct Sound
    {
        public string audioName;
        public AudioClip audioClip;
        [Range(0f, 1f)] public float volume;
        public AudioMixerGroup mixerGroup;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            soundObject = CreateSoundObject();
            Debug.Log(soundObject.GetComponent<AudioSource>().volume);
            PoolManager.Instance.CreatePool(soundObject);
        }

        for (int i = 0; i < soundList.Length; i++)
        {
            sounds.Add(soundList[i].audioName, soundList[i]);
        }

        mainCameraTransform = Camera.main.transform;
    }

    public GameObject PlaySound(string soundName, Vector3 position)
    {
        GameObject audioObject = PoolManager.Instance.ReuseObject(soundObject, position, Quaternion.identity);
        AudioSource audioSource = audioObject.GetComponent<AudioSource>();
        
        audioSource.clip = sounds[soundName].audioClip;
        audioSource.volume = sounds[soundName].volume;
        audioSource.outputAudioMixerGroup = sounds[soundName].mixerGroup;

        audioSource.loop = false;
        audioSource.Play();
        
        return audioObject;
    }
    
    public GameObject PlayLoopingSoundAtCamera(string soundName, bool playInstantly = true)
    {
        GameObject loopingSoundObject = CreateSoundObject();

        loopingSoundObject.transform.position = mainCameraTransform.position;
        loopingSoundObject.transform.SetParent(mainCameraTransform);

        AudioSource audioSource = loopingSoundObject.GetComponent<AudioSource>();
        
        audioSource.clip = sounds[soundName].audioClip;
        audioSource.volume = sounds[soundName].volume;
        audioSource.outputAudioMixerGroup = sounds[soundName].mixerGroup;
        
        audioSource.loop = true;
        audioSource.Play();

        return loopingSoundObject;
    }
    
    private GameObject CreateSoundObject()
    {
        GameObject newSoundObject = Instantiate(new GameObject());
        newSoundObject.AddComponent<AudioSource>();

        return newSoundObject;
    }

    public void AdjustGlobalSoundVolume(float volumeInPercent)
    {
        float minValue = -80;
        float maxValue = 20;

        float mixerVolume = Mathf.Lerp(minValue, maxValue, volumeInPercent);
        mixer.SetFloat("MasterVolume", mixerVolume);
    }
}
