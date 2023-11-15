using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingot : MonoBehaviour
{
    // Start is called before the first frame update

    private Rigidbody2D _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            GameManager.Instance.SetScore(GameManager.Instance.GetScore() + 1);
            Debug.Log(GameManager.Instance.GetScore());
            Destroy(gameObject);
            #region Pablo
            AkSoundEngine.PostEvent("Play_Ingots", this.gameObject);
            #endregion
        }
    }
}
