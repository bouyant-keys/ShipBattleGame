using System;
using System.Collections;
using UnityEngine;

public class BreakableObj : MonoBehaviour
{
    public int _hitPoints = 2;
    float _animOffset;
    [SerializeField] ParticleSystem[] _breakParticles;
    [SerializeField] Animation _anim;

    private void Awake() 
    {
        _breakParticles = GetComponentsInChildren<ParticleSystem>();
        _animOffset = UnityEngine.Random.Range(0f, 2f);

        StartCoroutine(StartAnimation());
    }

    private IEnumerator StartAnimation()
    {
        yield return new WaitForSeconds(_animOffset);
        _anim.Play();
    }

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
        foreach (ParticleSystem particles in _breakParticles)
        {
            particles.Play();
        }

        StartCoroutine(Deactivate());
    }

    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
