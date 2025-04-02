using UnityEngine;

public class Tile : MonoBehaviour
{
    public int currentX;
    public int currentY;
    public ChessPiece chessPiece { get; set; }

    private Renderer objectRenderer;
    [SerializeField] private TileColorConfig tileColorConfig;

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    public void SetHoverMaterial()
    {
        objectRenderer.material = tileColorConfig.hoverMaterial;
    }

    public void SetSelectMaterial()
    {
        objectRenderer.material = tileColorConfig.selectMaterial;
    }

    public void SetExitMaterial()
    {
        objectRenderer.material = tileColorConfig.exitMaterial;
    }

    public void SetValidMaterial()
    {
        objectRenderer.material = tileColorConfig.validMoveMaterial;
    }
}