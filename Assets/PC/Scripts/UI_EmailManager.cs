using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class UI_EmailManager : MonoBehaviour
{
    [SerializeField] private GameObject emailButtonPrefab; 
    [SerializeField] private Transform spawnPoint;          
    [SerializeField] private float emailSpacing = 50f;    

    [SerializeField] private TextMeshProUGUI emailSubjectText;  
    [SerializeField] private TextMeshProUGUI emailSenderText;   
    [SerializeField] private TextMeshProUGUI emailContentText;  

    [SerializeField] private List<Email> emailList; 
    private List<GameObject> emailButtons = new List<GameObject>(); 

    private void Start()
    {
        DisplayEmails(); 
    }

    //display all emails
    private void DisplayEmails()
    {
        Vector3 nextPos = spawnPoint.position;

        foreach (var email in emailList)
        {
            GameObject emailButton = Instantiate(emailButtonPrefab, spawnPoint);
            emailButton.GetComponent<UI_EmailButton>().Setup(email);

            emailButton.transform.position = nextPos;
            nextPos.y -= emailSpacing; 
            emailButtons.Add(emailButton);
        }
    }

    //function for button to display email content
    public void Display(Email email)
    {
        emailSubjectText.text = email.subject;
        emailSenderText.text = $"From: {email.sender}";
        emailContentText.text = email.content;
    }
}
