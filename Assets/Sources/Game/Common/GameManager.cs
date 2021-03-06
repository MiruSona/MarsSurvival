using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections.Generic;

public class GameManager : SingleTon<GameManager> {

    #region 참조
    private SystemDAO.FileManager file_manager;
    private SystemDAO.GameManagerData gm_data;
    private SystemDAO.MapManager map_manager;
    private SystemDAO.GlobalState global_state;
    private SystemDAO.StormData storm_data;
    private GameDAO.PlayerData player_data;
    private GameDAO.SpaceShipData spaceship_data;
    private GameDAO.ArtifactData artifact_data;
    private GameDAO.BuffData buff_data;
    private GameObject ending_menu;
    private GameObject die_menu;
    private GameObject health_warning;
    private GameObject help_menu;
    private GameObject build_menu;
    private GameObject build_armor;
    private GameObject upgrade_menu;
    private GameObject review_menu;
    private GameObject give_crystal_menu;
    private GameObject pause_menu;
    private GameObject crystal_panel;
    private GameObject metalbio_panel;
    private AudioSource audio_source;
    #endregion

    #region 배경음
    private AudioClip bg_sound;
    private AudioClip storm_sound;
    #endregion

    #region 빌드
    private List<int> build_index = new List<int>();              //인덱스
    private GameObject[] build_spawn = new GameObject[Define.build_num_limit];  //소환된 리스트
    [System.NonSerialized]
    public List<GameDAO.BuildData> build_spawn_datas = new List<GameDAO.BuildData>();   //소환된 리스트의 데이터들
    [System.NonSerialized]
    public bool turret_empty = false;
    [System.NonSerialized]
    public bool shield_empty = false;
    [System.NonSerialized]
    public bool mine_empty = false;

    //프리팹
    private GameObject turret_prefab;   //터렛
    private GameObject shield_prefab;   //실드
    private GameObject mine_prefab;     //지뢰

    //풀
    private Stack<GameObject> turret_pool = new Stack<GameObject>(); //터렛
    private Stack<GameObject> shield_pool = new Stack<GameObject>(); //실드
    private Stack<GameObject> mine_pool = new Stack<GameObject>();  //지뢰 
    #endregion

    #region 자원
    #region 자원 프리팹
    private GameObject rock_small_prefab;
    private GameObject tree_small_prefab;
    private GameObject rock_medium_prefab;
    private GameObject tree_medium_prefab;
    private GameObject rock_large_prefab;
    private GameObject tree_large_prefab;
    #endregion

    #region 자원 풀
    private Substance rock_substance, tree_substance;
    private GameObject[] rock_small_list = new GameObject[Define.sub_small_num_half];       //작은 돌 리스트
    private GameObject[] tree_small_list = new GameObject[Define.sub_small_num_half];       //작은 나무 리스트
    private GameObject[] rock_medium_list = new GameObject[Define.sub_medium_num_half];     //중간 돌 리스트
    private GameObject[] tree_medium_list = new GameObject[Define.sub_medium_num_half];     //중간 나무 리스트
    private GameObject[] rock_large_list = new GameObject[Define.sub_large_num_half];       //큰 돌 리스트
    private GameObject[] tree_large_list = new GameObject[Define.sub_large_num_half];       //큰 나무 리스트 
    #endregion

    #endregion

    #region 큰 자원
    //큰 자원 프리펩
    private GameObject big_sub_prefab;

    //큰 자원 풀
    [System.NonSerialized]
    public GameObject[] big_sub_pool = new GameObject[Define.big_substance_num];

    //큰 자원 소환된것들 풀
    private List<GameObject> big_sub_spawned = new List<GameObject>();
    #endregion

    #region 몬스터
    private int monster_spawn_num = 0;  //현재 소환 수
    private Stack<int> monster_index_list = new Stack<int>();   //인덱스 스텍
    [System.NonSerialized]
    public GameObject[] monster_spawn_list = new GameObject[Define.monster_spawn_num_limit];   //소환된 몬스터 리스트

    #region 몬스터 프리팹
    private GameObject slime_weak_melee_prefab;
    private GameObject slime_weak_range_prefab;
    private GameObject slime_normal_melee_prefab;
    private GameObject slime_normal_range_prefab;
    private GameObject slime_strong_melee_prefab;
    private GameObject slime_strong_range_prefab;

    private GameObject larva_weak_prefab;
    private GameObject larva_normal_prefab;

    private GameObject sandmonster_weak_prefab;
    private GameObject sandmonster_normal_prefab;
    #endregion

    #region 몬스터 풀
    private Stack<GameObject> slime_weak_melee_pool = new Stack<GameObject>();
    private Stack<GameObject> slime_weak_range_pool = new Stack<GameObject>();
    private Stack<GameObject> slime_normal_melee_pool = new Stack<GameObject>();
    private Stack<GameObject> slime_normal_range_pool = new Stack<GameObject>();
    private Stack<GameObject> slime_strong_melee_pool = new Stack<GameObject>();
    private Stack<GameObject> slime_strong_range_pool = new Stack<GameObject>();

    private Stack<GameObject> larva_weak_pool = new Stack<GameObject>();
    private Stack<GameObject> larva_normal_pool = new Stack<GameObject>();

    private Stack<GameObject> sandmonster_weak_pool = new Stack<GameObject>();
    private Stack<GameObject> sandmonster_normal_pool = new Stack<GameObject>();
    #endregion

    #region 몬스터 소환 확률
    private GameDAO.MonsterSpawnRate total_rate = new GameDAO.MonsterSpawnRate();
    private GameDAO.MonsterSpawnRate slime_total_rate = new GameDAO.MonsterSpawnRate();
    private GameDAO.MonsterSpawnRate other_total_rate = new GameDAO.MonsterSpawnRate();

    private GameDAO.MonsterSpawnRate slime_weak_melee_rate = new GameDAO.MonsterSpawnRate();
    private GameDAO.MonsterSpawnRate slime_weak_range_rate = new GameDAO.MonsterSpawnRate();
    private GameDAO.MonsterSpawnRate slime_normal_melee_rate = new GameDAO.MonsterSpawnRate();
    private GameDAO.MonsterSpawnRate slime_normal_range_rate = new GameDAO.MonsterSpawnRate();
    private GameDAO.MonsterSpawnRate slime_strong_melee_rate = new GameDAO.MonsterSpawnRate();
    private GameDAO.MonsterSpawnRate slime_strong_range_rate = new GameDAO.MonsterSpawnRate();

    private GameDAO.MonsterSpawnRate larva_weak_rate = new GameDAO.MonsterSpawnRate();
    private GameDAO.MonsterSpawnRate larva_normal_rate = new GameDAO.MonsterSpawnRate();

    private GameDAO.MonsterSpawnRate sandmonster_weak_rate = new GameDAO.MonsterSpawnRate();
    private GameDAO.MonsterSpawnRate sandmonster_normal_rate = new GameDAO.MonsterSpawnRate();
    #endregion

    #endregion

    #region 보스

    //프리펩
    private GameObject rock_boss_prefab;
    private GameObject tree_boss_prefab;

    private GameObject alien_boss_prefab;
    private GameObject jellyfish_boss_prefab;
    private GameObject sandworm_boss_prefab;

    //전체 스택
    private Queue<GameObject> boss_pool_queue = new Queue<GameObject>();
    private GameObject[] sp_boss_pool = new GameObject[Define.sp_boss_kind_num]; 

    //현재 소환된 보스
    [System.NonSerialized]
    public GameObject boss_spawn = null;
    [System.NonSerialized]
    public GameObject sp_boss_spawn = null;

    //보스 소환 여부
    [System.NonSerialized]
    public bool boss_left_spawned = false;
    [System.NonSerialized]
    public bool boss_right_spawned = false;
    [System.NonSerialized]
    public int sp_boss_step = 0;

    #endregion

    //쿨타임
    private GameDAO.Timer substance_regen_timer = new GameDAO.Timer(0, Define.Day_Cycle);
    private GameDAO.Timer monster_regen_timer = new GameDAO.Timer(0, Define.monster_regen_term);
    private GameDAO.Timer boss_regen_timer = new GameDAO.Timer(0, Define.boss_regen_term);
    private GameDAO.Timer big_sub_regen_timer = new GameDAO.Timer(0, Define.big_substance_regen_term);
    [System.NonSerialized]
    public GameDAO.Timer sp_boss_regen_timer = new GameDAO.Timer(0, Define.sp_boss_regen_term);

    //세이브 여부
    private bool check_save = false;
    private bool check_end_save = false;

    //리뷰 본지 여부
    private bool check_review = false;

