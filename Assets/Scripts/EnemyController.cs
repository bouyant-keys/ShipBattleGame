using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public TankTypes _tank;
    public MeshRenderer _baseVis;
    public MeshRenderer _cannonVis;
    public ParticleSystem _tankExplosion;
    public BoxCollider _collider;
    GameManager _gameManager;
    Transform _playerPos;
    EnemyNavMesh _navMesh;
    EnemyCannon _cannon;
    AudioManager _audioManager;

    public LayerMask _playerLayer, _obstacleLayer;
    Vector3 _lastKnownPlayerPos;
    bool _playerInSightRange, _playerInAttackRange;
    bool _localInputEnabled = true;

    State _currentState;
    enum State
    {
        Default, //Based on tank,  Default State is either Idle or Patroling
        Idle, //Agent stands still and looks around, default for stationary tanks
        Patroling, //Agent moves around between randomized points, default for mobile tanks
        Chasing, //Agent chases after Player tank
        Attacking, //Agent fires at player and stays in place
        Escaping //Agent Attacks before fleeing out of line of sight
    }

    // Start is called before the first frame update
    void Start()
    {
        _navMesh = this.GetComponent<EnemyNavMesh>();
        _playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        _cannon = GetComponentInChildren<EnemyCannon>();

        _navMesh.SetUp(_tank);
        _cannon._maxBulletsFired = _tank._maxBulletsFireable;
        /** Not Implemented
        if (_tank._isInvisible)
        {
            _baseVisual.gameObject.SetActive(false);
            _cannonVisual.gameObject.SetActive(false);
            //Set smoke moving particle effect active
        }
        else
        {
            _baseVisual.materials[1].color = _tank._tankColor;
            _cannonVisual.materials[1].color = _tank._tankColor;
        } 
        **/
        _baseVis.materials[1].color = _tank._tankColor;
        _cannonVis.materials[1].color = _tank._tankColor;

        _currentState = State.Default;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager._inputEnabled && _localInputEnabled)
        {
            switch (_currentState)
            {
                case State.Default:
                    if (_tank._canMove) _currentState = State.Patroling;
                    else _currentState = State.Idle;
                    Update();
                    break;
                case State.Idle:
                    _navMesh.Idle();
                    break;
                case State.Patroling:
                    _navMesh.Patrol();
                    break;
                case State.Chasing:
                    _navMesh.Chase(_lastKnownPlayerPos);
                    break;
                case State.Attacking:
                    _navMesh.Attack();
                    break;
                case State.Escaping:
                    _navMesh.Escape(_playerPos.position);
                    break;
            }

            CheckRadar();
        }
        else if (!_localInputEnabled && _tank._canMove)
        {
            _navMesh.StopResetAgent();
        }
    }

    //Determines which state the tank should be in, based on if the player is nearby and within sight
    private void CheckRadar()
    {
        if (_tank._canMove)
        {
            _playerInSightRange = Physics.CheckSphere(transform.position, _tank._sightRange, _playerLayer);
        }
        _playerInAttackRange = Physics.CheckSphere(transform.position, _tank._attackRange, _playerLayer);
        
        if (_playerInAttackRange || _playerInSightRange)
        {
            CheckLineOfSight();
        }
        else
        {
            _currentState = State.Default;
        }
    }

    private void CheckLineOfSight()
    {
        Vector3 rayDir = _playerPos.position - transform.position;
        bool isSightObstructed = Physics.Raycast(transform.position, rayDir, rayDir.magnitude, _obstacleLayer);
        
        if (isSightObstructed || !_tank._canFire)
        {
            _currentState = State.Default;
        }
        else
        {
            _lastKnownPlayerPos = _playerPos.position;

            if (_playerInAttackRange)
            {
                if (!_tank._isOffensive && _tank._canMove) _currentState = State.Escaping; 
                else _currentState = State.Attacking;
            }
            else if (_playerInSightRange && _currentState != State.Escaping) _currentState = State.Chasing;
        }
    }

    public void EnemyHit()
    {
        if (!_localInputEnabled) return;

        _localInputEnabled = false;
        _baseVis.enabled = false;
        _cannonVis.enabled = false;
        _collider.enabled = false;

        _tankExplosion.Play(); //Plays explosion effect
        _audioManager.Play(_audioManager._tankExplosion);
        Invoke("Die", _tankExplosion.main.duration);
    }

    private void Die()
    {
        gameObject.SetActive(false);
        _gameManager.CheckRemainingEnemies();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            EnemyHit();
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Attack Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _tank._attackRange);
        if (_tank._canMove)
        {
            //Sight Range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _tank._sightRange);
        }
    }
}
