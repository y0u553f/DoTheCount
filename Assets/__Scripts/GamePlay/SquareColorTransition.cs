using UnityEngine.UI;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class SquareColorTransition : MonoBehaviour
{
    private Material _material;
    private Color _originalBaseColor;

    private void Awake()
    {
        // Initialize the material and store the original color
        Image image = GetComponent<Image>();
        _material = new Material(Shader.Find("UI/SquareColorTransition"));
        image.material = _material;
        _originalBaseColor = image.color;
        _material.SetColor("_BaseColor", _originalBaseColor);
    }

    // Call this function to transition to a new color
    public void TransitionToColor(Color targetColor, float animationDuration)
    {
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        _animationCoroutine = StartCoroutine(AnimateTransition(targetColor, animationDuration));
    }

    private Coroutine _animationCoroutine;

    private IEnumerator AnimateTransition(Color targetColor, float animationDuration)
    {
        print("material" + _material + "targetColor:" + targetColor);
        // Set the target color in the shader
        _material.SetColor("_NewColor", targetColor);

        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime / animationDuration;
            timer = Mathf.Clamp01(timer);

            // Update the progress in the shader
            _material.SetFloat("_Progress", timer);

            // Animate softness from 0.5 to 0
            float softness = Mathf.Lerp(0.5f, 0f, timer);
            _material.SetFloat("_Softness", softness);

            yield return null;
        }

        // After the transition, update the base color to the target color
        UpdateBaseColor(targetColor);
    }

    // Call this to instantly update the base color without animation
    public void UpdateBaseColor(Color newBaseColor)
    {
        _material.SetColor("_BaseColor", newBaseColor);
        GetComponent<Image>().color = newBaseColor;
    }

    // Call this to reset to the original color
    public void ResetToOriginalColor()
    {
        UpdateBaseColor(_originalBaseColor);
    }

    private void OnDisable()
    {
        // Clean up the coroutine if the object is disabled
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);
    }
}