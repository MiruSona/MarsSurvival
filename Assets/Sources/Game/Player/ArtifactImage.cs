using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArtifactImage : SingleTon<ArtifactImage> {

    //참조
    private Image artifact_image;
    private AudioSource audio_source;
    private AudioClip get_sound;

    //색
    private GameDAO.Timer delay_timer = new GameDAO.Timer(0, 1f);
    private Color color = new Color(1f, 1f, 1f, 0f);

    //위치
    private Vector2 position = new Vector2(0,-10f);

    //값
    private const float init_alpha = 0f;
    private const float init_y = -10f;
    private const float up_speed = 0.3f;

    //초기화
    void Start () {
        artifact_image = transform.FindChild("Image").GetComponent<Image>();
        audio_source = GetComponent<AudioSource>();
        get_sound = Resources.Load<AudioClip>("Sound/SoundEffect/Artifact_Get");
    }
	
	//이미지 갱신
	void Update () {
        //이미지 true 이면
        if (artifact_image.gameObject.activeSelf)
        {
            //0보다 아래일때 색 / 위치++
            if (artifact_image.rectTransform.anchoredPosition.y < 0)
            {
                //색
                color.a += 1.0f / (-init_y / up_speed);
                artifact_image.color = color;

                //위치
                position.y += up_speed;
                artifact_image.rectTransform.anchoredPosition = position;
            }
            else
            {
                if (delay_timer.timer < delay_timer.term)
                    delay_timer.timer += Time.deltaTime;
                else
                {
                    artifact_image.gameObject.SetActive(false);
                }
            }
        }
	}

    //아티펙트 보여주기
    public void ShowArtifact(Sprite _sprite)
    {
        //스프라이트 변경
        artifact_image.sprite = _sprite;
        //색 변경
        color.a = init_alpha;
        artifact_image.color = color;
        //위치 변경
        position.y = init_y;
        artifact_image.rectTransform.anchoredPosition = position;
        //타이머 초기화
        delay_timer.timer = 0;
        //획득 사운드
        audio_source.PlayOneShot(get_sound);
        //true
        artifact_image.gameObject.SetActive(true);
    }
}
