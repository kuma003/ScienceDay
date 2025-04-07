using UnityEngine;

public class Nozzle : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    [SerializeField] private float _thrust = 1f; // 推力 (0.0 - 1.0)

    [SerializeField] private float particleMax = 15; // 単位時間当たり粒子数


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _particleSystem = gameObject.GetComponent<ParticleSystem>();
        thrust = _thrust;
    }
    
    public float thrust {
        set
        {
            if (value is float.NaN || _particleSystem is null) return;
            _thrust = Mathf.Clamp(value, 0, 1);

        }
        get { return _thrust; }
    }

    public void Update()
    {    
        var em = _particleSystem.emission;
        em.rateOverTime = Mathf.Lerp(0, particleMax, _thrust);
    }
}
