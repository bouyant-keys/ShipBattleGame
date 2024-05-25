using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] GameObject _movingTutorial;
    [SerializeField] GameObject _firingTutorial;

    public void ActivateFiringTutorial()
    {
        _movingTutorial.SetActive(false);
        _firingTutorial.SetActive(true);
    }

    public void ActivateMovingTutorial()
    {
        _movingTutorial.SetActive(true);
        _firingTutorial.SetActive(false);
    }
}
