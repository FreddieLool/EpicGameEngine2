
using EpicTileEngine;

public class Program
{
    private static Queue<string> commandHistory = new Queue<string>(10);

    static void Main()
    {
        ChessDemo chessBoard = new ChessDemo(8, 8);
        MovementManager movementManager = new MovementManager();
        ChessCommandHandler commandHandler = new ChessCommandHandler(chessBoard, movementManager);

        RenderWelcomeMessage();
        chessBoard.Render(chessBoard);

        while (true)
        {
            ClearCurrentCommandLine();

            Console.SetCursorPosition(0, 0);
            Console.Write("> ");
            string command = Console.ReadLine()?.Trim();

            if (string.Equals(command, "exit", StringComparison.OrdinalIgnoreCase)) break;

            bool isValidCommand = commandHandler.HandleCommand(command);
            if (isValidCommand)
            {
                UpdateCommandHistory(command);
            }

//            chessBoard.Render(chessBoard);
            RenderCommandHistoryAtBottom();
        }
    }

    private static void ClearCurrentCommandLine()
    {
        Console.SetCursorPosition(0, 0);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, 0);
    }

    private static void RenderWelcomeMessage()
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