    //카메라 초기화
    void Awake() {
        //카메라값 초기화
        Define.camera_height = 2f * Camera.main.orthographicSize;
        Define.camera_width = 2f * Camera.main.orthographicSize * Camera.main.aspect;
        Define.camera_height_half = Camera.main.orthographicSize;
        Define.camera_width_half = Camera.main.orthographicSize * Camera.main.aspect;
    }

    //초기화
    void Start() {

        //FPS 고정
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        #region 참조
        file_manager = SystemDAO.instance.file_manager;
        gm_data = SystemDAO.instance.gm_data;
        map_manager = SystemDAO.instance.map_manager;
        global_state = SystemDAO.instance.global_state;
        storm_data = SystemDAO.instance.storm_data;
        player_data = GameDAO.instance.player_data;
        spaceship_data = GameDAO.instance.spaceship_data;
        artifact_data = GameDAO.instance.artifact_data;
        buff_data = GameDAO.instance.buff_data;
        ending_menu = transform.FindChild("UIManager").FindChild("UICanvas").FindChild("EndingPanel").gameObject;
        die_menu = transform.FindChild("UIManager").FindChild("UICanvas").FindChild("DieMenu").gameObject;
        health_warning = transform.FindChild("UIManager").FindChild("UICanvas").FindChild("HealthWarnig").gameObject;
        help_menu = transform.FindChild("UIManager").FindChild("UICanvas").FindChild("HelpMenu").gameObject;
        build_menu = transform.FindChild("UIManager").FindChild("UICanvas").FindChild("BuildMenu").gameObject;
        build_armor = transform.FindChild("UIManager").FindChild("UICanvas").FindChild("BuildMenu").FindChild("Armor_Pop").gameObject;
        upgrade_menu = transform.FindChild("UIManager").FindChild("UICanvas").FindChild("UpgradeMenu").gameObject;
        review_menu = transform.FindChild("UIManager").FindChild("UICanvas").FindChild("ReviewMenu").gameObject;
        give_crystal_menu = transform.FindChild("UIManager").FindChild("UICanvas").FindChild("GiveMenu").gameObject;
        pause_menu = transform.FindChild("UIManager").FindChild("UICanvas").FindChild("PauseMenu").gameObject;
        crystal_panel = transform.FindChild("UIManager").FindChild("UICanvas").FindChild("CrystalPanel").gameObject;
        metalbio_panel = transform.FindChild("UIManager").FindChild("UICanvas").FindChild("MetalBioPanel").gameObject;
        audio_source = GetComponent<AudioSource>();
        #endregion

        #region 배경음 초기화
        bg_sound = Resources.Load<AudioClip>("Sound/BackgroundSound/WindBGM");
        storm_sound = Resources.Load<AudioClip>("Sound/BackgroundSound/StormWindBGM");
        #endregion

        #region 프리팹 초기화
        //빌드
        turret_prefab = Resources.Load("Prefab/Game/Build/Turret") as GameObject;
        shield_prefab = Resources.Load("Prefab/Game/Build/Shield") as GameObject;
        mine_prefab = Resources.Load("Prefab/Game/Build/Mine") as GameObject;

        //몬스터
        slime_weak_melee_prefab = Resources.Load("Prefab/Game/Monster/Slime_Weak_Melee") as GameObject;
        slime_weak_range_prefab = Resources.Load("Prefab/Game/Monster/Slime_Weak_Range") as GameObject;
        slime_normal_melee_prefab = Resources.Load("Prefab/Game/Monster/Slime_Normal_Melee") as GameObject;
        slime_normal_range_prefab = Resources.Load("Prefab/Game/Monster/Slime_Normal_Range") as GameObject;
        slime_strong_melee_prefab = Resources.Load("Prefab/Game/Monster/Slime_Strong_Melee") as GameObject;
        slime_strong_range_prefab = Resources.Load("Prefab/Game/Monster/Slime_Strong_Range") as GameObject;

        larva_weak_prefab = Resources.Load("Prefab/Game/Monster/Larva_Weak") as GameObject;
        larva_normal_prefab = Resources.Load("Prefab/Game/Monster/Larva_Normal") as GameObject;

        sandmonster_weak_prefab = Resources.Load("Prefab/Game/Monster/SandMonster_Weak") as GameObject;
        sandmonster_normal_prefab = Resources.Load("Prefab/Game/Monster/SandMonster_Normal") as GameObject;

        //보스
        rock_boss_prefab = Resources.Load("Prefab/Game/Boss/Rock_Boss") as GameObject;
        tree_boss_prefab = Resources.Load("Prefab/Game/Boss/Tree_Boss") as GameObject;

        alien_boss_prefab = Resources.Load("Prefab/Game/SPBoss/Alien_Boss") as GameObject;
        jellyfish_boss_prefab = Resources.Load("Prefab/Game/SPBoss/Jellyfish_Boss") as GameObject;
        sandworm_boss_prefab = Resources.Load("Prefab/Game/SPBoss/Sandworm_Boss") as GameObject;

        //자원
        rock_small_prefab = Resources.Load("Prefab/Game/Substance/Rock_Small") as GameObject;
        tree_small_prefab = Resources.Load("Prefab/Game/Substance/Tree_Small") as GameObject;
        rock_medium_prefab = Resources.Load("Prefab/Game/Substance/Rock_Medium") as GameObject;
        tree_medium_prefab = Resources.Load("Prefab/Game/Substance/Tree_Medium") as GameObject;
        rock_large_prefab = Resources.Load("Prefab/Game/Substance/Rock_Large") as GameObject;
        tree_large_prefab = Resources.Load("Prefab/Game/Substance/Tree_Large") as GameObject;

        //큰 자원
        big_sub_prefab = Resources.Load("Prefab/Game/Substance/BigTreeMetal") as GameObject;
        #endregion

        //땅 위치 초기화
        Transform ground_transform = GameObject.FindWithTag("Ground").transform;
        float ground_local_position = ground_transform.localPosition.y;
        float ground_local_scale = ground_transform.localScale.y;
        float ground_hight = Resources.Load<Sprite>("Sprite/Game/Background/Ground_v0.2").bounds.size.y / 2;

        Define.ground_position = ground_local_position + (ground_hight * ground_local_scale);

        //위치 로드 or 랜덤 위치 생성
        map_manager.InitPoints();

        //초기화       
        InitSubstance();
        InitBigSubstance();
        InitMonsters();
        InitBoss();
        InitSPBoss();
        InitBuilds();
    }

    //백키 누를때
    void Update() {

        //백키 누르면 일시정지메뉴 && 다른 메뉴들은 꺼짐
        if (Input.GetKeyDown(KeyCode.Escape) && gm_data.state == SystemDAO.GameManagerData.State.isPlay) {
            if (pause_menu.activeSelf & !help_menu.activeSelf) {
                pause_menu.SetActive(false);
                Time.timeScale = 1;

            } else if (!build_menu.activeSelf & !upgrade_menu.activeSelf 
                        & !help_menu.activeSelf & !crystal_panel.activeSelf 
                        & !metalbio_panel.activeSelf & !give_crystal_menu.activeSelf) {

                pause_menu.SetActive(true);
                Time.timeScale = 0;
            }

            if (build_menu.activeSelf) {
                if(build_armor.activeSelf) {
                    build_armor.SetActive(false);
                }else {
                    build_menu.SetActive(false);
                }
                Time.timeScale = 1;
            }

            if (upgrade_menu.activeSelf) {
                upgrade_menu.SetActive(false);
                Time.timeScale = 1;
            }

            if (help_menu.activeSelf) {
                help_menu.SetActive(false);
                if (!pause_menu.activeSelf)
                    Time.timeScale = 1;
            }
            if (crystal_panel.activeSelf) {
                crystal_panel.SetActive(false);
                if (!pause_menu.activeSelf)
                    Time.timeScale = 1;
            }
            if (metalbio_panel.activeSelf) {
                metalbio_panel.SetActive(false);
                if (!pause_menu.activeSelf)
                    Time.timeScale = 1;
            }
            if (give_crystal_menu.activeSelf)
            {
                give_crystal_menu.SetActive(false);
                if (!pause_menu.activeSelf)
                    Time.timeScale = 1;
            }
        }
    }

