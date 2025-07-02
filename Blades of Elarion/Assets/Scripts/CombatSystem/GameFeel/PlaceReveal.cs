using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlaceReveal : MonoBehaviour
{
    public TMP_Text placeNameText;
    public string placeName;

    public void RevealPlace()
    {
        placeNameText.text = placeName;
        StartCoroutine(PlaceNameAnimation());
    }

    IEnumerator PlaceNameAnimation()
    {
        float duration = 0.5f;
        float elapsedTime = 0f;
        Color startColor = new Color(placeNameText.color.r, placeNameText.color.g, placeNameText.color.b, 0f);
        Color endColor = new Color(placeNameText.color.r, placeNameText.color.g, placeNameText.color.b, 1f);

        while (elapsedTime < duration)
        {
            placeNameText.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        placeNameText.color = endColor;
        elapsedTime = 0f; 

        yield return new WaitForSeconds(2f);
        while (elapsedTime < duration)
        {
            placeNameText.color = Color.Lerp(endColor, startColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        placeNameText.color = startColor;

        yield return new WaitForSeconds(2f); // Wait for 2 seconds before hiding the text
        placeNameText.text = string.Empty;
    }
}
