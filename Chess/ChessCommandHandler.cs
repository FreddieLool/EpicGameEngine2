using EpicTileEngine;
using System.Diagnostics;

internal class ChessCommandHandler : CommandHandler
{
    private Tilemap chessBoard;
    private MovementManager movementManager;
    private TileObject selectedPiece;
    private List<Position> highlightedPositions = [];

    public ChessCommandHandler(Tilemap chessBoard, MovementManager movementManager)
    {
        this.chessBoard = chessBoard;
        this.movementManager = movementManager;

        RegisterChessCommands();
    }

    private void RegisterChessCommands()
    {
        RegisterCommand("select", parts =>
        {
            if (parts.Length != 2)
            {
                Trace.WriteLine("Invalid select syntax. Usage: select [position]");
                return false;
            }

            Position pos = ConvertNotationToPosition(parts[1]);
            Tile tile = chessBoard.GetTile(pos);
            if (tile != null && tile.Occupant != null)
            {
                selectedPiece = tile.Occupant;
                selectedPiece.CurrentTile = tile;  // Ensure this line exists or is correctly handled elsewhere
                highlightedPositions = movementManager.GetValidMoves((ChessPiece)selectedPiece, chessBoard).ToList();
                Trace.WriteLine($"Selected {selectedPiece.Name} at {parts[1]}, current tile: {selectedPiece.CurrentTile.Position}");
                return true;
            }

            else
            {
                Trace.WriteLine("No piece at the specified position.");
                return false;
            }
        });

        RegisterCommand("deselect", parts =>
        {
            selectedPiece = null;
            highlightedPositions.Clear();
            Trace.WriteLine("Selection cleared.");
            return true;
        });

        RegisterCommand("move", parts =>
        {
            if (selectedPiece == null)
            {
                Trace.WriteLine("No piece selected. Use 'select' command first.");
                return false;
            }

            if (parts.Length == 2)
            {
                Position to = ConvertNotationToPosition(parts[1]);

                Trace.WriteLine($"Attempting to move {selectedPiece.Name} from {selectedPiece.CurrentTile?.Position}.");
                Tile fromTile = selectedPiece.CurrentTile;

                if (fromTile != null)
                {
                    bool result = movementManager.TryMove(selectedPiece, to, chessBoard);
                    Trace.WriteLine($"Move result: {result}");
                    return result;
                }

                Trace.WriteLine("Selected piece cannot move to the specified position.");
                return false;
            }

            Trace.WriteLine("Invalid move syntax. Usage: move [to]");
            return false;
        });

        // Continue with other command registrations
    }

    private Position ConvertNotationToPosition(string notation)
    {
        int x = notation[0] - 'a';
        int y = 8 - (notation[1] - '0');
        return new Position(x, y);
    }
}