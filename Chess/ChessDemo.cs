using EpicTileEngine;

internal class ChessDemo : Tilemap/*, IRenderer*/
{
    public delegate void GameResetHandler();
    public event GameResetHandler OnGameReset;

    public Actor whitePlayer;
    public Actor blackPlayer;

    public ChessDemo(int width, int height) : base(width, height)
    {
        // Ensure dimensions are properly set
        if (width != 8 || height != 8)
        {
            throw new ArgumentException("Chess board must be 8x8.");
        }

        InitializeGame();
    }

    private void InitializeGame()
    {
        whitePlayer = new Actor(1, "White");
        blackPlayer = new Actor(2, "Black");
        InitializeChessPieces();
    }

    public void ResetBoard()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                this[new Position(x, y)].RemoveOccupant();
            }
        }
    }

    public void ResetGame()
    {
        // Clear the board
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                this[new Position(x, y)].RemoveOccupant();
            }
        }

        // Reinitialize da chess pieces
        InitializeChessPieces();

        // Trigger the event (reset selection, state, turns)
        OnGameReset?.Invoke();

        CommandHandler.DisplayNotification("Game has been restarted.", ConsoleColor.Blue);
    }

    private void InitializeChessPieces()
    {
        // Normal chess setup 

        // Black pieces
        PlaceMajorPieces(0, Color.Black, blackPlayer);
        PlacePawns(1, Color.Black, blackPlayer);

        // White pieces
        PlaceMajorPieces(Height - 1, Color.White, whitePlayer);  // Should be row 7 if Height is 8
        PlacePawns(Height - 2, Color.White, whitePlayer);       // Should be row 6 if Height is 8
    }

    private void PlaceMajorPieces(int row, Color color, Actor player)
    {
        if (row < 0 || row >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(row), "Row index is out of the bounds of the Tilemap.");
        }

        ChessPiece[] pieces =
        [
            new ChessPiece(PieceType.Rook, color, player.Id),
            new ChessPiece(PieceType.Knight, color, player.Id),
            new ChessPiece(PieceType.Bishop, color, player.Id),
            new ChessPiece(PieceType.Queen, color, player.Id),
            new ChessPiece(PieceType.King, color, player.Id),
            new ChessPiece(PieceType.Bishop, color, player.Id),
            new ChessPiece(PieceType.Knight, color, player.Id),
            new ChessPiece(PieceType.Rook, color, player.Id)
        ];

        for (int i = 0; i < pieces.Length; i++)
        {
            if (i < 0 || i >= Width)
            {
                continue; // Skip iteration if the column index is out of bounds
            }
            this[new Position(i, row)].SetOccupant(pieces[i]);
            player.AddTileObject(pieces[i]);
        }
    }

    private void PlacePawns(int row, Color color, Actor player)
    {
        // Place pawns and assign them to the actor
        for (int col = 0; col < Width; col++)
        {
            var pawn = new ChessPiece(PieceType.Pawn, color, player.Id);
            this[new Position(col, row)].SetOccupant(pawn);
            player.AddTileObject(pawn);
        }
    }

    private void PlacePiece(ChessPiece piece, string notation)
    {
        Position position = ConvertNotationToPosition(notation);
        this[position].SetOccupant(piece);
        if (piece.Color == Color.White)
        {
            whitePlayer.AddTileObject(piece);
        }
        else
        {
            blackPlayer.AddTileObject(piece);
        }
    }

    public void SetupCustomFormation(string formationName)
    {
        switch (formationName.ToLower())
        {
            case "white strat":
                PlacePiece(new ChessPiece(PieceType.Pawn, Color.Black, blackPlayer.Id), "c5");
                PlacePiece(new ChessPiece(PieceType.Pawn, Color.Black, blackPlayer.Id), "c4");
                PlacePiece(new ChessPiece(PieceType.Pawn, Color.Black, blackPlayer.Id), "b4");
                PlacePiece(new ChessPiece(PieceType.Pawn, Color.Black, blackPlayer.Id), "d3");
                PlacePiece(new ChessPiece(PieceType.Pawn, Color.Black, blackPlayer.Id), "b2");
                PlacePiece(new ChessPiece(PieceType.Pawn, Color.Black, blackPlayer.Id), "c2");
                PlacePiece(new ChessPiece(PieceType.Pawn, Color.Black, blackPlayer.Id), "e2");
                PlacePiece(new ChessPiece(PieceType.Bishop, Color.Black, blackPlayer.Id), "a3");
                PlacePiece(new ChessPiece(PieceType.Bishop, Color.Black, blackPlayer.Id), "d1");
                PlacePiece(new ChessPiece(PieceType.Knight, Color.Black, blackPlayer.Id), "b1");
                PlacePiece(new ChessPiece(PieceType.Knight, Color.Black, blackPlayer.Id), "e1");
                PlacePiece(new ChessPiece(PieceType.King, Color.Black, blackPlayer.Id), "c1");
                PlacePiece(new ChessPiece(PieceType.Queen, Color.Black, blackPlayer.Id), "a2");
                PlacePiece(new ChessPiece(PieceType.Rook, Color.Black, blackPlayer.Id), "b3");
                PlacePiece(new ChessPiece(PieceType.Rook, Color.Black, blackPlayer.Id), "d2");

                PlacePiece(new ChessPiece(PieceType.King, Color.White, whitePlayer.Id), "f2");
                PlacePiece(new ChessPiece(PieceType.Pawn, Color.White, whitePlayer.Id), "h2");
                break;


            case "anastasia's mate":
                PlacePiece(new ChessPiece(PieceType.King, Color.Black, blackPlayer.Id), "h7");
                PlacePiece(new ChessPiece(PieceType.Pawn, Color.Black, blackPlayer.Id), "g7");
                PlacePiece(new ChessPiece(PieceType.Knight, Color.White, whitePlayer.Id), "e7");
                PlacePiece(new ChessPiece(PieceType.Rook, Color.White, whitePlayer.Id), "e3");
                PlacePiece(new ChessPiece(PieceType.King, Color.White, whitePlayer.Id), "g1");
                break;

            case "smothered mate":
                PlacePiece(new ChessPiece(PieceType.King, Color.Black, blackPlayer.Id), "a2");
                break;

            case "back rank mate":
                PlacePiece(new ChessPiece(PieceType.King, Color.Black, blackPlayer.Id), "a2");
                break;

            case "scholar's mate":
                PlacePiece(new ChessPiece(PieceType.King, Color.Black, blackPlayer.Id), "a2");
                break;

            case "fool's mate":
                PlacePiece(new ChessPiece(PieceType.King, Color.Black, blackPlayer.Id), "a2");
                break;

            default:
                CommandHandler.DisplayNotification("Unknown formation name.", ConsoleColor.Red);
                break;
        }
    }

    public Position ConvertNotationToPosition(string notation)
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
            CommandHandler.DisplayNotification("Invalid position notation. Please enter a valid position.", ConsoleColor.Red);
            return new Position(0, 0); // Default position?
        }
    }

    public string ConvertPositionToNotation(Position position)
    {
        char file = (char)('a' + position.X);
        int rank = 8 - position.Y;
        return $"{file}{rank}";
    }
}