using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UIElements;

public class GearyEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float _velocity;
    [SerializeField] private Vector2 _knockBack;
    [SerializeField] private float _gravityScale;
    public int damage;

    public Transform groundCheck;
    public Vector2 groundCheckSize;
    public LayerMask groundLayerMask;

    public Rigidbody2D RB;
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        RB.gravityScale = _gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        HandleCollision();
    }

    public void HandleCollision()
    {
        if (!Physics2D.OverlapBox(groundCheck.position, groundCheckSize,0, groundLayerMask)) //if the object is no longer colliding with the ground
        {
            //destroy object
            Debug.Log("i am death");
            Destroy(gameObject, 0.5f);
            
        }
 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
