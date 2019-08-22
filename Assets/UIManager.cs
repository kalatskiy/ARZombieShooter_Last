using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Text _score;

    [SerializeField]
    private Text _ammoCount;

    private int _currentScore;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _currentScore = 0;
    }
    public void Score(int price)
    {
        _currentScore += price;
        _score.text = "Score: " + _currentScore;
    }
}
