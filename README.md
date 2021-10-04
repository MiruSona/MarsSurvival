# MarsSurvival

## 프로젝트 설명
* 횡스크롤 서바이벌 로그라이크 모바일 게임
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
* 서브 프로그래머 및 그래픽 1인, 메인 프로그래머 1인, 기획 2인
* 나의 역할 : 메인 프로그래머

## 개발 내역
* ToDo리스트 : [링크](https://github.com/MiruSona/MarsSurvival/blob/main/Document/Todo.xlsx)
* 패치내역 : [링크](https://github.com/MiruSona/MarsSurvival/blob/main/Document/Patch.xlsx)
* 버그 및 수정 리스트 : [링크](https://github.com/MiruSona/MarsSurvival/blob/main/Document/Bug%26Fix.xlsx)

## 개발 기능
* 시간 개념 : 날짜, 날씨, 월드타임
	* 현재 시간 : 프레임 델타타임값 누적
	* 게임 시간 : 현재 시간 % 하루를 세는 상수값
	* 모든 인게임 시스템은 게임 시간을 따름(몬스터, 배경 등)
	* 상태값 : 낮, 밤, 낮폭풍, 밤폭풍 상태를 나타넴. 상태에 따라 등장하는 몬스터 조절
	* 레벨값 : 시간에 따라 난이도가 상승
	* 날짜값도 따로 저장
* DAO : Data Access Object
	* 모든 상호작용 가능한 인게임 객체의 데이터 선언을 모아놓음
	* 자주 참조하는 단일 데이터(플레이어)의 경우 DAO에 정의하여 접근이 쉽도록 만듬
* 플레이어
	* 기본적인 이동은 좌우 버튼 모양 터치로 구현
	* 공격 : 공격 버튼 터치 시 총알 발사
		* 미리 총알을 생성해 "사용할 총알 Stack"에 저장(Push)하는 식으로 총알 풀을 구현
		* 총알 발사 콜 시 스텍에서 총알을 꺼내(Pop) 활성화
		* 총알이 역할을 다한 후(몬스터 공격, 화면 넘어감 등) "사용한 총알 stack"에 넣음(Push)
		* 재장전(공격 중지 시 재장전) 시 "사용한 총알 Stack"에서 "사용 가능한 총알 Stack"으로 옮겨준다
	* 도구 : 자원을 인식하면 자동으로 채취
		* 자원 인식은 투명한 충돌체를 플레이어 앞쪽에 배치해 구현
	* 수리 : 자원이 일정량 이상 있고 우주선 근처에 있으면 자동으로 실행
		* 우주선 인식은 플레이어 앞쪽에 있는 투명한 충돌체로 함
	* 빌드 : 터렛, 지뢰 등의 건물을 짓는 기능. UI에서 처리
		* 짓길 원하는 건물의 데이터는 DAO에서 접근
		* 건물 선택 시 위치 선택 UI가 활성화 되고 워하는 위치를 누르면 건물 생성
		* 지은 건물의 객체는 GameManager에서 관리
	* 업그레이드 : UI에서 업그레이드 처리
		* 플레이어의 데이터를 DAO에서 접근
		* 버튼 누르면 요구사항(자원 등)을 체크 후 업그레이드 실행
	* 대사 : 플레이어가 일정 시간마다 대사를 자막으로 보여줌
		* 대사는 txt파일로 저장해두어 게임 초기화 시 불러옴
		* 대사는 PlayerTalk 스크립트에서 관리
* 몬스터
	*
* 자원
	*
* 보스
	*