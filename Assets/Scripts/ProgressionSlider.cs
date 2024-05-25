using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionSlider : MonoBehaviour
{
    Slider _slider;

    void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.value = SceneLoader._currentLevelIndex;
    }
}
