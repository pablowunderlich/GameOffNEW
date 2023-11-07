using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerVoice : MonoBehaviour
{
   private AudioSource _voice;
    
    [SerializeField] GameObject _voicePrefab;
    [SerializeField] private int xTime = 5;
    [SerializeField] private int yTime = 10;
    bool hasLanded;
   


    // Start is called before the first frame update
    void Start()
    {
       // _voice = GetComponent<AudioSource>();
        
       // StartCoroutine(VoicePlay());
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    private void PlayVoice()
    {
        var voice = Instantiate(_voicePrefab, new Vector2(0,0), Quaternion.identity);
        
        new WaitForSeconds(xTime);
        Destroy(voice);
        
        
    }


    IEnumerator VoicePlay()
    {
        yield return new WaitForSeconds(Random.Range(xTime,yTime));
        PlayVoice();
        StartCoroutine(VoicePlay());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.CompareTag("Floor") && hasLanded == false)
        {
            
            hasLanded = true;
        }
    }
}
