using UnityEngine;
using System.Collections;

public class SPBossSpawn : MonoBehaviour {

    //참조
    private SystemDAO.GameManagerData gm_data;
    private CameraFollow main_camera_follow;
    private Transform main_camera_trans;

    //컴포넌트
    private ParticleSystem particle_system;

    //속도값
    private Vector2 velocity;

    //스무스값
    public float smooth_time_x = 0.05f;
    public float smooth_time_y = 0.05f;

    //초기화
    void Awake()
    {
        gm_data = SystemDAO.instance.gm_data;
        main_camera_follow = Camera.main.GetComponent<CameraFollow>();
        main_camera_trans = Camera.main.transform;
        particle_system = GetComponent<ParticleSystem>();
    }

    //켜질때 설정
    void OnEnable()
    {
        gm_data.state = SystemDAO.GameManagerData.State.isPause;
        main_camera_follow.enabled = false;
    }

    void Update()
    {
        //경계 못넘어가도록
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 0, 0), transform.position.z);

        if (particle_system.isPlaying)
        {
            //카메라 부드럽게 움직이도록 조절
            float posX = Mathf.SmoothDamp(main_camera_trans.position.x, transform.position.x, ref velocity.x, smooth_time_x);
            float posY = Mathf.SmoothDamp(main_camera_trans.position.y, transform.position.y, ref velocity.y, smooth_time_y);
            float posZ = main_camera_trans.position.z;

            //조절한 값을 넣어준다.
            main_camera_trans.position = new Vector3(posX, posY, posZ);
        }
        else
        {
            gm_data.state = SystemDAO.GameManagerData.State.isPlay;
            main_camera_follow.enabled = true;
            gameObject.SetActive(false);
        }
    }
}
