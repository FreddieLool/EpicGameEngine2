
using EpicGameEngine;
using EpicTileEngine;
using System.Diagnostics;

public class Program
{
    private static Queue<string> commandHistory = new(10);
    private static ChessCommandHandler _chessCommandHandler;
    private static ChessDemo _chessBoard;
    private static ChessTurnManager _chessTurnManager;
    private static MovementManager _movementManager;
    private static Renderer _renderer;

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        _chessBoard = new ChessDemo(8, 8);
        _chessTurnManager = new ChessTurnManager(_chessBoard.whitePlayer, _chessBoard.blackPlayer);
        _movementManager = new MovementManager(_chessTurnManager);
        _chessCommandHandler = new ChessCommandHandler(_chessBoard, _movementManager, _chessTurnManager);
        _renderer = new Renderer();

        // sub to OnGameReset
        _chessBoard.OnGameReset += _movementManager.ResetSelectionAndState;
        _chessBoard.OnGameReset += _chessTurnManager.ResetTurns;

        MainLoop();
    }

    /// <summary>
    /// Main loop of the program handling user input and game rendering.
    /// </summary>
    static void MainLoop()
    {
        RenderWelcomeMessage();
        RenderGame();
        DisplayGameState();  // Initial state display

        while (true)
        {
            ClearCurrentCommandLine();
            Console.Write("> ");
            string? command = Console.ReadLine()?.Trim();

            bool isValidCommand = _chessCommandHandler.HandleCommand(command);
            if (isValidCommand)
            {
                UpdateCommandHistory(command);
                RenderGame();
            }

            if (string.Equals(command, "exit", StringComparison.OrdinalIgnoreCase)) break;

            RenderGame();
            DisplayGameState(); // refresh every cmd
            RenderCommandHistoryAtBottom();
        }
    }

    /// <summary>
    /// Renders the game board and pieces.
    /// </summary>
    private static void RenderGame()
    {
        // User defined colors
        _renderer.HighlightedColor = ConsoleColor.Green;
        _renderer.Actor1PieceColor = ConsoleColor.Yellow;
        _renderer.Actor2PieceColor = ConsoleColor.Gray;
        _renderer.BackgroundColor = ConsoleColor.Black;
        _renderer.SelectedColor = ConsoleColor.Red;

        // Get highlighted positions (valid movements for selected piece)
        _renderer.HighlightedPositions = _movementManager.HighlightedPositions;

        // Render!
        _renderer.Render(_chessBoard, _movementManager.SelectedPiece, _movementManager.ShowValidMovementsHighlighted);
    }

    /// <summary>
    /// Displays the current game state, and captures.
    /// </summary>
    public static void DisplayGameState()
    {
        int stateLine = 2; // Line number for the game state
        Console.SetCursorPosition(0, stateLine);
        Console.Write(new string(' ', Console.WindowWidth));

        Actor currentPlayer = _chessTurnManager.GetPlayingActor();

        Console.SetCursorPosition(0, stateLine);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("State: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{currentPlayer.Name}'s turn");

        if (_movementManager.LastMoveWasCapture)
        {
            string pieceName = _chessCommandHandler.StripPrefixFromName(_movementManager.LastCapturedPiece.Name);
            string positionNotation = _chessBoard.ConvertPositionToNotation(_movementManager.LastCapturedPiecePosition);

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

            CommandHandler.DisplayCenteredNotification($"{currentPlayer.Name} captures {pieceName} at {positionNotation}", ConsoleColor.Red);
        }

        Console.ResetColor();
    }


    /// <summary>
    /// Clears the current command line.
    /// </summary>
    private static void ClearCurrentCommandLine()
    {
        Console.SetCursorPosition(0, 0);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, 0);
    }

    /// <summary>
    /// Renders the welcome message at the start of the game.
    /// </summary>
    public static void RenderWelcomeMessage()
    {
        string welcomeMessage = @"                
                                                                         |\_
                                                                        /  .\_
            ______              _           __  _______                |   ___)
           /_  __/__ ______ _  (_)__  ___ _/ / / ___/ /  ___ ___ ___   |    \   
            / / / -_) __/  ' \/ / _ \/ _ `/ / / /__/ _ \/ -_|_-<(_-<   |  =  |  
           /_/  \__/_/ /_/_/_/_/_//_/\_,_/_/  \___/_//_/\__/___/___/   /_____\     
                                                                      [_______]
        ";

        int maxWidth = welcomeMessage.Split('\n').Max(line => line.Length);

        int consoleWidth = Console.WindowWidth;
        int consoleHeight = Console.WindowHeight;

        // Center horizontally
        int startX = (consoleWidth - maxWidth) / 2;
        // starts 2 lines from top
        int startY = 2;

        // properly positioned
        foreach (var line in welcomeMessage.Split('\n'))
        {
            Console.SetCursorPosition(startX, startY++);
            ConsoleRGB.WriteLine(line, ConsoleColor.DarkGray);
        }

        Console.ResetColor();  // Reset the console color
    }

    /// <summary>
    /// Updates the command history.
    /// </summary>
    private static void UpdateCommandHistory(string command)
    {
        if (commandHistory.Count >= 10)
        {
            commandHistory.Dequeue();
        }
        commandHistory.Enqueue(command);
    }

    /// <summary>
    /// Performs a spiral demo by highlighting tiles in a spiral order.
    /// </summary>
    public static void PerformSpiralDemo()
    {
        Console.WriteLine("Press any key to start the spiral demo...");
        Console.ReadKey(true);

        _chessBoard.ResetBoard();

        int centerX = _chessBoard.Width / 2;
        int centerY = _chessBoard.Height / 2;

        // Adjust center position for even dimensions
        if (_chessBoard.Width % 2 == 0) centerX -= 1;
        if (_chessBoard.Height % 2 == 0) centerY -= 1;


        Position centerPosition = new(centerX, centerY);
        ChessPiece spiralPiece = new(PieceType.Rook, Color.White, 1);
        _chessBoard[centerPosition].SetOccupant(spiralPiece);
        spiralPiece.CurrentTile = _chessBoard[centerPosition];

        RenderGame();

        // Get tiles
        var tilesInSpiral = _chessBoard.GetTilesInSpiralOrder(centerPosition).ToList();
        foreach (var tile in tilesInSpiral)
        {
            Console.WriteLine("Press any key for the next move...");
            Console.ReadKey(true);

            // Highlight
            tile.IsHighlighted = true;
            RenderGame();
        }

        ClearHighlights();
        RenderGame();
    }

    /// <summary>
    /// Highlights a specific tile.
    /// </summary>
    public static void HighlightTile(Position pos)
    {
        _chessBoard[pos].IsHighlighted = true;
    }

    /// <summary>
    /// Clears all highlights from the board.
    /// </summary>
    public static void ClearHighlights()
    {
        foreach (var tile in _chessBoard)
        {
            tile.IsHighlighted = false;
            tile.RemoveOccupant();
        }
    }

    /// <summary>
    /// Renders the command history at the bottom of the console.
    /// </summary>
    private static void RenderCommandHistoryAtBottom()
    {
        int historyStartLine = Console.WindowHeight - commandHistory.Count - 2;
        Console.SetCursorPosition(0, historyStartLine - 1); // label pos
        Console.WriteLine("History:");

        foreach (string cmd in commandHistory)
        {
            Console.WriteLine(cmd);
        }
    }

    /// <summary>
    /// Shows the credits with a curtain animation and matrix effect.
    /// </summary>
    public static void ShowCredits()
    {
        Console.CursorVisible = false;
        int width = Console.WindowWidth;
        int height = Console.WindowHeight;
        Random rand = new();
        string chars = "▌▀▌";  // Characters to use for the curtain
        double phase = 0.0;       // Phase to create the wave animation
        int curtainSpeed = 10; // open & close speed (less is faster)
        int stepSize = 3; // cuz it takes ages to do it normally

        // Draw a single frame of curtain with animated wavy edges
        void drawCurtain(int size)
        {
            Console.Clear();
            for (int i = 0; i < height; i++)
            {
                // Calculate wavy edge offset with an animating phase
                int waveOffset = (int)(Math.Sin(i * 0.3 + phase) * 5);  // 0.3 for frequency, 5 for amplitude

                int leftEdge = Math.Max(0, size + waveOffset);  // Ensure it doesn't go negative
                int rightEdge = Math.Min(width, width - size + waveOffset);  // Ensures it doesn't exceed screen width

                for (int j = 0; j < leftEdge; j++)
                {
                    Console.SetCursorPosition(j, i);
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(chars[rand.Next(chars.Length)]);
                }
                for (int j = width - 1; j >= rightEdge; j--)
                {
                    Console.SetCursorPosition(j, i);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(chars[rand.Next(chars.Length)]);
                }
            }
        }

        // Closing the curtain
        for (int i = 0; i < width / 2 + 5; i += stepSize)  // extend beyond the middle for overlap due to waves (only when closing)
        {
            drawCurtain(i);
            phase -= 0.85;  // - phase to animate the wave going down
            Thread.Sleep(curtainSpeed);
        }

        Thread.Sleep(333);  // Wait a lil bit

        // Opening the curtain
        int leftEdge;
        for (int i = width / 2 + 5; i > 0; i -= stepSize)
        {
            leftEdge = Math.Max(0, i); // Adjusted leftEdge calculation to continue opening the curtain until the edge reaches the very left
            drawCurtain(leftEdge);
            phase += 0.55;  // + phase during opening (wave going up)
            Thread.Sleep(curtainSpeed);
        }

        Console.Clear();
        StartMatrixEffect(width, height, 5);
        Console.Clear();

        ShowLogoAnimation(width, height); 

        ShowAsciiArt(width, height);

        Console.ReadKey();
        Console.CursorVisible = true;
        Console.Clear();

        RenderWelcomeMessage();
        RenderGame();
    }

    /// <summary>
    /// Shows ASCII art with animations.
    /// </summary>
    private static void ShowAsciiArt(int width, int height)
    {
        // ASCII art block
        string asciiArt = @"
 The Epic Tile Engine was developed by..
      __          __        _     _      _                     _____              _      _ 
      \ \        / /       | |   | |    (_)                   |  __ \            (_)    | |
       \ \  /\  / /_ _  ___| |   | |     _  __ _ _ __ ___     | |  | | __ _ _ __  _  ___| |
        \ \/  \/ / _` |/ _ \ |   | |    | |/ _` | '_ ` _ \    | |  | |/ _` | '_ \| |/ _ \ |
         \  /\  / (_| |  __/ |   | |____| | (_| | | | | | |   | |__| | (_| | | | | |  __/ |
          \/  \/ \__,_|\___|_|   |______|_|\__,_|_| |_| |_|   |_____/ \__,_|_| |_|_|\___|_|

       Special Thanks to our C# Teacher:
             ______  _______ _______    ______  _______ _          ______  _______ _______ 
            (  __  \(  ___  (  ____ )  (  ___ \(  ____ ( (    /|  (  __  \(  ___  (  ____ )
            | (  \  | (   ) | (    )|  | (   ) | (    \|  \  ( |  | (  \  | (   ) | (    )|
            | |   ) | |   | | (____)|  | (__/ /| (__   |   \ | |  | |   ) | |   | | (____)|
            | |   | | |   | |     __)  |  __ ( |  __)  | (\ \) |  | |   | | |   | |     __)
            | |   ) | |   | | (\ (     | (  \ \| (     | | \   |  | |   ) | |   | | (\ (   
            | (__/  | (___) | ) \ \__  | )___) | (____/| )  \  |  | (__/  | (___) | ) \ \__
            (______/(_______|/   \__/  |/ \___/(_______|/    )_)  (______/(_______|/   \__/

                                                 For his invaluable guidance and expertise.
        ";

        string[] asciiLines = asciiArt.Trim().Split('\n');
        int topCenterY = Math.Max(0, height / 4 - asciiLines.Length / 2) + 3; // + 3 lines down


        for (int i = 0; i < asciiLines.Length; i++)
        {
            int cursorY = topCenterY + i;
            if (cursorY >= 0 && cursorY < height)
            {
                if (asciiLines[i].Contains("Special Thanks"))
                    Console.ForegroundColor = ConsoleColor.Blue;
                else if (asciiLines[i].Contains("For his"))
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.White;

                Console.SetCursorPosition(width / 2 - asciiLines[i].Length / 2, cursorY);
                Console.WriteLine(asciiLines[i]);
            }
            Thread.Sleep(200);
        }

        // input to close
        Console.SetCursorPosition(0, height - 5);
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    /// <summary>
    /// Starts the Matrix effect with random words falling down the screen.
    /// </summary>
    static void StartMatrixEffect(int width, int height, int duration)
    {
        // List of words to randomly pick from
        string[] words = { "♛", "Wael", "Liam", "Daniel", "D0r ♚", "Dor-Ben-Dor", "♟", "♜", "♚", "♞" };
        Random rand = new();
        int[] yPositions = new int[width];
        int scrollSpeed = 177;

        // Initialize vertical positions
        for (int i = 0; i < width; i++)
        {
            yPositions[i] = rand.Next(height);
        }

        DateTime endTime = DateTime.Now.AddSeconds(duration);
        while (DateTime.Now < endTime)
        {
            for (int i = 0; i < width; i++)
            {
                // ensure it fits within the screen
                string randomWord = words[rand.Next(words.Length)];
                int xPos = i;

                // Adjust position if word exceeds the screen width
                if (xPos + randomWord.Length > width)
                    xPos = width - randomWord.Length;

                if (randomWord == "D0r ♚" || randomWord == "Dor-Ben-Dor")
                    Console.ForegroundColor = ConsoleColor.White;
                else if (randomWord == "♛" || randomWord == "♞")
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else if (randomWord == "♞" || randomWord == "♟")
                    Console.ForegroundColor = ConsoleColor.Blue;
                else
                    Console.ForegroundColor = ConsoleColor.Green;

                Console.SetCursorPosition(xPos, yPositions[i]);
                Console.Write(randomWord);

                // Update vert pos for scrolling effect
                yPositions[i] = (yPositions[i] + 1) % height;
            }

            // scrolling speed
            Thread.Sleep(scrollSpeed);
            Console.Clear();
        }
        Console.ResetColor();
    }

    /// <summary>
    /// Shows the tiltan logo animation with changing colors.
    /// </summary>
    private static void ShowLogoAnimation(int width, int height)
    {
        string frame = @"
        ─────────┌┌══▄▄═─┌────────────────────────────────
        ──────┌▄████████████──────────────────────────────
        ─────█████████████████───────┌─────────┌──────────
        ────███████████████████───▄█═▀───────┌────────────
        ───╒████████████████████─▄███████▄────────────────
        ───╒█████████████████████████████████─┌─█▀─═──────
        ────████████████████████████████████████═─────────
        ─────█████████████████████████████████────────────
        ───────███████████████████████████████────────────
        ─────╒▄████████████████████████████████░──────────
        ───▄███████████████████████████████████───────────
        ──█████████████████████████████████████▄┌─────────
        ─████████████████████████████████████▌────────────
        ─████████████████████▌╘███████████─────────└──────
        ─▀███████████████████──▐███─└──└┌──▄┌─────────────
        ──██████████████████────────────▐───▀─────────────
        ───└██████████████▀───────────────────────────────
        ─────░└▀███████▀──────────────────────────────────
        ──────────────────────────────────────────────────
        ───╒▄────▄█▄──╒██───┌═────────────────────────────
        ──╒██─────────▐██───██────────────────────────────
        ─└▀██└└──███──▐██──└██▌└──┌██─└██▄──███─└▀██──────
        ──╒██────███──▐██───██─────────╒██──██────██▌─────
        ──╒██────███──▐██───██─────┌▄▀▀▀██──██────███─────
        ──╒██────███──▐██───██───┌██───╒██──██────███─────
        ───██▄┌──███──▐██───███───███─┌███──██────███─────
        ──────────────────────────────────────────────────
        ";

        ConsoleColor[] colors = { ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Yellow };

        int displayTime = 997; // display time for all frames
        int frameDisplayDuration = displayTime / colors.Length;

        string[] lines = frame.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

        for (int i = 0; i < colors.Length; i++)
        {
            Console.Clear();
            Console.ForegroundColor = colors[i];
            int verticalStart = Math.Max(0, (height - lines.Length) / 2); // This might need adjusting... NOT to cause a crash

            for (int j = 0; j < lines.Length; j++)
            {
                // Trim each line individually to maintain leading and trailing spaces
                string line = lines[j];
                Console.SetCursorPosition((width - line.Length) / 2, verticalStart + j); // Center line horizontally
                Console.WriteLine(line);
            }

            Thread.Sleep(frameDisplayDuration);
        }

        Console.ResetColor();
        Console.Clear();
    }
}
