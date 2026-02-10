using UnityEngine;

[CreateAssetMenu(fileName = "PlaceableObjectData", menuName = "Placement/PlaceableObjectData", order = 0)]
public class PlaceableObjectData : ScriptableObject
{
    public GameObject actualPrefab;
    public GameObject ghostPrefab;
    public string displayName;
    // You can add more fields like icon, cost, etc.
}
