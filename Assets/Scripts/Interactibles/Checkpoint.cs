using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private bool _showSprite; // bool to check if they should see sprite 
    SpriteRenderer spriteRenderer = null; // 
  
    void Start()
    {
        spriteRenderer = transform.Find("SpawnerSprite").GetComponent<SpriteRenderer>(); // locate the sprite
    }

    // Update is called once per frame
    void Update()
    {
        ShowSprite();
    }

    void ShowSprite() // enable or disable the sprite
    {
        if (_showSprite)
        {
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col) // check what version of this object was hit by player
    {
        if (col.GetComponent<Player>())
        {
            Debug.Log("something is happning");
            if (gameObject.CompareTag("StartPoint"))
            {
                // do nothing i guess? Might be used to initialize things
            }
            if (gameObject.CompareTag("CheckPoint"))
            {
                // refresh lives?
                Player.Instance.currentHealth.Value = 2;
                //play sound
                // set new respawnn location

                Player.Instance.SetSpawnLocation(gameObject.transform.position);
                Debug.Log("Reached checkpoint");
            }
            if (gameObject.CompareTag("EndPoint"))
            {
                //congrats the  level has passed, move to the next one
            }
        }
    }
}