    //관리
    void FixedUpdate() {

        switch (gm_data.state) {
            //초기화(로드)
            case SystemDAO.GameManagerData.State.isInit:

                #region 초기화
                //로드
                file_manager.LoadAll();

                //시간 초기화
                global_state.Init();
                //폭풍 체크
                storm_data.StormCycle();
                //플레이 타임 체크
                global_state.CheckGlobalTime(storm_data.enable);

                //플레이어 위치 이동
                Transform player_trans = Player.instance.transform;
                Transform main_camera_trans = Camera.main.transform;
                player_trans.position = map_manager.GetPlayerRealPoint(-2.0f);
                main_camera_trans.position = new Vector3(player_trans.position.x, main_camera_trans.position.y, main_camera_trans.position.z);

                //빌드 소환
                //인덱스 제거 , 소환 및 데이터 & 위치 초기화
                for (int i = 0; i < build_spawn_datas.ToArray().Length; i++) {
                    build_index.Remove(build_spawn_datas[i].index);
                    LoadBuild(build_spawn_datas[i]);
                }

                //몬스터 소환
                LoadMonster();

                //보스 소환
                LoadBoss();

                //특수 보스 소환
                LoadSPBoss();

                //큰 자원 소환
                LoadBigSubstance();

                //아티펙트 체크
                CheckArtifact();

                //옵션 적용
                if (file_manager.option_data.sound)
                    AudioListener.volume = 1;
                else
                    AudioListener.volume = 0;

                //뉴게임일 경우 튜토리얼 보여주기 + 인트로
                if (!file_manager.have_save) {
                    Time.timeScale = 0;
                    help_menu.SetActive(true);
                }

                //광고 보석 지급
                if (file_manager.ad_data.look_ad_num > 0) {
                    player_data.my_subdata.crystal += file_manager.ad_data.shown_num;
                    file_manager.ad_data.look_ad_num = 0;
                    file_manager.SaveADData();
                }

                //플레이
                if (player_data.state == GameDAO.PlayerData.State.isPoisoned)
                    gm_data.state = SystemDAO.GameManagerData.State.isPause;
                else
                    gm_data.state = SystemDAO.GameManagerData.State.isPlay;


                //쿠폰 체크
                MarketService.instance.CheckPurchasedItem("test_crystal");
                MarketService.instance.CheckPurchasedItem("crystal_medium");
                MarketService.instance.CheckPurchasedItem("crystal_large");
                #endregion

                break;

            //플레이
            case SystemDAO.GameManagerData.State.isPlay:

                #region 플레이

                //폭풍 체크
                storm_data.StormCycle();
                //플레이 타임 체크
                global_state.CheckGlobalTime(storm_data.enable);

                //플레이어 체력 체크(체력 경고)
                if (!file_manager.see_hp_warnig && file_manager.ad_data.ad_rebirth)
                    if (player_data.hp / player_data.max_hp <= 0.2f)
                        health_warning.SetActive(true);


                #region 세이브 체크
                if (global_state.day_num % Define.Save_Day_Cycle == 0) {
                    if (!check_save) {
                        file_manager.SaveAll();
                        check_save = true;
                    }
                } else {
                    if (check_save)
                        check_save = false;
                }
                #endregion

                #region 리뷰 체크
                //day 체크
                if (global_state.day_num % Define.Review_Cycle == 0 && global_state.day_num > Define.Secotr1_Open_Day) {
                    //1초 까지만
                    if (global_state.real_time < (global_state.day_num - 1) * Define.DayCount_Cycle + 1f) {
                        //리뷰 봤는지 여부
                        if (!check_review && !file_manager.see_review) {
                            //리뷰 켜기
                            review_menu.SetActive(true);
                            check_review = true;
                        }
                    }
                } else {
                    if (check_review)
                        check_review = false;
                }
                #endregion

                #region 크리스탈 주는 날 체크
                //day 체크
                if (global_state.day_num == Define.Give_Crystal_Day) {
                    //1초 까지만
                    if (global_state.real_time < (global_state.day_num - 1) * Define.DayCount_Cycle + 1f) {
                        //메뉴 봤는지 여부
                        if (!file_manager.give_crystal) {
                            //메뉴 켜기
                            give_crystal_menu.SetActive(true);
                            check_review = true;
                        }
                    }
                }
                #endregion

                #region 버프체크
                CheckBuff();
                #endregion

                #region 배경음 조절
                switch (global_state.state) {
                    case Define.Day:
                    case Define.Night:
                        if (audio_source.clip != bg_sound) {
                            audio_source.clip = bg_sound;
                            audio_source.Play();
                        }
                        break;

                    case Define.DayStorm:
                    case Define.NightStorm:
                        if (audio_source.clip != storm_sound) {
                            audio_source.clip = storm_sound;
                            audio_source.Play();
                        }
                        break;
                }
                #endregion

                #region 아티펙트

                #endregion

                #region 빌드
                //pool 체크
                if (turret_pool.ToArray().Length == 0)
                    turret_empty = true;
                else
                    turret_empty = false;

                if (shield_pool.ToArray().Length == 0)
                    shield_empty = true;
                else
                    shield_empty = false;

                if (mine_pool.ToArray().Length == 0)
                    mine_empty = true;
                else
                    mine_empty = false;

                //Enable
                for (int i = 0; i < Define.build_num_limit; i++) {
                    if (build_spawn[i] != null)
                        if (map_manager.CheckEnableBuild(i))
                            build_spawn[i].SetActive(true);
                }

                //Destroy
                for (int i = 0; i < Define.build_num_limit; i++) {
                    if (build_spawn[i] != null)
                        if (map_manager.CheckDestroyBuild(i))
                            build_spawn[i].GetComponent<BuildCommon>().DestroyWithDisable();
                }

                #endregion

                #region 몬스터 리젠

                //몬스터 리젠 쿨타임
                if (monster_regen_timer.timer < monster_regen_timer.term) {
                    //몬스터가 꽉 차지 않을 경우에만 돌아감
                    if (monster_spawn_num < Define.monster_spawn_num_limit && monster_index_list.ToArray().Length != 0)
                        monster_regen_timer.timer += Time.deltaTime;
                    else
                        monster_regen_timer.timer = 0;
                } else {
                    //기본 리젠
                    RegenMonsters();
                }

                #endregion

                #region 보스 리젠
                //보스 리젠 쿨타임
                if (global_state.level == 1) {
                    if (boss_regen_timer.timer < boss_regen_timer.term)
                        boss_regen_timer.timer += Time.fixedDeltaTime;
                    else {
                        //보스 소환 가능한지 체크
                        if (map_manager.CheckSpawnBoss())
                            RegenBoss();
                    }
                }
                if (global_state.level == 2) {
                    if (boss_regen_timer.timer < boss_regen_timer.term - Define.boss_regen_advance)
                        boss_regen_timer.timer += Time.fixedDeltaTime;
                    else {
                        //보스 소환 가능한지 체크
                        if (map_manager.CheckSpawnBoss())
                            RegenBoss();
                    }
                }
                #endregion

                #region 특수 보스 리젠
                //우주선 수리 단계에 따라 소환
                if (2 <= spaceship_data.step && spaceship_data.step < Define.spaceship_step)
                {
                    //현재 수리단계랑 보스 단계랑 맞는지 체크
                    if (spaceship_data.step >= sp_boss_step)
                    {
                        //타이머
                        if (sp_boss_regen_timer.timer < sp_boss_regen_timer.term)
                            sp_boss_regen_timer.timer += Time.fixedDeltaTime;
                        else
                        {
                            RegenSPBoss();
                            sp_boss_regen_timer.timer = 0;
                        }
                    }
                }
                #endregion

                #region 자원 리젠

                //자원 리젠 쿨타임
                if (substance_regen_timer.timer < substance_regen_timer.term)
                    substance_regen_timer.timer += Time.deltaTime;

                for (int i = 0; i < Define.sub_small_num_half; i++) {
                    rock_substance = rock_small_list[i].GetComponent<Substance>();
                    tree_substance = tree_small_list[i].GetComponent<Substance>();

                    //리젠 시간 되면 파괴여부 확인 후 리젠
                    if (substance_regen_timer.timer >= substance_regen_timer.term) {
                        if (rock_substance.destroyed)
                            rock_substance.RegenSubstance();

                        if (tree_substance.destroyed)
                            tree_substance.RegenSubstance();
                    }

                    //enable 범위 안에 들어오면 Active -> true
                    if (map_manager.CheckEnableSubstance(rock_substance.index, rock_substance.sector))
                        rock_small_list[i].SetActive(true);
                    if (map_manager.CheckEnableSubstance(tree_substance.index, tree_substance.sector))
                        tree_small_list[i].SetActive(true);

                    //disable 범위 밖으로 나가면 Active -> false
                    if (map_manager.CheckDisableSubstance(rock_substance.index, rock_substance.sector))
                        rock_small_list[i].SetActive(false);
                    if (map_manager.CheckDisableSubstance(tree_substance.index, tree_substance.sector))
                        tree_small_list[i].SetActive(false);
                }

                for (int i = 0; i < Define.sub_medium_num_half; i++) {
                    rock_substance = rock_medium_list[i].GetComponent<Substance>();
                    tree_substance = tree_medium_list[i].GetComponent<Substance>();

                    if (substance_regen_timer.timer >= substance_regen_timer.term) {
                        if (rock_substance.destroyed)
                            rock_substance.RegenSubstance();

                        if (tree_substance.destroyed)
                            tree_substance.RegenSubstance();
                    }

                    if (map_manager.CheckEnableSubstance(rock_substance.index, rock_substance.sector))
                        rock_medium_list[i].SetActive(true);
                    if (map_manager.CheckEnableSubstance(tree_substance.index, tree_substance.sector))
                        tree_medium_list[i].SetActive(true);

                    if (map_manager.CheckDisableSubstance(rock_substance.index, rock_substance.sector))
                        rock_medium_list[i].SetActive(false);
                    if (map_manager.CheckDisableSubstance(tree_substance.index, tree_substance.sector))
                        tree_medium_list[i].SetActive(false);
                }

                for (int i = 0; i < Define.sub_large_num_half; i++) {
                    rock_substance = rock_large_list[i].GetComponent<Substance>();
                    tree_substance = tree_large_list[i].GetComponent<Substance>();

                    if (substance_regen_timer.timer >= substance_regen_timer.term) {
                        if (rock_substance.destroyed)
                            rock_substance.RegenSubstance();

                        if (tree_substance.destroyed)
                            tree_substance.RegenSubstance();
                    }

                    if (map_manager.CheckEnableSubstance(rock_substance.index, rock_substance.sector))
                        rock_large_list[i].SetActive(true);
                    if (map_manager.CheckEnableSubstance(tree_substance.index, tree_substance.sector))
                        tree_large_list[i].SetActive(true);

                    if (map_manager.CheckDisableSubstance(rock_substance.index, rock_substance.sector))
                        rock_large_list[i].SetActive(false);
                    if (map_manager.CheckDisableSubstance(tree_substance.index, tree_substance.sector))
                        tree_large_list[i].SetActive(false);
                }

                //쿨타임 다되면 초기화
                if (substance_regen_timer.timer >= substance_regen_timer.term)
                    substance_regen_timer.timer = 0;
                #endregion

                #region 큰 자원 리젠
                //자원이 하나라도 생성됬으면 실행X
                if (big_sub_spawned.ToArray().Length == 0) {
                    //큰 자원 리젠 쿨타임
                    if (big_sub_regen_timer.timer < big_sub_regen_timer.term)
                        big_sub_regen_timer.timer += Time.fixedDeltaTime;
                    else {
                        RegenBigSubstance();
                    }
                }

                //큰 자원 Enable
                if (big_sub_spawned.ToArray().Length != 0) {
                    for (int i = 0; i < Define.big_substance_num; i++) {
                        if (map_manager.CheckEnableBigSubstance(i))
                            if (big_sub_spawned.Contains(big_sub_pool[i]))
                                big_sub_pool[i].SetActive(true);
                    }
                }

                #endregion

                #endregion

                break;

            //일시정지
            case SystemDAO.GameManagerData.State.isPause:


                break;

            //죽음
            case SystemDAO.GameManagerData.State.isDead:

                file_manager.SaveAll();
                for (int i = 0; i < monster_spawn_list.Length; i++) {
                    if (monster_spawn_list[i] != null)
                        monster_spawn_list[i].GetComponent<MonsterCommon>().DieWithDisable();
                }
                die_menu.SetActive(true);
                break;

            //엔딩
            case SystemDAO.GameManagerData.State.isEnd:

                //세이브 여부 확인
                if (!check_end_save) {
                    //클리어 횟수 +1
                    file_manager.clear_num++;

                    //클리어 횟수 및 아티펙트 저장
                    file_manager.SaveArtifact();

                    //파일 삭제
                    file_manager.DeleteBase();

                    check_end_save = true;
                }

                //엔딩 실행
                ending_menu.SetActive(true);

                break;
        }
    }

