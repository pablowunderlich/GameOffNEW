using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UIElements;

public class GearyEnemy : MonoBehaviour
{
    [SerializeField] private float _gravityScale;

    public Transform groundCheck;
    public Vector2 groundCheckSize;
    public LayerMask groundLayerMask;

    public Rigidbody2D RB;
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        RB.gravityScale = _gravityScale;
        #region Pablo
        AkSoundEngine.PostEvent("Play_Gearrs", this.gameObject);
        #endregion

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
          
           
            Destroy(gameObject, 0.5f);
            AkSoundEngine.PostEvent("Stop_Gearrs", this.gameObject);
            

        }

       
 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }

   
}
