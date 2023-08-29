using System.Collections;
using UnityEngine;

public class StationaryEnemyCannon : MonoBehaviour
{
    [SerializeField] Transform _turretPivot; //May be useless
    [SerializeField] Transform _projectileSpawn;
    public int _maxBulletsFired;
    public bool _idling = true;
    public bool _attacking = false;

    public TankTypes _tank;
    EnemyPool _enemyPool;
    Transform _player;
    //Quaternion _rotationToTarget;
    //Vector3 _direction;
    bool _rotatingPositive = true;
    [SerializeField] float _lookRotateAngle = 65f;
    [SerializeField] float _rotationOffset = 0f;

    float _percentElapsed;
    float _currentFrame = 0f;
    [SerializeField] float _totalFrames = 30f;
    [SerializeField] float _pauseTime = 1f;


    bool _alreadyAttacked;
    int _bulletsFired;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _enemyPool = GameObject.FindGameObjectWithTag("EnemyPool").GetComponent<EnemyPool>();
    }
    
    private void Update()
    {
        if (!GameManager._globalInputEnabled) return;

        if (_idling) IdleLooking();
        else if (_attacking) 
        {
            FacePlayer();
            Fire();
        }
    }

    private void IdleLooking()
    {
        //Rotate Cannon back and forth around y axis
        if (_currentFrame > _totalFrames)
        {
            _currentFrame = 0;
            StartCoroutine(PauseLook());
            return;
        } 

        _percentElapsed = _currentFrame / _totalFrames;

        float tempYRot = transform.rotation.y; //If something goes wrong, no rotation will occur

        if (_rotatingPositive) tempYRot = Mathf.Lerp(-_lookRotateAngle, _lookRotateAngle, _percentElapsed);
        else tempYRot = Mathf.Lerp(_lookRotateAngle, -_lookRotateAngle, _percentElapsed);
        
        transform.localEulerAngles = new Vector3(transform.rotation.x, tempYRot + _rotationOffset, transform.rotation.z);
        _currentFrame++;
    }

    IEnumerator PauseLook()
    {
        //print("Waiting between turns");
        _idling = false;
        yield return new WaitForSeconds(_pauseTime);
        _rotatingPositive = !_rotatingPositive;
        _idling = true;
    }

    private void FacePlayer()
    {
        Vector3 _direction = new Vector3(_player.position.x - transform.position.x, 0f, _player.position.z - transform.position.z);
        Quaternion _rotationToTarget = Quaternion.LookRotation(_direction);
        
        transform.rotation = _rotationToTarget;
    }

    public void Fire()
    {
        print("firing");
        //Fires in the transform.forward direction of the _projectileSpawn
        if (_bulletsFired < _maxBulletsFired && !_alreadyAttacked)
        {
            _alreadyAttacked = true;
            //Allows the tank to fire a spread of bullets before having a longer cooldown
            if (_bulletsFired == _maxBulletsFired - 1) Invoke(nameof(ResetAttack), _tank._fireCooldown);
            else Invoke(nameof(ResetAttack), _tank._fireDelay);

            _enemyPool.GetBulletFromPool(_projectileSpawn, _tank);
            _bulletsFired++;
        }
    }
    private void ResetAttack()
    {
        if (_bulletsFired == _maxBulletsFired)
        {
            _bulletsFired = 0;
        }
        _alreadyAttacked = false;
    }
}
