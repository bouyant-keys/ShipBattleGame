using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPool : MonoBehaviour, Pool
{
    AmmoTrackerUI _ammoTracker;

    public List<GameObject> _bulletPool = new List<GameObject>();
    public List<GameObject> _minePool = new List<GameObject>();

    [SerializeField] ShipTypes _playerTank;
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] GameObject _minePrefab;
    
    public int _bulletPoolSize = 5;
    public int _minePoolSize = 2;

    void Start()
    {
        _ammoTracker = GameObject.FindObjectOfType<AmmoTrackerUI>(true);

        //Generating bulletPool
        for (int i = 0; i < _bulletPoolSize; i++)
        {
            _bulletPool.Add(Instantiate(_bulletPrefab));

            Projectile bulletProperties = _bulletPool[i].GetComponent<Projectile>();
            bulletProperties._pool = this;
            bulletProperties._maxBounces = _playerTank._riccochets;
            _bulletPool[i].layer = LayerMask.NameToLayer("PlayerBullet");

            _bulletPool[i].SetActive(false);
        }

        //Generating minePool
        for (int i = 0; i < _minePoolSize; i++)
        {
            _minePool.Add(Instantiate(_minePrefab));
            _minePool[i].GetComponent<Mine>()._minePool = this;
            _minePool[i].SetActive(false);
        }
    }

    public virtual void GetBulletFromPool(Transform trans, ShipTypes tank)
    {
        if (_bulletPool.Count <= 0) return;

        GameObject projectile = _bulletPool[_bulletPool.Count - 1];
        projectile.transform.position = trans.position;
        projectile.transform.rotation = Quaternion.identity;
        projectile.gameObject.SetActive(true);

        projectile.GetComponent<Projectile>().Launch(_playerTank._bulletSpeed, trans.forward);

        _bulletPool.RemoveAt(_bulletPool.Count - 1);
        _ammoTracker.UpdateBullets(_bulletPool.Count);
    }

    public virtual void GetMineFromPool(Vector3 pos)
    {
        if (_minePool.Count == 0) return;

        GameObject mine = _minePool[_minePool.Count - 1];
        mine.transform.position = pos;
        mine.transform.rotation = Quaternion.identity; //Might be unnecessary
        mine.gameObject.SetActive(true);

        mine.GetComponent<Mine>().LayMine();
        _minePool.RemoveAt(_minePool.Count - 1);
        _ammoTracker.UpdateMines(_minePool.Count);
    }

    public void ReturnObjToPool(GameObject obj)
    {
        obj.SetActive(false);

        if (obj.GetComponent<Projectile>())
        {
            _bulletPool.Add(obj);
            //print("Updating bullets");
            _ammoTracker.UpdateBullets(_bulletPool.Count);
        } 
        else if (obj.GetComponent<Mine>())
        {
            _minePool.Add(obj);
            //print("Updating mines");
            _ammoTracker.UpdateMines(_minePool.Count);
        }
        else print("GameObject doesn't belong to a pool and cannot be returned one.");
    }
}

