using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody _rB;
    public SphereCollider _collider;
    public Pool _pool;
    public AudioSource _audioSource;
    public int _maxBounces = 0;

    [SerializeField] MeshRenderer _bulletVis;
    [SerializeField] ParticleSystem _bulletPop;
    [SerializeField] ParticleSystem _bulletFX;
    [SerializeField] GameObject _fastBulletTrail;

    Vector3 _direction;
    Vector3 _lastVelocity;
    [SerializeField] int _bounceCount = 0;
    float _currentSpeed;

    public void Launch(float speedMultiplier, Vector3 dir)
    {
        _bounceCount = 0;

        _bulletVis.enabled = true;
        _collider.enabled = true;
        _bulletFX.Play();

        _rB.velocity = dir * speedMultiplier * 10f;
        _bulletVis.transform.forward = dir;

        if (speedMultiplier >= 1.5f) //Make it so the fast bullet sounds work
        {
            AudioManager.instance.Play(AudioManager.instance._fastBulletFire);
            _audioSource.Play();
            _fastBulletTrail.SetActive(true);
        }
        else
        {
            AudioManager.instance.Play(AudioManager.instance._tankFire);
            _fastBulletTrail.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (!GameManager._globalInputEnabled)
        {
            _rB.velocity = Vector3.zero;
        }
        _lastVelocity = _rB.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //print($"obj: {collision.gameObject}, bounces: {_bounceCount}");
        switch (collision.gameObject.tag)
        {
            case "Projectile":
                BulletPop();
                break;
            case "Player":
                ReturnToPool();
                break;
            case "Enemy":
                ReturnToPool();
                break;
            default:
                CheckBounce(collision);
                break;
        }

    }

    void CheckBounce(Collision collision)
    {
        if (_bounceCount >= _maxBounces)
        {
            BulletPop();
            return;
        }
        _currentSpeed = _lastVelocity.magnitude;

        AudioManager.instance.Play(AudioManager.instance._bulletBounce);

        _direction = Vector3.Reflect(_lastVelocity.normalized, collision.GetContact(0).normal);
        _rB.velocity = _direction * _currentSpeed; //Mathf.Max(_currentSpeed, 0);
        _bulletVis.transform.forward = _direction;

        _bounceCount++;
    }


    private void BulletPop()
    {
        ResetBullet();

        AudioManager.instance.Play(AudioManager.instance._bulletPop);
        _bulletPop.Play();

        Invoke("ReturnToPool", _bulletPop.main.duration);
    }

    private void ResetBullet()
    {
        _bulletFX.Stop();
        _fastBulletTrail.SetActive(false);
        _bulletVis.enabled = false;

        _rB.velocity = Vector3.zero;
        _collider.enabled = false;

        _audioSource.Stop();
        _audioSource.time = 0;
    }

    private void ReturnToPool()
    {
        _pool.ReturnObjToPool(this.gameObject);
    }
}
