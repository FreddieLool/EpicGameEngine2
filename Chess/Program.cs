
using EpicGameEngine;
using EpicTileEngine;

public class Program
{
    private static Queue<string> commandHistory = new Queue<string>(10);
    private static ChessCommandHandler chessCommandHandler;
    private static ChessDemo chessBoard;
    private static ChessTurnManager turnManager;
    private static MovementManager movementManager;


    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        chessBoard = new ChessDemo(8, 8);
        turnManager = new ChessTurnManager(chessBoard.whitePlayer,chessBoard.blackPlayer);
        movementManager = new MovementManager(turnManager);
        
        
        chessCommandHandler = new ChessCommandHandler(chessBoard, movementManager,turnManager);

        // sub to OnGameReset
        chessBoard.OnGameReset += chessCommandHandler.ResetSelectionAndState;

        RegisterCommands();
        MainLoop();
    }

    static void MainLoop()
    {
        RenderWelcomeMessage();
        RenderGame();
        chessCommandHandler.DisplayGameState();  // Initial state display

        while (true)
        {
            ClearCurrentCommandLine();
            Console.Write("> ");
            string command = Console.ReadLine()?.Trim();

            if (string.Equals(command, "exit", StringComparison.OrdinalIgnoreCase)) break;

            bool isValidCommand = chessCommandHandler.HandleCommand(command);
            if (isValidCommand)
            {
                UpdateCommandHistory(command);
                RenderGame();
            }

            RenderGame();
            chessCommandHandler.DisplayGameState(); // refresh every cmd
            RenderCommandHistoryAtBottom();
        }
    }

    private static void RenderGame()
    {
        chessBoard.Render(chessBoard, chessCommandHandler.HighlightedPositions);
    }

    private static void ClearCurrentCommandLine()
    {
        Console.SetCursorPosition(0, 0);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, 0);
    }

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


    private static void UpdateCommandHistory(string command)
    {
        if (commandHistory.Count >= 10)
        {
            commandHistory.Dequeue();
        }
        commandHistory.Enqueue(command);
    }

    static void RegisterCommands()
    {
        chessCommandHandler.RegisterCommand("restart", args =>
        {
            
            chessBoard.ResetGame();
            turnManager.ResetTurns();
            chessBoard.Render(chessBoard, chessCommandHandler.HighlightedPositions);
            return true;
        }, "restart - Restarts a new game of chess.");

        chessCommandHandler.RegisterCommand("credits", args =>
        {
            CurtainAnimation();
            return true;
        }, "credits - Show game credits and information.");

        chessCommandHandler.RegisterCommand("spiral", args =>
        {
            PerformSpiralDemo();
            return true;
        }, "spiral - Demonstrates spiral thingy");
    }

    public static void PerformSpiralDemo()
    {
        Console.WriteLine("Press any key to start the spiral demo...");
        Console.ReadKey(true);

        chessBoard.ResetBoard();

        int centerX = chessBoard.Width / 2;
        int centerY = chessBoard.Height / 2;

        // Adjust center position for even dimensions
        if (chessBoard.Width % 2 == 0) centerX -= 1;
        if (chessBoard.Height % 2 == 0) centerY -= 1;

        Position centerPosition = new Position(centerX, centerY);
        ChessPiece spiralPiece = new ChessPiece(PieceType.Rook, Color.White, 1);
        chessBoard[centerPosition].SetOccupant(spiralPiece);
        spiralPiece.CurrentTile = chessBoard[centerPosition];

        RenderGame();

        // Get tiles
        var tilesInSpiral = chessBoard.GetTilesInSpiralOrder(centerPosition).ToList();
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



    public static void HighlightTile(Position pos)
    {
        chessBoard[pos].IsHighlighted = true;
    }

    public static void ClearHighlights()
    {
        foreach (var tile in chessBoard)
        {
            tile.IsHighlighted = false;
            tile.RemoveOccupant();
        }
    }



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

    public static void CurtainAnimation()
    {
        Console.CursorVisible = false;
        int width = Console.WindowWidth;
        int height = Console.WindowHeight;
        Random rand = new Random();
        string chars = "_";  // Characters to use for the curtain
        double phase = 0.0;       // Phase to create the wave animation
        int curtainSpeed = 10; // open & close speed (less is faster)

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
        for (int i = 0; i < width / 2 + 5; i++)  // extend beyond the middle for overlap due to waves (only when closing)
        {
            drawCurtain(i);
            phase -= 0.45;  // - phase to animate the wave going down
            Thread.Sleep(curtainSpeed);
        }

        Thread.Sleep(333);  // Wait a lil bit

        // Opening the curtain
        int leftEdge;
        for (int i = width / 2 + 5; i > 0; i--)
        {
            leftEdge = Math.Max(0, i); // Adjusted leftEdge calculation to continue opening the curtain until the edge reaches the very left
            drawCurtain(leftEdge);
            phase += 0.25;  // + phase during opening (wave going up)
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

    static void StartMatrixEffect(int width, int height, int duration)
    {
        // List of words to randomly pick from
        string[] words = { "7$7", "Wael", "Liam", "Daniel", "D0r", "Dor-Ben-Dor", "777", "$", "C#", "@" };
        Random rand = new Random();
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

                if (randomWord == "D0r" || randomWord == "Dor-Ben-Dor")
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (randomWord == "777" || randomWord == "7$7" || randomWord == "C#")
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

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

        int displayTime = 4000; // display time for all frames
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
