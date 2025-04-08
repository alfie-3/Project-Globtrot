using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_EmailButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI subjectText;  
    [SerializeField] private TextMeshProUGUI senderText;  
    private UI_EmailManager emailScript;
    private Email emailData; 

    public void Setup(Email email, UI_EmailManager manager)
    {
        emailScript = manager;

        emailData = email;
        subjectText.text = email.subject;
        senderText.text = $"From: {email.sender}";
        
        //takes first 5 words for preview
        string[] words = email.content.Split(' ');

        string preview = words.Length > 5 
            ? string.Join(" ", words, 0, 5) + "..." 
            : email.content;

    }

    //displays text
    public void OnClick()
    {
        emailScript.Display(emailData);
    }
}
