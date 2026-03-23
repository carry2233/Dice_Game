using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterPrefabEntry
{
    public int monsterTypeID;
    public GameObject prefab;
}

public class MonsterPrefabRegistry : MonoBehaviour
{
    public List<MonsterPrefabEntry> monsterPrefabs;

    private Dictionary<int, GameObject> prefabDict;

    private void Awake()
    {
        prefabDict = new Dictionary<int, GameObject>();
        foreach (var entry in monsterPrefabs)
        {
            if (!prefabDict.ContainsKey(entry.monsterTypeID))
            {
                prefabDict.Add(entry.monsterTypeID, entry.prefab);
            }
        }
    }

    public GameObject GetPrefabByID(int id)
    {
        if (prefabDict.TryGetValue(id, out GameObject prefab))
        {
            return prefab;
        }
        return null;
    }
}
