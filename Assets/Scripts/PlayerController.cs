using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public TankTypes _tank;
    public Transform _cannon;
    public Transform _projectileSpawn;
    public Transform _mineSpawn;
    public Rigidbody _rigidBody;
    public BoxCollider _collider;
    public MeshRenderer _baseVis;
    public MeshRenderer _cannonVis;
    public ParticleSystem _tankExplosion;
    GameManager _gameManager;
    UIManager _uiManager;
    PlayerPool _playerPool;
    AudioManager _audioManager;
    Transform _cameraTrans;

    public bool tankControls = false;
    public float _rotationSensitivity = 8f;
    bool _gameHasStarted = false;
    float _playerSpeed;
    float _horizontalForce;
    float _verticalForce;
    float _cameraZDist;
    bool _localInputEnabled = true;

    Quaternion _rotationToTarget;
    Transform _mouseWorldPos;
    Vector3 _directionToTarget;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        _playerPool = GameObject.FindGameObjectWithTag("PlayerPool").GetComponent<PlayerPool>();
        _mouseWorldPos = GameObject.FindGameObjectWithTag("MouseWorldPos").GetComponent<Transform>();
        _audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        _cameraTrans = GameObject.FindGameObjectWithTag("MainCamera").transform;

        _playerSpeed = _tank._tankSpeed * 5f;
    }
    
    void FixedUpdate()
    {
        if (GameManager._inputEnabled && _localInputEnabled)
        {
            if (tankControls) TankMovement();
            else CameraMovement();

            //Cannon face Mouse/Target
            _directionToTarget = new Vector3(_mouseWorldPos.position.x - transform.position.x, 0f, _mouseWorldPos.position.z - transform.position.z);
            _rotationToTarget = Quaternion.LookRotation(_directionToTarget);
            
            _cannon.rotation = _rotationToTarget;
        }
    }

    void Update()
    {
        if (GameManager._inputEnabled && _localInputEnabled)
        {
            HandleInputs();
        }

        if (!_gameHasStarted)
        {
            _gameManager.LevelStart();
            _gameHasStarted = true;
        }
    }

    private void TankMovement()
    {
        //Tank Controls for Base
        _horizontalForce = Input.GetAxis("Horizontal") * _rotationSensitivity;
        _verticalForce = Input.GetAxis("Vertical") * _playerSpeed;

        _rigidBody.rotation = Quaternion.Euler(0f, (_rigidBody.rotation.eulerAngles.y + _horizontalForce), 0f);
        _rigidBody.velocity = transform.forward * _verticalForce;
    }
    private void CameraMovement()
    {
        //Camera-based Controls for Base
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal == 0f && vertical == 0f)
        {
            _rigidBody.velocity = Vector3.zero;
            return;
        }
        Vector3 direction = new Vector3(horizontal, 0f, vertical);
        direction = direction.normalized * _playerSpeed;
        _rigidBody.velocity = direction;

        transform.forward = Vector3.Lerp(transform.forward, direction, _rotationSensitivity);
    }

    private void HandleInputs() //Handles firing, planting mines, and pausing
    {
        //Attack Inputs 
        if (Input.GetMouseButtonDown(0) && _tank._canFire)
        {
            if (_playerPool._bulletPool.Count == 0)
            {
                _audioManager.Play(_audioManager._misfire);
                return;
            }
            _playerPool.GetBulletFromPool(_projectileSpawn, _tank);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && _tank._canPlaceMines)
        {
            _playerPool.GetMineFromPool(_mineSpawn.position);
        }

        //Other inputs
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _uiManager.SetActiveMenu("Pause Menu");
        }
    }

    public void PlayerHit() //Freezes player, disables the collider and mesh, and plays the explosion effect
    {
        _localInputEnabled = false;
        _collider.enabled = false;
        _rigidBody.velocity = Vector3.zero;
        
        _baseVis.enabled = false;
        _cannonVis.enabled = false;

        _tankExplosion.Play(); //Plays explosion effect
        _audioManager.Play(_audioManager._tankExplosion);

        Invoke("Die", _tankExplosion.main.duration);
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
}
