
using EpicGameEngine;
using EpicTileEngine;

public class Program
{
    private static Queue<string> commandHistory = new Queue<string>(10);
    private static ChessCommandHandler chessCommandHandler;
    private static ChessDemo chessBoard;
    
    static void Main()
    {
        chessBoard = new ChessDemo(8, 8);
        MovementManager movementManager = new MovementManager();
        ChessTurnManager turnManager = new ChessTurnManager(chessBoard.whitePlayer,chessBoard.blackPlayer);
        
        
        chessCommandHandler = new ChessCommandHandler(chessBoard, movementManager,turnManager);

        // sub to OnGameReset
        chessBoard.OnGameReset += chessCommandHandler.ResetSelectionAndState;

        RegisterCommands();
        MainLoop();
    }

    static void MainLoop()
    {
        RenderWelcomeMessage();
        chessBoard.Render(chessBoard, chessCommandHandler.HighlightedPositions);

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
                chessBoard.Render(chessBoard, chessCommandHandler.HighlightedPositions);
            }

            chessBoard.Render(chessBoard, chessCommandHandler.HighlightedPositions);
            RenderCommandHistoryAtBottom();
        }
    }

    private static void ClearCurrentCommandLine()
    {
        Console.SetCursorPosition(0, 0);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, 0);
    }

    public static void RenderWelcomeMessage()
    {
        ConsoleRGB.WriteLine(@"
                                             _____ _                     _       
                                            /  __ \ |                   (_)      
                                            | /  \/ |__   ___  ___ ___   _  ___  
                                            | |   | '_ \ / _ \/ __/ __| | |/ _ \ 
                                            | \__/\ | | |  __/\__ \__ \_| | (_) |
                                             \____/_| |_|\___||___/___(_)_|\___/              
        ", ConsoleColor.Yellow);
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
        chessCommandHandler.RegisterCommand("start", args =>
        {
            chessBoard.ResetGame();
            chessBoard.Render(chessBoard, chessCommandHandler.HighlightedPositions);
            return true;
        }, "start - Starts or restarts a new game of chess.");
    }

    private static void RenderCommandHistoryAtBottom()
    {
        int historyStartLine = Console.WindowHeight - commandHistory.Count - 2;
        Console.SetCursorPosition(0, historyStartLine - 1); // Position for "History:" label
        Console.WriteLine("History:");

        foreach (string cmd in commandHistory)
        {
            Console.WriteLine(cmd);
        }
    }
}
