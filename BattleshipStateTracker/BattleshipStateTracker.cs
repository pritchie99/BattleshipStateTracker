using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FlareHR
{
    public enum Alignment {Vertical, Horizontal};
    public enum ShipType { None = 0, Destroyer = 2, Submarine = 3, Cruiser = 3, Battleship = 4, Carrier = 5};

    public class CellState
    {
        public bool WasFiredAt = false;
        public ShipType Ship = ShipType.None;
    }

    public class BattleshipStateTracker
    {
        // the range of valid integer coordinates is cached for code readability
        private IEnumerable<int> _validCoordinateRange = Enumerable.Range(1, 10);
        private CellState[,] _map = new CellState[10, 10];
        private List<ShipType> _ships = new List<ShipType>(5);

        /// <summary>
        /// Validates and converts the supplied combination of parameters, and updates the internal state variables such as _map and _ships
        /// </summary>
        /// <param name="ship">The ShipType to be added to the map</param>
        /// <param name="alignment">Whether the ship is to be aligned vertically or horizontally</param>
        /// <param name="cellCoordinates">The location on the map of the bottom/left-most cell of the ship</param>
        public void AddShip(ShipType ship, Alignment alignment, string cellCoordinates)
        {
            if (ship == ShipType.None)
            {
                throw new Exception($"Cannot place a ShipType of None on the map using method Init");
            }

            if (_ships.Contains(ship))
            {
                throw new Exception($"{ship.ToString()} has already been placed on the map.");
            }

            // translate the user friendly coordinate to integer coordinates used to accessing the array representation of the map
            SplitCoordinates(cellCoordinates, out int xCoord, out int yCoord);

            // add the ship to the map and to the internal list of placed ships
            PlaceShipOnMap(ship, alignment, xCoord, yCoord);
            _ships.Add(ship);
        }

        /// <summary>
        /// Internal method to implement the placement of the ship on the map.  The opposite end of the ship is calculated, and checks 
        /// are made that all cells of the ship fall within the map, and that none of those cells are occupied by a previously placed ship.
        /// </summary>
        /// <param name="ship">The requesteed ShipType to be added to the map</param>
        /// <param name="alignment">Whether the ship is to be aligned vertically or horizontally</param>
        /// <param name="shipStartX"></param>
        /// <param name="shipStartY"></param>
        private void PlaceShipOnMap(ShipType ship, Alignment alignment, int shipStartX, int shipStartY)
        {
            // First calculate the end coordinates of the ship using it's allignment and known length
            int shipEndX, shipEndY;
            if (alignment == Alignment.Vertical)
            {
                shipEndX = shipStartX;
                shipEndY = shipStartY + (int)ship - 1;
            }
            else
            {
                shipEndX = shipStartX + (int)ship - 1;
                shipEndY = shipStartY;
            }
            if (!_validCoordinateRange.Contains(shipStartX) || 
                !_validCoordinateRange.Contains(shipStartY) || 
                !_validCoordinateRange.Contains(shipEndX) || 
                !_validCoordinateRange.Contains(shipEndY))
            {
                throw new Exception("Attempting to place part of ship outside the map.");
            }

            // iterate across all requested cells to check these are not occupied by another ship
            for(int x = shipStartX; x <= shipEndX; x++)
            {
                for (int y = shipStartY; y <= shipEndY; y++)
                {
                    if(_map[x,y] != null && _map[x,y].Ship != ShipType.None)
                    {
                        // a cell has been found that is already used by a ship so unable to place the requested ship in the location specified
                        throw new Exception($"Attempting to place ship over another ship ({ _map[x, y].Ship.ToString()})");
                    }
                }
            }

            // All requested map cells are available so update the state of each cell with the current ShipType
            for (int x = shipStartX; x <= shipEndX; x++)
            {
                for (int y = shipStartY; y <= shipEndY; y++)
                {
                    if (_map[x, y] == null)
                    {
                        _map[x, y] = new CellState() { Ship = ship, WasFiredAt = false };
                    }
                    else if (_map[x, y].Ship != ShipType.None)
                    {
                        _map[x, y].Ship = ship;
                    }
                }
            }
        }

        /// <summary>
        /// Process a shot request by locating the cell, updating the cell that it has been fired at, and returning whether a 
        /// ship occupying that cell was hit.
        /// </summary>
        /// <param name="cellCoords">The location on the map of the bottom/left-most cell of the ship<</param>
        /// <returns>True if the cell was valid and contained was occupied by a ship.  False if not occupied by a ship, ie a miss.</returns>
        public bool RegisterShot(string cellCoords)
        {
            SplitCoordinates(cellCoords, out int xCoord, out int yCoord);
            var cellState = _map[xCoord, yCoord];
            if (cellState == null)
            {
                _map[xCoord, yCoord] = new CellState() { WasFiredAt = true, Ship = ShipType.None } ;
                return false;
            }

            if(!cellState.WasFiredAt)
            {
                cellState.WasFiredAt = true;
            }

            if (cellState.Ship != ShipType.None)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Internal method to split the user-friendly cell representation (A5) with the internal interger coordinates (1,5)
        /// </summary>
        /// <param name="cellCoords">The location on the map of the bottom/left-most cell of the ship<</param>
        /// <param name="xCoord">The internal integer value for the X value fo the coordinates</param>
        /// <param name="yCoord">The internal integer value for the Y value fo the coordinates</param>
        private void SplitCoordinates(string cellCoords, out int xCoord, out int yCoord)
        {
            try
            {
                yCoord = int.Parse(cellCoords.Substring(1));
                char xCoordChar = cellCoords.ToUpper()[0];
                xCoord = xCoordChar - 'A' + 1;
                if(_validCoordinateRange.Contains(xCoord) && _validCoordinateRange.Contains(yCoord))
                {
                    return;
                }
            }
            catch
            {
                throw new Exception("Invalid Cell coordinate string.  Cell Coordinates must be specified as a 2-3 character string starting with A-J followed by a number 1-10 eg 'B9'");
            }
        }

        /// <summary>
        /// Checks all occupied cells and returns whether they have all been fired at or not.
        /// </summary>
        /// <returns>True if all ships have ben sunk.  False if any occupied cells have not been fired at.</returns>
        public bool AllShipsSunk()
        {
            foreach(var cell in _map)
            {
                if(cell != null)
                {
                    Debug.WriteLine($"{cell.Ship.ToString(),-10}{cell.WasFiredAt}");
                    if (cell.Ship != ShipType.None && cell.WasFiredAt == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Returns the user-friendly cell representation (A5) with the internal interger coordinates (1,5)
        /// </summary>
        /// <param name="xCoord">The internal integer value for the X value fo the coordinates</param>
        /// <param name="yCoord">The internal integer value for the Y value fo the coordinates</param>
        /// <returns>User friendly coordinate representation, eg A5</returns>
        private string CoordinatesAsString(int xCoord, int yCoord)
        {
            if (_validCoordinateRange.Contains(xCoord) && _validCoordinateRange.Contains(xCoord))
            {
                return string.Format("{0}{1}", 'A' + xCoord - 1, yCoord);
            }
            throw new Exception("Internal error - a coordinate was outside the range 1-10");
        }
    }
}
