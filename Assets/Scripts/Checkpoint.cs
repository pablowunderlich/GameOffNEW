using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private bool _showSprite;
    GameObject spriteChild = GameObject.Find("SpawnerSprite");
    SpriteRenderer spriteRenderer = null;
    void Start()
    {
        spriteRenderer = spriteChild.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ShowSprite();
    }

    void ShowSprite()
    {
        if(_showSprite)
        {
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.enabled=false;
        }
    }
}
