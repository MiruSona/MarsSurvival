using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

    //참조
    private GameObject artifact_menu;

    void Start()
    {
        artifact_menu = transform.FindChild("Artifacts_Menu").gameObject;
    }

    void Update() {
        if(Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void StartBtn() {
        artifact_menu.SetActive(true);
    }
}
