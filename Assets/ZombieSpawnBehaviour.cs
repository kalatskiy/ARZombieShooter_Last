using System.Collections;
using UnityEngine;
using GoogleARCore;


public class ZombieSpawnBehaviour : MonoBehaviour
{
    public DetectedPlane _detectedPlane;
    public GameObject _zombie;
    private GameController _control;
    float _delayTime = 3;
    float _delay = 10f;

    public float _spawnRate = 5;
    float _timeControl;



    // Start is called before the first frame update
    void Start()
    {
        _control = GameController.Instance;
        _timeControl = 0;
    }

    void Update()
    {

        ZombieSpawn();
    }

    private void ZombieSpawn()
    {
        if (Time.time > _timeControl)
        {
            StartCoroutine(ZombieSpawnRoutine());
            _timeControl = Time.time + _spawnRate;
        }
    }

    IEnumerator ZombieSpawnRoutine()
    {
        Debug.Log("im here");
        int _rndx = Random.Range(-3, +3);
        int _rndz = Random.Range(0, 3);
        yield return new WaitForSeconds(3f);
        Instantiate(_zombie, transform.position /*+ new Vector3(_rndx, 0, _rndz)*/, Quaternion.Euler(0, 180, 0));
    }
}
