using EpicTileEngine;

public interface IRenderer
{
    void Render(Tilemap tilemap, TileObject selectedObject, bool showValidMovesHighlighted = true);
}