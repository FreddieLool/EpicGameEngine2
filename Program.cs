public class Program
{
    static void Main()
    {
        ChessDemo chessBoard = new ChessDemo(8, 8);
        ValidMoves moves = new ValidMoves();
        ConsoleRenderer ConsoleRenderer = new ConsoleRenderer();
        Console.WriteLine(@"
                                          ______ _                      _       
                                         / _____| |                    (_)      
                                        | /     | | _   ____  ___  ___  _  ___  
                                        | |     | || \ / _  )/___)/___)| |/ _ \ 
                                        | \_____| | | ( (/ /|___ |___ _| | |_| |
                                         \______|_| |_|\____(___/(___(_|_|\___/            
                                    ");
        chessBoard.Render(chessBoard);
        Console.SetCursorPosition(0, 0);
        //CommandHandler commandHandler = new CommandHandler(chessBoard); asd not implemented

        while (true)
        {
            Console.Write("> ");
            string command = Console.ReadLine();
            //commandHandler.HandleCommand(command);
        }
    }
}