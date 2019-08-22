using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _score;
    [SerializeField]
    private Text _ammoCount;
    [SerializeField]
    private Button _exitButton;

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

    private void ExitApplication()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        _exitButton.onClick.AddListener(ExitApplication);
    }

    private void OnDisable()
    {
        _exitButton.onClick.RemoveListener(ExitApplication);
    }
}
