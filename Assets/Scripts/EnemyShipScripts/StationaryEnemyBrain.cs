using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryEnemyBrain : MonoBehaviour, IEnemyTank
{
    public LayerMask _playerLayer, _obstacleLayer;

    [SerializeField] TankTypes _tank; //Provides all baseline info for specific tank movement, attack patterns, etc.
    [SerializeField] StationaryEnemyCannon _cannon; //Script used to fire projectiles
    [SerializeField] MeshRenderer _baseVis; //Used to change material color based on tank
    [SerializeField] MeshRenderer _cannonVis; //Used to change material color based on tank
    [SerializeField] ParticleSystem _tankExplosion;
    GameManager _gameManager;
    Transform _playerPos;

    bool _playerInAttackRange;
    bool _localInputEnabled = true;

    State _currentState = State.Idle;
    State _previousState;
    enum State
    {
        Idle, //Agent stands still and looks around, default for stationary tanks
        Holding, //Temp state after losing sight of the player, but before returning to idle
        Attacking, //Agent fires at player and stays in place
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _cannon = GetComponentInChildren<StationaryEnemyCannon>();
        _cannon._tank = _tank;
        _cannon._maxBulletsFired = _tank._maxBulletsFireable;
        
        _baseVis.materials[1].color = _tank._tankColor;
        _cannonVis.materials[1].color = _tank._tankColor;
    }

    // Update is called once per frame
    void Update()
    {
        //If input disabled, exit
        if (!GameManager._globalInputEnabled) return;
        else if (!_localInputEnabled) return;
        //Might need to ChangeState(State.Holding), when local and global input are disabled

        if (_currentState != State.Attacking) return;
        CheckLineOfSight();
    }

    private void CheckLineOfSight()
    {
        Vector3 rayDir = _playerPos.position - transform.position;
        bool isSightObstructed = Physics.Raycast(transform.position, rayDir, rayDir.magnitude, _obstacleLayer);

        if (!isSightObstructed)
        {
            ChangeState(State.Attacking);
            return;
        }
            
        if (_currentState == State.Attacking) StartCoroutine(MaintainAttack());
    }
    IEnumerator MaintainAttack() //Keep attacking for a short while even if the player is out of sight
    {
        //print("Lost sight, maintaining attack state");
        ChangeState(State.Holding);
        yield return new WaitForSeconds(2);
        //print("Returning to Idle State");
        ChangeState(State.Idle);
    }

    public void EnemyHit()
    {
        if (!_localInputEnabled) return;

        ChangeState(State.Idle);
        _localInputEnabled = false;
        _baseVis.enabled = false;
        _cannonVis.enabled = false;

        _tankExplosion.Play(); //Plays explosion effect
        AudioManager.instance.Play(AudioManager.instance._tankExplosion);
        Invoke("Die", _tankExplosion.main.duration);
    }
    private void Die()
    {
        gameObject.SetActive(false);
        _gameManager.CheckRemainingEnemies();
    }

    private void ChangeState(State state)
    {
        _previousState = _currentState;
        _currentState = state;

        //Values only set once on state change, so are easy to override
        if (state == State.Idle)
        {
            _cannon._idling = true;
            _cannon._attacking = false;
        }
        else if (state == State.Attacking)
        {
            _cannon._idling = false;
            _cannon._attacking = true;
        } 
        else if (state == State.Holding)
        {
            _cannon._idling = false;
            _cannon._attacking = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            EnemyHit();
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        CheckLineOfSight();
    }

    public int GetTankLevel()
    {
        return _tank._level;
    }
}
