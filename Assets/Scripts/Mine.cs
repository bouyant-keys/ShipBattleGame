using System;
using System.Collections;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] ParticleSystem _explosion;
    [SerializeField] float _blastRadius = 2f;

    public Pool _minePool;
    public bool isEnemyMine = false; //Only needs to be set by enemypool

    public void LayMine()
    {
        //Play layMine Sound
        StartCoroutine(StartExplosionTimer());
        gameObject.SetActive(true);
    }

    IEnumerator StartExplosionTimer()
    {
        //Total time for mine to explode: 10s, blinking starts after 6s
        yield return new WaitForSeconds(5f);
        _animator.SetBool("_blink", true);
        yield return new WaitForSeconds(3f);
        _animator.SetBool("_rapidBlink", true);
        yield return new WaitForSeconds(2f);
        Explode();
    }

    private void Explode()
    {
        StopCoroutine(StartExplosionTimer());

        AudioManager.instance.Play(AudioManager.instance._tankExplosion);
        _explosion.Play();

        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, _blastRadius);
        foreach (var hitCollider in hitColliders)
        {
            switch (hitCollider.tag)
            {
                case "Enemy":
                    hitCollider.GetComponent<IEnemyTank>().EnemyHit();
                    break;
                case "Player":
                    hitCollider.GetComponent<PlayerController>().PlayerHit();
                    break;
                case "Breakable":
                    print($"Mine destroyed obj: {hitCollider.name}");
                    hitCollider.GetComponent<BreakableObj>().Break();
                    break;
            }
        }

        _minePool.ReturnObjToPool(gameObject); 
    }

    private void Beep()
    {
        AudioManager.instance.Play(AudioManager.instance._mineBeep);
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.tag == "Projectile") Explode();

        if (trigger.gameObject.tag == "Player" && isEnemyMine) Explode();
        else if (trigger.gameObject.tag == "Enemy" && !isEnemyMine) Explode();
    }

    private void OnDrawGizmosSelected()
    {
        //Blast Radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _blastRadius);
    }
}
