using EpicGameEngine;
using EpicTileEngine;
using System.ComponentModel;
using System.Diagnostics;

internal class ChessCommandHandler : CommandHandler
{
    private ChessDemo _chessBoard;
    private MovementManager _chessMovementManager;
    private ChessTurnManager _chessTurnManager;


    public ChessCommandHandler(ChessDemo chessBoard, MovementManager movementManager, ChessTurnManager chessTurnManager)
    {
        this._chessBoard = chessBoard;
        this._chessMovementManager = movementManager;
        this._chessTurnManager = chessTurnManager;

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
                DisplayNotification("Invalid select syntax. Usage: select [position]");
                return false;
            }

            Position pos = _chessBoard.ConvertNotationToPosition(parts[1]);
            Tile tile = _chessBoard.GetTile(pos);
           

            if (tile != null && tile.Occupant != null)
            {
                Actor currentlyPlaying = _chessTurnManager.GetPlayingActor();

                if (!_chessTurnManager.IsActorChessPieceOwner(currentlyPlaying, tile.Occupant))
                {
                    DisplayNotification($"You can't select this piece");
                    return false;
                }

                _chessMovementManager.SelectedPiece = (ChessPiece)tile.Occupant;
                _chessMovementManager.SelectedPiece.CurrentTile = tile;
                _chessMovementManager.HighlightedPositions = _chessMovementManager.GetValidMoves(_chessMovementManager.SelectedPiece, _chessBoard).ToList();

                // Clear the invalid message since a valid selection occurred
                ClearErrorMessage();
                return true;
            }

            else
            {
                DisplayNotification("No piece at the specified position.");
                return false;
            }
        }, "select [position] - Select a piece at the specified position.");

        // ╔═════════════════════════════════════╗
        // ║          ~ Deselect Cmd ~         
        // ╚═════════════════════════════════════╝

        RegisterCommand("deselect", parts =>
        {
            _chessMovementManager.SelectedPiece = null;
            _chessMovementManager.HighlightedPositions.Clear();
            string positionInfo = _chessMovementManager.SelectedPiece != null ? $"from {_chessMovementManager.SelectedPiece.CurrentTile.Position}" : "";
            DisplayNotification("");
            return true;
        }, "deselect - Deselect the currently selected piece, if any.");


        // ╔═════════════════════════════════════╗
        // ║          ~ Move Cmd ~         
        // ╚═════════════════════════════════════╝

        RegisterCommand("move", parts =>
        {

            if (_chessMovementManager.SelectedPiece == null)
            {
                DisplayNotification("Usage: select [position], then move [to]", ConsoleColor.DarkYellow);
                return false;
            }
            Actor currentlyPlaying = _chessTurnManager.GetPlayingActor();
            if (!_chessTurnManager.IsActorChessPieceOwner(currentlyPlaying, _chessMovementManager.SelectedPiece))
            {
                DisplayNotification($"You can't move this piece.", ConsoleColor.Yellow);
                return false;
            }

            if (parts.Length == 2)
            {
                Position to = _chessBoard.ConvertNotationToPosition(parts[1]);

                Trace.WriteLine($"Attempting to move {_chessMovementManager.SelectedPiece.Name} from {_chessMovementManager.SelectedPiece.CurrentTile?.Position}.");
                
                Tile? fromTile = _chessMovementManager.SelectedPiece.CurrentTile;

                if (fromTile != null)
                {
                    bool result = _chessMovementManager.TryMove(_chessMovementManager.SelectedPiece, to, _chessBoard);
                    if (result)
                    {
                        _chessMovementManager.HighlightedPositions.Clear();
                        _chessMovementManager.SelectedPiece = null;
                        var currentPlayer = _chessTurnManager.GetPlayingActor();

                        // needs decoupling and cleaning...
                        // Check for check after move
                        if (_chessMovementManager.CheckForThreats(_chessMovementManager.FindKing(_chessBoard, currentPlayer.Id), _chessBoard))
                        {
                            _chessMovementManager.LastMoveWasCheckmate = true;

                            // Check for checkmate
                            if (_chessMovementManager.CheckForCheckmate(currentPlayer.Id, _chessBoard))
                            {
                                _chessMovementManager.LastMoveWasCheckmate = true;
                                Program.DisplayGameState();
                                return true;
                            }
                        }

                        _chessTurnManager.ChangeTurns();

                        ClearErrorMessage();
                        Program.DisplayGameState();
                    }
                    else
                    {
                        DisplayNotification("Move failed.", ConsoleColor.DarkYellow);
                    }
                    
                    return result;
                }
                DisplayNotification("Selected piece cannot move to the specified position.", ConsoleColor.DarkYellow);
                return false;
            }

            DisplayNotification("Invalid move syntax. Usage: move [to]");
            return false;
        }, "move [position] - Move the currently selected piece to the specified position.");

        // ╔═════════════════════════════════════╗
        // ║          ~ other cmds ~         
        // ╚═════════════════════════════════════╝

        RegisterCommand("restart", args =>
        {
            _chessBoard.ResetGame();
            return true;
        }, "restart - Restarts a new game of chess.");

        RegisterCommand("credits", args =>
        {
            Program.ShowCredits();
            return true;
        }, "credits - Show game credits and information.");

        RegisterCommand("spiral", args =>
        {
            Program.PerformSpiralDemo();
            return true;
        }, "spiral - Demonstrates spiral thingy");

        RegisterCommand("show", args =>
        {
            _chessMovementManager.ShowValidMovementsHighlighted = !(_chessMovementManager.ShowValidMovementsHighlighted);
            return true;
        }, "show - Toggles showing highlighted valid movements");

        RegisterCommand("start", args =>
        {
            if (args.Length != 2)
            {
                DisplayNotification("Usage: start [formationName]", ConsoleColor.Yellow);
                return false;
            }
            string formationName = args[1];
            _chessBoard.ResetBoard();
            _chessBoard.SetupCustomFormation(formationName);
            DisplayNotification($"Board set up with {formationName} formation.", ConsoleColor.Green);
            
            return true;
        }, "start [formationName] - Sets up the board with a specified chess problem formation.");

        RegisterCommand("formations", args =>
        {
            var formations = new List<string>
            {
                "excelsior",
                "anastasia's mate",
                "smothered mate",
                "back rank mate",
                "scholar's mate",
                "fool's mate"
            };

            DisplayNotification("Available Formations:", ConsoleColor.Yellow);
            foreach (var formation in formations)
            {
                DisplayNotification($"- {formation}", ConsoleColor.Cyan);
            }

            return true;
        }, "formations - Displays all available chess problem formations.");
    }

    public string StripPrefixFromName(string fullName)
    {
        // prefix is separated by a space
        int lastSpaceIndex = fullName.LastIndexOf(' ');
        return lastSpaceIndex == -1 ? fullName : fullName.Substring(lastSpaceIndex + 1);
    }
}