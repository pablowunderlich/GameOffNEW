using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingot : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D _rb;
    [Range(1, 3)][SerializeField] int currentLevel;
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
                switch (currentLevel)
            {
            case 3:
                GameManager.Instance.SetScore3(GameManager.Instance.GetScore3() + 1);
                Debug.Log(GameManager.Instance.GetScore3());
                break;
            case 2:
                GameManager.Instance.SetScore2(GameManager.Instance.GetScore2() + 1);
                Debug.Log(GameManager.Instance.GetScore2());
                break;
            case 1:
                GameManager.Instance.SetScore1(GameManager.Instance.GetScore1() + 1);
                Debug.Log(GameManager.Instance.GetScore1());
                break;
            default:
                print ("Error in the ingot lvl switch");
                break;
            }
            Destroy(gameObject);
            #region Pablo
            AkSoundEngine.PostEvent("Play_Ingots", this.gameObject);
            #endregion
        }
    }
}
