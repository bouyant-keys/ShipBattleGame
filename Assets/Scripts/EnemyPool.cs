using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour, Pool
{
    public List<GameObject> _bulletPool = new List<GameObject>();
    public GameObject _bulletPrefab;

    public List<GameObject> _minePool = new List<GameObject>();
    public GameObject _minePrefab;

    public int _bulletPoolSize = 5; //Pool size should change per level & number of tanks
    public int _minePoolSize = 4; //Mine pool should be max 5-6 for multiple tanks and 4 for one tank

    private void Start()
    {
        //Generating bulletPool
        for (int i = 0; i < _bulletPoolSize; i++)
        {
            _bulletPool.Add(Instantiate(_bulletPrefab));

            _bulletPool[i].GetComponent<Projectile>()._pool = this;
            _bulletPool[i].layer = LayerMask.NameToLayer("EnemyBullet");

            _bulletPool[i].SetActive(false);
        }

        //Generating minePool
        for (int i = 0; i < _minePoolSize; i++)
        {
            _minePool.Add(Instantiate(_minePrefab));
            Mine mineInfo = _minePool[i].GetComponent<Mine>();
            mineInfo._minePool = this;
            mineInfo.isEnemyMine = true;
            _minePool[i].SetActive(false);
        }
    }
    
    public virtual void GetBulletFromPool(Transform trans, ShipTypes tank)
    {
        if (_bulletPool.Count <= 0) return;
        Projectile _projScript;

        GameObject projectile = _bulletPool[_bulletPool.Count - 1];
        projectile.transform.position = trans.position;
        projectile.transform.rotation = Quaternion.identity;
        projectile.gameObject.SetActive(true);

        _projScript = projectile.GetComponent<Projectile>(); //Use to alter projectile based on Tank Type Obj
        _projScript._maxBounces = tank._riccochets;
        _projScript.Launch(tank._bulletSpeed, trans.forward);

        _bulletPool.RemoveAt(_bulletPool.Count - 1);
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
    }

    public void ReturnObjToPool(GameObject obj)
    {
        obj.SetActive(false);

        if (obj.GetComponent<Projectile>()) _bulletPool.Add(obj);
        else if (obj.GetComponent<Mine>()) _minePool.Add(obj);
        else print("GameObject doesn't belong to a pool and cannot be returned one.");
    }
}
