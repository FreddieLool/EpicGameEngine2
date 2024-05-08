using EpicGameEngine;
using EpicTileEngine;
using System.ComponentModel;
using System.Diagnostics;

internal class ChessCommandHandler : CommandHandler
{
    private Tilemap chessBoard;
    private MovementManager movementManager;
    private ChessTurnManager chessTurnManager;
    private TileObject selectedPiece;
    private List<Position> highlightedPositions = new List<Position>();
    public List<Position> HighlightedPositions => highlightedPositions;


    public ChessCommandHandler(Tilemap chessBoard, MovementManager movementManager, ChessTurnManager turnManager)
    {
        this.chessBoard = chessBoard;
        this.movementManager = movementManager;
        this.chessTurnManager = turnManager;

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
                Actor currentlyPlaying = chessTurnManager.GetPlayingActor();
                if (!chessTurnManager.IsPieceBelongsToPlayer(currentlyPlaying, tile.Occupant))
                {
                    DisplayNotificationMessage($"You can't select this piece");
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
            Actor currentlyPlaying = chessTurnManager.GetPlayingActor();
            if (!chessTurnManager.IsPieceBelongsToPlayer(currentlyPlaying,selectedPiece))
            {
                DisplayNotificationMessage($"You can't move this piece", ConsoleColor.Yellow);
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
                        highlightedPositions.Clear();
                        selectedPiece = null;
                        var currentPlayer = chessTurnManager.GetPlayingActor();

                        // Check for check after move
                        if (movementManager.CheckForThreats(movementManager.FindKing(chessBoard, currentPlayer.Id), chessBoard))
                        {
                            movementManager.LastMoveWasCheckmate = true;

                            // Check for checkmate
                            if (movementManager.CheckForCheckmate(currentPlayer.Id, chessBoard))
                            {
                                movementManager.LastMoveWasCheckmate = true;
                                DisplayGameState();
                                return true;
                            }
                        }
                        chessTurnManager.ChangeTurns();

                        ClearErrorMessage();
                        DisplayGameState();
                    }
                    else
                    {
                        DisplayNotificationMessage("Move failed.", ConsoleColor.DarkYellow);
                    }
                    
                    return result;
                }
                DisplayNotificationMessage("Selected piece cannot move to the specified position.", ConsoleColor.DarkYellow);
                return false;
            }

            DisplayNotificationMessage("Invalid move syntax. Usage: move [to]");
            return false;
        }, "move [position] - Move the currently selected piece to the specified position.");
    }

    public void DisplayGameState()
    {
        int stateLine = 2; // number for the game state
        Console.SetCursorPosition(0, stateLine);
        Console.Write(new string(' ', Console.WindowWidth));

        Actor currentPlayer = chessTurnManager.GetPlayingActor();
        string stateMessage = $"State: {currentPlayer.Name}'s turn";

        Console.SetCursorPosition(0, stateLine);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("State: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{currentPlayer.Name}'s turn");

        if (movementManager.LastMoveWasCapture)
        {
            string pieceName = StripPrefixFromName(movementManager.LastCapturedPiece.Name);
            string positionNotation = ConvertPositionToNotation(movementManager.LastCapturedPiecePosition);

            // "captures" message parts
            Console.SetCursorPosition(0, stateLine + 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{currentPlayer.Name} captures ");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(pieceName);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" at ");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(positionNotation);
        }

        if (movementManager.LastMoveWasCheckmate)
        {
            Console.SetCursorPosition(0, stateLine + 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{currentPlayer.Name} checkmates opponent!");
        }

        Console.ResetColor();
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

    private string ConvertPositionToNotation(Position position)
    {
        char file = (char)('a' + position.X);
        int rank = 8 - position.Y;
        return $"{file}{rank}";
    }

    private string StripPrefixFromName(string fullName)
    {
        // prefix is separated by a space
        int lastSpaceIndex = fullName.LastIndexOf(' ');
        return lastSpaceIndex == -1 ? fullName : fullName.Substring(lastSpaceIndex + 1);
    }

    public void ResetSelectionAndState()
    {
        selectedPiece = null; 
        highlightedPositions.Clear();
    }
}