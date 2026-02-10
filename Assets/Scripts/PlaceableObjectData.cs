using UnityEngine;

[CreateAssetMenu(fileName = "PlaceableObjectData", menuName = "Placement/PlaceableObjectData", order = 0)]
public class PlaceableObjectData : ScriptableObject
{
    public GameObject actualPrefab;
    public GameObject ghostPrefab;
    public string displayName;

    [System.Serializable]
    public struct ResourceCost
    {
        public string resourceType;
        public int amount;
    }
    public ResourceCost[] cost;
    // You can add more fields like icon, etc.
}
