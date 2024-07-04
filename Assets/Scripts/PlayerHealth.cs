using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    [SerializeField] private int maxHealth = 10;
    private void Start()
    {
        health = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
    }
}
