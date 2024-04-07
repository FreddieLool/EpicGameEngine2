using EpicTileEngine;
using System;
using System.Collections.Generic;

namespace EpicGameEngine
{
    public class Program
    {
        private static Queue<string> commandHistory = new Queue<string>(10);

        static void Main()
        {
            ChessDemo chessBoard = new ChessDemo(8, 8);
            MovementManager movementManager = new MovementManager();
            CommandHandler commandHandler = new CommandHandler(movementManager, chessBoard);

            RenderWelcomeMessage();
            chessBoard.Render(chessBoard);

            while (true)
            {
                // Clear the previous command line
                ClearCurrentCommandLine();

                // Set the cursor for the next command input at the top
                Console.SetCursorPosition(0, 0);
                Console.Write("> ");
                string command = Console.ReadLine()?.Trim();

                if (string.Equals(command, "exit", StringComparison.OrdinalIgnoreCase)) break;

                bool isValidCommand = commandHandler.HandleCommand(command);
                if (isValidCommand)
                {
                    UpdateCommandHistory(command);
                }

                RenderCommandHistoryAtBottom();
            }
        }

        private static void ClearCurrentCommandLine()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(new string(' ', Console.WindowWidth)); // Overwrite the line with spaces
            Console.SetCursorPosition(0, 0); // Reset cursor position
        }

        private static void RenderWelcomeMessage()
        {
            // Welcome message code remains the same
        }

        private static void UpdateCommandHistory(string command)
        {
            if (commandHistory.Count >= 10)
            {
                commandHistory.Dequeue();
            }
            commandHistory.Enqueue(command);
            // Optional: Console.Clear() here might interfere with the chess board rendering
        }

        private static void RenderCommandHistoryAtBottom()
        {
            // Calculate where to start printing the history based on the window height and history count
            int historyStartLine = Console.WindowHeight - commandHistory.Count - 2;
            Console.SetCursorPosition(0, historyStartLine - 1); // Position for "History:" label
            Console.WriteLine("History:");

            foreach (string cmd in commandHistory)
            {
                Console.WriteLine(cmd);
            }
        }
    }
}
