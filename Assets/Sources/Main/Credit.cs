using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Credit : MonoBehaviour {

    //참조
    private Image panel;
    private Text title;
    private Text person;

    //알파값
    private Color dark_alpha = new Color(0, 0, 0, 0);
    private Color white_alpha = new Color(1, 1, 1, 0);
    private const float delta_amount = 0.1f;

    //알파값 +- 여부
    public enum Alpha
    {
        Add,
        Stay,
        Sub
    }
    public Alpha alpha_enum = Alpha.Add;

	//초기화
	void Awake () {
        panel = GetComponent<Image>();
        title = transform.FindChild("title").GetComponent<Text>();
        person = transform.FindChild("Person").GetComponent<Text>();
    }

    //Enable마다
    void OnEnable()
    {
        panel.color = dark_alpha;
        title.color = white_alpha;
        person.color = white_alpha;
        alpha_enum = Alpha.Add;
    }
	
	void Update () {
        //판넬 칼라
        Color panel_color = panel.color;

        switch (alpha_enum)
        {
            case Alpha.Add:

                if (panel_color.a < 1f)
                    panel_color.a += delta_amount;
                else
                    alpha_enum = Alpha.Stay;
                break;

            case Alpha.Sub:

                if (panel_color.a > 0f)
                    panel_color.a -= delta_amount;
                else
                    gameObject.SetActive(false);
                break;
        }

        panel.color = panel_color;

        //텍스트 칼라
        Color text_color = title.color;
        text_color.a = panel_color.a;

        title.color = text_color;
        person.color = text_color;
    }
}
