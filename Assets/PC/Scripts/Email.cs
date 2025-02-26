using UnityEngine;

[CreateAssetMenu(fileName = "NewEmail", menuName = "Email System/Email")]
public class Email : ScriptableObject
{
    public string subject;    // Subject of the email
    public string sender;     // Sender of the email
    [TextArea] public string content; // Full content of the email
}