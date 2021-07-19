using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetCrystal : MonoBehaviour {


    //참조
    private Image crystal_img;
    private SystemDAO.GlobalState global_state;
    private SystemDAO.GameManagerData gm_data;

    //타이머
    private GameDAO.Timer stay_timer = new GameDAO.Timer(0f, 2f);

    //알파값 관련
    private bool alpha_add = true;

    //delta 값(위치 / 알파값)
    private const float alpha_add_value = 0.02f;
    private const float pos_add_value = 0.005f;

    //초기화
    private Vector3 init_pos = Vector3.zero;
    private Color Init_color = new Color(1f, 1f, 1f, 0f); 

    //초기화
    void Awake () {
        crystal_img = transform.FindChild("ImgCanvas").FindChild("Image").GetComponent<Image>();
    }

    //켜지면 진행하는거
    void OnEnable()
    {
        crystal_img.color = Init_color;
        alpha_add = true;
    }
	
	//이미지 보여줌 - 알파값 변화
	void FixedUpdate () {
        ShowImage();
    }

    //이미지 보여주기(알파값 변화)
    private void ShowImage()
    {
        //알파값 & 위치 조절
        Color color = crystal_img.color;

        //알파값이 0보다 작거나 같으면 종료.
        if (color.a < 0f)
        {
            crystal_img.color = Init_color;
            gameObject.SetActive(false);
        }

        //알파값이 1보다 작은 동안 알파값 및 위치 ++ or --
        if (color.a < 1f)
        {
            if (alpha_add)
            {
                color.a += alpha_add_value;
                crystal_img.transform.Translate(new Vector3(0, pos_add_value, 0));
            }
            else
            {
                color.a -= alpha_add_value;
                crystal_img.transform.Translate(new Vector3(0, -pos_add_value, 0));
            }
        }
        else
        {
            //잠깐 대기
            if (stay_timer.timer < stay_timer.term)
                stay_timer.timer += Time.deltaTime;
            else
            {
                //대기 시간 끝나면 다시--
                color.a -= alpha_add_value;
                alpha_add = false;
                stay_timer.timer = 0f;
            }
        }

        //색 적용
        crystal_img.color = color;
    }
}
