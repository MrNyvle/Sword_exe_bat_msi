using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CircleCollider2D _circleCollider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Explosion()
    {
        _circleCollider.enabled = true;
    }

    public void ExplosionFinished()
    {
        Destroy(this.gameObject);
    }
}
