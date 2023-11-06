using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearySpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public float velocity;
    public bool isSpawningToLeft;
    public float interval;

    public GameObject Geary;

    void Start()
    {
       // Geary = GetComponent<GameObject>();
        InvokeRepeating("Spawn", 0, interval);
    }

    private void Spawn()
    {
        GameObject GearyToSpawn = Instantiate(Geary, transform.position, Quaternion.identity);
        Rigidbody2D RB = GearyToSpawn.GetComponent<Rigidbody2D>();

        if(RB != null)
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
