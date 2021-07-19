using UnityEngine;
using System.Collections;

public class ParticleAutoOff : MonoBehaviour {

    //컴포넌트
    private ParticleSystem particle_system;

    //초기화
    void Start()
    {
        particle_system = GetComponent<ParticleSystem>();
    }

    //이펙트 끝나면 꺼지게
    void Update()
    {
        if (particle_system.isStopped)
            gameObject.SetActive(false);
    }
}
