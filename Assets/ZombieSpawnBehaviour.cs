using System.Collections;
using UnityEngine;
using GoogleARCore;


public class ZombieSpawnBehaviour : MonoBehaviour
{    
    [SerializeField]
    private GameObject _zombie;
    private GameController _control;
    private float _spawnRate = 5;
    float _timeControl;

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
        Instantiate(_zombie, transform.position , Quaternion.Euler(0, 180, 0));
    }
}
