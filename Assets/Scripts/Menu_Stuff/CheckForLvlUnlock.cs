using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ScriptableObjectArchitecture;

public class CheckForLvlUnlock : MonoBehaviour
{
    [SerializeField] FloatReference _scoreLvl1;
    [SerializeField] FloatReference _scoreLvl2;
    [SerializeField] FloatReference _scoreLvl3; 
    //[SerializeField] GameObject lock2;
    [SerializeField] Button lvl2;
    //[SerializeField] GameObject lock3;
    [SerializeField] Button lvl3;
    // Start is called before the first frame update
    void OnEnable()
    {      
        if(_scoreLvl1.Value >= 25)
        {
            //lock2.SetActive(false);
            lvl2.interactable=true;
        }
        if(_scoreLvl2.Value >= 25)
        {
            //lock3.SetActive(false);
            lvl2.interactable=true;
        }
    }
}
