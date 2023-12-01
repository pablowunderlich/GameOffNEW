using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;
using UnityEngine.SceneManagement;

public class exitFlag : MonoBehaviour
{
    [SerializeField] GameObject insuficientesIngots;
    [SerializeField] FloatReference ingotCounter;
    [SerializeField] bool endgame;
    private void OnTriggerEnter2D(Collider2D col) // check what version of this object was hit by player
    {
        if (col.GetComponent<Player>())
        {
            if (ingotCounter.Value >= 25)
            {
                if(endgame)
                SceneManager.LoadScene(5, LoadSceneMode.Single);
                
                SceneManager.LoadScene(1, LoadSceneMode.Single);
            }
            else
            {
                Instantiate(insuficientesIngots, transform.position, Quaternion.identity);
                Debug.Log("NO INGOTS, NO SERVICE");
            }
        }
    }
}
