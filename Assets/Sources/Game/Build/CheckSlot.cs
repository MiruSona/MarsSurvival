using UnityEngine;
using System.Collections;

public class CheckSlot : MonoBehaviour {

    //참조
    private BuildSlot build_slot;

    //어느거인지
    public enum Slot
    {
        Slot1,
        Slot2
    }
    public Slot slot = Slot.Slot1;

	//초기화
	void Start () {
        build_slot = Player.instance.transform.FindChild("BuildSlot").GetComponent<BuildSlot>();
    }

    //건물 확인
    void OnTriggerStay2D(Collider2D col)
    {
        //이미 지은 건물이 있는지 확인
        if (col.CompareTag("Build"))
        {
            if (slot == Slot.Slot1)
                build_slot.slot1_bool = false;
            else
                build_slot.slot2_bool = false;
        }
    }

    //건물에서 벗어나면 다시 지을 수 있게
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Build"))
        {
            if (slot == Slot.Slot1)
                build_slot.slot1_bool = true;
            else
                build_slot.slot2_bool = true;
        }
    }
}
