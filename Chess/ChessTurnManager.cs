using EpicTileEngine;

namespace EpicGameEngine
{
    public class ChessTurnManager
    {
        public Actor blackPlayer;
        public Actor whitePlayer;

        public ChessTurnManager(Actor whitePlayer, Actor blackPlayer)
        { 
            this.whitePlayer = whitePlayer; 
            this.blackPlayer = blackPlayer;
            whitePlayer.ChangePlayingStatus(true);

        }

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

        public void ResetTurns()
        {
            whitePlayer.ChangePlayingStatus(true);
            blackPlayer.ChangePlayingStatus(false);
        }
    }
}
