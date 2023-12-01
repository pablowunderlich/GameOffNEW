using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BubbleEnable : MonoBehaviour
{
    [SerializeField] Image[] bubbles;
    private float desiredAlpha = 0.75f;
    private float currentAlpha;
    [SerializeField] bool fadeUp;
    [SerializeField] TMP_Text barkText;
    void OnEnable()
    {
        currentAlpha = 0;
        fadeUp = true;
        Invoke("DisableSelf",4f);
    }
    public void BarkTextChange(string textToSay)
    {
        barkText.text = textToSay;
    }
    void Update()
    {
            currentAlpha = Mathf.MoveTowards( currentAlpha, desiredAlpha, 2.0f * Time.deltaTime);
            Color newColor = bubbles[0].color;
            Color newColor2 = bubbles[1].color;
            Color newColor3 = bubbles[2].color;
            Color newColor4 = bubbles[3].color;
            newColor.a = currentAlpha;
            newColor2.a = currentAlpha;
            newColor3.a = currentAlpha;
            newColor4.a = currentAlpha;
            bubbles[0].color = newColor;
            bubbles[1].color = newColor;
            bubbles[2].color = newColor;
            bubbles[3].color = newColor;
    }
    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector2.zero,Camera.main.transform.rotation * Vector2.up);
    }
    void DisableSelf()
    {
        fadeUp = false;
        gameObject.SetActive(false);
    }
}
