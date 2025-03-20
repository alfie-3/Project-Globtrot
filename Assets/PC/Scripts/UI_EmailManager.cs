using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class UI_EmailManager : MonoBehaviour
{
    [SerializeField] private GameObject emailButtonPrefab; 
    [SerializeField] private Transform spawnPoint;          
    private float emailSpacing = 60.0f;    

    [SerializeField] private TextMeshProUGUI emailSubjectText;  
    [SerializeField] private TextMeshProUGUI emailSenderText;   
    [SerializeField] private TextMeshProUGUI emailContentText;  

    [SerializeField] private List<Email> emailList; 
    private List<GameObject> emailButtons = new List<GameObject>(); 
    private int nextEmailIndex = 0;
    private bool isMinimised = true;
    private float originalEmailSpacing;

    //instead of displaying all emails this function will call the next email in the list instead
    public void SummonNextEmail()
    {
        if (nextEmailIndex >= emailList.Count)
        {
            Debug.Log("No more emails to summon.");
            return;
        }

        Vector3 nextPos = spawnPoint.position - new Vector3(0, emailSpacing * emailButtons.Count, 0);

        Email emailScript = emailList[nextEmailIndex];
        GameObject emailButton = Instantiate(emailButtonPrefab, spawnPoint);
        emailButton.GetComponent<UI_EmailButton>().Setup(emailScript);

        emailButton.transform.position = nextPos;
        emailButtons.Add(emailButton);

        originalEmailSpacing = emailSpacing;

        nextEmailIndex++; 
    }

    // function for button to display email content
    public void Display(Email email)
    {
        emailSubjectText.text = email.subject;
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

    }
}