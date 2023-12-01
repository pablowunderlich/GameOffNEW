using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ScriptableObjectArchitecture;

public class LevelManager : MonoBehaviour
{
    [SerializeField] FloatReference _scoreLvl1;
    [SerializeField] FloatReference _scoreLvl2;
    [SerializeField] FloatReference _scoreLvl3; 
    public void LoadLevel1()
    {
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }
    public void LoadLevel2()
    {
        SceneManager.LoadScene(3, LoadSceneMode.Single);
    }
    public void LoadLevel3()
    {
        SceneManager.LoadScene(4, LoadSceneMode.Single);
    }
    public void LoadIntroMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
