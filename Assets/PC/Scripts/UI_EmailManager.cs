using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

public class UI_EmailManager : MonoBehaviour
{
    public int StartingEmails = 1;
    [SerializeField] private GameObject emailButtonPrefab; 
    [SerializeField] private Transform spawnPoint;          
    [SerializeField] private float emailSpacing = 50.0f;    
    private float originalEmailSpacing;

    [SerializeField] private TextMeshProUGUI emailSubjectText;  
    [SerializeField] private TextMeshProUGUI emailSenderText;   
    [SerializeField] private TextMeshProUGUI emailContentText;  

    [SerializeField] private List<Email> emailList; 
    private List<GameObject> emailButtons = new List<GameObject>(); 
    private int index = 0;
    private Vector3 nextPos;
    private bool isMinimised = true;

    private void Start()
    {
        for(int i = 0; i < StartingEmails; i++)
        {
            DisplayEmail();
        }
        originalEmailSpacing = emailSpacing;
    }

    // instead of displaying all emails this function will call the next email in the list instead
    public void SummonNextEmail()
    {
        if (index >= emailList.Count)
        {
            Debug.Log("No more emails to summon.");
            return;
        }
        DisplayEmail();
    }

    // display singular email
    private void DisplayEmail()
    {
        nextPos = spawnPoint.position - new Vector3(0, emailSpacing * emailButtons.Count, 0);
        Email emailScript = emailList[index];
        GameObject emailButton = Instantiate(emailButtonPrefab, spawnPoint);
        emailButton.GetComponent<UI_EmailButton>().Setup(emailScript);

        emailButton.transform.position = nextPos;
        emailButtons.Add(emailButton);
        index++; 
    }

    // function for button to display email content
    public void Display(Email email)
    {
        emailSubjectText.text = $"Subject: {email.subject}";
        emailSenderText.text = $"From: {email.sender}";
        emailContentText.text = email.content;
    }

    public void ChangeSpacing()
    {
        if(isMinimised)
        {
            emailSpacing = emailSpacing*1.75f;
            isMinimised = false;
        }
        else if(isMinimised == false)
        {
            emailSpacing = originalEmailSpacing;
            isMinimised = true;
        }

        Debug.Log(emailSpacing);
    }
}