    //랜덤 인덱스 반환
    private int GetRandomIndex(List<int> _index_list) {
        //랜덤값 생성
        int random = Random.Range(0, _index_list.ToArray().Length);
        int send_index = 0;

        //리스트에서 랜덤위치의 값을 저장
        send_index = _index_list[random];
        //랜덤위치의 값을 제거
        _index_list.RemoveAt(random);

        //보냄
        return send_index;
    }

    //아티펙트 체크
    private void CheckArtifact() {
        for (int i = 0; i < artifact_data.artifcat.Length; i++) {
            if (artifact_data.artifcat[i].activate) {
                switch (artifact_data.artifcat[i].name) {
                    //체력 20%
                    case "Diary":
                        player_data.ArtifactDiary(artifact_data.artifcat[i].value);
                        break;
                    //공격 50%
                    case "GraveStone":
                        player_data.weapon_data.ArtifactGraveStone(artifact_data.artifcat[i].value);
                        break;
                    //이속 20%
                    case "FootPrint":
                        player_data.ArtifactFootPrint(artifact_data.artifcat[i].value);
                        break;
                    //도구 속도 10%
                    case "Fragment":
                        player_data.tool_data.ArtifactFragment(artifact_data.artifcat[i].value);
                        break;
                    //식량 +100
                    case "Amber":
                        GameDAO.instance.meal_data.ArtifactAmber(artifact_data.artifcat[i].value);
                        break;
                }
            }
        }
    }

    //버프 체크
    public void CheckBuff()
    {
        //버프 켜졌는지 체크
        if (buff_data.enable)
        {
            //버프 적용했는지 확인
            if (!buff_data.apply)
            {
                //버프 종류에 따라 적용
                switch (buff_data.buff_kind)
                {
                    case Define.BuffAttack:
                        for (int i = 0; i < Define.weapon_step_max; i++)
                            for (int j = 0; j < Define.weapon_level_max; j++)
                                player_data.weapon_data.atk[i, j] *= buff_data.value;
                        break;

                    case Define.BuffPlayerSpeed:
                        player_data.speed *= buff_data.value;
                        player_data.max_speed *= buff_data.value;
                        break;

                    case Define.BuffToolSpeed:
                        for (int i = 0; i < player_data.tool_data.speed.Length; i++)
                            player_data.tool_data.speed[i] *= buff_data.value;
                        break;
                }
                //적용 여부 표시 & 초기화 가능하게 해두기
                buff_data.apply = true;
                buff_data.init = false;
            }

            //타이머 체크
            if (buff_data.buff_timer.timer < buff_data.buff_timer.term)
                buff_data.buff_timer.timer += Time.deltaTime;
            else
            {
                //버프 종류 바꾸기
                if (buff_data.buff_kind != (Define.buff_kind_num - 1))
                    buff_data.buff_kind++;
                else
                    buff_data.buff_kind = 0;

                buff_data.enable = false;
                buff_data.buff_timer.timer = 0;
            }
        }

        //버프 해제시 다시 초기화
        if (!buff_data.enable && !buff_data.init)
        {
            //초기화
            for (int i = 0; i < Define.weapon_step_max; i++)
                for (int j = 0; j < Define.weapon_level_max; j++)
                    player_data.weapon_data.atk[i, j] = new GameDAO.WeaponData().atk[i,j];

            player_data.speed = new GameDAO.PlayerData().speed;
            player_data.max_speed = new GameDAO.PlayerData().max_speed;

            for (int i = 0; i < player_data.tool_data.speed.Length; i++)
                player_data.tool_data.speed[i] = new GameDAO.ToolData().speed[i];

            //아티펙트 다시 적용
            CheckArtifact();

            //초기화 여부 표시 및 버프 다시 적용 가능하게
            buff_data.init = true;
            buff_data.apply = false;

            //타이머 초기화
            buff_data.buff_timer.timer = 0;
        }
    }

    #region 빌드 관련 함수

    //초기화
    private void InitBuilds() {
        //인덱스 초기화
        for (int i = 0; i < Define.build_num_limit; i++)
            build_index.Add(i);

        //Pool 초기화
        //터렛
        for (int i = 0; i < Define.turret_num_limit; i++)
            turret_pool.Push(Instantiate(turret_prefab));

        //실드
        for (int i = 0; i < Define.shield_num_limit; i++)
            shield_pool.Push(Instantiate(shield_prefab));

        //지뢰
        for (int i = 0; i < Define.mine_num_limit; i++)
            mine_pool.Push(Instantiate(mine_prefab));
    }

