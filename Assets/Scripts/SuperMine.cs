using System.Collections;
using UnityEngine;

public class SuperMine : MonoBehaviour
{
    BossPool _pool;
    [SerializeField] Animator _animator;
    [SerializeField] ParticleSystem _explosion;
    [SerializeField] float _blastRadius = 5f;


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
                case "Player":
                    hitCollider.GetComponent<PlayerController>().PlayerHit();
                    break;
                case "Breakable":
                    print($"Mine destroyed obj: {hitCollider.name}");
                    hitCollider.GetComponent<BreakableObj>().Break();
                    break;
            }
        }

        _pool.ReturnObjToPool(gameObject); 
    }

    private void Beep()
    {
        AudioManager.instance.Play(AudioManager.instance._mineBeep);
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.tag == "Projectile" || trigger.gameObject.tag == "Player") Explode();
    }

    private void OnDrawGizmosSelected()
    {
        //Blast Radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _blastRadius);
    }
}
