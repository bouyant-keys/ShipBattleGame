using UnityEngine;

public class PlayerController : MonoBehaviour, IDataPersistence
{
    [SerializeField] ShipTypes _ship;
    [SerializeField] GameObject _crown;
    [SerializeField] Transform _cannon;
    [SerializeField] Transform _projectileSpawn;
    [SerializeField] Transform _mineSpawn;
    [SerializeField] Rigidbody _rigidBody;
    [SerializeField] BoxCollider _collider;
    [SerializeField] MeshRenderer _baseVis;
    [SerializeField] MeshRenderer _cannonVis;
    [SerializeField] ParticleSystem _explosion;
    [SerializeField] ParticleSystem _ripples;
    GameManager _gameManager;
    UIManager _uiManager;
    PlayerPool _playerPool;
    AudioManager _audioManager;

    [SerializeField] float _rotationSensitivity = 8f;
    float _playerSpeed;
    float _horizontalForce;
    float _verticalForce;
    bool _localInputEnabled = true;
    bool _crownActive = false;

    Quaternion _rotationToTarget;
    Transform _mouseWorldPos;
    Vector3 _directionToTarget;

    private void Awake() 
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        _playerPool = GameObject.FindGameObjectWithTag("PlayerPool").GetComponent<PlayerPool>();
        _audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        _mouseWorldPos = GameObject.FindGameObjectWithTag("MouseWorldPos").transform;

        _playerSpeed = _ship._tankSpeed * 5f;
    }

    private void Start() {
        GameObject.FindGameObjectWithTag("DataManager").GetComponent<DataPersistenceManager>().LoadGame();
        _crown.SetActive(_crownActive);
    }
    
    void FixedUpdate()
    {
        if (!GameManager._globalInputEnabled || !_localInputEnabled) return;

        TankMovement();

        //Cannon face Mouse/Target
        _directionToTarget = new Vector3(_mouseWorldPos.position.x - transform.position.x, 0f, _mouseWorldPos.position.z - transform.position.z);
        _rotationToTarget = Quaternion.LookRotation(_directionToTarget);
            
        _cannon.rotation = _rotationToTarget;
    }

    void Update()
    {
        if (!GameManager._globalInputEnabled || !_localInputEnabled) return;
        
        HandleInputs();
    }

    private void TankMovement()
    {
        //Tank Controls for Base
        _horizontalForce = Input.GetAxis("Horizontal") * _rotationSensitivity;
        _verticalForce = Input.GetAxis("Vertical") * _playerSpeed;

        _rigidBody.rotation = Quaternion.Euler(0f, (_rigidBody.rotation.eulerAngles.y + _horizontalForce), 0f);
        _rigidBody.velocity = transform.forward * _verticalForce;
    }

    private void HandleInputs() //Handles firing, planting mines, and pausing
    {
        //Attack Inputs 
        if (Input.GetMouseButtonDown(0) && _ship._canFire)
        {
            if (_playerPool._bulletPool.Count == 0)
            {
                _audioManager.Play(_audioManager._misfire);
                return;
            }
            _playerPool.GetBulletFromPool(_projectileSpawn, _ship);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && _ship._canPlaceMines)
        {
            _playerPool.GetMineFromPool(_mineSpawn.position);
        }

        //Other inputs
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _uiManager.Pause();
        }
    }

    public void PlayerHit() //Freezes player, disables the collider and mesh, and plays the explosion effect
    {
        _localInputEnabled = false;
        _collider.enabled = false;
        _rigidBody.velocity = Vector3.zero;
        
        _baseVis.enabled = false;
        _cannonVis.enabled = false;

        _ripples.Stop();
        _explosion.Play(); //Plays explosion effect
        _audioManager.Play(_audioManager._tankExplosion);

        Invoke("Die", _explosion.main.duration);
    }

    private void Die() //Updates GameManager upon death
    {
        _gameManager.PlayerDestroyed();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            PlayerHit();
        }
    }

    public void SaveData(GameData data)
    {
        //Nothing to save
    }

    public void LoadData(GameData data)
    {
        _crownActive = data.completedGame;
    }
}