    //로드
    public void LoadBuild(GameDAO.BuildData _build_data) {
        BuildCommon build_temp = null;
        Stack<GameObject> build_pool = null;
        Vector3 real_point = map_manager.GetBuildRealPoint(0, _build_data.index);

        switch (_build_data.pool) {
            case GameDAO.BuildData.Pool.Turret:
                build_pool = turret_pool;
                break;

            case GameDAO.BuildData.Pool.Shield:
                build_pool = shield_pool;
                break;

            case GameDAO.BuildData.Pool.Mine:
                build_pool = mine_pool;
                break;
        }

        //빌드 받아옴
        build_spawn[_build_data.index] = build_pool.Pop();
        //BuildCommon 가져옴
        build_temp = build_spawn[_build_data.index].GetComponent<BuildCommon>();
        //데이터 초기화        
        build_temp.Init(real_point, _build_data);
    }

    //스폰
    public bool SpawnBuild(GameDAO.BuildData.Pool _pool, Transform _position, float _offset_x, float _offset_y) {
        int index_temp = 0;
        BuildCommon build_temp = null;
        Stack<GameObject> build_pool = null;
        bool send_bool = false;

        //풀에 따라서
        switch (_pool) {
            case GameDAO.BuildData.Pool.Turret:
                build_pool = turret_pool;
                break;

            case GameDAO.BuildData.Pool.Shield:
                build_pool = shield_pool;
                break;

            case GameDAO.BuildData.Pool.Mine:
                build_pool = mine_pool;
                break;
        }

        if (build_pool.ToArray().Length != 0) {
            //인덱스 가져옴(앞에서)
            if (build_index.ToArray().Length != 0) {
                index_temp = build_index[0];
                build_index.RemoveAt(0);
            }

            //빌드 받아옴
            build_spawn[index_temp] = build_pool.Pop();
            //BuildCommon 가져옴
            build_temp = build_spawn[index_temp].GetComponent<BuildCommon>();
            //인덱스 부여
            build_temp.my_data.index = index_temp;
            //빌드 초기화
            Vector3 position = _position.position;
            position.x += _offset_x;
            position.y += _offset_y;
            build_temp.Init(position);

            //데이터 저장
            build_spawn_datas.Add(build_temp.my_data);

            send_bool = true;
        } else {
            send_bool = false;
        }

        return send_bool;
    }

    //파괴
    public void DestroyBuild(int _index, GameDAO.BuildData.Pool _pool) {
        //현재 빌드 리스트에서 빼옴
        GameObject build = build_spawn[_index];
        //데이터 제거
        build_spawn_datas.Remove(build.GetComponent<BuildCommon>().my_data);
        //현재 빌드 리스트에서 제거
        build_spawn[_index] = null;

        //인덱스 돌려줌
        build_index.Add(_index);

        //풀에 다시 넣어줌
        switch (_pool) {
            case GameDAO.BuildData.Pool.Turret:
                turret_pool.Push(build);
                break;

            case GameDAO.BuildData.Pool.Shield:
                shield_pool.Push(build);
                break;

            case GameDAO.BuildData.Pool.Mine:
                mine_pool.Push(build);
                break;
        }
    }

    #endregion

    #region 몬스터 관련 함수

    //몬스터 초기화
    private void InitMonsters() {
        //인덱스 초기화
        for (int i = 0; i < Define.monster_spawn_num_limit; i++)
            monster_index_list.Push(i);

        //몬스터 제한 수 만큼 미리 생성
        for (int i = 0; i < Define.monster_num_limit; i++) {
            slime_weak_melee_pool.Push(Instantiate(slime_weak_melee_prefab));
            slime_weak_range_pool.Push(Instantiate(slime_weak_range_prefab));
            slime_normal_melee_pool.Push(Instantiate(slime_normal_melee_prefab));
            slime_normal_range_pool.Push(Instantiate(slime_normal_range_prefab));
            slime_strong_melee_pool.Push(Instantiate(slime_strong_melee_prefab));
            slime_strong_range_pool.Push(Instantiate(slime_strong_range_prefab));

            larva_weak_pool.Push(Instantiate(larva_weak_prefab));
            larva_normal_pool.Push(Instantiate(larva_normal_prefab));

            sandmonster_weak_pool.Push(Instantiate(sandmonster_weak_prefab));
            sandmonster_normal_pool.Push(Instantiate(sandmonster_normal_prefab));
        }

        #region 몬스터 소환 확률 초기화

        #region 슬라임
        //약한 슬라임
        slime_weak_melee_rate.SetRate(50, 20, 0, Define.Day);
        slime_weak_melee_rate.SetRate(40, 10, 0, Define.Night);
        slime_weak_melee_rate.SetRate(47, 10, 0, Define.DayStorm);
        slime_weak_melee_rate.SetRate(40, 0, 0, Define.NightStorm);

        total_rate.AddAllRate(slime_weak_melee_rate.global_state);
        slime_total_rate.AddAllRate(slime_weak_melee_rate.global_state);

        slime_weak_range_rate.SetRate(50, 20, 0, Define.Day);
        slime_weak_range_rate.SetRate(40, 10, 0, Define.Night);
        slime_weak_range_rate.SetRate(47, 10, 0, Define.DayStorm);
        slime_weak_range_rate.SetRate(40, 0, 0, Define.NightStorm);

        total_rate.AddAllRate(slime_weak_range_rate.global_state);
        slime_total_rate.AddAllRate(slime_weak_range_rate.global_state);

        //보통 슬라임
        slime_normal_melee_rate.SetRate(0, 25, 30, Define.Day);
        slime_normal_melee_rate.SetRate(1, 15, 10, Define.Night);
        slime_normal_melee_rate.SetRate(1, 15, 10, Define.DayStorm);
        slime_normal_melee_rate.SetRate(1, 20, 10, Define.NightStorm);

        total_rate.AddAllRate(slime_normal_melee_rate.global_state);
        slime_total_rate.AddAllRate(slime_normal_melee_rate.global_state);

        slime_normal_range_rate.SetRate(0, 25, 30, Define.Day);
        slime_normal_range_rate.SetRate(1, 15, 10, Define.Night);
        slime_normal_range_rate.SetRate(1, 15, 10, Define.DayStorm);
        slime_normal_range_rate.SetRate(1, 20, 10, Define.NightStorm);

        total_rate.AddAllRate(slime_normal_range_rate.global_state);
        slime_total_rate.AddAllRate(slime_normal_range_rate.global_state);

        //강한 슬라임
        slime_strong_melee_rate.SetRate(0, 0, 20, Define.Day);
        slime_strong_melee_rate.SetRate(0, 1, 15, Define.Night);
        slime_strong_melee_rate.SetRate(0, 1, 15, Define.DayStorm);
        slime_strong_melee_rate.SetRate(0, 1, 20, Define.NightStorm);

        total_rate.AddAllRate(slime_strong_melee_rate.global_state);
        slime_total_rate.AddAllRate(slime_strong_melee_rate.global_state);

        slime_strong_range_rate.SetRate(0, 0, 20, Define.Day);
        slime_strong_range_rate.SetRate(0, 1, 15, Define.Night);
        slime_strong_range_rate.SetRate(0, 1, 15, Define.DayStorm);
        slime_strong_range_rate.SetRate(0, 1, 20, Define.NightStorm);

        total_rate.AddAllRate(slime_strong_range_rate.global_state);
        slime_total_rate.AddAllRate(slime_strong_range_rate.global_state);
        #endregion

        #region 그 외
        //약한 라바
        larva_weak_rate.SetRate(0, 0, 0, Define.Day);
        larva_weak_rate.SetRate(0, 30, 20, Define.Night);
        larva_weak_rate.SetRate(0, 0, 0, Define.DayStorm);
        larva_weak_rate.SetRate(0, 15, 5, Define.NightStorm);

        total_rate.AddAllRate(larva_weak_rate.global_state);
        other_total_rate.AddAllRate(larva_weak_rate.global_state);

        //보통 라바
        larva_normal_rate.SetRate(0, 0, 0, Define.Day);
        larva_normal_rate.SetRate(0, 0, 30, Define.Night);
        larva_normal_rate.SetRate(0, 0, 0, Define.DayStorm);
        larva_normal_rate.SetRate(0, 1, 15, Define.NightStorm);

        total_rate.AddAllRate(larva_normal_rate.global_state);
        other_total_rate.AddAllRate(larva_normal_rate.global_state);

        //약한 폭풍 몬스터
        sandmonster_weak_rate.SetRate(0, 0, 0, Define.Day);
        sandmonster_weak_rate.SetRate(0, 0, 0, Define.Night);
        sandmonster_weak_rate.SetRate(0, 30, 20, Define.DayStorm);
        sandmonster_weak_rate.SetRate(0, 15, 5, Define.NightStorm);

        total_rate.AddAllRate(sandmonster_weak_rate.global_state);
        other_total_rate.AddAllRate(sandmonster_weak_rate.global_state);

        //보통 폭풍 몬스터
        sandmonster_normal_rate.SetRate(0, 0, 0, Define.Day);
        sandmonster_normal_rate.SetRate(0, 0, 0, Define.Night);
        sandmonster_normal_rate.SetRate(0, 0, 30, Define.DayStorm);
        sandmonster_normal_rate.SetRate(0, 1, 15, Define.NightStorm);

        total_rate.AddAllRate(sandmonster_normal_rate.global_state);
        other_total_rate.AddAllRate(sandmonster_normal_rate.global_state);
        #endregion

        #endregion
    }

