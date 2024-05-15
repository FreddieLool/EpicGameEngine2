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

    /// <summary>
    /// Initializes the game with players and chess pieces.
    /// </summary>
    private void InitializeGame()
    {
        whitePlayer = new Actor(1, "White");
        blackPlayer = new Actor(2, "Black");
        InitializeLimitedChessPieces();
        //InitializeChessPieces();
    }

    /// <summary>
    /// Resets the board by removing all pieces.
    /// </summary>
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

    /// <summary>
    /// Resets the game, reinitializes the pieces, and triggers the reset event.
    /// </summary>
    public void ResetGame()
    {
        ResetBoard();

        // Reinitialize da chess pieces
        InitializeChessPieces();

        // Trigger the event (reset selection, state, turns)
        OnGameReset?.Invoke();

        CommandHandler.DisplayNotification("Game has been restarted.", ConsoleColor.Blue);
    }

    /// <summary>
    /// Initializes the chess pieces in their starting positions.
    /// </summary>
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

    /// <summary>
    /// Places the major pieces (Rook, Knight, Bishop, Queen, King) on the specified row.
    /// </summary>
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

    /// <summary>
    /// Places the pawns on the specified row.
    /// </summary>
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

    /// <summary>
    /// Places a specific piece on the board using chess notation.
    /// </summary>
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

    /// <summary>
    /// Sets up a custom formation on the board.
    /// </summary>
    public void SetupCustomFormation(string formationName)
    {
        ResetBoard();
        switch (formationName.ToLower())
        {
            case "1":
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


            case "2":
                PlacePiece(new ChessPiece(PieceType.King, Color.Black, blackPlayer.Id), "h7");
                PlacePiece(new ChessPiece(PieceType.Pawn, Color.Black, blackPlayer.Id), "g7");
                PlacePiece(new ChessPiece(PieceType.Knight, Color.White, whitePlayer.Id), "e7");
                PlacePiece(new ChessPiece(PieceType.Rook, Color.White, whitePlayer.Id), "e3");
                PlacePiece(new ChessPiece(PieceType.King, Color.White, whitePlayer.Id), "g1");
                break;

            case "checkmate1":
                PlacePiece(new ChessPiece(PieceType.King, Color.Black, blackPlayer.Id), "h8");
                PlacePiece(new ChessPiece(PieceType.Rook, Color.White, whitePlayer.Id), "g6");
                PlacePiece(new ChessPiece(PieceType.King, Color.White, whitePlayer.Id), "f7");
                break;


            case "scholarsmate":
                // Scholar's Mate: A classic checkmate in 4 moves
                PlacePiece(new ChessPiece(PieceType.King, Color.Black, blackPlayer.Id), "e8");
                PlacePiece(new ChessPiece(PieceType.Pawn, Color.Black, blackPlayer.Id), "e5");
                PlacePiece(new ChessPiece(PieceType.Pawn, Color.Black, blackPlayer.Id), "f7");

                PlacePiece(new ChessPiece(PieceType.King, Color.White, whitePlayer.Id), "e1");
                PlacePiece(new ChessPiece(PieceType.Queen, Color.White, whitePlayer.Id), "h5");
                PlacePiece(new ChessPiece(PieceType.Bishop, Color.White, whitePlayer.Id), "c4");
                break;

            case "5":
                PlacePiece(new ChessPiece(PieceType.King, Color.Black, blackPlayer.Id), "a2");
                break;

            case "6":
                PlacePiece(new ChessPiece(PieceType.King, Color.Black, blackPlayer.Id), "a2");
                break;

            default:
                CommandHandler.DisplayNotification("Unknown formation name.", ConsoleColor.Red);
                break;
        }
    }

    /// <summary>
    /// Converts chess notation to a board position.
    /// </summary>
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

    /// <summary>
    /// Converts a board position to chess notation.
    /// </summary>
    public string ConvertPositionToNotation(Position position)
    {
        char file = (char)('a' + position.X);
        int rank = 8 - position.Y;
        return $"{file}{rank}";
    }

    /// <summary>
    /// Initializes a limited set of chess pieces for debugging purposes.
    /// </summary>
    private void InitializeLimitedChessPieces()
    {
        ResetBoard();

        // Black pieces
        PlaceSpecificPieces(new[]
        {
        new PiecePlacement(PieceType.Pawn, Color.Black, new Position(0, 1)),
        new PiecePlacement(PieceType.Pawn, Color.Black, new Position(1, 1)),
        new PiecePlacement(PieceType.Queen, Color.Black, new Position(3, 0)),
        new PiecePlacement(PieceType.King, Color.Black, new Position(4, 0))
    }, blackPlayer);

        // White pieces
        PlaceSpecificPieces(new[]
        {
        new PiecePlacement(PieceType.Pawn, Color.White, new Position(0, 6)),
        new PiecePlacement(PieceType.Pawn, Color.White, new Position(1, 6)),
        new PiecePlacement(PieceType.Queen, Color.White, new Position(3, 7)),
        new PiecePlacement(PieceType.King, Color.White, new Position(4, 7))
    }, whitePlayer);
    }

    /// <summary>
    /// Structure to simplify passing piece placement info.
    /// </summary>
    private struct PiecePlacement
    {
        public PieceType Type { get; }
        public Color Color { get; }
        public Position Position { get; }

        public PiecePlacement(PieceType type, Color color, Position position)
        {
            Type = type;
            Color = color;
            Position = position;
        }
    }

    /// <summary>
    /// Places specific pieces on the board.
    /// </summary>
    private void PlaceSpecificPieces(PiecePlacement[] pieces, Actor player)
    {
        foreach (var piece in pieces)
        {
            var chessPiece = new ChessPiece(piece.Type, piece.Color, player.Id);
            this[piece.Position].SetOccupant(chessPiece);
            player.AddTileObject(chessPiece);
        }
    }
}