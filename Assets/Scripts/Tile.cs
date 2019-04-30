using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // SpriteRenderer components should be disabled by default
    [SerializeField] private GameObject playerHighlightPrefab = null;

    public uint X { get; private set; }
    public uint Y { get; private set; }

    private GameManager gameManager;
    private List<Tile> adjacents;
    private SpriteRenderer playerHighlight;

    private void Awake()
    {
        gameManager = GameManager.GetInstance();
        adjacents = new List<Tile>();
        playerHighlight = Instantiate(playerHighlightPrefab, transform).GetComponent<SpriteRenderer>();
    }

    /*private void OnMouseEnter()
    {
        if (gameManager.GetPhase() == Phase.Night)
        {
            foreach (Tile t in adjacents)
            {
                t.Highlight(true);
            }
        }
        playerHighlight.enabled = true;
    }*/

    private void OnMouseExit()
    {
        if (gameManager.GetPhase() == Phase.Night)
        {
            foreach (Tile t in adjacents)
            {
                t.Highlight(false);
            }
        }
        playerHighlight.enabled = false;
    }

    private void OnMouseOver()
    {
        if (!playerHighlight.enabled)
        {
            if (gameManager.GetPhase() == Phase.Night)
            {
                foreach (Tile t in adjacents)
                {
                    t.Highlight(true);
                }
            }
            playerHighlight.enabled = true;
        }
    }

    private void OnMouseUp()
    {
        gameManager.OnTileClick(X, Y);
    }

    public void SetPosition(uint x, uint y)
    {
        X = x;
        Y = y;
    }

    public void AddAdjacentTile(Tile tile)
    {
        adjacents.Add(tile);
    }

    public void Highlight(bool enabled)
    {
        playerHighlight.enabled = enabled;
    }
}
