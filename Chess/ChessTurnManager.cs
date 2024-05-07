using EpicTileEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGameEngine
{
    internal class ChessTurnManager
    {
        Actor whitePlayer;
        Actor blackPlayer;
        public ChessTurnManager(Actor whitePlayer,Actor blackPlayer) { 
            this.whitePlayer = whitePlayer; 
            this.blackPlayer = blackPlayer;
            whitePlayer.ChangePlayingStatus(true);
        }

      public bool IsPieceBelongsToPlayer(Actor currentPlayer,TileObject currentPiece)
        {
            if(currentPlayer == null) return false;
            if (currentPlayer.IsPlaying)
            {
                //Picked The CorrectObject
                if (currentPlayer.Id == currentPiece.ActorId) return true;
                else return false;
            }
            else
            {
                return false;
                //PlayerIsNotPlaying
            }
        }
        public Actor GetWhoPlaying()
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
        
    }

}
