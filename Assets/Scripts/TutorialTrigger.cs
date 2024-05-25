using UnityEngine;
using UnityEngine.Events;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent _triggerEvent;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Player")
        {
            _triggerEvent.Invoke();
        }
    }
}   
