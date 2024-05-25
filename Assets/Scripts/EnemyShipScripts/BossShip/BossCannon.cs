using System;
using UnityEngine;

public class BossCannon : MonoBehaviour
{
    [SerializeField] Transform _turretPivot; //May be useless
    [SerializeField] Transform _projectileSpawn;
    [SerializeField] Transform _mineSpawn;
    public int _maxBulletsFired;

    BossPool _bossPool;
    Transform _player;
    Quaternion _rotationToTarget;
    Vector3 _direction;
    bool _alreadyAttacked;
    int _bulletsFired;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _bossPool = GameObject.FindGameObjectWithTag("BossPool").GetComponent<BossPool>();
    }
    
    private void Update()
    {
        _direction = new Vector3(_player.position.x - transform.position.x, 0f, _player.position.z - transform.position.z);
        _rotationToTarget = Quaternion.LookRotation(_direction);
        
        //transform.position = _turretPivot.position;
        transform.rotation = _rotationToTarget;
    }

    public void CloseAttack()
    {
        //Fires in the transform.forward direction of the _projectileSpawn
        if (_bulletsFired < _maxBulletsFired && !_alreadyAttacked)
        {
            _alreadyAttacked = true;
            //Allows the tank to fire a spread of bullets before having a longer cooldown
            if (_bulletsFired == _maxBulletsFired - 1) Invoke(nameof(ResetAttack), BossBrain._bossShip._fireCooldown);
            else Invoke(nameof(ResetAttack), BossBrain._bossShip._fireDelay);

            _bossPool.GetBulletFromPool(_projectileSpawn, BossBrain._bossShip);
            _bulletsFired++;
        }
    }

    
    public void RangedAttack()
    {
        float temp = UnityEngine.Random.value;
        if (temp > 0.5f) LaunchSuperMine();
        else LaunchBulletRing();
    }

    private void LaunchSuperMine()
    {
        throw new NotImplementedException();
    }

    private void LaunchBulletRing()
    {
        throw new NotImplementedException();
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
            _bossPool.GetMineFromPool(_mineSpawn.position);
        }
    }
}
