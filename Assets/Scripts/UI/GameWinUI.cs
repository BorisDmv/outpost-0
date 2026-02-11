using UnityEngine;

public class GameWinUI : MonoBehaviour
{
    public static GameWinUI Instance { get; private set; }

    [Header("UI")]
    public GameObject panel;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void ShowWin()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
        Time.timeScale = 0f;
    }
}
