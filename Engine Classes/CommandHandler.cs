using System;
using System.Collections.Generic;

namespace EpicTileEngine
{
    /// <summary>
    /// Handles commands and their associated actions, providing support for command registration, execution, and help display.
    /// </summary>
    public class CommandHandler
    {
        /// <summary>
        /// Stores command names and their corresponding actions.
        /// </summary>
        private readonly Dictionary<string, Func<string[], bool>> commands;

        /// <summary>
        /// Stores descriptions for each registered command.
        /// </summary>
        private readonly Dictionary<string, string> commandDescriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler"/> class, setting up the default help command.
        /// </summary>
        public CommandHandler()
        {
            commands = new Dictionary<string, Func<string[], bool>>();
            commandDescriptions = new Dictionary<string, string>();

            // Register the 'help' command
            RegisterCommand("help", args => DisplayHelp(), "help - Display all available commands and their usage.");
        }

        /// <summary>
        /// Registers a new command with the specified action and description.
        /// </summary>
        /// <param name="commandName">The name of the command to register.</param>
        /// <param name="action">The action to execute when the command is invoked.</param>
        /// <param name="description">A brief description of the command for help purposes.</param>
        public void RegisterCommand(string commandName, Func<string[], bool> action, string description)
        {
            commands[commandName.ToLower()] = action;
            commandDescriptions[commandName.ToLower()] = description;
        }

        /// <summary>
        /// Processes the specified input as a command.
        /// </summary>
        /// <param name="input">The input string to parse and execute.</param>
        /// <returns>True if the command is recognized and executed successfully; otherwise, false.</returns>
        public bool HandleCommand(string input)
        {
            // Split the input into individual parts (spaces as delimiters)
            string[] parts = input.Trim().Split();
            // The first element is the command name, converted to lowercase for case-insensitive matching
            string commandName = parts[0].ToLower();

            if (commands.ContainsKey(commandName))
            {
                return commands[commandName](parts);
            }
            else
            {
                DisplayNotification($"[Unknown command: {commandName}] - Use 'help' to display the list of commands");
                return false;
            }
        }

        /// <summary>
        /// Displays a list of all available commands and their descriptions.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool DisplayHelp()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 1);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Available commands:");
            foreach (var description in commandDescriptions.Values)
            {
                Console.WriteLine(" - " + description);
            }
            Console.ResetColor();
            return true;
        }

        /// <summary>
        /// Displays a notification message with an optional color. Defaults to red for errors.
        /// </summary>
        /// <param name="errorMessage">The message to be displayed.</param>
        /// <param name="color">The color of the message text.</param>
        public static void DisplayNotification(string errorMessage, ConsoleColor color = ConsoleColor.Red)
        {
            ClearErrorMessage();
            ConsoleRGB.WriteLine("[!]: " + errorMessage, color);
            Console.ResetColor();
        }

        /// <summary>
        /// Clears the console and re-renders the welcome message.
        /// </summary>
        public static void ClearErrorMessage()
        {
            Console.Clear();
            Program.RenderWelcomeMessage();
            Console.SetCursorPosition(0, 1);
        }
    }
}