// ✅ UniqueIDAssigner.cs
using UnityEngine;

[DisallowMultipleComponent]
public class UniqueIDAssigner : MonoBehaviour
{
    public string uniqueID;

    private void Awake()
    {
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = System.Guid.NewGuid().ToString();
        }
    }
}