    //몬스터 생성
    private void RegenMonsters() {
        //랜덤값
        int random = 0;
        //최소값
        int min = 0;

        //현제 소환된 몬스터 수가 제한 넘으면 실행 X
        if (monster_spawn_num >= Define.monster_spawn_num_limit)
            return;

        //몬스터 리젠 간격 랜덤 - 상태에 따라서
        if (global_state.state == Define.Day)
            Define.monster_regen_term = Random.Range(Define.monster_regen_term_min, Define.monster_regen_term_max);
        else
            Define.monster_regen_term = Random.Range(Define.monster_regen_notday_term_min, Define.monster_regen_notday_term_max);

        //전체 랜덤값
        int current_total_rate = total_rate.global_state[global_state.state].sector[global_state.level];
        if (map_manager.CheckSafeZone()) {
            current_total_rate += Define.monster_penalty_rate;
        }
        random = Random.Range(0, current_total_rate);

        #region 소환

        //---------- 근접 약한 슬라임 ----------
        SpawnMonster(ref min, ref random, slime_weak_melee_rate, slime_weak_melee_pool, current_total_rate);

        //---------- 원거리 약한 슬라임 ----------
        SpawnMonster(ref min, ref random, slime_weak_range_rate, slime_weak_range_pool, current_total_rate);

        //---------- 근접 보통 슬라임 ----------
        SpawnMonster(ref min, ref random, slime_normal_melee_rate, slime_normal_melee_pool, current_total_rate);

        //---------- 원거리 보통 슬라임 ----------
        SpawnMonster(ref min, ref random, slime_normal_range_rate, slime_normal_range_pool, current_total_rate);

        //---------- 근거리 강한 슬라임 ----------
        SpawnMonster(ref min, ref random, slime_strong_melee_rate, slime_strong_melee_pool, current_total_rate);

        //---------- 원거리 강한 슬라임 ----------
        SpawnMonster(ref min, ref random, slime_strong_range_rate, slime_strong_range_pool, current_total_rate);

        //---------- 약한 라바 ----------
        SpawnMonster(ref min, ref random, larva_weak_rate, larva_weak_pool, current_total_rate);

        //---------- 보통 라바 ----------
        SpawnMonster(ref min, ref random, larva_normal_rate, larva_normal_pool, current_total_rate);

        //---------- 약한 폭풍 몬스터 ----------
        SpawnMonster(ref min, ref random, sandmonster_weak_rate, sandmonster_weak_pool, current_total_rate);

        //---------- 보통 폭풍 몬스터 ----------
        SpawnMonster(ref min, ref random, sandmonster_normal_rate, sandmonster_normal_pool, current_total_rate);

        #endregion

        #region 추가 소환

        //추가 리젠 랜덤값
        int spawn_add = 0;

        #region 다른몹 추가 소환 확률
        spawn_add = Random.Range(0, 10000);

        if (spawn_add <= Define.other_spawn_add_rate) {
            //다시 초기화
            int add_random = 0;
            int add_min = 0;

            int other_add_rate = other_total_rate.global_state[global_state.state].sector[global_state.level];
            if (map_manager.CheckSafeZone()) {
                other_add_rate += Define.monster_penalty_rate;
            }
            add_random = Random.Range(0, other_add_rate);

            //딜레이
            GameDAO.Timer delay = new GameDAO.Timer(0, 10f);
            while (delay.timer < delay.term)
                delay.timer += Time.deltaTime;

            //스폰
            //---------- 약한 라바 ----------
            SpawnMonster(ref add_min, ref add_random, larva_weak_rate, larva_weak_pool, other_add_rate);

            //---------- 보통 라바 ----------
            SpawnMonster(ref add_min, ref add_random, larva_normal_rate, larva_normal_pool, other_add_rate);

            //---------- 약한 폭풍 몬스터 ----------
            SpawnMonster(ref add_min, ref add_random, sandmonster_weak_rate, sandmonster_weak_pool, other_add_rate);

            //---------- 보통 폭풍 몬스터 ----------
            SpawnMonster(ref add_min, ref add_random, sandmonster_normal_rate, sandmonster_normal_pool, other_add_rate);

            //타이머 초기화
            monster_regen_timer.timer = 0;

            return;
        }
        #endregion

        #region 슬라임 추가 소환 확률
        spawn_add = Random.Range(0, 10000);

        if (spawn_add <= Define.slime_spawn_add_rate) {
            //다시 초기화
            int add_random = 0;
            int add_min = 0;

            int slime_add_rate = slime_total_rate.global_state[global_state.state].sector[global_state.level];
            if (map_manager.CheckSafeZone()) {
                slime_add_rate += Define.monster_penalty_rate;
            }
            add_random = Random.Range(0, slime_add_rate);

            //딜레이
            GameDAO.Timer delay = new GameDAO.Timer(0, 10f);
            while (delay.timer < delay.term)
                delay.timer += Time.deltaTime;

            //스폰
            //---------- 근접 약한 슬라임 ----------
            SpawnMonster(ref add_min, ref add_random, slime_weak_melee_rate, slime_weak_melee_pool, slime_add_rate);

            //---------- 원거리 약한 슬라임 ----------
            SpawnMonster(ref add_min, ref add_random, slime_weak_range_rate, slime_weak_range_pool, slime_add_rate);

            //---------- 근접 보통 슬라임 ----------
            SpawnMonster(ref add_min, ref add_random, slime_normal_melee_rate, slime_normal_melee_pool, slime_add_rate);

            //---------- 원거리 보통 슬라임 ----------
            SpawnMonster(ref add_min, ref add_random, slime_normal_range_rate, slime_normal_range_pool, slime_add_rate);

            //---------- 근거리 강한 슬라임 ----------
            SpawnMonster(ref add_min, ref add_random, slime_strong_melee_rate, slime_strong_melee_pool, slime_add_rate);

            //---------- 원거리 강한 슬라임 ----------
            SpawnMonster(ref add_min, ref add_random, slime_strong_range_rate, slime_strong_range_pool, slime_add_rate);

            //타이머 초기화
            monster_regen_timer.timer = 0;

            return;
        }
        #endregion

        #endregion

        //타이머 초기화
        monster_regen_timer.timer = 0;
    }

    //몬스터 소환
    private void SpawnMonster(ref int _min, ref int _random, GameDAO.MonsterSpawnRate _monster_spawn_rate, Stack<GameObject> _pool, int _total_rate) {
        int index_temp = 0;
        MonsterCommon monster_temp = null;
        bool pool_empty = false;

        if (_monster_spawn_rate.CheckRate(_min, _random, global_state.level, global_state.state)) {
            if (_pool.ToArray().Length != 0) {
                //인덱스 가져옴
                if (monster_index_list.ToArray().Length != 0) {
                    index_temp = monster_index_list.Pop();
                    //현제 몬스터 수++
                    monster_spawn_num++;
                    //몬스터 받아옴
                    monster_spawn_list[index_temp] = _pool.Pop();
                    //MonsterCommon 가져옴
                    monster_temp = monster_spawn_list[index_temp].GetComponent<MonsterCommon>();
                    //인덱스 부여
                    monster_temp.monster_data.index = index_temp;
                    //몬스터 리젠
                    monster_temp.Regen();
                } else
                    pool_empty = true;
            } else
                pool_empty = true;
        }

        //최소값++
        _min += _monster_spawn_rate.global_state[global_state.state].sector[global_state.level];

        //풀 비었으면 랜덤 다시 돌려준다
        if (pool_empty)
            _random = Random.Range(_min, _total_rate);

    }

