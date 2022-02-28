# MarsSurvival

## 프로젝트 설명
* 2D 횡스크롤 서바이벌 로그라이크 모바일 게임
* 부서진 우주선을 수리하기 위해 자원을 캐고 습격해오는 적들을 물리치면 살아남는
서바이벌 어드밴쳐 게임
* 죽으면 처음부터 시작, 랜덤 자원 생성 등의 로그라이크 요소 존재
* 구글 플레이 스토어에 한국 및 해외에 출시해 3만다운로드까지 달성 했으나 내부사정으로 내려감
* 헝그리앱 기사 : [링크](http://www.hungryapp.co.kr/news/news_view.php?bcode=news&pid=45507&catecode=002&rtype=B&page=1&searchtype=subject&searchstr=&tcnt=&tbcnt=&block=&mn=&mx=)

## 영상(이미지 클릭 시 유투브로 이동)
[![MarsSurvival](https://img.youtube.com/vi/9xh5RbYUEX4/0.jpg)](https://youtu.be/9xh5RbYUEX4 "MarsSurvival")

## 개발환경
* Unity 5.3.1f1
* 구글 플러그인(결제 시스템)
* 안드로이드(모바일)

## 프로젝트 팀원 및 역할
* 프로그래머 1인, 프로그래머 + 그래픽 1인, 기획 2인
* 나의 역할 : 프로그래머

## 주요 개발 기능
* 레벨 디자인
	* 몬스터, 자원, 보스를 GlobalState 및 Sector에 따라 리젠율을 조정해 난이도 조정
	* Sector
		* 우주선에서 부터 정해둔 거리만큼 나눠둔 3개의 구역
		* 가상좌표계를 관리하는 MapManager Class에서 처리
		* 우주선에서 먼 구역일 수록 좋은 자원, 어려운 몬스터가 등장
		* 이 외에 카메라 시야 밖 일정 구역 안에서만 몬스터가 생성되도록 함
	* GlobalState
		* 날짜/밤낮/날씨 등 시간개념 관련 값을 처리하는 Class
		* 일정 날짜가 지날때 마다 전체 난이도 상승
		* 난이도에 따라 몬스터의 강함, 수 증가 및 리젠 시간 감소
		* 보스 또한 일정 난이도 이상이 되어야만 등장
		* 밤에만 전용으로 나오는 몬스터가 등장
		* 모래폭풍이 일정 확률로 불면 이때만 등장하는 몬스터가 등장
		* 상세 설명은 아래에
	* 최종 몬스터 생성 방식
		* 몬스터 종류별로 GlobalState 및 구역에 따른 가중치를 초기화
			* 가중치가 0이면 소환안함, 그 외에는 높을 수록 소환될 확률이 높음
			* ex) 약한 슬라임이 낮 + 0구역에 소환될 가중치 = 50
			* 모든 몬스터 소환 가중치를 total_rate에 저장
		* GlobalState에 따라 리젠 시간 체크
		* 확률 관련값 초기화
			* random = 0 ~ (GlobalState에 따른 total_rate값)
			* min = 0
		* min <= random < (min + 몬스터 소환 가중치) 가 true면 몬스터 소환
		* 그 후 소환한 몬스터 확률을 min값에 더함
		* 4, 5번 과정을 모든 몬스터 종류를 체크할 때까지 반복

* GlobalState 상세
	* 플레이 시간 : 프레임 델타타임값 누적
	* 게임 시간 : 플레이 시간 % 하루를 세는 상수값
	* 모든 인게임 시스템은 게임 시간을 따름(몬스터, 배경 등)
	* 상태값 : 낮, 밤, 낮폭풍, 밤폭풍 상태를 나타넴. 상태에 따라 등장하는 몬스터 조절
	* 레벨값 : 시간에 따라 난이도가 상승
	* 날짜값 : 플레이시간 / 하루를 세는 상수값 + 1
	* CheckGlobalTime 함수를 통해 맴버변수 업데이트

* 가상 좌표계(MapManager)
	* 인게임 Object 좌표 생성, 관리, 저장을 편리하게 하기 위한 Class
	* 가상좌표 = 현실 좌표 * 가상좌표 최대치 / 현실좌표 최대치
	* 미니맵에 표시되는 플레이어의 좌표 저장
	* 미니맵의 Sector 구분하는 위치값 계산
	* 자원 리젠 시 필요한 랜덤 위치 생성 및 기존 자원과 겹치는지 체크
	* 몬스터 리젠 시 필요한 랜덤 위치 생성
	* 자원, 몬스터 등의 Object가 설정해둔 범위를 벗어났는지 여부 체크
	* 미니맵, 자원 및 몬스터 랜덤 좌표 생성, 범위 벗어난 Object 체크 등을 계산함

* Save/Load
	* 저장 구현
		* Object의 저장할 데이터를 GameDAO Class에 미리 정의
		* 시스템 및 GameDAO에 있는 데이터들을 FileManager의 맴버변수로 저장
		* BinaryFormatter로 데이터를 직렬화(Serialize)해 MemoryStream에 저장
		* MemoryStream으로 다시 String으로 변환해 PlayerPrefab을 통해 데이터 저장
	* 로드 구현
		* PlayerPrefab에서 key값으로 원하는 데이터의 string 로드
		* 로드한 string이 있을 경우 BinaryFormatter를 통해 다시 역직렬화 후 형변환
	* 저장 시점
		* 지정한 게임상 일수가 될 때 마다 저장
		* 죽을 때 저장
		* 엔딩 볼 때 저장
	* 로드 시점 : 게임 초기화할 때
	
## 개발 내역
* ToDo리스트 : [링크](https://github.com/MiruSona/MarsSurvival/blob/main/Document/Todo.xlsx)
* 패치내역 : [링크](https://github.com/MiruSona/MarsSurvival/blob/main/Document/Patch.xlsx)
* 버그 및 수정 리스트 : [링크](https://github.com/MiruSona/MarsSurvival/blob/main/Document/Bug%26Fix.xlsx)