using EpicGameEngine;
using EpicTileEngine;
using System.ComponentModel;
using System.Diagnostics;

internal class ChessCommandHandler : CommandHandler
{
    private ChessDemo _chessBoard;
    private MovementManager _chessMovementManager;
    private ChessTurnManager _chessTurnManager;

    /// <summary>
    /// Initializes a new instance of the ChessCommandHandler class.
    /// </summary>
    /// <param name="chessBoard">The chess board instance.</param>
    /// <param name="movementManager">The movement manager instance.</param>
    /// <param name="chessTurnManager">The chess turn manager instance.</param>
    public ChessCommandHandler(ChessDemo chessBoard, MovementManager movementManager, ChessTurnManager chessTurnManager)
    {
        this._chessBoard = chessBoard;
        this._chessMovementManager = movementManager;
        this._chessTurnManager = chessTurnManager;

        RegisterChessCommands();
    }

    /// <summary>
    /// Registers chess commands and their respective actions.
    /// </summary>
    private void RegisterChessCommands()
    {
        RegisterCommand("select", HandleSelectCommand, "select [position] - Select a piece at the specified position.");
        RegisterCommand("deselect", HandleDeselectCommand, "deselect - Deselect the currently selected piece, if any.");
        RegisterCommand("move", HandleMoveCommand, "move [position] - Move the currently selected piece to the specified position.");
        RegisterCommand("restart", args => { _chessBoard.ResetGame(); return true; }, "restart - Restarts a new game of chess.");
        RegisterCommand("credits", args => { Program.ShowCredits(); return true; }, "credits - Show game credits and information.");
        RegisterCommand("spiral", args => { Program.PerformSpiralDemo(); return true; }, "spiral - Demonstrates spiral thingy");
        RegisterCommand("show", args => { _chessMovementManager.ShowValidMovementsHighlighted = !_chessMovementManager.ShowValidMovementsHighlighted; return true; }, "show - Toggles showing highlighted valid movements");
        RegisterCommand("start", HandleStartCommand, "start [formationName] - Sets up the board with a specified chess problem formation.");
        RegisterCommand("formations", DisplayFormations, "formations - Displays all chess problem formations.");
    }

    /// <summary>
    /// Handles the select command.
    /// </summary>
    /// <param name="parts">The command arguments.</param>
    /// <returns>True if the command is executed successfully, otherwise false.</returns>
    private bool HandleSelectCommand(string[] parts)
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

            ClearErrorMessage();
            DisplayNotification($"selected: {StripPrefixFromName(_chessMovementManager.SelectedPiece.Name)}", ConsoleColor.DarkGray);
            return true;
        }

        DisplayNotification("No piece at the specified position.", ConsoleColor.DarkGray);
        return false;
    }

    /// <summary>
    /// Handles the deselect command.
    /// </summary>
    /// <param name="parts">The command arguments.</param>
    /// <returns>True if the command is executed successfully, otherwise false.</returns>
    private bool HandleDeselectCommand(string[] parts)
    {
        if(_chessMovementManager.SelectedPiece != null)
        {
            DisplayNotification($"deselected: {StripPrefixFromName(_chessMovementManager.SelectedPiece.Name)}", ConsoleColor.DarkGray);
            _chessMovementManager.SelectedPiece = null;
        }
        _chessMovementManager.HighlightedPositions.Clear();
        return true;
    }

    /// <summary>
    /// Handles the move command.
    /// </summary>
    /// <param name="parts">The command arguments.</param>
    /// <returns>True if the command is executed successfully, otherwise false.</returns>
    private bool HandleMoveCommand(string[] parts)
    {
        if (_chessMovementManager.SelectedPiece == null)
        {
            DisplayNotification("Usage: select [position], then move [to]", ConsoleColor.DarkGray);
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
                    Program.DisplayGameState();
                    DisplayNotification($"moved: {StripPrefixFromName(_chessMovementManager.SelectedPiece.Name)}", ConsoleColor.DarkGray);
                    _chessMovementManager.HighlightedPositions.Clear();
                    _chessMovementManager.SelectedPiece = null;
                    _chessTurnManager.ChangeTurns();
                }
                else
                {
                    DisplayNotification("Move failed. Check yo self.", ConsoleColor.Red);
                }

                return result;
            }
            DisplayNotification("Selected piece cannot move to the specified position.", ConsoleColor.Red);
            return false;
        }

        DisplayNotification("Invalid move syntax. Usage: move [to]");
        return false;
    }

    /// <summary>
    /// Handles the start command to set up a custom formation.
    /// </summary>
    /// <param name="args">The command arguments.</param>
    /// <returns>True if the command is executed successfully, otherwise false.</returns>
    private bool HandleStartCommand(string[] args)
    {
        if (args.Length != 2)
        {
            DisplayNotification("Usage: start [formationName]", ConsoleColor.Yellow);
            return false;
        }
        string formationName = args[1];
        _chessBoard.ResetGame();
        _chessBoard.SetupCustomFormation(formationName);
        DisplayNotification("Setting up formation...", ConsoleColor.Cyan);
        DisplayCenteredNotification($"Formation: {formationName} has been set up.");
        return true;
    }

    private bool DisplayFormations(string[] args)
    {
        var formations = new List<string>
        {
            "1: Challenge 1",
            "2: Challenge 2",
            "checkmate1: Simple Checkmate",
            "4: Scholar's Mate",
            "5: Boden's Mate",
            "6: Back Rank Mate",
            "7 Smothered Mate"
        };

        DisplayNotification("Showing available formations...", ConsoleColor.Cyan);

        // Clear previous centered notifications
        ClearPreviousCenteredNotification(formations.Count);

        // Concatenate formations into a single string with new lines
        string formationList = string.Join("\n", formations);

        DisplayCenteredNotification(formationList, ConsoleColor.Cyan);

        return true;
    }

    /// <summary>
    /// Strips the prefix from the piece name.
    /// </summary>
    /// <param name="fullName">The full name of the piece.</param>
    /// <returns>The name without the prefix.</returns>
    public string StripPrefixFromName(string fullName)
    {
        // prefix is separated by a space
        int lastSpaceIndex = fullName.LastIndexOf(' ');
        return lastSpaceIndex == -1 ? fullName : fullName.Substring(lastSpaceIndex + 1);
    }
}