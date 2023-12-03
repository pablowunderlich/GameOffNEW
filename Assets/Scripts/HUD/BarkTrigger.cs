using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarkTrigger : MonoBehaviour
{
    [SerializeField] string textToSay;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            Player.Instance.Bark(textToSay);
            #region Pablo
            AkSoundEngine.PostEvent("Play_Ingots", this.gameObject);
            #endregion
            Destroy(gameObject);
        }
    }
}
