using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [SerializeField] private AudioSource _audioSource;

    private float _volumeValue = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    public void PlaySFXClip(AudioClip audioClip)
    {
        _audioSource.clip = audioClip;

        _audioSource.volume = _volumeValue;

        _audioSource.Play();

        float clipLength = _audioSource.clip.length;
    }
}
