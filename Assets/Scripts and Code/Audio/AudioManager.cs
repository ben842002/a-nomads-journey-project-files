using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] sounds;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        //else
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        //DontDestroyOnLoad(gameObject);

        // loop through sounds array to add AudioSource so we can actually play the audio
        for (int i = 0; i < sounds.Length; i++)
        {
            // add the AudioSource and cache it in a variable (caching for simplicity and understanding)
            sounds[i].source = gameObject.AddComponent<AudioSource>();
            AudioSource source = sounds[i].source;

            // make the AudioSource's attributes equal to the values given in that specific sound element
            // (attributes are default values when creating an AudioSource)
            source.clip = sounds[i].clip;
            source.outputAudioMixerGroup = sounds[i].audioMixerGroup;
            source.volume = sounds[i].volume;
            source.pitch = sounds[i].pitch;
            source.loop = sounds[i].loop;
        }
    }

    private void Start()
    {   
        if (SceneManager.GetActiveScene().buildIndex == 0)
            Play("Menu");
    }

    public void Play(string soundName)
    {   
        // NEW SYNTAX (Alternative to for-looping because it makes coding faster. However, generally use for loops for coding in general)
        // In the sounds array (1st parameter), find the sound where sounds.name is equal to the parameter string Soundname (2nd param)
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogError("Sound: " + soundName + " was not found!");
            return;
        }
        s.source.Play();
    }

    public void Stop(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogError("Sound: " + soundName + " was not found!");
            return;
        }
        s.source.Stop();
    }
}
