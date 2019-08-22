using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarScript : MonoBehaviour
{   
    public Image _hpbar;    
    public float maxHealth = 100;
    public float health;
    private int mCurrentValue;
    public static HpBarScript Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {        
        health = maxHealth;        
    }

    public void Damage(float damage)
    {
        health -= damage;
        _hpbar.fillAmount = (health / maxHealth);
    }
}
