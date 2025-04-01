using UnityEngine.UI;
using UnityEngine;

public class UI_BuildingSelection: MonoBehaviour {

    [SerializeField]
    RectTransform ItemsPanel;




    public void ScrolPanel(float dir) {
        ItemsPanel.position += new Vector3 (0, dir * 150, 0); //75
    }

    public void SetVisabiltiy(bool visabiltiy) {
        GetComponent<Canvas>().enabled = visabiltiy;
    }


}