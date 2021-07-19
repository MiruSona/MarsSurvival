using UnityEngine;
using System.Collections;

public class Tool : SingleTon<Tool> {

    //참조
    private LineRenderer line_renderer;
    private SpriteRenderer sprite_renderer;
    private GameDAO.PlayerData player_data;
    private GameDAO.ToolData tool_data;
    private GameObject tool_particle;

    //사운드
    private AudioSource audio_source;
    private AudioClip gather_sound;
    private GameDAO.Timer sound_timer;

    //레이저 거리계산
    private float distance = 0f;                        //대상과 자신사이 거리
    private Vector3 my_position = Vector3.zero;         //자신 위치
    private Vector3 target_position = Vector3.zero;     //대상 위치
    private float length = 0f;                          //길이
    private float length_mul = 1f;                      //길이 길어지는 값
    private float x_my_offset = 0.3f;                   //시작 위치 x값 오프셋
    private float y_target_offset = -0.15f;               //도달 위치 y값 오프셋

    //크기조절
    private Vector3 scale = new Vector3(1f,1f,1f);
    
    //초기화
    void Awake() {
        //참조
        player_data = GameDAO.instance.player_data;
        tool_data = player_data.tool_data;

        //스프라이트
        sprite_renderer = GetComponent<SpriteRenderer>();

        //라인 렌더러
        line_renderer = GetComponent<LineRenderer>();
        line_renderer.sortingLayerName = "Tool&Weapon";

        //파티클
        tool_particle = transform.FindChild("Tool_Particle").gameObject;

        //모양 초기화
        UpdateShape();

        //사운드
        sound_timer.term = 0.5f;    //소리 간격
        sound_timer.timer = 0f;     //소리 타이머

        audio_source = GetComponent<AudioSource>();
        gather_sound = Resources.Load<AudioClip>("Sound/SoundEffect/Gather");

        gameObject.SetActive(false);
    }

    //활성화 시
    void OnEnable() {
        //레이저 길이 초기화
        length = 0f;

        //자신 위치 초기화
        my_position = transform.position;
        my_position.x += x_my_offset * player_data.facing;  //오프셋
        
        //타겟 위치 초기화         
        target_position = player_data.tool_data.target_position;
        target_position.y += y_target_offset;   //오프셋

        //파티클 위치 초기화
        tool_particle.transform.position = target_position;

        //자신과 타겟 사이 거리 초기화
        distance = Vector3.Distance(my_position, target_position);

        //모양 업데이트
        UpdateShape();
    }

    //비활성화 시
    void OnDisable() {
        //레이저 튀어나가는거 방지
        line_renderer.enabled = false;
        //파티클 중지
        tool_particle.SetActive(false);
    }

    //모양 설정(스프라이트 및 라인랜더러 / 파티클 설정)
    public void UpdateShape()
    {
        //스프라이트 변경
        sprite_renderer.sprite = tool_data.sprites[tool_data.step];

        //자신 크기 변경
        switch (tool_data.step)
        {
            case 0:
                scale.x = 1f;
                scale.y = 1f;
                break;
            case 1:
                scale.x = 1f;
                scale.y = 1f;
                break;
            case 2:
                scale.x = 1.2f;
                scale.y = 1.2f;
                break;
        }
        transform.localScale = scale;

        //라인 랜더러 변경
        line_renderer.SetColors(tool_data.colors[tool_data.step], tool_data.colors[tool_data.step]);
        line_renderer.SetWidth(tool_data.line_width[tool_data.step], tool_data.line_width[tool_data.step]);

        //파티클 변경
        tool_particle.GetComponent<ParticleSystem>().startColor = tool_data.colors[tool_data.step];
    }

    //도구 처리
    void FixedUpdate() {
        //플레이어가 채취 상태일 때만 작동하도록
        if (player_data.movement != GameDAO.PlayerData.Movement.isGatherSchedule)
            if (player_data.movement != GameDAO.PlayerData.Movement.isRepairSchedule)
                return;

        //사운드
        if (sound_timer.timer < sound_timer.term / tool_data.speed[tool_data.step]) {
            sound_timer.timer += Time.deltaTime;
        } else {
            audio_source.PlayOneShot(gather_sound);
            sound_timer.timer = 0;
        }

        //레이저 발사
        if (length < distance) {
            //길이값 계산
            length += 0.1f / length_mul * tool_data.speed[tool_data.step];    //길이값
            float x = Mathf.Lerp(0, distance, length);  //실제 길이값
            //실제 길이값으로 레이저 위치 계산
            Vector3 laser_point = x * Vector3.Normalize(target_position - my_position) + my_position;

            //라인 랜더러 그리기 시작 위치 설정
            line_renderer.SetPosition(0, my_position);

            //라인 랜더러 그리기 레이저 위치 설정
            line_renderer.SetPosition(1, laser_point);

            //라인 랜더러 켜기
            if (line_renderer.enabled == false)
                line_renderer.enabled = true;
        } else {
            //레이저 최대 길이면 파티클 켜기
            tool_particle.SetActive(true);
        }

    }
}
