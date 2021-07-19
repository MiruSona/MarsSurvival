using UnityEngine;
using System.Collections;

public abstract class ParticleCommon : MonoBehaviour {

    //참조
    protected ParticleSystem particle_system;

    //초기화
    void Start () {
        particle_system = GetComponent<ParticleSystem>();

        ChildInit();
    }
	
	//파티클 멈추면 파괴되게
	void Update () {
        
        if (particle_system.isStopped)
            Destroy(gameObject);

        ChildUpdate();
    }

    //상속
    protected abstract void ChildInit();
    protected virtual void ChildUpdate() { }
}
