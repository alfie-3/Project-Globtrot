using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_StoreSettings : MonoBehaviour
{
    [SerializeField] private TMP_InputField titleInput;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text titleDisplay;

    public void OnTitleChanged()
    {
        string inputText = titleInput.text; 
        title.text = inputText;
        titleDisplay.text = inputText;

    }

}
