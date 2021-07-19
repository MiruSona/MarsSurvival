using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SystemDAO : SingleTon<SystemDAO> {

    //----------------------- 맵(가상위치)관리자 -----------------------
    public class MapManager {
        #region 변수
        //가상 위치
        //플레이어 위치
        public float player_point;
        public int player_sector;
        //자원 가상위치 값들[구역][인덱스] = 위치값
        public int[][] substance_points = new int[Define.sector_total_num][]
            {
                new int[Define.sub_sectors[0].point_num],
                new int[Define.sub_sectors[1].point_num],
                new int[Define.sub_sectors[2].point_num]
            };

        //큰자원
        public float[] big_substance_points = new float[Define.big_substance_num];

        //몬스터
        public float[] monster_points = new float[Define.monster_spawn_num_limit];  //몬스터 위치리스트

        //보스
        public float boss_point = 0;    //보스 위치 리스트

        //빌드
        public float[] build_points = new float[Define.build_num_limit];   //빌드 위치 리스트 
        #endregion

        #region 초기화
        //위치 배정 초기화
        public void InitPoints() {
            //주인공 위치는 0에서 시작
            player_point = 0;

            //자원 위치 부여
            InitSubstancePoints();
        }

        //각 구역 위치 설정
        public void InitSubstancePoints() {
            for (int i = 0; i < Define.sector_total_num; i++) {
                SetRandomSubstancePoints(
                    substance_points[i],
                    Define.sub_sectors[i].sector_right,
                    Define.sub_sectors[i].index_right
                    );

                SetRandomSubstancePoints(
                    substance_points[i],
                    Define.sub_sectors[i].sector_left,
                    Define.sub_sectors[i].index_left
                    );
            }
        }
        #endregion

        #region 공용
        //위치 겹치는지 체크
        public bool CheckPointOverlap(int _point, int[] _check_points) {
            //겹치면 true 안겹치면 false
            bool send_bool = false;
            //양옆으로 offset만큼 더 확인
            int x_offset = 2;
            foreach (int check_point in _check_points) {
                if ((_point - x_offset) <= check_point && check_point <= (_point + x_offset))
                    send_bool = true;
            }

            return send_bool;
        }

        //현실 위치 -> 가상 위치
        public int ConvertToVirtualPoint(Transform _transform) {
            //가상 좌표 = 현실 좌표 * 가상좌표최대치 / 현실좌표최대치
            int send_point = (int)(_transform.position.x * Define.virtual_point.max / Define.real_point.max);

            return send_point;
        }

        public float ConvertToVirtualPointF(Transform _transform) {
            //가상 좌표 = 현실 좌표 * 가상좌표최대치 / 현실좌표최대치
            float send_point = _transform.position.x * Define.virtual_point.max / Define.real_point.max;

            return send_point;
        }

        //가상 위치 -> 현실 위치
        public Vector3 ConvertToRealPoint(int _point, float _height) {
            int send_point = (int)(_point * Define.real_point.max / Define.virtual_point.max);
            Vector3 send_vector3 = new Vector3(send_point, _height, 0);
            return send_vector3;
        }

        public Vector3 ConvertToRealPointF(float _point, float _height) {
            float send_point = _point * Define.real_point.max / Define.virtual_point.max;
            Vector3 send_vector3 = new Vector3(send_point, _height, 0);
            return send_vector3;
        }

        public bool CheckSafeZone() {
            bool send_bool = false;
            if (Define.safe_zone_range.min <= player_point && player_point <= Define.safe_zone_range.max) {
                send_bool = true;
            }
            return send_bool;
        }
        #endregion

        #region 플레이어
        //플레이어 실제 위치 반환
        public Vector3 GetPlayerRealPoint(float _sprite_height) {
            return ConvertToRealPointF(player_point, _sprite_height);
        }

        //플레이어 가상 위치 설정
        public void SetPlayerPoint(Transform _transform) {
            player_point = ConvertToVirtualPointF(_transform);
            for (int i = 0; i < Define.sector_total_num; i++) {
                if (Define.player_sectors[i].sector_left.min < player_point && player_point <= Define.player_sectors[i].sector_left.max)
                    player_sector = i;

                if (Define.player_sectors[i].sector_right.min <= player_point && player_point < Define.player_sectors[i].sector_right.max)
                    player_sector = i;
            }
        }
        #endregion

        #region 빌드
        //빌드 실제 위치 반환
        public Vector3 GetBuildRealPoint(float _sprite_height, int _index) {
            return ConvertToRealPointF(build_points[_index], _sprite_height);
        }

        //빌드 가상 위치 설정
        public void SetBuildPoint(Transform _transform, int _index) {
            build_points[_index] = ConvertToVirtualPointF(_transform);
        }

        //파괴된 빌드 가상 위치 설정
        public void DestroyBuildPoint(int _index) {
            //오른쪽 끝 좌표로
            build_points[_index] = Define.init_vpos_plus;
        }

        //빌드 파괴 체크
        public bool CheckDestroyBuild(int _index) {
            bool send_bool = false;
            float point = build_points[_index];
            float distance = Math.Abs(player_point - point);

            //각 빌드와 플레이어 사이 거리가 일정량 이상이면 true
            if (distance > Define.build_destroy_distance)
                send_bool = true;

            return send_bool;
        }

        //빌드 enable 체크
        public bool CheckEnableBuild(int _index) {
            bool send_bool = false;
            float point = build_points[_index];
            float enable_point_min = Define.build_enable_point.min + player_point;
            float enable_point_max = Define.build_enable_point.max + player_point;

            //각 포인트가 enable 범위에 들어오면 true
            if (enable_point_min <= point && point <= enable_point_max)
                send_bool = true;

            return send_bool;
        }

        //빌드 disable 체크
        public bool CheckDisableBuild(int _index) {
            bool send_bool = false;
            float point = build_points[_index];
            float disable_point_min = Define.build_disable_point.min + player_point;
            float disable_point_max = Define.build_disable_point.max + player_point;

            //각 포인트가 enable 범위에 들어오면 true
            if (point < disable_point_min || point > disable_point_max)
                send_bool = true;

            return send_bool;
        }
        #endregion

        #region 몬스터
        //몬스터 실제 위치 반환
        public Vector3 GetMonsterRealPoint(float _sprite_height, int _index) {
            return ConvertToRealPointF(monster_points[_index], _sprite_height);
        }

        //몬스터 가상 위치 설정
        public void SetMonsterPoint(Transform _transform, int _index) {
            monster_points[_index] = ConvertToVirtualPointF(_transform);
        }

        //몬스터 랜덤 가상 위치 추가
        public void AddRandomMonsterPoint(int _index, int _level) {
            //0이면 왼쪽, 1이면 오른쪽
            int choose_range = UnityEngine.Random.Range(0, 2);
            float random = 0;
            //왼쪽 / 오른쪽 최대, 최소 범위
            float left_range_min = player_point + Define.monster_left_range.min;
            float left_range_max = player_point + Define.monster_left_range.max;
            float right_range_min = player_point + Define.monster_right_range.min;
            float right_range_max = player_point + Define.monster_right_range.max;

            //레벨에 따른 범위 못벗어 나게
            if (Define.player_sectors[_level].sector_left.min > left_range_min) {
                left_range_min = right_range_min;
                left_range_max = right_range_max;
            }

            if (Define.player_sectors[_level].sector_right.max < right_range_max) {
                right_range_min = left_range_min;
                right_range_max = left_range_max;
            }

            if (choose_range == 0) {
                random = UnityEngine.Random.Range(left_range_min, left_range_max);
                monster_points[_index] = random;
            } else {
                random = UnityEngine.Random.Range(right_range_min, right_range_max);
                monster_points[_index] = random;
            }
        }

        //파괴된 몬스터 가상 위치 설정
        public void DestroyMonsterPoint(int _index) {
            //오른쪽 끝 좌표로
            monster_points[_index] = Define.init_vpos_plus;
        }

        //몬스터 disable 체크
        public bool CheckDisableMonster(int _index) {
            bool send_bool = false;
            float point = monster_points[_index];
            float disable_point_min = Define.monster_disable_point.min + player_point;
            float disable_point_max = Define.monster_disable_point.max + player_point;

            //각 포인트가 enable 범위에 들어오면 true
            if (point <= disable_point_min)
                send_bool = true;

            if (point >= disable_point_max)
                send_bool = true;

            return send_bool;
        }
        #endregion

        #region 보스
        //보스 실제 위치 반환
        public Vector3 GetBossRealPoint() {
            return ConvertToRealPointF(boss_point, 0f);
        }

        //보스 가상 위치 설정
        public void SetBossPoint(Transform _transform) {
            boss_point = ConvertToVirtualPointF(_transform);
        }

        //보스 랜덤 가상 위치 추가
        public void AddRandomBossPoint(int _level) {
            float random = 0;

            float min = 0;
            float max = 0;

            //섹터가 하나 이상 풀렸을 때만
            if (instance.global_state.level > 0) {
                if (player_point < 0) {
                    min = Define.boss_left_range[instance.global_state.level - 1].min;
                    max = Define.boss_left_range[instance.global_state.level - 1].max;
                } else {
                    min = Define.boss_right_range[instance.global_state.level - 1].min;
                    max = Define.boss_right_range[instance.global_state.level - 1].max;
                }
            }

            random = UnityEngine.Random.Range(min, max);
            boss_point = random;
        }

        //파괴된 보스 가상 위치 설정
        public void DestroyBossPoint() {
            //오른쪽 끝 좌표로
            boss_point = Define.init_vpos_plus;
        }

        //보스 disable 체크
        public bool CheckDisableBoss() {
            bool send_bool = false;

            //각 포인트가 disable 범위에 들어오면 true
            if (boss_point < 0)
            {
                if (player_point >= 0)
                    send_bool = true;
            }
            else
            {
                if (player_point < 0)
                    send_bool = true;
            }

            return send_bool;
        }

        //주인공 위치 체크
        public bool CheckSpawnBoss() {
            bool send_bool = false;

            float check_point = 0;

            if (instance.global_state.level > 0) {
                if (player_point < 0) {
                    check_point = Define.boss_left_range[instance.global_state.level - 1].max;
                    if (player_point - 10 <= check_point)
                        send_bool = true;
                } else {
                    check_point = Define.boss_right_range[instance.global_state.level - 1].min;
                    if (player_point + 10 >= check_point)
                        send_bool = true;
                }
            }

            return send_bool;
        }
        #endregion

        #region 자원
        //자원 enable 체크
        public bool CheckEnableSubstance(int _index, int _sector) {
            bool send_bool = false;
            int point = substance_points[_sector][_index];
            float enable_point_min = Define.substance_enable_point.min + player_point;
            float enable_point_max = Define.substance_enable_point.max + player_point;

            //각 포인트가 enable 범위에 들어오면 true
            if (enable_point_min <= point && point <= enable_point_max)
                send_bool = true;

            return send_bool;
        }

        //자원 disable 체크
        public bool CheckDisableSubstance(int _index, int _sector) {
            bool send_bool = false;
            int point = substance_points[_sector][_index];
            float disable_point_min = Define.substance_disable_point.min + player_point;
            float disable_point_max = Define.substance_disable_point.max + player_point;

            //각 포인트가 enable 범위에 들어오면 true
            if (point <= disable_point_min)
                send_bool = true;

            if (point >= disable_point_max)
                send_bool = true;

            return send_bool;
        }

        //자원 실제 위치 반환
        public Vector3 GetSubstanceRealPoint(int _sector, int _index, float _sprite_hieght) {
            if (_sector >= Define.sector_total_num)
                return Vector3.zero;

            Vector3 send_vector = new Vector3(0, 10, 0);

            send_vector = ConvertToRealPoint(substance_points[_sector][_index], _sprite_hieght);

            return send_vector;
        }

        //파괴된 자원 가상 위치 설정
        public void SetDestroyedSubstancePoint(int _index, int _sector, float _x) {
            if (_x >= 0)
                substance_points[_sector][_index] = Define.init_vpos_plus;
            else
                substance_points[_sector][_index] = Define.init_vpos_minus;

        }

        //자원 가상 위치 무작위 설정
        public void SetRandomSubstancePoint(int _index, int _sector) {
            int random = 0;

            if (substance_points[_sector][_index] < 0) {
                do {
                    random = UnityEngine.Random.Range(Define.sub_sectors[_sector].sector_left.min, Define.sub_sectors[_sector].sector_left.max);
                } while (CheckPointOverlap(random, substance_points[_sector]));
            } else {
                do {
                    random = UnityEngine.Random.Range(Define.sub_sectors[_sector].sector_right.min, Define.sub_sectors[_sector].sector_right.max);
                } while (CheckPointOverlap(random, substance_points[_sector]));
            }

            substance_points[_sector][_index] = random;
        }

        //자원 가상 위치 무작위 설정(여러개)
        public void SetRandomSubstancePoints(int[] _points, Define.Range _range, Define.Range _index) {
            int random = 0;
            bool over_bound = false;

            //랜덤값 부여
            if (_range.min >= 0) {
                random = _range.min;
                for (int i = _index.min; i < _index.max; i++) {
                    if (!over_bound)
                        random += UnityEngine.Random.Range(Define.substance_interval.min, Define.substance_interval.max);

                    if (random >= _range.max)
                        over_bound = true;

                    if (over_bound)
                        do {
                            random = UnityEngine.Random.Range(_range.min, _range.max);
                        } while (CheckPointOverlap(random, _points));

                    _points[i] = random;
                }
            } else if (_range.min < 0) {
                random = _range.max;
                for (int i = _index.min; i < _index.max; i++) {
                    if (!over_bound)
                        random -= UnityEngine.Random.Range(Define.substance_interval.min, Define.substance_interval.max);

                    if (random <= _range.min)
                        over_bound = true;

                    if (over_bound)
                        do {
                            random = UnityEngine.Random.Range(_range.min, _range.max);
                        } while (CheckPointOverlap(random, _points));

                    _points[i] = random;
                }
            }
        }
        #endregion

        #region 큰 자원
        //큰 자원 실제 위치 반환
        public Vector3 GetBigSubstaceRealPoint(int _index, float _height) {
            return ConvertToRealPointF(big_substance_points[_index], _height);
        }

        //큰 자원 가상 위치 설정
        public void SetBigSubstacePoint(Transform _transform, int _index) {
            big_substance_points[_index] = ConvertToVirtualPointF(_transform);
        }

        //큰 자원 랜덤 가상 위치 생성
        public void SetRandomBigSubstancePoint() {
            int min_left = Define.big_substance_left_range[instance.global_state.level - 1].min;
            int max_left = Define.big_substance_left_range[instance.global_state.level - 1].max;
            int min_right = Define.big_substance_right_range[instance.global_state.level - 1].min;
            int max_right = Define.big_substance_right_range[instance.global_state.level - 1].max;

            for (int i = 0; i < big_substance_points.Length; i++) {
                if (i < big_substance_points.Length / 2)
                    big_substance_points[i] = UnityEngine.Random.Range(min_left, max_left);
                else
                    big_substance_points[i] = UnityEngine.Random.Range(min_right, max_right);
            }
        }

        //파괴된 큰 자원 가상 위치 설정
        public void DestroyBigSubstancePoint(int _index) {
            //오른쪽 끝 좌표로
            big_substance_points[_index] = Define.init_vpos_plus;
        }

        //큰 자원 disable 체크
        public bool CheckDisableBigSubstance(int _index) {
            bool send_bool = false;
            float point = big_substance_points[_index];
            float disable_point_min = Define.big_substance_disable_point.min + player_point;
            float disable_point_max = Define.big_substance_disable_point.max + player_point;

            //각 포인트가 disable 범위에 들어오면 true
            if (point <= disable_point_min)
                send_bool = true;

            if (point >= disable_point_max)
                send_bool = true;

            return send_bool;
        }

        //큰 자원 enable 체크
        public bool CheckEnableBigSubstance(int _index) {
            bool send_bool = false;
            float point = big_substance_points[_index];
            float enable_point_min = Define.big_substance_enable_point.min + player_point;
            float enable_point_max = Define.big_substance_enable_point.max + player_point;

            //각 포인트가 enable 범위에 들어오면 true
            if (enable_point_min <= point && point <= enable_point_max)
                send_bool = true;

            return send_bool;
        }
        #endregion

    }
    public MapManager map_manager = new MapManager();

    //----------------------- 파일 관리자 -----------------------
    public class FileManager {
        //최초 생성 확인
        public bool have_save = false;

        //리뷰 보았는지 여부
        public bool see_review = false;

        //경고창 봤는지 여부
        public bool see_hp_warnig = false;

        //클리어 횟수
        public int clear_num = 0;        

        #region 기록 데이터
        //설정 기록
        [Serializable]
        public class OptionData {
            //사운드
            public bool sound = true;

            public void Init() {
                sound = true;
            }
        }
        public OptionData option_data = new OptionData();

        //시스템 기록
        [Serializable]
        public class SystemData {
            //전체 상태
            public float total_play_time = 0;
            public int level = 0;
            public int state = 0;

            //모레 폭풍
            public float storm_cool_time = 0f;
            public float storm_enable_time = 0f;
            public bool storm_enable = false;

            public void Init() {
                total_play_time = 0;
                level = 0;
                state = 0;
                storm_cool_time = 0f;
                storm_enable_time = 0f;
                storm_enable = false;
            }
        }
        public SystemData system_data = new SystemData();

        //광고 기록
        [Serializable]
        public class ADData {
            //전체 상태
            public int day = 0;
            public int look_ad_num = 0;
            public int shown_num = 0;
            public bool ad_rebirth = false;

            public void Init() {
                day = 0;
                look_ad_num = 0;
                shown_num = 0;
                ad_rebirth = false;
            }
        }
        public ADData ad_data = new ADData();

        //아티펙트 기록
        [Serializable]
        public class ArtifactData {
            public bool[] active_list = new bool[Define.artifact_num];

            public void Init() {
                active_list = new bool[Define.artifact_num];
            }
        }
        public ArtifactData artifact_data = new ArtifactData();

        //우주선 기록
        [Serializable]
        public class SpaceShipData {
            public float step_hp = 0;

            public void Init() {
                step_hp = 0;
            }
        }
        public SpaceShipData spaceship_data = new SpaceShipData();

        //플레이어 기록
        [Serializable]
        public class PlayerData {
            //가상 위치
            public float point = 0;

            //스탯
            public float hp = 1200f;     //체력

            //상태
            public GameDAO.PlayerData.State state = GameDAO.PlayerData.State.isAlive;

            //자원
            public int metal = 0;       //금속
            public int bio = 0;         //바이오

            //도구
            public int tool_step = 0;        //진화 정도

            //무기
            public int weapon_level = 0;   //강화 정도
            public int weapon_step = 0;    //진화 정도

            //중독 시간
            public float poison_left_time = 0f;

            public void Init() {
                point = 0;

                hp = 1200f;

                metal = 0;
                bio = 0;

                tool_step = 0;

                weapon_level = 0;
                weapon_step = 0;

                poison_left_time = 0f;
            }
        }
        public PlayerData player_data = new PlayerData();
        public int crystal_data = 0;

        //빌드 기록
        [Serializable]
        public class BuildData {
            public float[] build_points = new float[Define.build_num_limit];

            public int turret_step = 0;
            public int shield_step = 0;

            public List<GameDAO.BuildData> build_datas = new List<GameDAO.BuildData>();

            public void Init() {
                build_points = new float[Define.build_num_limit];

                turret_step = 0;
                shield_step = 0;

                build_datas.Clear();
            }
        }
        public BuildData build_data = new BuildData();

        //몬스터 기록
        [Serializable]
        public class MonsterData {
            public float[] points = new float[Define.monster_spawn_num_limit];
            public GameDAO.MonsterData[] datas =
                new GameDAO.MonsterData[Define.monster_spawn_num_limit]
                {
                    null,null,null,null
                };

            public void Init() {
                points = new float[Define.monster_spawn_num_limit];
                datas =
                new GameDAO.MonsterData[Define.monster_spawn_num_limit]
                {
                    null,null,null,null
                };
            }
        }
        public MonsterData monster_data = new MonsterData();

        //보스 기록
        [Serializable]
        public class BossData {
            //GM
            public float regen_term = Define.boss_regen_min;
            public bool left_spawned = false;
            public bool right_spawned = false;
            public GameDAO.BossData data = null;

            //MapManager
            public float point = 0;            

            public void Init() {
                regen_term = Define.boss_regen_min;
                left_spawned = false;
                right_spawned = false;
                data = null;

                point = 0;               
            }
        }
        public BossData boss_data = new BossData();

        //큰 자원 기록
        [Serializable]
        public class BigSubstanceData {
            public GameDAO.SubstanceData[] left_subdata =
                new GameDAO.SubstanceData[Define.big_substance_num]
                {
                    new GameDAO.SubstanceData(200,200,0),
                    new GameDAO.SubstanceData(200,200,0)
                };

            public float[] points =
                new float[Define.big_substance_num]
                {
                    Define.init_vpos_plus,
                    Define.init_vpos_plus
                };

            public void Init() {
                left_subdata =
                new GameDAO.SubstanceData[Define.big_substance_num]
                {
                    new GameDAO.SubstanceData(200,200,0),
                    new GameDAO.SubstanceData(200,200,0)
                };

                points = new float[Define.big_substance_num]
                {
                    Define.init_vpos_plus,
                    Define.init_vpos_plus
                };
            }
        }
        public BigSubstanceData big_sub_data = new BigSubstanceData();
        #endregion

        //데이터 기록
        public void RecordDatas() {
            //세이브 표시
            have_save = true;

            #region 참조
            GameManager gm = GameManager.instance;
            GlobalState gs = instance.global_state;
            StormData std = instance.storm_data;
            MapManager mm = instance.map_manager;
            GameDAO.PlayerData pd = GameDAO.instance.player_data;
            GameDAO.SpaceShipData sd = GameDAO.instance.spaceship_data;
            List<GameDAO.BuildData> bd = GameManager.instance.build_spawn_datas;
            GameObject[] md = GameManager.instance.monster_spawn_list;
            int t_step = GameDAO.instance.turret_data.step;
            int s_step = GameDAO.instance.shield_data.step;
            GameObject[] bspd = GameManager.instance.big_sub_pool;
            #endregion

            #region 플레이어 데이터
            player_data.point = mm.player_point;
            player_data.hp = pd.hp;
            player_data.state = pd.state;
            player_data.metal = pd.my_subdata.metal;
            player_data.bio = pd.my_subdata.bio;
            crystal_data = pd.my_subdata.crystal;
            player_data.tool_step = pd.tool_data.step;
            player_data.weapon_level = pd.weapon_data.level;
            player_data.weapon_step = pd.weapon_data.step;
            player_data.poison_left_time = pd.poisoned_timer.timer;
            #endregion

            #region 빌드 데이터
            build_data.build_points = mm.build_points;
            build_data.turret_step = t_step;
            build_data.shield_step = s_step;
            build_data.build_datas = bd;
            #endregion

            #region 시스템 데이터
            system_data.total_play_time = gs.real_time;
            system_data.level = gs.level;
            system_data.state = gs.state;
            system_data.storm_cool_time = std.cool_time.timer;
            system_data.storm_enable_time = std.enable_time.timer;
            system_data.storm_enable = std.enable;
            #endregion

            //우주선 데이터
            spaceship_data.step_hp = sd.step_hp;

            #region 몬스터 데이터
            monster_data.points = mm.monster_points;
            for (int i = 0; i < md.Length; i++) {
                if (md[i] == null)
                    monster_data.datas[i] = null;
                else
                    monster_data.datas[i] = md[i].GetComponent<MonsterCommon>().monster_data;
            }
            #endregion
            
            #region 보스 데이터
            boss_data.regen_term = Define.boss_regen_term;
            boss_data.left_spawned = gm.boss_left_spawned;
            boss_data.right_spawned = gm.boss_right_spawned;
            if (gm.boss_spawn == null)
                boss_data.data = null;
            else
                boss_data.data = gm.boss_spawn.GetComponent<BossCommon>().boss_data;

            boss_data.point = mm.boss_point; 
            #endregion

            #region 큰 자원 데이터
            for (int i = 0; i < bspd.Length; i++)
                big_sub_data.left_subdata[i] = bspd[i].GetComponent<BigSubstance>().my_data;
            big_sub_data.points = mm.big_substance_points;
            #endregion
        }

        //데이터 로드
        public void LoadDatas() {
            #region 참조
            GameManager gm = GameManager.instance;
            GlobalState gs = instance.global_state;
            StormData std = instance.storm_data;
            MapManager mm = instance.map_manager;
            GameDAO.PlayerData pd = GameDAO.instance.player_data;
            GameDAO.SpaceShipData sd = GameDAO.instance.spaceship_data;
            List<GameDAO.BuildData> bd = GameManager.instance.build_spawn_datas;
            GameDAO.TurretData t_data = GameDAO.instance.turret_data;
            GameDAO.ShieldData s_data = GameDAO.instance.shield_data;
            GameObject[] bspd = GameManager.instance.big_sub_pool;
            #endregion
            
            #region 플레이어 데이터
            mm.player_point = player_data.point;
            pd.hp = player_data.hp;
            pd.state = player_data.state;
            pd.my_subdata.metal = player_data.metal;
            pd.my_subdata.bio = player_data.bio;
            pd.my_subdata.crystal = crystal_data;
            pd.tool_data.step = player_data.tool_step;
            pd.weapon_data.level = player_data.weapon_level;
            pd.weapon_data.step = player_data.weapon_step;
            pd.poisoned_timer.timer = player_data.poison_left_time;
            #endregion
            
            #region 빌드 데이터
            mm.build_points = build_data.build_points;
            t_data.step = build_data.turret_step;
            s_data.step = build_data.shield_step;
            for (int i = 0; i < build_data.build_datas.ToArray().Length; i++)
                bd.Add(build_data.build_datas[i]);
            #endregion
            
            #region 시스템 데이터
            gs.real_time = system_data.total_play_time;
            gs.level = system_data.level;
            gs.state = system_data.state;
            std.cool_time.timer = system_data.storm_cool_time;
            std.enable_time.timer = system_data.storm_enable_time;
            std.enable = system_data.storm_enable;
            #endregion

            //우주선 데이터
            sd.step_hp = spaceship_data.step_hp;

            //몬스터 데이터
            mm.monster_points = monster_data.points;
            
            #region 보스 데이터
            Define.boss_regen_term = boss_data.regen_term;
            gm.boss_left_spawned = boss_data.left_spawned;
            gm.boss_right_spawned = boss_data.right_spawned;
            mm.boss_point = boss_data.point; 
            #endregion

            #region 큰 자원 데이터
            for (int i = 0; i < bspd.Length; i++)
                bspd[i].GetComponent<BigSubstance>().my_data = big_sub_data.left_subdata[i];
            mm.big_substance_points = big_sub_data.points;
            #endregion
        }

        //아티펙트 기록
        public void SaveArtifact() {
            //기록
            GameDAO.ArtifactData data = GameDAO.instance.artifact_data;
            for (int i = 0; i < data.artifcat.Length; i++)
                artifact_data.active_list[i] = data.artifcat[i].activate;

            //세이브
            SaveFile(Define.clear_num_key);
            SaveFile(Define.artifact_data_key);
        }

        //아티펙트 로드
        public void LoadArtifact() {
            //로드
            LoadFile(Define.clear_num_key);
            LoadFile(Define.artifact_data_key);

            //적용
            GameDAO.ArtifactData data = GameDAO.instance.artifact_data;
            data.SetArtifact(clear_num);
            for (int i = 0; i < data.artifcat.Length; i++)
                data.artifcat[i].activate = artifact_data.active_list[i];
        }

        //광고 세이브
        public void SaveADData() {
            SaveFile(Define.ad_data_key);
        }

        //광고 로드
        public void LoadADData() {
            LoadFile(Define.ad_data_key);
        }

        //리뷰 세이브
        public void SaveSeeReview()
        {
            SaveFile(Define.see_review_key);
        }

        //체력 세이브
        public void SaveSeeHPWarning()
        {
            SaveFile(Define.see_hp_warning_key);
        }

        #region 모든 데이터 처리

        public void Init() {
            system_data.Init();
            spaceship_data.Init();
            player_data.Init();
            build_data.Init();
            monster_data.Init();
            boss_data.Init();
            big_sub_data.Init();
        }

        //모든 데이터 저장(파일)
        public void SaveAll() {
            //기록
            RecordDatas();
            //저장
            SaveFile(Define.crystal_data_key);
            SaveFile(Define.have_save_key);
            SaveFile(Define.option_data_key);
            SaveFile(Define.player_data_key);
            SaveFile(Define.build_data_key);
            SaveFile(Define.system_data_key);
            SaveFile(Define.spaceship_data_key);
            SaveFile(Define.monster_data_key);
            SaveFile(Define.boss_data_key);
            SaveFile(Define.big_sub_data_key);
        }

        //모든 데이터 로드(파일)
        public void LoadAll() {
            //로드
            LoadFile(Define.crystal_data_key);
            LoadFile(Define.have_save_key);
            LoadFile(Define.see_review_key);
            LoadFile(Define.see_hp_warning_key);
            LoadFile(Define.option_data_key);
            LoadFile(Define.player_data_key);
            LoadFile(Define.build_data_key);
            LoadFile(Define.system_data_key);
            LoadFile(Define.spaceship_data_key);
            LoadFile(Define.monster_data_key);
            LoadFile(Define.boss_data_key);
            LoadFile(Define.big_sub_data_key);
            //불러오기
            LoadDatas();
        }

        //베이스 데이터 제거(아티펙트 제외)
        public void DeleteBase() {
            have_save = false;
            SaveFile(Define.have_save_key);

            ad_data.ad_rebirth = false;
            SaveADData();

            PlayerPrefs.DeleteKey(Define.system_data_key);
            PlayerPrefs.DeleteKey(Define.spaceship_data_key);
            PlayerPrefs.DeleteKey(Define.player_data_key);
            PlayerPrefs.DeleteKey(Define.build_data_key);
            PlayerPrefs.DeleteKey(Define.monster_data_key);
            PlayerPrefs.DeleteKey(Define.boss_data_key);
            PlayerPrefs.DeleteKey(Define.big_sub_data_key);
            PlayerPrefs.DeleteKey(Define.see_hp_warning_key);
            system_data.Init();
            spaceship_data.Init();
            player_data.Init();
            build_data.Init();
            monster_data.Init();
            boss_data.Init();
            big_sub_data.Init();
        }

        //모든 데이터 제거
        public void DeletAllSave() {
            PlayerPrefs.DeleteAll();
            have_save = false;
            system_data.Init();
            spaceship_data.Init();
            player_data.Init();
            build_data.Init();
            monster_data.Init();
            boss_data.Init();
            big_sub_data.Init();
        }

        #endregion

        #region 공용

        //세이브
        public void SaveFile(string key) {
            var binary_formatter = new BinaryFormatter();
            var memory_stream = new MemoryStream();

            switch (key) {
                case Define.crystal_data_key:
                    binary_formatter.Serialize(memory_stream, crystal_data);
                    break;

                case Define.ad_data_key:
                    binary_formatter.Serialize(memory_stream, ad_data);
                    break;

                case Define.have_save_key:
                    binary_formatter.Serialize(memory_stream, have_save);
                    break;

                case Define.see_review_key:
                    binary_formatter.Serialize(memory_stream, see_review);
                    break;

                case Define.see_hp_warning_key:
                    binary_formatter.Serialize(memory_stream, see_hp_warnig);
                    break;

                case Define.clear_num_key:
                    binary_formatter.Serialize(memory_stream, clear_num);
                    break;

                case Define.option_data_key:
                    binary_formatter.Serialize(memory_stream, option_data);
                    break;

                case Define.player_data_key:
                    binary_formatter.Serialize(memory_stream, player_data);
                    break;

                case Define.build_data_key:
                    binary_formatter.Serialize(memory_stream, build_data);
                    break;

                case Define.system_data_key:
                    binary_formatter.Serialize(memory_stream, system_data);
                    break;

                case Define.artifact_data_key:
                    binary_formatter.Serialize(memory_stream, artifact_data);
                    break;

                case Define.spaceship_data_key:
                    binary_formatter.Serialize(memory_stream, spaceship_data);
                    break;

                case Define.monster_data_key:
                    binary_formatter.Serialize(memory_stream, monster_data);
                    break;

                case Define.boss_data_key:
                    binary_formatter.Serialize(memory_stream, boss_data);
                    break;

                case Define.big_sub_data_key:
                    binary_formatter.Serialize(memory_stream, big_sub_data);
                    break;
            }

            PlayerPrefs.SetString(key, Convert.ToBase64String(memory_stream.GetBuffer()));
        }

        //로드
        public void LoadFile(string key) {
            var data = PlayerPrefs.GetString(key);

            //저장된게 있을 경우 받아와서 저장
            if (!string.IsNullOrEmpty(data)) {
                //로드
                var binary_formatter = new BinaryFormatter();
                var memory_stream = new MemoryStream(Convert.FromBase64String(data));

                switch (key) {
                    case Define.crystal_data_key:
                        crystal_data = (int)binary_formatter.Deserialize(memory_stream);
                        break;

                    case Define.ad_data_key:
                        ad_data = (ADData)binary_formatter.Deserialize(memory_stream);
                        break;

                    case Define.have_save_key:
                        have_save = (bool)binary_formatter.Deserialize(memory_stream);
                        break;

                    case Define.see_review_key:
                        see_review = (bool)binary_formatter.Deserialize(memory_stream);
                        break;

                    case Define.see_hp_warning_key:
                        see_hp_warnig = (bool)binary_formatter.Deserialize(memory_stream);
                        break;

                    case Define.clear_num_key:
                        clear_num = (int)binary_formatter.Deserialize(memory_stream);
                        break;

                    case Define.option_data_key:
                        option_data = (OptionData)binary_formatter.Deserialize(memory_stream);
                        break;

                    case Define.player_data_key:
                        player_data = (PlayerData)binary_formatter.Deserialize(memory_stream);
                        break;

                    case Define.build_data_key:
                        build_data = (BuildData)binary_formatter.Deserialize(memory_stream);
                        break;

                    case Define.system_data_key:
                        system_data = (SystemData)binary_formatter.Deserialize(memory_stream);
                        break;

                    case Define.artifact_data_key:
                        artifact_data = (ArtifactData)binary_formatter.Deserialize(memory_stream);
                        break;

                    case Define.spaceship_data_key:
                        spaceship_data = (SpaceShipData)binary_formatter.Deserialize(memory_stream);
                        break;

                    case Define.monster_data_key:
                        monster_data = (MonsterData)binary_formatter.Deserialize(memory_stream);
                        break;

                    case Define.boss_data_key:
                        boss_data = (BossData)binary_formatter.Deserialize(memory_stream);
                        break;

                    case Define.big_sub_data_key:
                        big_sub_data = (BigSubstanceData)binary_formatter.Deserialize(memory_stream);
                        break;
                }
            }
        }

        //세이브 있는지 체크
        public void CheckSave() {
            LoadFile(Define.have_save_key);
        }

        #endregion
    }
    public FileManager file_manager = new FileManager();

    //----------------------- 게임 관리자 데이터 -----------------------
    public class GameManagerData {
        public enum State {
            isInit,     //초기화
            isPlay,     //게임진행
            isPause,    //일시정지
            isDead,     //죽었을 시
            isEnd       //엔딩
        }
        public State state = State.isInit;
    }
    public GameManagerData gm_data = new GameManagerData();

    //----------------------- 날씨 / 시간 -----------------------
    //전체 시간 /날씨 조절
    public class GlobalState {
        #region 변수
        //실제 시간 -> 미리 해떠있게 설정
        public float real_time = Define.DayCount_Cycle / 4.0f;
        //플레이 시간
        public float seconds_time = 0;
        //게임상 시간
        public float game_time = 0;
        //몇일 지났는지
        public int day_num = 1;
        //현재 난이도
        public int level = 0;
        //현재 상태 -> 0 = 낮, 1 = 밤, 2 = 낮폭풍, 3 = 밤폭풍
        public int state = 0;
        #endregion

        #region 함수
        public void Init() {
            //1초단위 시간
            seconds_time = Mathf.Floor(real_time);
            day_num = (int)(seconds_time / Define.DayCount_Cycle) + 1;
        }

        //시간 흐름에 따라 전부 체크
        public void CheckGlobalTime(bool _storm_enable) {
            //실제 시간
            real_time += Time.deltaTime;
            //1초단위 시간
            seconds_time = Mathf.Floor(real_time);
            //게임상 시간
            game_time = real_time % Define.DayCount_Cycle;

            //현재 날짜 = 플레이 타임 / 날짜 카운트 주기
            if (seconds_time % Define.DayCount_Cycle == 0)
                day_num = (int)(seconds_time / Define.DayCount_Cycle) + 1;

            //날짜가 일정량되면 level++
            if (day_num >= Define.Secotr1_Open_Day && level == 0)
                level = 1;
            if (day_num >= Define.Secotr2_Open_Day && level == 1)
                level = 2;

            switch (state) {
                case Define.Day:
                case Define.DayStorm:
                    //폭풍 안불때
                    if (!_storm_enable) {
                        //폭풍 안부니까 안부는걸로 변경
                        if (state == Define.DayStorm)
                            state = Define.Day;

                        //낮밤 바뀔때면 밤으로
                        if ((seconds_time % Define.Day_Cycle == 0) && (seconds_time % Define.Night_Cycle != 0))
                            state = Define.Night;
                    } else {//폭풍 불때
                        //낮밤 바뀔때면 밤폭풍 / 아니면 낮폭풍
                        if ((seconds_time % Define.Day_Cycle == 0) && (seconds_time % Define.Night_Cycle != 0))
                            state = Define.NightStorm;
                        else
                            state = Define.DayStorm;
                    }
                    break;

                case Define.Night:
                case Define.NightStorm:
                    //폭풍 안불때
                    if (!_storm_enable) {
                        //폭풍 안부니까 안부는걸로 변경
                        if (state == Define.NightStorm)
                            state = Define.Night;

                        //낮밤 바뀔때면 낮으로
                        if (seconds_time % Define.Night_Cycle == 0)
                            state = Define.Day;
                    } else {//폭풍 불때
                        //낮밤 바뀔때면 낮폭풍 / 아니면 밤폭풍
                        if (seconds_time % Define.Night_Cycle == 0)
                            state = Define.DayStorm;
                        else
                            state = Define.NightStorm;
                    }
                    break;
            }
        }
        #endregion
    }
    public GlobalState global_state = new GlobalState();

    //모레폭풍 데이터
    public class StormData {
        public bool enable = false;
        public GameDAO.Timer cool_time = new GameDAO.Timer(0, Define.Storm_Cool_Cycle);
        public GameDAO.Timer enable_time = new GameDAO.Timer(0, Define.Storm_Enable_Cycle);

        //폭풍 주기 체크
        public void StormCycle() {
            if (!enable) {
                if (cool_time.timer < cool_time.term)
                    cool_time.timer += Time.fixedDeltaTime;
                else {
                    enable = true;
                    cool_time.term = UnityEngine.Random.Range(Define.Day_Cycle + Define.Day_1D3_Cycle, Define.DayCount_Cycle * 2);
                    cool_time.timer = 0;
                }
            } else {
                if (enable_time.timer < enable_time.term)
                    enable_time.timer += Time.fixedDeltaTime;
                else {
                    enable = false;
                    enable_time.term = UnityEngine.Random.Range(Define.Day_Cycle - Define.Day_1D3_Cycle, Define.Day_Cycle);
                    enable_time.timer = 0;
                }
            }
        }
    }
    public StormData storm_data = new StormData();
    
}