    //몬스터 로드
    private void LoadMonster() {
        GameDAO.MonsterData[] datas_temp = file_manager.monster_data.datas;
        MonsterCommon monster_temp = null;

        for (int i = 0; i < Define.monster_spawn_num_limit; i++) {
            if (datas_temp[i] != null) {
                //현제 몬스터 수++
                monster_spawn_num++;

                #region 풀에서 가져오기
                switch (datas_temp[i].pool) {
                    case GameDAO.MonsterData.Pool.Slime_Weak_Melee:
                        monster_spawn_list[i] = slime_weak_melee_pool.Pop();
                        break;
                    case GameDAO.MonsterData.Pool.Slime_Weak_Range:
                        monster_spawn_list[i] = slime_weak_range_pool.Pop();
                        break;
                    case GameDAO.MonsterData.Pool.Slime_Normal_Melee:
                        monster_spawn_list[i] = slime_normal_melee_pool.Pop();
                        break;
                    case GameDAO.MonsterData.Pool.Slime_Normal_Range:
                        monster_spawn_list[i] = slime_normal_melee_pool.Pop();
                        break;
                    case GameDAO.MonsterData.Pool.Slime_Strong_Melee:
                        monster_spawn_list[i] = slime_strong_melee_pool.Pop();
                        break;
                    case GameDAO.MonsterData.Pool.Slime_Strong_Range:
                        monster_spawn_list[i] = slime_strong_range_pool.Pop();
                        break;
                    case GameDAO.MonsterData.Pool.Larva_Weak:
                        monster_spawn_list[i] = larva_weak_pool.Pop();
                        break;
                    case GameDAO.MonsterData.Pool.Larva_Normal:
                        monster_spawn_list[i] = larva_normal_pool.Pop();
                        break;
                    case GameDAO.MonsterData.Pool.SandMonster_Weak:
                        monster_spawn_list[i] = sandmonster_weak_pool.Pop();
                        break;
                    case GameDAO.MonsterData.Pool.SandMonster_Normal:
                        monster_spawn_list[i] = sandmonster_normal_pool.Pop();
                        break;
                }
                #endregion

                monster_temp = monster_spawn_list[i].GetComponent<MonsterCommon>();
                monster_temp.Load(datas_temp[i]);
            }
        }
    }

    //몬스터 제거(되돌려준다)
    public void DestroyMonster(int _index, GameDAO.MonsterData.Pool _pool) {
        //현재 몬스터 리스트에서 빼옴
        GameObject monster = monster_spawn_list[_index];
        //현재 몬스터 리스트에서 제거
        monster_spawn_list[_index] = null;

        //인덱스 돌려줌
        monster_index_list.Push(_index);
        //현재 몬스터 수--
        monster_spawn_num--;

        //몬스터 풀에 다시 넣어줌
        switch (_pool) {
            case GameDAO.MonsterData.Pool.Slime_Weak_Melee:
                slime_weak_melee_pool.Push(monster);
                break;
            case GameDAO.MonsterData.Pool.Slime_Weak_Range:
                slime_weak_range_pool.Push(monster);
                break;
            case GameDAO.MonsterData.Pool.Slime_Normal_Melee:
                slime_normal_melee_pool.Push(monster);
                break;
            case GameDAO.MonsterData.Pool.Slime_Normal_Range:
                slime_normal_range_pool.Push(monster);
                break;
            case GameDAO.MonsterData.Pool.Slime_Strong_Melee:
                slime_strong_melee_pool.Push(monster);
                break;
            case GameDAO.MonsterData.Pool.Slime_Strong_Range:
                slime_strong_range_pool.Push(monster);
                break;
            case GameDAO.MonsterData.Pool.Larva_Weak:
                larva_weak_pool.Push(monster);
                break;
            case GameDAO.MonsterData.Pool.Larva_Normal:
                larva_normal_pool.Push(monster);
                break;
            case GameDAO.MonsterData.Pool.SandMonster_Weak:
                sandmonster_weak_pool.Push(monster);
                break;
            case GameDAO.MonsterData.Pool.SandMonster_Normal:
                sandmonster_normal_pool.Push(monster);
                break;
        }
    }

    #endregion

    #region 보스 관련 함수
    //보스 초기화
    private void InitBoss() {
        //보스 제한 수 만큼 미리 생성
        for (int i = 0; i < Define.boss_kind_num; i++) {
            boss_pool_queue.Enqueue(Instantiate(tree_boss_prefab));
            boss_pool_queue.Enqueue(Instantiate(rock_boss_prefab));
        }
    }
    
    private void RegenBoss() {
        //현재 보스가 소환되있으면 진행X
        if (boss_spawn != null)
            return;

        //양쪽 모두 소환됬었다면
        if (boss_left_spawned && boss_right_spawned) {
            //보스 리젠 간격++
            if (Define.boss_regen_term < Define.boss_regen_max)
                Define.boss_regen_term += Define.boss_regen_interval;
            else
                Define.boss_regen_term = Define.boss_regen_min;

            //타이머 초기화
            boss_regen_timer.timer = 0;

            //소환 여부 초기화
            boss_left_spawned = false;
            boss_right_spawned = false;

            return;
        }

        //보스 소환 여부 체크(좌/우)
        if (map_manager.player_point < 0) {
            if (boss_left_spawned)
                return;
            else
                boss_left_spawned = true;
        } else {
            if (boss_right_spawned)
                return;
            else
                boss_right_spawned = true;
        }

        //소환
        if (boss_pool_queue.ToArray().Length != 0) {
            boss_spawn = boss_pool_queue.Dequeue();
            boss_spawn.GetComponent<BossCommon>().Regen();
        }
    }

    //보스 제거
    public void DestroyBoss() {
        //현재 보스에서 빼옴
        GameObject boss = boss_spawn;
        //현재 보스 제거
        boss_spawn = null;
        //보스 풀에 복귀
        boss_pool_queue.Enqueue(boss);
    }

    //보스 로드
    private void LoadBoss() {
        GameDAO.BossData data = file_manager.boss_data.data;
        BossCommon boss = null;

        if (data != null) {
            #region 풀에서 가져오기
            switch (data.pool) {
                case GameDAO.BossData.Pool.Rock_Boss:
                    boss_spawn = boss_pool_queue.Dequeue();
                    boss_pool_queue.Enqueue(boss_spawn);
                    boss_spawn = null;
                    boss_spawn = boss_pool_queue.Dequeue();
                    break;
                case GameDAO.BossData.Pool.Tree_Boss:
                    boss_spawn = boss_pool_queue.Dequeue();
                    break;
            }
            #endregion

            boss = boss_spawn.GetComponent<BossCommon>();
            boss.Load(data);
        }
    }
    #endregion

    #region 특수 보스 관련 함수

    //초기화 특수 보스
    private void InitSPBoss()
    {
        //보스 생성
        sp_boss_pool[0] = Instantiate(jellyfish_boss_prefab);
        sp_boss_pool[1] = Instantiate(alien_boss_prefab);
        sp_boss_pool[2] = Instantiate(sandworm_boss_prefab);
    }

    //리젠 특수 보스
    private void RegenSPBoss()
    {
        if (sp_boss_spawn != null)
            return;

        switch (spaceship_data.step)
        {
            case 2:
                sp_boss_step = 3;
                sp_boss_spawn = sp_boss_pool[0];
                break;

            case 3:
                sp_boss_step = 4;
                sp_boss_spawn = sp_boss_pool[1];
                break;

            case 4:
                sp_boss_step = 5;
                sp_boss_spawn = sp_boss_pool[2];
                break;
        }
        sp_boss_spawn.GetComponent<SPBossCommon>().Regen();
    }

    //파괴 특수 보스
    public void DestroySPBoss()
    {
        sp_boss_spawn = null;
    }

    //로드 특수 보스
    private void LoadSPBoss()
    {
        GameDAO.SPBossData data = file_manager.sp_boss_data.data;
        SPBossCommon boss = null;

        if (data != null)
        {
            sp_boss_spawn = sp_boss_pool[file_manager.sp_boss_data.sp_boss_step - Define.sp_boss_kind_num];
            boss = sp_boss_spawn.GetComponent<SPBossCommon>();
            boss.Load(data);
        }
    }
    #endregion

