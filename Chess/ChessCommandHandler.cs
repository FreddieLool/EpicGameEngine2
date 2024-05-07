using EpicGameEngine;
using EpicTileEngine;
using System.Diagnostics;

internal class ChessCommandHandler : CommandHandler
{
    private Tilemap chessBoard;
    private MovementManager movementManager;
    private ChessTurnManager chessTurnManager;
    private TileObject selectedPiece;
    private List<Position> highlightedPositions = new List<Position>();
    public List<Position> HighlightedPositions => highlightedPositions;

    public ChessCommandHandler(Tilemap chessBoard, MovementManager movementManager,ChessTurnManager chessTurn)
    {
        this.chessBoard = chessBoard;
        this.movementManager = movementManager;
        this.chessTurnManager = chessTurn;

        RegisterChessCommands();
    }

    private void RegisterChessCommands()
    {
        
        // ╔═════════════════════════════════════╗
        // ║          ~ Select Cmd ~             ║
        // ╚═════════════════════════════════════╝

        RegisterCommand("select", parts =>
        {
            if (parts.Length != 2)
            {
                Trace.WriteLine("Invalid select syntax. Usage: select [position]");
                DisplayNotificationMessage("Invalid select syntax. Usage: select [position]");
                return false;
            }

            Position pos = ConvertNotationToPosition(parts[1]);
            Tile tile = chessBoard.GetTile(pos);
           

            if (tile != null && tile.Occupant != null)
            {
                Actor currentlyPlaying = chessTurnManager.GetWhoPlaying();
                if (!chessTurnManager.IsPieceBelongsToPlayer(currentlyPlaying, tile.Occupant))
                {
                    DisplayNotificationMessage($"You can't move This Piece");
                    return false;
                }
                selectedPiece = tile.Occupant;
                selectedPiece.CurrentTile = tile;
                highlightedPositions = movementManager.GetValidMoves((ChessPiece)selectedPiece, chessBoard).ToList();
                Trace.WriteLine($"Selected {selectedPiece.Name} at {parts[1]}, current tile: {selectedPiece.CurrentTile.Position}");
               

                // Clear the invalid message since a valid selection occurred
                ClearErrorMessage();
                return true;
            }

            else
            {
                DisplayNotificationMessage("No piece at the specified position.");
                Trace.WriteLine("No piece at the specified position.");
                return false;
            }
        }, "select [position] - Select a piece at the specified position.");

        // ╔═════════════════════════════════════╗
        // ║          ~ Deselect Cmd ~         
        // ╚═════════════════════════════════════╝

        RegisterCommand("deselect", parts =>
        {
            selectedPiece = null;
            highlightedPositions.Clear();
            string positionInfo = selectedPiece != null ? $"from {selectedPiece.CurrentTile.Position}" : "";
            Trace.WriteLine($"Deselection {positionInfo}.");
            DisplayNotificationMessage("");
            return true;
        }, "deselect - Deselect the currently selected piece, if any.");


        // ╔═════════════════════════════════════╗
        // ║          ~ Move Cmd ~         
        // ╚═════════════════════════════════════╝

        RegisterCommand("move", parts =>
        {

            if (selectedPiece == null)
            {
                DisplayNotificationMessage("Usage: select [position], then move [to]", ConsoleColor.DarkYellow);
                Trace.WriteLine("No piece selected. Use 'select' command first.");
                return false;
            }
            Actor currentlyPlaying = chessTurnManager.GetWhoPlaying();
            if (!chessTurnManager.IsPieceBelongsToPlayer(currentlyPlaying,selectedPiece))
            {
                DisplayNotificationMessage($"You can't move This Piece");
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
                    if (result)
                    {
                        // Update the highlighted positions for the new location of the piece
                        selectedPiece = chessBoard.GetTile(to).Occupant; // Update the selected piece reference
                        highlightedPositions = movementManager.GetValidMoves((ChessPiece)selectedPiece, chessBoard).ToList();
                        Trace.WriteLine($"Move successful to {to}. Valid moves updated.");
                        chessTurnManager.ChangeTurns();
                        ClearErrorMessage();
                    }
                    else
                    {
                        Trace.WriteLine("Move failed, keeping current selection and valid moves.");
                        DisplayNotificationMessage("Move failed.", ConsoleColor.DarkYellow);
                    }
                    
                    return result;
                }

                Trace.WriteLine("Selected piece cannot move to the specified position.");
                DisplayNotificationMessage("Selected piece cannot move to the specified position.", ConsoleColor.DarkYellow);
                return false;
            }

            DisplayNotificationMessage("Invalid move syntax. Usage: move [to]");
            Trace.WriteLine("Invalid move syntax. Usage: move [to]");
            return false;
        }, "move [position] - Move the currently selected piece to the specified position.");
    }

    private Position ConvertNotationToPosition(string notation)
    {
        try
        {
            int x = notation[0] - 'a';
            int y = 8 - (notation[1] - '0');
            return new Position(x, y);
        }
        catch (IndexOutOfRangeException)
        {
            // invalid notation
            DisplayNotificationMessage("Invalid position notation. Please enter a valid position.", ConsoleColor.Red);
            return new Position(0, 0); // Default position?
        }
    }

    public void ResetSelectionAndState()
    {
        selectedPiece = null; // Clear the selected piece
        highlightedPositions.Clear(); // Clear any highlighted positions
    }
}