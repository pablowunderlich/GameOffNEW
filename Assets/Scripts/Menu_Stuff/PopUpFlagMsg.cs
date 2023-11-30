using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpFlagMsg : MonoBehaviour
{
    public void Start()
    {   
        Destroy(gameObject,2f);
    } 
    public void Update()
    {   
        transform.Translate(Vector3.up * Time.deltaTime);
    } 
}
