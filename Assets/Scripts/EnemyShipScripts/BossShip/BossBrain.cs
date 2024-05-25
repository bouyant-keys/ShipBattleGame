using UnityEngine;

public class BossBrain : MonoBehaviour, IEnemyTank
{
    public static ShipTypes _bossShip; //Provides all baseline info for specific tank movement, attack patterns, etc.
    public LayerMask _playerLayer, _obstacleLayer;
    public int _shipLevel => _bossShip._level; //Used by gamemanager to determine highest lvl tank on screen
    public int _health = 4;

    [SerializeField] MeshRenderer _baseVis; //Used to change material color based on tank
    [SerializeField] MeshRenderer _cannonVis; //Used to change material color based on tank
    [SerializeField] ParticleSystem _explosion;
    [SerializeField] ParticleSystem _ripples;
    GameManager _gameManager;
    Transform _playerPos;
    BossNavigation _navMesh;
    BossCannon _cannon;

    bool _playerInAttackRange;
    bool _localInputEnabled = true;
    bool _agentStopped;

    State _currentState = State.Patroling;
    enum State
    {
        Patroling, //Agent moves around between randomized points, default for mobile tanks
        Attack_Close, 
        Attack_Distance,
        Escaping //Agent Attacks before fleeing out of line of sight
    }

    // Start is called before the first frame update
    void Start()
    {
        _navMesh = GetComponent<BossNavigation>();
        _playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        
        _baseVis.materials[1].color = _bossShip._tankColor;
        _cannonVis.materials[1].color = _bossShip._tankColor;
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
            case State.Attack_Close:
                _cannon.CloseAttack();
                _navMesh.Attack();
                break;
            case State.Attack_Distance:
                _cannon.RangedAttack();
                _navMesh.Attack();
                break;
            case State.Escaping:
                _navMesh.Escape(_playerPos.position);
                break;
        }
    }

    private void CheckLineOfSight()
    {
        Vector3 rayDir = _playerPos.position - transform.position;
        bool isSightObstructed = Physics.Raycast(transform.position, rayDir, rayDir.magnitude, _obstacleLayer);
        
        if (_playerInAttackRange && !isSightObstructed) _currentState = State.Attack_Close;
        else _currentState = State.Attack_Distance;
    }

    public void EnemyHit()
    {
        if (!_localInputEnabled) return;

        _health--;
        _currentState = State.Escaping;

        if (_health > 0) return;

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
        Gizmos.DrawWireSphere(transform.position, _bossShip._attackRange);
        if (_bossShip._canMove)
        {
            //Sight Range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _bossShip._sightRange);
        }
    }

    public int GetTankLevel()
    {
        return _bossShip._level;
    }
}
