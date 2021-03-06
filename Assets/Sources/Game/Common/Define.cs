using UnityEngine;

public static class Define{

    #region 공통
    //카메라 가로, 세로
    public static float camera_height = 0;
    public static float camera_width = 0;
    public static float camera_height_half = 0;
    public static float camera_width_half = 0;
    #endregion

    #region 구조체 선언

    //범위
    public struct Range
    {
        public int min;
        public int max;

        public Range(int _min, int _max)
        {
            min = _min;
            max = _max;
        }
    }

    //구역
    public struct Sector
    {
        public int point_num;       //가지고있는 포인트 수
        public Range index_left;    //왼쪽 구역 인덱스 범위
        public Range index_right;   //오른쪽 구역 인덱스 범위
        public Range sector_left;   //왼쪽 구역 범위
        public Range sector_right;  //오른쪽 구역 범위

        public Sector(int _point_num, Range _left, Range _right)
        {
            int point_num_half = (int)(_point_num / 2.0);
            point_num = _point_num;
            index_left = new Range(0, point_num_half);
            index_right = new Range(point_num_half, _point_num);
            sector_left = _left;
            sector_right = _right;
        }

        public Sector(Range _left, Range _right)
        {
            point_num = 0;
            index_left = new Range();
            index_right = new Range();
            sector_left = _left;
            sector_right = _right;
        }
    }
    //구역 수
    public const int sector_total_num = 3;
    #endregion
    
    #region 시스템 관리

    //가상 위치 범위
    public static readonly Range virtual_point = new Range(-240, 240);
    //실제 위치 범위
    public static readonly Range real_point = new Range(-355, 355);

    //날씨 + 밤낮
    public const int Day = 0;
    public const int Night = 1;
    public const int DayStorm = 2;
    public const int NightStorm = 3;

    //날짜 / 밤낮 / 폭풍 주기
    public const float DayCount_Cycle = 400f;
    public const float Day_Cycle = DayCount_Cycle / 2.0f;
    public const float Day_1D3_Cycle = Day_Cycle / 3.0f;
    public const float Night_Cycle = DayCount_Cycle;

    public static float Storm_Enable_Cycle = Day_Cycle - Day_1D3_Cycle;
    public static float Storm_Cool_Cycle = Day_Cycle + Day_1D3_Cycle;

    //난이도 변경
    public const int Secotr1_Open_Day = 5;
    public const int Secotr2_Open_Day = 15;

    //세이브 주기
    public const int Save_Day_Cycle = 3;

    //리뷰 주기
    public const int Review_Cycle = 3;

    //크리스탈 주는 날
    public const int Give_Crystal_Day = 5;

    //전체 구역 범위
    public static readonly Sector[] player_sectors = new Sector[sector_total_num]
    {
        new Sector(new Range(-60, 0), new Range(0, 60)),
        new Sector(new Range(-140, -60), new Range(60, 140)),
        new Sector(new Range(-240, -140), new Range(140, 240))
    };

    //플레이어 안전 지대
    public static readonly Range safe_zone_range = new Range(-10, 10);
    public const int monster_penalty_rate = 150;

    //빛 범위 관련
    public static float light_range_max = 180f;
    public static float light_range_min = 55f;
    //max - min
    public static float light_range_depth = light_range_max - light_range_min;
    //빛 범위 주기
    public static float light_range_cycle = light_range_depth / (Day_Cycle - (Day_1D3_Cycle * 2));
    #endregion

    #region 난이도 관리
        
    #region 몬스터
    //슬라임
    public const float hard_slime_w = 1.4f;
    public const float hard_slime_n = 1.3f;
    public const float hard_slime_s = 1.2f;

    //라바
    public const float hard_larva_w = 1.3f;
    public const float hard_larva_n = 1.2f;

    //모래
    public const float hard_sandmonster_w = 1.3f;
    public const float hard_sandmonster_n = 1.2f; 
    #endregion

    //보스
    public const float hard_rock_boss = 1.3f;
    public const float hard_tree_boss = 1.3f;

    //특수 보스
    public const float hard_jellyfish_spboss = 1.2f;
    public const float hard_alien_spboss = 1.2f;
    public const float hard_sandworm_spboss = 1.2f;
    #endregion

    #region 아티펙트

    public const int artifact_num = 5;

    #endregion

    #region 플레이어
    //도구 스탭 제한
    public const int tool_step_max = 3;

    //무기
    public const int weapon_step_max = 3;   //스탭 제한
    public const int weapon_level_max = 3;  //레벨 제한
    //종합 제한
    public const int weapon_total_max = weapon_step_max * weapon_level_max;

    //자원
    public const int player_sub_max = 9999;

