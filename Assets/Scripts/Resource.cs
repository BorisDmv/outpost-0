using UnityEngine;
public class Resource : MonoBehaviour
{
    public int amount = 10;



    // Called when collected by a ResourceCollector
    public void Collect(int collectAmount)
    {
        amount -= collectAmount;
        if (amount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
