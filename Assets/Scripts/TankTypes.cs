using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tank", menuName = "Tank")]
public class TankTypes : ScriptableObject
{
    public string _name;
    public int _level;
    public Color _tankColor;
    public float _sightRange = 1;
    public float _attackRange = 1; //if tank is stationary only use attack range, use it as a sight range. Therefore attacks on sight, and has ranged attacks

    public bool _canMove;
    public bool _isOffensive;
    public bool _canFire; //Only false for yellow tank
    public bool _canPlaceMines;
    public bool _isInvisible;

    public float _tankSpeed;
    public float _bulletSpeed;
    public float _fireDelay;
    public float _fireCooldown;
    public int _maxBulletsFireable;
    public int _riccochets;

}
