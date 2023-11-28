using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager Instance { get; private set; } // singleton, there is only one game manager in the scene
    private float _scoreLvl1;
    private float _scoreLvl2;
    private float _scoreLvl3;    
    public TMP_Text counter;

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

        DontDestroyOnLoad(gameObject);
        #endregion
    }
    void Start()
    {
        _scoreLvl1 = 0;
        _scoreLvl2 = 0;
        _scoreLvl3 = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region getters and setters
    public float GetScore1() { return _scoreLvl1; }
    public float GetScore2() { return _scoreLvl2; }
    public float GetScore3() { return _scoreLvl3; }
    public void SetScore1(float newScore) { _scoreLvl1 = newScore; counter.text = newScore.ToString();}
    public void SetScore2(float newScore) { _scoreLvl2 = newScore; }
    public void SetScore3(float newScore) { _scoreLvl3 = newScore; }
    #endregion
}
