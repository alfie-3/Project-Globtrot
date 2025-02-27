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

    private int nextEmailIndex = 0;

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

        nextEmailIndex++; 
    }

    // function for button to display email content
    public void Display(Email email)
    {
        emailSubjectText.text = email.subject;
        emailSenderText.text = $"From: {email.sender}";
        emailContentText.text = email.content;
    }
}