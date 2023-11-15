using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager Instance { get; private set; } // singleton, there is only one game manager in the scene

        

    private float _score; // here for now, might move to a different location if score is levelBound

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
        _score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region getters and setters
    public float GetScore() { return _score; }
    public void SetScore(float newScore) { _score = newScore; }
    #endregion
}
