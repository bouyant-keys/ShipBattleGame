using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource _audioSource;
    public AudioClip[] _levelMusic = new AudioClip[4]; //0 - Round Start, 1 - Intro, 2 - Round End, 3 - Round Failed

    List<AudioClip[]> _tankSongArrays;
    public AudioClip[] _brownTank = new AudioClip[1];
    public AudioClip[] _greyTank = new AudioClip[2];
    public AudioClip[] _tealTank = new AudioClip[2];
    public AudioClip[] _yellowTank = new AudioClip[3];
    public AudioClip[] _pinkTank = new AudioClip[3];

    bool _isLevelMusicPlaying = false;

    private void Awake()
    {
        _tankSongArrays = new List<AudioClip[]> {_brownTank, _greyTank, _tealTank, _yellowTank, _pinkTank};
    }

    public void StartMusic(int enemyLevel , int enemyNum) //Array is 5 by 3
    {
        //For indexing
        if (enemyLevel > _tankSongArrays.Count) enemyLevel = _tankSongArrays.Count;
        enemyLevel--;

        if (enemyNum > _tankSongArrays[enemyLevel].Length) enemyNum = _tankSongArrays[enemyLevel].Length;
        enemyNum--;

        _audioSource.clip = _tankSongArrays[enemyLevel][enemyNum];
        _audioSource.time = 0.3f;
        _audioSource.Play();
    }

    public void UpdateMusic(int enemyLevel, int enemyNum)
    {
        if (_isLevelMusicPlaying) return;

        //For indexing
        if (enemyLevel > _tankSongArrays.Count) enemyLevel = _tankSongArrays.Count;
        enemyLevel--;

        if (enemyNum > _tankSongArrays[enemyLevel].Length) enemyNum = _tankSongArrays[enemyLevel].Length;
        enemyNum--;
        

        float musicOffset = _audioSource.time;
        _audioSource.Stop(); //maybe change idk 
        _audioSource.clip = _tankSongArrays[enemyLevel][enemyNum];
        _audioSource.time = musicOffset;
        _audioSource.Play();
    }

    public void StartLevelMusic(int index)
    {
        _audioSource.clip = _levelMusic[index];
        _audioSource.Play();
    }

    public void EndLevelMusic(bool win)
    {
        if (_isLevelMusicPlaying) return;

        _audioSource.Stop();
        _audioSource.time = 0f;

        if (win) _audioSource.clip = _levelMusic[2]; //Round End Music
        else _audioSource.clip = _levelMusic[3]; //Round Failed Music
        _audioSource.Play();

        _isLevelMusicPlaying = true;
    }
}
