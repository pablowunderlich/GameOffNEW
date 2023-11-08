using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
        #region Pablo
        AkSoundEngine.PostEvent("Play_Acid", this.gameObject);
        #endregion
    }
}
