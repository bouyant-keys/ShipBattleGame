using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip _bulletBounce;
    public AudioClip _bulletPop;
    public AudioClip _tankExplosion;
    public AudioClip _tankFire;
    public AudioClip _misfire;
    public AudioClip _fastBulletFire;
    public AudioClip _mineBeep;

    AudioSource _audioSource;

    //Singleton pattern
    public static AudioManager instance;
    private void Awake() 
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        _audioSource = this.GetComponent<AudioSource>();
    }

    public void Play(AudioClip audio)
    {
        _audioSource.PlayOneShot(audio);
    }
}
