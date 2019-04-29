using UnityEngine;

public class Tile : MonoBehaviour
{
    // SpriteRenderer components should be disabled by default
    [SerializeField] private GameObject playerHighlightPrefab = null;

    public uint X { get; private set; }
    public uint Y { get; private set; }

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

    private void OnMouseUp()
    {
        GameManager.GetInstance().OnTileClick(X, Y);
    }

    public void SetPosition(uint x, uint y)
    {
        X = x;
        Y = y;
    }
}
