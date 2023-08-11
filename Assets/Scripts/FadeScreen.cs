using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    public Animation _animation;

    private void Awake()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    public void FadeIn()
    {
        _animation.Play("FadeIn");
    }

    public void FadeOut()
    {
        _animation.Play("FadeOut");
    }

    public void SetScreenActive()
    {
        gameObject.SetActive(true);
    }

    public void SetScreenInactive()
    {
        gameObject.SetActive(false);
    }
}
