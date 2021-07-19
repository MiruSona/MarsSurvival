using UnityEngine;
using System.Collections;

public class BuildEffet : MonoBehaviour {

    //참조
    private Animator animator;

	//초기화
	void Start () {
        animator = GetComponent<Animator>();
	}

    void Update()
    {
        if (animator.GetBool("Destroy"))
        {
            Destroy(gameObject);
        }
    }
}
