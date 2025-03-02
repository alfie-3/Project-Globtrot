using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mess List", menuName = "Lists/Mess")]
public class MessListSO : ScriptableObject
{
    [SerializeField] List<MessController> Messes = new();

    public MessController GetMessRandom()
    {
        if (Messes.Count == 0)
        {
            Debug.LogWarning("No messes in list");
            return null;
        }

        return Messes[Random.Range(0, Messes.Count)];
    }
}
