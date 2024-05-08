using System.Diagnostics;

public class ChessAI
{
    private Process stockfish;

    public ChessAI()
    {
        stockfish = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = @"stockfish\stockfish-windows-x86-64-avx2.exe",

                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        stockfish.Start();
    }

    public string GetBestMove(string fen)
    {
        stockfish.StandardInput.WriteLine($"position fen {fen}");
        stockfish.StandardInput.WriteLine("go");

        string output;
        while ((output = stockfish.StandardOutput.ReadLine()) != null)
        {
            if (output.StartsWith("bestmove"))
            {
                return output.Split(' ')[1];  // returns move like "e2e4"
            }
        }
        return null;
    }
}
