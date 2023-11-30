using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ScriptableObjectArchitecture;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager Instance { get; private set; } // singleton, there is only one game manager in the scene

    public FloatReference _scoreLvl1;
    public FloatReference _scoreLvl2;
    public FloatReference _scoreLvl3;    
    public TMP_Text counter;
    [SerializeField] [Range(1, 3)] int currentLevel;

     private void Awake()
     {
         #region simpleton stuff
         if (Instance == null)
         {
             Instance = this;
         }
         else if (Instance != this)
         {
             Destroy(gameObject);
         }

         //DontDestroyOnLoad(gameObject);
         #endregion
     }
    private void Start()
    {
        switch (currentLevel)
        {
        case 3:
            counter.text = _scoreLvl3.Value.ToString();
            break;
        case 2:
            counter.text = _scoreLvl2.Value.ToString();
            break;
        case 1:
            counter.text = _scoreLvl1.Value.ToString();
            break;
        default:
            print ("Incorrect level selected on GameManager");
            break;
        }
    }

    #region getters and setters
    public float GetScore1() { return _scoreLvl1.Value; }
    public float GetScore2() { return _scoreLvl2.Value; }
    public float GetScore3() { return _scoreLvl3.Value; }
    public void SetScore1(float newScore) { _scoreLvl1.Value = newScore; counter.text = newScore.ToString();}
    public void SetScore2(float newScore) { _scoreLvl2.Value = newScore; counter.text = newScore.ToString();}
    public void SetScore3(float newScore) { _scoreLvl3.Value = newScore; counter.text = newScore.ToString();}
    #endregion
}
