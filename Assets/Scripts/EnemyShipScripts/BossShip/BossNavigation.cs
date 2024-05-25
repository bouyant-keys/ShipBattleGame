using UnityEngine;
using UnityEngine.AI;

public class BossNavigation : MonoBehaviour
{
    MobileEnemyCannon _enemyCannon;
    NavMeshAgent _agent;
    Vector3 _playerPosition;

    Vector3[] _bossSpawnPoints;
    
    public Vector3 _currentWalkPoint;
    public Vector3 _generatedWalkPoint;
    public LayerMask _groundLayer, _obstacleLayer;
    public float _minWalkRange;
    public float _maxWalkRange;
    bool _walkPointSet = false;
    bool _retreatPointSet = false;
    bool _alreadyAttacked;

    private void Awake() 
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("BossSpawnPoint");
        _bossSpawnPoints = new Vector3[spawnPoints.Length];

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            _bossSpawnPoints[i] = spawnPoints[i].transform.position;
        }
        
        _agent = GetComponent<NavMeshAgent>();
        _enemyCannon = GetComponentInChildren<MobileEnemyCannon>();


        //Implement Tank scrptobj data here
        _agent.name = BossBrain._bossShip._name;
        _agent.speed = BossBrain._bossShip._tankSpeed * 2;
        _enemyCannon._maxBulletsFired = BossBrain._bossShip._maxBulletsFireable;
    }

    public bool StopResetAgent()
    {
        _agent.isStopped = true;
        _walkPointSet = false;
        _retreatPointSet = false;

        return _agent.isStopped;
    }

    public void Patrol()
    {
        if (!_walkPointSet) SearchWalkPoint();
        if (_walkPointSet) _agent.SetDestination(_currentWalkPoint);

        //Walkpoint reached
        if (_agent.remainingDistance < 2f)
        {
            _walkPointSet = false;
            if (BossBrain._bossShip._canPlaceMines) _enemyCannon.PlaceMine();
        }
    }

    private void SearchWalkPoint()
    {
        //Generate a point in the range and add it to the transform position
        float dist = UnityEngine.Random.Range(_minWalkRange, _maxWalkRange);
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle.normalized * dist;
        Vector3 tempPoint = new Vector3(transform.position.x + randomPoint.x, 
                                        transform.position.y + 1.5f, 
                                        transform.position.z + randomPoint.y);

        //If walkpoint is in a walkable area
        bool isWalkable = Physics.Raycast(tempPoint, -transform.up, 3f, _groundLayer);
        bool isObstructed = Physics.Raycast(tempPoint, -transform.up, 3f, _obstacleLayer);
        if (isWalkable && !isObstructed)
        {
            _currentWalkPoint = tempPoint;
            _walkPointSet = true;
        }
    }

    public void Chase(Vector3 _lastKnownPos)
    {
        _currentWalkPoint = _lastKnownPos;
        _agent.SetDestination(_currentWalkPoint);
        
        _walkPointSet = true;
    }

    public void Attack()
    {
        //Update to something else
        _agent.SetDestination(transform.position);
    }

    public void Escape(Vector3 playerPos)
    {
        _playerPosition = playerPos;

        if (!_retreatPointSet) GenerateRetreatPoint();
        if (_retreatPointSet) _agent.SetDestination(_currentWalkPoint);

        //Walkpoint reached
        if (_agent.remainingDistance < 2f)
        {
            _retreatPointSet = false;
        }
    }

    private void GenerateRetreatPoint()
    {
        _agent.SetDestination(transform.position);
        //Finds the direction to retreat (dir away from player), then creates a random point in the WalkRange in that direction
        Vector2 retreatDir = new Vector2(transform.position.x - _playerPosition.x, transform.position.z - _playerPosition.z).normalized;
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle.normalized;

        float dist = UnityEngine.Random.Range(_minWalkRange, _maxWalkRange);
        Vector2 sumVector = (retreatDir + randomPoint).normalized * dist;
        Vector3 tempPoint = new Vector3(transform.position.x + sumVector.x, transform.position.y + 1.5f, transform.position.z + sumVector.y);
        _generatedWalkPoint = tempPoint;

        //If walkpoint is in a walkable area, out of sight of the player
        if (PointIsOutOfSight(tempPoint))
        {
            _currentWalkPoint = tempPoint;
            _retreatPointSet = true;
        }
    }
    private bool PointIsOutOfSight(Vector3 pointPos)
    {
        Vector3 rayDir = _playerPosition - pointPos;

        bool isOutOfSight = Physics.Raycast(pointPos, rayDir, rayDir.magnitude, _obstacleLayer);
        bool isWalkable = Physics.Raycast(pointPos, -transform.up, 3f, _groundLayer);
        bool isObstructed = Physics.Raycast(pointPos, -transform.up, 3f, _obstacleLayer);

        if (isOutOfSight && isWalkable && !isObstructed)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
