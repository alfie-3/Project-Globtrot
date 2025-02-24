using UnityEngine;

[CreateAssetMenu(fileName = "New Character Reference", menuName = "CharacterData/Reference")]
public class CharacterReferenceData : ScriptableObject
{
    [field: SerializeField] public string PlayerReferenceID { get; private set; }
    [field: SerializeField] public GameObject PlayerPrefab {  get; private set; }
}
