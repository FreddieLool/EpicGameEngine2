﻿using System.Diagnostics;

namespace EpicTileEngine
{
    public class TileActionManager
    {
        // Delegate for validating movement. Returns true if the move is valid.
        public Func<TileObject, Position, Tilemap, bool> ValidateMove { get; set; }

        // Delegate for handling interactions between two tile objects (e.g., combat).
        // Returns true if the moving object can move into the tile with another object.
        public Func<TileObject, TileObject, Tilemap, bool> OnInteract { get; set; }

        // Action to perform after a move is executed, such as updating game state or triggering events.
        public Action<TileObject, Tile> OnMove { get; set; }


        /// <summary>
        /// Attempts to move a TileObject to a new position within the given Tilemap.
        /// </summary>
        /// <param name="mover">The TileObject attempting the move.</param>
        /// <param name="targetPosition">The desired position to move to.</param>
        /// <param name="board">The Tilemap where the move is taking place.</param>
        /// <returns>True if the move is successful, otherwise false.</returns>
        public virtual bool TryMove(TileObject mover, Position targetPosition, Tilemap board)
        {
            Trace.WriteLine($"Attempt to move from {mover.CurrentTile.Position} to {targetPosition}");

            if (!board.IsPositionValid(targetPosition))
            {
                Trace.WriteLine("Move out of bounds.");
                return false;
            }

            Tile targetTile = board.GetTile(targetPosition);

            // Check if the target tile is passable and not occupied by a friendly piece
            if (!targetTile.IsPassable || (targetTile.Occupant != null && targetTile.Occupant.ActorId == mover.ActorId))
            {
                Trace.WriteLine("Move blocked or occupied by a friendly piece.");
                return false;
            }

            // Additional debug information:
            Trace.WriteLine($"Target tile passable: {targetTile.IsPassable}, Occupied by self: {targetTile.Occupant?.ActorId == mover.ActorId}");

            Position currentPos = mover.CurrentTile.Position;
            Position moveVector = new Position(targetPosition.X - currentPos.X, targetPosition.Y - currentPos.Y);
            int moveDistance = Math.Max(Math.Abs(moveVector.X), Math.Abs(moveVector.Y));

            // Check if the move is allowed based on the mover's capabilities
            bool isMoveAllowed = mover.MovementCapabilities.Any(capability =>
                moveVector.X == capability.direction.X * moveDistance &&
                moveVector.Y == capability.direction.Y * moveDistance &&
                moveDistance <= capability.maxSteps);

/*            foreach (var (direction, maxSteps) in mover.MovementCapabilities)
            {
                if (moveVector.X == direction.X * moveDistance && moveVector.Y == direction.Y * moveDistance && moveDistance <= maxSteps)
                {
                    isMoveAllowed = true;
                    break;
                }
            }*/

            Trace.WriteLine($"Is move allowed: {isMoveAllowed}");
            if (!isMoveAllowed)
                return false;

            // Further validate the move using custom logic, if any
            if (ValidateMove != null && !ValidateMove(mover, targetPosition, board))
            {
                Trace.WriteLine("Move failed validation.");
                return false;
            }


            // Determine if this is a passing movement or a landing movement
            bool isPassing = DeterminePassingLogic(mover, targetTile);

            if (isPassing)
            {
                targetTile.TriggerOnPass(mover); // Trigger pass event
            }
            else
            {
                // Handle landing logic
                if (targetTile.Occupant != null)
                {
                    bool interactionResult = OnInteract.Invoke(mover, targetTile.Occupant, board);
                    if (!interactionResult) return false; // Interaction can block movement
                }

                mover.CurrentTile.RemoveOccupant(); // Clear current tile
                targetTile.SetOccupant(mover);      // Set new occupant
                mover.CurrentTile = targetTile;     // Update current tile reference

                OnMove?.Invoke(mover, targetTile);  // Trigger move event
            }

            return true;
        }

        /// <summary>
        /// Determines if a move is considered a passing move based on game logic.
        /// </summary>
        /// <param name="mover">The TileObject attempting to move.</param>
        /// <param name="targetTile">The destination Tile of the move.</param>
        /// <returns>True if the move should be treated as a pass; otherwise, false.</returns>
        protected bool DeterminePassingLogic(TileObject mover, Tile targetTile)
        {
            // Example logic: Check if the mover has a specific capability (canFly, ..)
            // and the target tile is not a landing tile type but a passable type (clouds, water, ..)
            if (mover.CanFly && targetTile.Type == TileType.Cloud)
            {
                return true; // The mover passes over the tile
            }

            // More complex logic might involve checking movement distance, mover state, or other properties
            // Example: Check if the mover is performing a dash that allows bypassing certain tile types
            if (mover.IsDashing && targetTile.Type == TileType.Water)
            {
                return true; // The mover passes over water without stopping
            }

            // Default to false if none of the special conditions for passing are met
            return false;
        }


    }
}