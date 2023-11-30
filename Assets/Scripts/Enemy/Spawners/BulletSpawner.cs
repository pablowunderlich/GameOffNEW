using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public float velocity;
    public Transform spawnPoint;
    public bool isSpawningToLeft;
    public float interval;

    public GameObject Bullet;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Spawn", 0, interval);
    }

    private void Spawn()
    {
        GameObject BulletToSpawn = Instantiate(Bullet, spawnPoint.position, Quaternion.identity);
        Rigidbody2D RB = BulletToSpawn.GetComponent<Rigidbody2D>();
        AkSoundEngine.PostEvent("Play_NPC_TurretFire", this.gameObject);

        if (RB != null)
        {
            if (isSpawningToLeft)
            {
                RB.velocity = new Vector2(-velocity, 0);
            }
            else
            {
                RB.velocity = new Vector2(velocity, 0);
            }
        }
    }
}
