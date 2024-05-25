using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    public Animation _animation;

    public void FadeIn()
    {
        _animation.gameObject.SetActive(true);
        _animation.Play("FadeIn");
        StartCoroutine(DisableScreen());
    }

    IEnumerator DisableScreen()
    {
        yield return new WaitForSeconds(_animation.GetClip("FadeIn").length - 0.05f);
        _animation.gameObject.SetActive(false);
    }

    public void FadeOut()
    {
        _animation.gameObject.SetActive(true);
        _animation.Play("FadeOut");
    }
}
