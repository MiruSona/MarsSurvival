using UnityEngine;
using System.Collections;

public class BuildEffet : MonoBehaviour {

    //참조
    private Animator animator;

	//초기화
	void Start () {
        animator = GetComponent<Animator>();

        //위치 이동
        SpriteRenderer sprite_renderer = GetComponent<SpriteRenderer>();
        Vector3 position = transform.localPosition;
        position.y = Define.ground_position + ((sprite_renderer.sprite.bounds.size.y / 2) * transform.localScale.y);
        transform.localPosition = position;
    }

    void Update()
    {
        if (animator.GetBool("Destroy"))
        {
            Destroy(gameObject);
        }
    }
}
