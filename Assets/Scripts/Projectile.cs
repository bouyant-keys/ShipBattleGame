using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody _rB;
    public SphereCollider _collider;
    public Pool _pool;
    public AudioManager _audioManager;
    public AudioSource _audioSource;
    public GameObject[] _trails = new GameObject[2]; //0 is normal speed trail, 1 is fast trail
    public ParticleSystem _bulletPop; //0 is smoke pop
    public MeshRenderer _bulletVis;
    public float _startTime;
    public int _maxBounces = 2;

    Vector3 _direction;
    Vector3 _lastVelocity;
    Vector3 _collisionPos;
    [SerializeField] int _bounceCount = 0;
    float _currentSpeed;

    private void Awake()
    {
        _audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    public void Launch(float speedMultiplier, Vector3 dir)
    {
        _startTime = Time.time;
        _bulletVis.enabled = true;
        _collider.enabled = true;

        _rB.velocity = (dir * speedMultiplier * 10f);
        _bulletVis.transform.forward = dir;

        if (speedMultiplier >= 1.5f) //Make it so the fast bullet sounds work
        {
            _audioManager.Play(_audioManager._fastBulletFire);
            _audioSource.Play();
            _trails[1].SetActive(true);
        }
        else
        {
            _audioManager.Play(_audioManager._tankFire);
            _trails[0].SetActive(true);
        }
    }

    private void LateUpdate()
    {
        if (!GameManager._inputEnabled)
        {
            _rB.velocity = Vector3.zero;
        }
        _lastVelocity = _rB.velocity;
    }

    /**
     * 
     *  TODO: Projectiles keep colliding where bulletpops occur, maybe colliding with 
     * 
     * */


    private void OnCollisionEnter(Collision collision)
    {
        _currentSpeed = _lastVelocity.magnitude;
        _collisionPos = collision.GetContact(0).point;

        switch (collision.gameObject.tag)
        {
            case "Projectile":
                print("Projectile Collision");
                float collisionLife = collision.gameObject.GetComponent<Projectile>()._startTime;
                if (_startTime < collisionLife)
                {
                    BulletPop();
                    break;
                }
                ReturnToPool();
                break;
            case "Player":
                ReturnToPool();
                break;
            case "Enemy":
                ReturnToPool();
                break;
        }

        if (_bounceCount < _maxBounces) //Bounce off wall
        {
            _audioManager.Play(_audioManager._bulletBounce);

            _direction = Vector3.Reflect(_lastVelocity.normalized, collision.GetContact(0).normal);
            _rB.velocity = _direction * Mathf.Max(_currentSpeed, 0);
            _bulletVis.transform.forward = _direction;

            _bounceCount++;
        }
        else //Bullet pops with effect and returns to pool
        {
            BulletPop();
        }
    }

    private void BulletPop()
    {
        ResetBullet();
        
        _audioManager.Play(_audioManager._bulletPop);
        _bulletPop.Play();

        Invoke("ReturnToPool", _bulletPop.main.duration);
    }

    private void ResetBullet()
    {
        _trails[0].SetActive(false);
        _trails[1].SetActive(false);
        _bulletVis.enabled = false;

        _bounceCount = 0;
        _rB.velocity = Vector3.zero;
        _rB.rotation = Quaternion.identity;
        _collider.enabled = false;
        transform.position = _collisionPos;

        _audioSource.Stop();
        _audioSource.time = 0;
    }

    private void ReturnToPool()
    {
        _pool.ReturnObjToPool(this.gameObject);
    }
}
