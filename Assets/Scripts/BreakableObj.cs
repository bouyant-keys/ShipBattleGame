using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObj : MonoBehaviour
{
    public int _hitPoints = 2;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            _hitPoints--;

            if (_hitPoints == 0)
            {
                Break();
            }
        }
    }

    public void Break()
    {
        //Play Breaking Particle Effect
        gameObject.SetActive(false);
    }
}
