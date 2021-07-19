using UnityEngine;
using System.Collections;

public class PoisonTrap : ParticleCommon{

    //참조
    private SystemDAO.GameManagerData gm_data;
    private GameDAO.PlayerData player_data;
    private CircleCollider2D c_collider;

    //사운드
    private AudioSource audio_source;
    private AudioClip poison_sound;

    //collider 크기 조절
    private const float size_max = 8.0f;
    private const float size_add = 0.12f;
    
    //빠른 초기화
    void Awake()
    {
        gm_data = SystemDAO.instance.gm_data;
        player_data = GameDAO.instance.player_data;
        audio_source = GetComponent<AudioSource>();
        poison_sound = Resources.Load<AudioClip>("Sound/SoundEffect/PoisonGas");
    }

    //중독시 파기
    void FixedUpdate()
    {
        if (player_data.state == GameDAO.PlayerData.State.isPoisoned)
            Destroy(gameObject);
    }

    //상속
    //초기화
    protected override void ChildInit()
    {
        c_collider = GetComponent<CircleCollider2D>();
        audio_source.PlayOneShot(poison_sound);
    }

    //collider가 계속 커지게
    protected override void ChildUpdate()
    {
        if (!particle_system.isStopped)
        {
            if (c_collider.radius < size_max)
                c_collider.radius += size_add;
        }
    }

    //닿으면 중독상태로 변경
    void OnTriggerStay2D(Collider2D _col)
    {
        if (_col.CompareTag("Player"))
        {
            if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
                return;

            if(player_data.state == GameDAO.PlayerData.State.isAlive || player_data.state == GameDAO.PlayerData.State.isDamaged)
            {
                player_data.state = GameDAO.PlayerData.State.isPoisoned;
                gm_data.state = SystemDAO.GameManagerData.State.isPause;
            }                             
        }
    }
}
