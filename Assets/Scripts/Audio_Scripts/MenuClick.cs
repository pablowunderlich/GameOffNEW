using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuClick : MonoBehaviour
{
    // Start is called before the first frame update


    public void MenuClickThis()
    {
        AkSoundEngine.PostEvent("Play_UI_Click", this.gameObject); 
    }
}
