namespace EpicTileEngine
{
    public class CommandHandler
    {
        private readonly Dictionary<string, Func<string[], bool>> commands;
        private readonly Dictionary<string, string> commandDescriptions;


        public CommandHandler()
        {
            commands = new Dictionary<string, Func<string[], bool>>();
            commandDescriptions = new Dictionary<string, string>();

            // Register the 'help' command
            RegisterCommand("help", args => DisplayHelp(), "help - Display all available commands and their usage.");
        }

        public void RegisterCommand(string commandName, Func<string[], bool> action, string description)
        {
            commands[commandName.ToLower()] = action;
            commandDescriptions[commandName.ToLower()] = description;
        }

        public void RegisterCommandDescription(string commandName, string description)
        {
            commandDescriptions[commandName.ToLower()] = description;
        }

        public bool HandleCommand(string input)
        {
            string[] parts = input.Trim().Split();
            string commandName = parts[0].ToLower();
            if (commands.ContainsKey(commandName))
            {
                return commands[commandName](parts);
            }
            else
            {
                DisplayNotificationMessage($"[Unknown command: {commandName}]");
                return false;
            }
        }

        private bool DisplayHelp()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Available commands:");
            foreach (var description in commandDescriptions.Values)
            {
                Console.WriteLine(" - " + description);
            }
            Console.ResetColor();
            return true;
        }

        public static void DisplayNotificationMessage(string errorMessage, ConsoleColor color = ConsoleColor.Red)
        {
            //Console.ForegroundColor = ConsoleColor.Red;
            ClearErrorMessage();
            ConsoleRGB.WriteLine("[!]: " + errorMessage, color);
            Console.ResetColor();
        }

        public static void ClearErrorMessage()
        {
            Console.Clear();
            Program.RenderWelcomeMessage();
            Console.SetCursorPosition(0, 1);
        }

    }
}