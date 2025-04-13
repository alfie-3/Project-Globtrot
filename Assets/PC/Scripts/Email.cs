using UnityEngine;

[CreateAssetMenu(fileName = "NewEmail", menuName = "PC Data/Email")]
public class Email : ScriptableObject
{
    public string subject;    // Subject of the email
    public string sender;     // Sender of the email
    public bool urgent;
    [TextArea(3, 10)] public string content; // Full content of the email
}