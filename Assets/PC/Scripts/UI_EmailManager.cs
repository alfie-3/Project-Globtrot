using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class UI_EmailManager : MonoBehaviour
{
    [SerializeField] private GameObject emailButtonPrefab; 
    [SerializeField] private Transform spawnPoint;          
    private float emailSpacing = 50.0f;
    [Space]
    [SerializeField] private TextMeshProUGUI emailSubjectText;  
    [SerializeField] private TextMeshProUGUI emailSenderText;   
    [SerializeField] private TextMeshProUGUI emailContentText;
    [Space]
    [SerializeField] private GameObject emailNotification;

    private List<GameObject> emailButtons = new List<GameObject>(); 
    private bool isMinimised = true;
    private float originalEmailSpacing;

    private void OnEnable()
    {
        GameStateManager.OnDayChanged += (current) => { AddDailyEmails(); };
    }

    private void OnDisable()
    {
        GameStateManager.OnDayChanged -= (current) => { AddDailyEmails(); };
    }

    public void AddEmail(Email email)
    {
        UI_EmailButton emailButton = Instantiate(emailButtonPrefab, spawnPoint).GetComponent<UI_EmailButton>();
        emailButton.GetComponent<UI_EmailButton>().Setup(email, this);
        emailButton.transform.SetAsFirstSibling();
    }

    public void AddDailyEmails()
    {
        DayData dayData = GameStateManager.Instance.GetCurrentDayData();

        if (dayData == null) return;

        foreach (Email email in dayData.DayEmails)
        {
            AddEmail(email);

            if (email.urgent)
            {
                emailNotification.SetActive(true);
            }
        }
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

    private void OnDestroy()
    {
        GameStateManager.OnDayChanged -= (current) => { AddDailyEmails(); };
    }
}