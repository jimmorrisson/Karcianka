using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayer : MonoBehaviour
{
    [SerializeField] private Image healthUI;

    public int Life { get; private set; }
    
    private void Start()
    {
        Life = 100;
    }

    public void TakeDamage(int amount)
    {
        Life -= amount;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        healthUI.transform.localScale = new Vector3(Life / 100.0f, 1, 1);
    }
}
