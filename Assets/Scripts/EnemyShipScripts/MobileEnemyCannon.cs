using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileEnemyCannon : MonoBehaviour
{
    [SerializeField] Transform _turretPivot; //May be useless
    [SerializeField] Transform _projectileSpawn;
    [SerializeField] Transform _mineSpawn;
    public int _maxBulletsFired;

    EnemyPool _enemyPool;
    Transform _player;
    Quaternion _rotationToTarget;
    Vector3 _direction;
    bool _alreadyAttacked;
    int _bulletsFired;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _enemyPool = GameObject.FindGameObjectWithTag("EnemyPool").GetComponent<EnemyPool>();
    }
    
    private void Update()
    {
        _direction = new Vector3(_player.position.x - transform.position.x, 0f, _player.position.z - transform.position.z);
        _rotationToTarget = Quaternion.LookRotation(_direction);
        
        //transform.position = _turretPivot.position;
        transform.rotation = _rotationToTarget;
    }

    public void Fire(ShipTypes ship)
    {
        //Fires in the transform.forward direction of the _projectileSpawn
        if (_bulletsFired < _maxBulletsFired && !_alreadyAttacked)
        {
            _alreadyAttacked = true;
            //Allows the tank to fire a spread of bullets before having a longer cooldown
            if (_bulletsFired == _maxBulletsFired - 1) Invoke(nameof(ResetAttack), ship._fireCooldown);
            else Invoke(nameof(ResetAttack), ship._fireDelay);

            _enemyPool.GetBulletFromPool(_projectileSpawn, ship);
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

    public void PlaceMine()
    {
        float num = UnityEngine.Random.value;
        if (num < 0.3)
        {
            _enemyPool.GetMineFromPool(_mineSpawn.position);
        }
    }
}