    //방어구 관련
    //수치
    public const float normal_cloth = 0.2f;
    public const float special_cloth = 0.5f;
    public const float special_cloth_speed = 1.2f;

    //정의값
    public const int NO_CLOTH = 0;
    public const int NORMAL_CLOTH = 1;
    public const int SPECIAL_CLOTH = 2;
    #endregion

    #region 버프 관련
    //버프 주는 주기
    public const int Buff_Cycle = 3;

    //버프 종류
    public const int buff_kind_num = 3;

    //버프 값
    public const int BuffAttack = 0;
    public const int BuffPlayerSpeed = 1;
    public const int BuffToolSpeed = 2;
    #endregion

    #region 우주선
    //우주선 수리 단계
    public const int spaceship_step = 5;
    //우주선 회복양
    public const float spaceship_recovery = 0.5f; 
    #endregion

    #region 빌드
    //스탭 제한
    public const int build_step_max = 3;

    //enable / disable
    public static readonly Range build_enable_point = new Range(-10, 10);
    public static readonly Range build_disable_point = new Range(-10, 10);
    public static readonly float build_destroy_distance = 100f;

    //갯수 제한
    public const int turret_num_limit = 2;
    public const int shield_num_limit = 2;
    public const int mine_num_limit = 4;
    public const int build_num_limit = turret_num_limit + shield_num_limit + mine_num_limit; 
    #endregion

    //---------------------------- 자원 및 몬스터 / 보스 관리 ----------------------------

    #region 공통

    //땅 위치(땅의 로컬 위치 + 높이)
    public static float ground_position;

    //파괴시 이동할 위치
    public static readonly Vector3 init_pos_plus = new Vector3(400, 0, 0);
    public static readonly Vector3 init_pos_minus = new Vector3(-400, 0, 0);
    public static readonly int init_vpos_plus = 400;
    public static readonly int init_vpos_minus = -400;
    #endregion
    
    #region 몬스터

    //몬스터 소환 수 제한
    public const int monster_spawn_num_limit = 4;
    //몬스터 생성 수 제한(종류당 x마리씩)
    public const int monster_num_limit = 2;
    //몬스터 자원 드랍 확률 최대값
    public const float monster_drop_rate_max = 10000;

    //몬스터 소환 범위
    public static readonly Range monster_left_range = new Range(-20, -14);
    public static readonly Range monster_right_range = new Range(14, 20);

    //몬스터 리젠 시간
    //낮일때
    public static float monster_regen_term_min = 30f;
    public static float monster_regen_term_max = 50f;
    //낮 아닐때
    public static float monster_regen_notday_term_min = 20f;
    public static float monster_regen_notday_term_max = 30f;
    public static float monster_regen_term = monster_regen_term_min;

    //몬스터 추가 리젠 확률
    public const int slime_spawn_add_rate = 1500;
    public const int other_spawn_add_rate = 100;

    //몬스터 disable
    public static readonly Range monster_disable_point = new Range(-23, 23);
    #endregion

    #region 보스

    //보스 종류 수
    public const int boss_kind_num = 2;
    //보스 풀 최대수
    public const int boss_pool_num = boss_kind_num * boss_kind_num;

    //보스 소환 범위
    public static readonly Range[] boss_left_range = 
        new Range[2] {
                new Range(-136,-132),
                new Range(-200,-132)
            };
    public static readonly Range[] boss_right_range = 
        new Range[2] {
                new Range(132,136),
                new Range(132,200)
            };

    //보스 리젠 시간
    public const float boss_regen_advance = Day_Cycle;                  //리젠 가속
    public static float boss_regen_min = DayCount_Cycle;                //리젠 최소시간
    public static float boss_regen_max = DayCount_Cycle + Day_Cycle;    //리젠 최대시간
    public static float boss_regen_term = boss_regen_min;               //현재 리젠시간
    public static float boss_regen_interval = Day_1D3_Cycle;            //누적값

    //보스 disable
    public static readonly Range boss_disable_point = new Range(-23, 23);

    #endregion

    #region 특수 보스

    //특수 보스 종류 수
    public const int sp_boss_kind_num = 3;

    //특수 보스 소환 범위
    public static readonly Range sp_boss_left_range = new Range(-200, -132);
    public static readonly Range sp_boss_right_range = new Range(132, 200);
    public static readonly float sp_boss_area = 7f;

    //특수 보스 리젠 시간
    public const float sp_boss_regen_term = Day_1D3_Cycle;
    #endregion

    #region 자원

    #region 자원 수

    //전체 자원 수
    public static readonly int substance_total_num = 80;

    //자원 간격
    public static readonly Range substance_interval = new Range(5, 10);

