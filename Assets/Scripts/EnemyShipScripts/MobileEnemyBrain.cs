using UnityEngine;

public class MobileEnemyBrain : MonoBehaviour, IEnemyTank
{
    public LayerMask _playerLayer, _obstacleLayer;
    public int _shipLevel => _ship._level; //Used by gamemanager to determine highest lvl tank on screen

    [SerializeField] ShipTypes _ship; //Provides all baseline info for specific tank movement, attack patterns, etc.
    [SerializeField] MeshRenderer _baseVis; //Used to change material color based on tank
    [SerializeField] MeshRenderer _cannonVis; //Used to change material color based on tank
    [SerializeField] ParticleSystem _explosion;
    [SerializeField] ParticleSystem _ripples;
    GameManager _gameManager;
    Transform _playerPos;
    MobileEnemyNavigation _navMesh;

    Vector3 _lastKnownPlayerPos;
    bool _playerInSightRange, _playerInAttackRange;
    bool _localInputEnabled = true;
    bool _agentStopped;

    State _currentState = State.Patroling;
    enum State
    {
        Patroling, //Agent moves around between randomized points, default for mobile tanks
        Chasing, //Agent chases after Player tank
        Attacking, //Agent fires at player and stays in place
        Escaping //Agent Attacks before fleeing out of line of sight
    }

    // Start is called before the first frame update
    void Start()
    {
        _navMesh = this.GetComponent<MobileEnemyNavigation>();
        _playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        _navMesh.SetUp(_ship);
        
        _baseVis.materials[1].color = _ship._tankColor;
        _cannonVis.materials[1].color = _ship._tankColor;
    }

    // Update is called once per frame
    void Update()
    {
        //If input disabled, exit
        if (!GameManager._globalInputEnabled) return;
        else if (!_localInputEnabled)
        {
            //Stops moving tanks from moving
            if (!_agentStopped)
            {
                _agentStopped = _navMesh.StopResetAgent();
                return;
            } 
            else return;
        }

        switch (_currentState)
        {
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

    //Determines which state the tank should be in, based on if the player is nearby and within sight
    private void CheckRadar()
    {
        if (_ship._canMove)
        {
            _playerInSightRange = Physics.CheckSphere(transform.position, _ship._sightRange, _playerLayer);
        }
        _playerInAttackRange = Physics.CheckSphere(transform.position, _ship._attackRange, _playerLayer);
        
        if (_playerInAttackRange || _playerInSightRange)
        {
            CheckLineOfSight();
        }
        else
        {
            _currentState = State.Patroling;
        }
    }

    private void CheckLineOfSight()
    {
        Vector3 rayDir = _playerPos.position - transform.position;
        bool isSightObstructed = Physics.Raycast(transform.position, rayDir, rayDir.magnitude, _obstacleLayer);
        
        if (isSightObstructed || !_ship._canFire)
        {
            _currentState = State.Patroling;
        }
        else
        {
            _lastKnownPlayerPos = _playerPos.position;

            if (_playerInAttackRange)
            {
                if (!_ship._isOffensive) _currentState = State.Escaping; 
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

        _ripples.Stop();
        _explosion.Play(); //Plays explosion effect
        AudioManager.instance.Play(AudioManager.instance._tankExplosion);

        Invoke("Die", _explosion.main.duration);
    }

    private void Die()
    {
        gameObject.SetActive(false);
        _gameManager.EnemyDestroyed();
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
        Gizmos.DrawWireSphere(transform.position, _ship._attackRange);
        if (_ship._canMove)
        {
            //Sight Range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _ship._sightRange);
        }
    }

    public int GetTankLevel()
    {
        return _ship._level;
    }
}
