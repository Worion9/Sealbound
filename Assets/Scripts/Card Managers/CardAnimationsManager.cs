using UnityEngine;

public class CardAnimationsManager : MonoBehaviour
{
    public void AnimateCard(string animationName, GameObject objectToAnimate)
    {
        if (objectToAnimate != null)
        {
            // Sprawdü, czy obiekt ma Animator
            if (objectToAnimate.TryGetComponent<Animator>(out var objectAnimator))
            {
                // Ustawienie bool play na true
                objectAnimator.SetBool(animationName, true);
                Debug.Log($"Animacja {animationName} zosta≥a uruchomiona.");
            }
            else
            {
                Debug.LogError("Animator not found on the provided GameObject!");
            }
        }
    }
}