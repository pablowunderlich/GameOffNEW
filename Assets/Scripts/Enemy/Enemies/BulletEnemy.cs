using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    public Transform groundCheck;
    public Vector2 groundCheckSize;
    public LayerMask groundLayerMask;

    private void Update()
    {
        HandleCollision();
    }

    public void HandleCollision()
    {
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayerMask)) //if the object is no longer colliding with the ground
        {
            //destroy object

            
            Destroy(gameObject);
            AkSoundEngine.PostEvent("Play_NPC_BullerHit", gameObject);

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
