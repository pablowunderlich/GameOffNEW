using UnityEngine;
using UnityEditor;
using ScriptableObjectArchitecture;
using UnityEngine.SceneManagement;

class Example : MonoBehaviour
{
    [SerializeField] FloatReference _scoreLvl1;
    [SerializeField] FloatReference _scoreLvl2;
    [SerializeField] FloatReference _scoreLvl3; 
    void Start()
    {
        if (Application.isPlaying)
        {
            _scoreLvl1.Value = 0;
            _scoreLvl2.Value = 0;
            _scoreLvl3.Value = 0;
        }
            SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}