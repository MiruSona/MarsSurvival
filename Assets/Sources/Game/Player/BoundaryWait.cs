using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoundaryWait : MonoBehaviour {

    //참조
    private Image wait_bar;
    private GameDAO.Timer wait_timer = new GameDAO.Timer(0, 5f);
    [System.NonSerialized]
    public Transform target;
    private Transform player;
    private GameDAO.PlayerData player_data;
    private SystemDAO.GameManagerData gm_data;

    //값
    private Vector3 left_pos = new Vector3();
    private Vector3 right_pos = new Vector3();

    //초기화
    void Start () {
        wait_bar = transform.FindChild("WaitBar").GetComponent<Image>();
        player = Player.instance.transform;
        player_data = GameDAO.instance.player_data;
        gm_data = SystemDAO.instance.gm_data;
    }
	

	void FixedUpdate () {

        //플레이중 아니면 실행 X
        if (gm_data.state != SystemDAO.GameManagerData.State.isPlay)
            return;

        //타겟 있으면 실행
        if (target != null)
        {
            //위치 초기화
            left_pos = player.localPosition;
            right_pos = player.localPosition;
            left_pos.x = Define.real_point.min + 3f;
            right_pos.x = Define.real_point.max - 3f;

            //바 활성화
            wait_bar.gameObject.SetActive(true);
            //반전 방지
            wait_bar.rectTransform.localScale = new Vector3(player_data.facing, 1f, 1f);
        }
        else
        {
            //타겟 없으면 실행X 및 시간 초기화
            wait_timer.timer = 0;
            wait_bar.gameObject.SetActive(false);
        }

        //켜져 있으면 실행
        if (wait_bar.IsActive())
        {
            //바 표시
            wait_bar.fillAmount = (wait_timer.term - wait_timer.timer) / wait_timer.term;

            //타이머 체크 및 이동
            if (wait_timer.timer < wait_timer.term)
                wait_timer.timer += Time.deltaTime;
            else
            {
                if (target.localPosition.x < 0)
                    player.localPosition = right_pos;
                else
                    player.localPosition = left_pos;
            }
        }
	}
}
