using EpicTileEngine;
using System;

namespace EpicGameEngine
{
    public class CommandHandler
    {
        private readonly ITileActionManager _movementManager;
        private readonly Tilemap _chessBoard;

        public CommandHandler(ITileActionManager movementManager, Tilemap chessBoard)
        {
            _movementManager = movementManager;
            _chessBoard = chessBoard;
        }

        public bool HandleCommand(string command)
        {
            string[] parts = command.Split(' ');
            if (parts.Length == 3 && parts[0].ToLower() == "move")
            {
                Position fromPos = ConvertNotationToPosition(parts[1]);
                Position toPos = ConvertNotationToPosition(parts[2]);
                Tile fromTile = _chessBoard.GetTile(fromPos);

                if (fromTile.Occupant != null)
                {
                    return _movementManager.TryMove(fromTile.Occupant, toPos, _chessBoard);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[No piece at starting position]");
                    Console.ResetColor();
                    return false;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Invalid command]");
                Console.ResetColor();
                return false;
            }
        }

        private Position ConvertNotationToPosition(string notation)
        {
            int x = notation[0] - 'a';
            int y = 8 - (notation[1] - '0');
            return new Position(x, y);
        }
    }
}