    //자원 생성
    private void InitSubstance() {
        //오브젝트 받아오기 or 생성하기

        //자원 초기화
        //자원 인덱스 리스트
        List<int>[] sector_indexes = new List<int>[Define.sector_total_num] { new List<int>(), new List<int>(), new List<int>() };
        for (int sector = 0; sector < sector_indexes.Length; sector++) {
            for (int i = 0; i < Define.sub_sectors[sector].point_num; i++) {
                sector_indexes[sector].Add(i);
            }
        }

        //-------------------------------------- 자원 생성 --------------------------------------


        //------------------- 작은 크기 자원 -------------------

        //각 구역별 인덱스 최대치(갯수)
        int[] sub_index_max = new int[Define.sector_total_num];
        sub_index_max[0] = (int)(Define.sub_small_num_half * Define.sub_small_sector_rate[0]);
        for (int i = 1; i < Define.sector_total_num; i++)
            sub_index_max[i] = sub_index_max[i - 1] + (int)(Define.sub_small_num_half * Define.sub_small_sector_rate[i]);

        //리스트 초기화 및 배치
        for (int list_index = 0; list_index < Define.sub_small_num_half; list_index++) {
            //오브젝트 생성
            rock_small_list[list_index] = Instantiate(rock_small_prefab);
            tree_small_list[list_index] = Instantiate(tree_small_prefab);

            //substance 초기화
            rock_substance = rock_small_list[list_index].GetComponent<Substance>();
            tree_substance = tree_small_list[list_index].GetComponent<Substance>();

            //구역 구분 비율
            if (list_index < sub_index_max[0]) {
                //인덱스 부여
                rock_substance.index = GetRandomIndex(sector_indexes[0]);
                tree_substance.index = GetRandomIndex(sector_indexes[0]);
                //구역 표시
                rock_substance.sector = 0;
                tree_substance.sector = 0;
                //위치 지정
                rock_small_list[list_index].transform.position = map_manager.GetSubstanceRealPoint(0, rock_substance.index, rock_substance.sprite_hieght);
                tree_small_list[list_index].transform.position = map_manager.GetSubstanceRealPoint(0, tree_substance.index, tree_substance.sprite_hieght);
            }

            for (int sector = 1; sector < Define.sector_total_num; sector++) {
                if (sub_index_max[sector - 1] <= list_index && list_index < sub_index_max[sector]) {
                    //인덱스 부여
                    rock_substance.index = GetRandomIndex(sector_indexes[sector]);
                    tree_substance.index = GetRandomIndex(sector_indexes[sector]);
                    //구역 표시
                    rock_substance.sector = sector;
                    tree_substance.sector = sector;
                    //위치 지정
                    rock_small_list[list_index].transform.position = map_manager.GetSubstanceRealPoint(sector, rock_substance.index, rock_substance.sprite_hieght);
                    tree_small_list[list_index].transform.position = map_manager.GetSubstanceRealPoint(sector, tree_substance.index, tree_substance.sprite_hieght);
                }
            }
        }



        //------------------- 중간 크기 자원 -------------------

        sub_index_max[0] = (int)(Define.sub_medium_num_half * Define.sub_medium_sector_rate[0]);
        for (int i = 1; i < Define.sector_total_num; i++)
            sub_index_max[i] = sub_index_max[i - 1] + (int)(Define.sub_medium_num_half * Define.sub_medium_sector_rate[i]);


        //리스트 초기화 및 배치
        for (int list_index = 0; list_index < Define.sub_medium_num_half; list_index++) {
            //오브젝트 생성
            rock_medium_list[list_index] = Instantiate(rock_medium_prefab);
            tree_medium_list[list_index] = Instantiate(tree_medium_prefab);

            //substance 초기화
            rock_substance = rock_medium_list[list_index].GetComponent<Substance>();
            tree_substance = tree_medium_list[list_index].GetComponent<Substance>();

            //구역 구분 비율
            if (list_index < sub_index_max[0]) {
                //인덱스 부여
                rock_substance.index = GetRandomIndex(sector_indexes[0]);
                tree_substance.index = GetRandomIndex(sector_indexes[0]);
                //구역 표시
                rock_substance.sector = 0;
                tree_substance.sector = 0;
                //위치 지정
                rock_medium_list[list_index].transform.position = map_manager.GetSubstanceRealPoint(0, rock_substance.index, rock_substance.sprite_hieght);
                tree_medium_list[list_index].transform.position = map_manager.GetSubstanceRealPoint(0, tree_substance.index, tree_substance.sprite_hieght);
            }

            for (int sector = 1; sector < Define.sector_total_num; sector++) {
                if (sub_index_max[sector - 1] <= list_index && list_index < sub_index_max[sector]) {
                    //인덱스 부여
                    rock_substance.index = GetRandomIndex(sector_indexes[sector]);
                    tree_substance.index = GetRandomIndex(sector_indexes[sector]);
                    //구역 표시
                    rock_substance.sector = sector;
                    tree_substance.sector = sector;
                    //위치 지정
                    rock_medium_list[list_index].transform.position = map_manager.GetSubstanceRealPoint(sector, rock_substance.index, rock_substance.sprite_hieght);
                    tree_medium_list[list_index].transform.position = map_manager.GetSubstanceRealPoint(sector, tree_substance.index, tree_substance.sprite_hieght);
                }
            }

        }


        //------------------- 큰 크기 자원 -------------------

        sub_index_max[0] = (int)(Define.sub_large_num_half * Define.sub_large_sector_rate[0]);
        for (int i = 1; i < Define.sector_total_num; i++)
            sub_index_max[i] = sub_index_max[i - 1] + (int)(Define.sub_large_num_half * Define.sub_large_sector_rate[i]);

        //리스트 초기화 및 배치
        for (int list_index = 0; list_index < Define.sub_large_num_half; list_index++) {
            //오브젝트 생성
            rock_large_list[list_index] = Instantiate(rock_large_prefab);
            tree_large_list[list_index] = Instantiate(tree_large_prefab);

            //substance 초기화
            rock_substance = rock_large_list[list_index].GetComponent<Substance>();
            tree_substance = tree_large_list[list_index].GetComponent<Substance>();

            //구역 구분 비율
            if (list_index < sub_index_max[0]) {
                //인덱스 부여
                rock_substance.index = GetRandomIndex(sector_indexes[0]);
                tree_substance.index = GetRandomIndex(sector_indexes[0]);
                //구역 표시
                rock_substance.sector = 0;
                tree_substance.sector = 0;
                //위치 지정
                rock_large_list[list_index].transform.position = map_manager.GetSubstanceRealPoint(0, rock_substance.index, rock_substance.sprite_hieght);
                tree_large_list[list_index].transform.position = map_manager.GetSubstanceRealPoint(0, tree_substance.index, tree_substance.sprite_hieght);
            }

            for (int sector = 1; sector < Define.sector_total_num; sector++) {
                if (sub_index_max[sector - 1] <= list_index && list_index < sub_index_max[sector]) {
                    //인덱스 부여
                    rock_substance.index = GetRandomIndex(sector_indexes[sector]);
                    tree_substance.index = GetRandomIndex(sector_indexes[sector]);
                    //구역 표시
                    rock_substance.sector = sector;
                    tree_substance.sector = sector;
                    //위치 지정
                    rock_large_list[list_index].transform.position = map_manager.GetSubstanceRealPoint(sector, rock_substance.index, rock_substance.sprite_hieght);
                    tree_large_list[list_index].transform.position = map_manager.GetSubstanceRealPoint(sector, tree_substance.index, tree_substance.sprite_hieght);
                }
            }

        }

    }

    #region 큰 자원 관련 함수
    //큰 자원 초기화
    private void InitBigSubstance() {
        //미리 생성
        for (int i = 0; i < Define.big_substance_num; i++) {
            //풀 생성
            big_sub_pool[i] = Instantiate(big_sub_prefab);
            //인덱스 부여
            big_sub_pool[i].GetComponent<BigSubstance>().index = i;
        }
    }

    //큰 자원 생성
    private void RegenBigSubstance() {
        //레벨 따라 리젠
        if (global_state.level == 0)
            return;

        //소환
        for (int i = 0; i < Define.big_substance_num; i++) {
            big_sub_spawned.Add(big_sub_pool[i]);
            map_manager.SetRandomBigSubstancePoint();
            big_sub_spawned[i].GetComponent<BigSubstance>().Regen();
        }

        big_sub_regen_timer.timer = 0;
    }

    //큰 자원 로드
    private void LoadBigSubstance() {
        //레벨 따라 리젠
        if (global_state.level == 0)
            return;

        //소환
        for (int i = 0; i < Define.big_substance_num; i++) {
            //아직 파괴 안된 자원
            if (map_manager.big_substance_points[i] != Define.init_vpos_plus) {
                big_sub_spawned.Add(big_sub_pool[i]);
                big_sub_pool[i].GetComponent<BigSubstance>().LoadRegen();
            }
            //파괴된 자원
            else {
                big_sub_pool[i].GetComponent<BigSubstance>().LoadRegen();
            }
        }
    }

    //큰 자원 제거
    public void DestroyBigSubstance(GameObject _game_object) {
        big_sub_spawned.Remove(_game_object);
    }
    #endregion

}
