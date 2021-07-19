using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HelpMenu : MonoBehaviour {

    //참조
    private Image picture;
    private Text page;
    private Sprite[] tutorial_sprites = new Sprite[4];
    private int current_page = 0;
    public GameObject pause_menu;

    //초기화
    void Start () {
        picture = transform.FindChild("Picture").GetComponent<Image>();
        page = transform.FindChild("Page").GetComponent<Text>();
        for (int i = 0; i < tutorial_sprites.Length; i++)
            tutorial_sprites[i] = Resources.Load<Sprite>("Sprite/Game/Tutorial/Tutorial_" + i + "_" + TextTrans.instance.languageCode);

        page.text = (current_page + 1) + "/" + tutorial_sprites.Length;
        picture.sprite = tutorial_sprites[current_page];
    }

    //버튼
    public void RightBtn()
    {
        if (current_page == tutorial_sprites.Length - 1)
            current_page = 0;
        else
            current_page++;

        picture.sprite = tutorial_sprites[current_page];
        page.text = (current_page + 1) + "/" + tutorial_sprites.Length;
    }

    public void LeftBtn()
    {
        if (current_page == 0)
            current_page = tutorial_sprites.Length - 1;
        else
            current_page--;

        picture.sprite = tutorial_sprites[current_page];
        page.text = (current_page + 1) + "/" + tutorial_sprites.Length;
    }

    //컨티뉴 버튼
    public void Resume()
    {
        if(pause_menu.activeSelf)
            gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
