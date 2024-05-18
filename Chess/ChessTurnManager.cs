using EpicTileEngine;

namespace EpicGameEngine
{
    public class ChessTurnManager
    {
        public Actor blackPlayer;
        public Actor whitePlayer;

        /// <summary>
        /// Initializes a new instance of the ChessTurnManager class.
        /// </summary>
        /// <param name="whitePlayer">The white player.</param>
        /// <param name="blackPlayer">The black player.</param>
        public ChessTurnManager(Actor whitePlayer, Actor blackPlayer)
        { 
            this.whitePlayer = whitePlayer; 
            this.blackPlayer = blackPlayer;
            whitePlayer.ChangePlayingStatus(true);

        }

        /// <summary>
        /// Checks if the current piece belongs to the current player.
        /// </summary>
        /// <param name="currentPlayer">The current player.</param>
        /// <param name="currentPiece">The piece to check.</param>
        /// <returns>True if the piece belongs to the current player, otherwise false.</returns>
        public bool IsActorChessPieceOwner(Actor currentPlayer, TileObject currentPiece)
        {
            if (currentPlayer == null) return false;

            if (currentPlayer.IsPlaying)
            {
                //Picked The CorrectObject
                if (currentPlayer.Id == currentPiece.ActorId) return true;
                else 
                return false;
            }
            else
            {
                return false;
                //PlayerIsNotPlaying
            }
        }

        /// <summary>
        /// Gets the player whose turn it is.
        /// </summary>
        /// <returns>The player whose turn it is.</returns>
        public Actor GetPlayingActor()
        {
            if (whitePlayer.IsPlaying)
            {
                return whitePlayer;
            }
            else
            {
                return blackPlayer;
            }
        }

        /// <summary>
        /// Gets the opponent player of the current player.
        /// </summary>
        /// <returns>The opponent player.</returns>
        public Actor GetOpponentActor()
        {
            if (whitePlayer.IsPlaying)
            {
                return blackPlayer;
            }
            else
            {
                return whitePlayer;
            }
        }

        /// <summary>
        /// Changes the turn to the next player.
        /// </summary>
        public void ChangeTurns()
        {
            if (whitePlayer.IsPlaying)
            {
                whitePlayer.ChangePlayingStatus(false);
                blackPlayer.ChangePlayingStatus(true);
            }
            else 
            {
                whitePlayer.ChangePlayingStatus(true);
                blackPlayer.ChangePlayingStatus(false);
            }
        }

        /// <summary>
        /// Resets the turn to the white player.
        /// </summary>
        public void ResetTurns()
        {
            whitePlayer.ChangePlayingStatus(true);
            blackPlayer.ChangePlayingStatus(false);
        }
    }
}