    //각 사이즈별 자원 전체 수 -> 전체 수 * 각 자원 비율 * 각 자원 전체 비율 / 자원 종류
    public static readonly int sub_small_num_half = substance_total_num * 9 / 20 / 2;
    public static readonly int sub_medium_num_half = substance_total_num * 7 / 20 / 2;
    public static readonly int sub_large_num_half = substance_total_num * 4 / 20 / 2;

    public static readonly int sub_small_num = sub_small_num_half * 2;
    public static readonly int sub_medium_num = sub_medium_num_half * 2;
    public static readonly int sub_large_num = sub_large_num_half * 2;

    //각 사이즈별 자원이 구역별로 차지하는 비율
    //비율 / 구역별 전체 비율
    public static readonly float[] sub_small_sector_rate = new float[sector_total_num]
        {
            4.0f / 9.0f,
            3.0f / 9.0f,
            2.0f / 9.0f
        };

    public static readonly float[] sub_medium_sector_rate = new float[sector_total_num]
        {
            1.0f / 7.0f,
            3.0f / 7.0f,
            3.0f / 7.0f
        };

    public static readonly float[] sub_large_sector_rate = new float[sector_total_num]
        {
            0.0f,
            1.0f / 4.0f,
            3.0f / 4.0f
        };

    //각 구역 자원 수
    public static readonly int[] sub_sector_num = new int[3] {
        //0구역 갯수
        (int)(sub_small_num_half * sub_small_sector_rate[0] * 2) +
        (int)(sub_medium_num_half * sub_medium_sector_rate[0] * 2) +
        (int)(sub_large_num_half * sub_large_sector_rate[0] * 2),
        //1구역 갯수
        (int)(sub_small_num_half * sub_small_sector_rate[1] * 2) +
        (int)(sub_medium_num_half * sub_medium_sector_rate[1] * 2) +
        (int)(sub_large_num_half * sub_large_sector_rate[1] * 2),
        //2구역 갯수
        (int)(sub_small_num_half * sub_small_sector_rate[2] * 2) +
        (int)(sub_medium_num_half * sub_medium_sector_rate[2] * 2) +
        (int)(sub_large_num_half * sub_large_sector_rate[2] * 2)
    };
    #endregion

    //---------------------------- 자원 Enable / Disable ----------------------------
    public static readonly Range substance_enable_point = new Range(-10, 10);
    public static readonly Range substance_disable_point = new Range(-20, 20);

    //---------------------------- 자원위치 ----------------------------

    //구역 정보(sectors[x번 구역].정보)                          
    public static readonly Sector[] sub_sectors = new Sector[sector_total_num]
        {
            new Sector(sub_sector_num[0], new Range(-60, -10), new Range(10, 60)),
            new Sector(sub_sector_num[1], new Range(-140, -66), new Range(66, 140)),
            new Sector(sub_sector_num[2], new Range(-240, -146), new Range(146, 240))
        };
    #endregion

    #region 큰 자원
    //큰 자원 수
    public const int big_substance_num = 2;

    //큰 자원 소환 범위
    public static readonly Range[] big_substance_left_range =
        new Range[2] {
                new Range(-136,-132),
                new Range(-200,-132)
            };
    public static readonly Range[] big_substance_right_range =
        new Range[2] {
                new Range(132,136),
                new Range(132,200)
            };

    //큰 자원 리젠 시간
    public static float big_substance_regen_term = DayCount_Cycle;

    //큰 자원 enable / disable
    public static readonly Range big_substance_enable_point = new Range(-10, 10);
    public static readonly Range big_substance_disable_point = new Range(-20, 20);
    #endregion

    //---------------------------- 기록 관리 ----------------------------

    public const string hard_mod_key = "HardMod";
    public const string crystal_data_key = "CrystalData";
    public const string ad_data_key = "ADData";
    public const string commercial_off_key = "CommercialOff";
    public const string cloth_buy_key = "ClothBuy";
    public const string have_save_key = "HaveSave";
    public const string see_review_key = "SeeReview";
    public const string see_hp_warning_key = "SeeHPWarning";
    public const string give_crystal_key = "GiveCrystalKey";
    public const string clear_num_key = "ClearNum";
    public const string option_data_key = "OptionData";
    public const string player_data_key = "PlayerData";
    public const string build_data_key = "BuildData";
    public const string system_data_key = "SystemData";
    public const string artifact_data_key = "ArtifactData";
    public const string buff_data_key = "BuffData";
    public const string spaceship_data_key = "SpaceshipData";
    public const string monster_data_key = "MonsterData";
    public const string boss_data_key = "BossData";
    public const string sp_boss_data_key = "SPBossData";
    public const string big_sub_data_key = "BigSubDataKey";
    public const string language_code_key = "LanguageCodeKey";
}
