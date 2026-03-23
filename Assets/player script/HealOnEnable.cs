using UnityEngine;

public class HealOnEnable : MonoBehaviour
{
    public int healAmount = 1;

    private void OnEnable()
    {
        PlayerHealthSystem player = FindObjectOfType<PlayerHealthSystem>();
        if (player != null)
        {
            player.HealOnObjectActivation(healAmount);
        }
    }
}
