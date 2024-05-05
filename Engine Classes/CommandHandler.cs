namespace EpicTileEngine
{
    public class CommandHandler
    {
        private readonly Dictionary<string, Func<string[], bool>> commands;

        public CommandHandler()
        {
            commands = new Dictionary<string, Func<string[], bool>>();
        }

        public void RegisterCommand(string commandName, Func<string[], bool> action)
        {
            commands[commandName.ToLower()] = action;
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Unknown command]");
                Console.ResetColor();
                return false;
            }
        }
    }
}