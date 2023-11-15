using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) // check if player collides, if so respawn the player and set it's health value to max
    {
        if(collision.GetComponent<Player>())
        {
            Player.Instance.currentHealth.Value = 2;
            Player.Instance.Respawn();
        }
    }
}
