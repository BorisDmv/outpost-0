using UnityEngine;

public class Base : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 200;
    public int currentHealth = 200;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        if (currentHealth <= 0)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        if (GameOverUI.Instance != null)
        {
            GameOverUI.Instance.ShowGameOver();
        }
        else
        {
            Debug.LogWarning("GameOverUI instance not found.");
        }
    }
}
