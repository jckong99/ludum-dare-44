using UnityEngine;

public class Tile : MonoBehaviour
{
    // SpriteRenderer components should be disabled by default
    [SerializeField] private GameObject playerHighlightPrefab = null;

    private SpriteRenderer playerHighlight;

    private void Awake()
    {
        playerHighlight = Instantiate(playerHighlightPrefab, transform).GetComponent<SpriteRenderer>();
    }

    private void OnMouseEnter()
    {
        playerHighlight.enabled = true;
    }

    private void OnMouseExit()
    {
        playerHighlight.enabled = false;
    }
}
