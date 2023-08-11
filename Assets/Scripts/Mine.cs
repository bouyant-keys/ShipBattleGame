using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public Animator _animator;
    public Pool _minePool;
    public float _blastRadius = 2f;
    public bool isEnemyMine;

    public void LayMine()
    {
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
        //Play Explosion Effect
        StopCoroutine(StartExplosionTimer());

        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, _blastRadius);
        foreach (var hitCollider in hitColliders)
        {
            switch (hitCollider.tag)
            {
                case "Enemy":
                    hitCollider.GetComponent<EnemyController>().EnemyHit();
                    break;
                case "Player":
                    hitCollider.GetComponent<PlayerController>().PlayerHit();
                    break;
                case "Breakable Obj":
                    hitCollider.GetComponent<BreakableObj>().Break();
                    break;
            }
        }

        _minePool.ReturnObjToPool(gameObject); 
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.tag == "Projectile") Explode();

        if (trigger.gameObject.tag == "Player" && _minePool.name == "EnemyPool") Explode();
        else if (trigger.gameObject.tag == "Enemy" && _minePool.name == "PlayerPool") Explode();
    }

    private void OnDrawGizmosSelected()
    {
        //Blast Radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _blastRadius);
    }
}
