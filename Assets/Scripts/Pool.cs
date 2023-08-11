using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    //Consider making lists into arrays, requires sizable refactoring
    public List<GameObject> _bulletPool = new List<GameObject>();
    public GameObject _bulletPrefab;

    public List<GameObject> _minePool = new List<GameObject>();
    public GameObject _minePrefab;


    public virtual void GetBulletFromPool(Transform trans, TankTypes tank)
    {
        if (_bulletPool.Count <= 0) return;
        Projectile _projScriptInst;

        GameObject projectile = _bulletPool[_bulletPool.Count - 1];
        projectile.transform.position = trans.position;
        projectile.transform.rotation = Quaternion.identity;
        projectile.gameObject.SetActive(true);

        _projScriptInst = projectile.GetComponent<Projectile>(); //Use to alter projectile based on Tank Type Obj
        _projScriptInst._maxBounces = tank._riccochets;
        _projScriptInst.Launch(tank._bulletSpeed, trans.forward);

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
