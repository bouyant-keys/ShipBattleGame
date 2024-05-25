using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource _audioSource;
    public AudioClip[] _levelMusic = new AudioClip[4]; //0 - Round Start, 1 - Intro, 2 - Round End, 3 - Round Failed

    public AudioClip _startClip;
    public AudioClip _loopClip;

    bool _isLevelMusicPlaying = false;

    public void PlayStartMusic()
    {
        _audioSource.clip = _startClip;
        if (!_loopClip) 
        {
            _audioSource.loop = true;
            _audioSource.Play();
        }
        else 
        {
            _audioSource.Play();
            StartCoroutine(ChangeMusicToLoop());
        }
    }

    IEnumerator ChangeMusicToLoop()
    {
        yield return new WaitForSeconds(_startClip.length - 0.1f);
        _audioSource.clip = _loopClip;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    public void StartLevelMusic(int index)
    {
        _audioSource.clip = _levelMusic[index];
        _audioSource.loop = false;
        _audioSource.Play();
    }

    public void EndLevelMusic(bool win)
    {
        if (_isLevelMusicPlaying) return;

        _audioSource.Stop();
        _audioSource.loop = false;
        _audioSource.time = 0f;

        if (win) _audioSource.clip = _levelMusic[2]; //Round End Music
        else _audioSource.clip = _levelMusic[3]; //Round Failed Music
        _audioSource.Play();

        _isLevelMusicPlaying = true;
    }
}
