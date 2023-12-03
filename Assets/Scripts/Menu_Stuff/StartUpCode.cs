using UnityEngine;
using UnityEditor;
using ScriptableObjectArchitecture;
using UnityEngine.SceneManagement;

class Example : MonoBehaviour
{
    [SerializeField] FloatReference _scoreLvl1;
    [SerializeField] FloatReference _scoreLvl2;
    [SerializeField] FloatReference _scoreLvl3; 
    [SerializeField] IntReference currentLives; 
    void Start()
    {
        _scoreLvl1.Value = 0;
        _scoreLvl2.Value = 0;
        _scoreLvl3.Value = 0;
        currentLives.Value = 5;
        Invoke("SetupAll",1f);
    }
    void SetupAll()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}