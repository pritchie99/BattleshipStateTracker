using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
        private IEnumerable<int> _coordRange = Enumerable.Range(1, 10);
        private CellState[,] _map = new CellState[10, 10];
        private List<ShipType> _ships = new List<ShipType>(5);

        public BattleshipStateTracker()
        {
        }

        public void Init(ShipType ship, Alignment direction, string cellCoords)
        {
            if(ship == ShipType.None)
                throw new Exception($"Cannot place a ShipType of None on the map using method Init");

            if (_ships.Contains(ship))
                throw new Exception($"{ship.ToString()} has already been placed on the map.");

            SplitCoordinates(cellCoords, out int xCoord, out int yCoord);
            PlaceShip(ship, direction, xCoord, yCoord);
            _ships.Add(ship);
        }

        private void PlaceShip(ShipType ship, Alignment alignment, int shipStartX, int shipStartY)
        {
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
            if (!_coordRange.Contains(shipStartX) || 
                !_coordRange.Contains(shipStartY) || 
                !_coordRange.Contains(shipEndX) || 
                !_coordRange.Contains(shipEndY))
            {
                throw new Exception("Attempting to place part of ship outside the map.");
            }

            for(int x = shipStartX; x <= shipEndX; x++)
            {
                for (int y = shipStartY; y <= shipEndY; y++)
                {
                    if(_map[x,y] != null && _map[x,y].Ship != ShipType.None)
                    {
                        throw new Exception($"Attempting to place ship over another ship ({ _map[x, y].Ship.ToString()})");
                    }
                }
            }
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


        private void SplitCoordinates(string cellCoords, out int xCoord, out int yCoord)
        {
            try
            {
                yCoord = int.Parse(cellCoords.Substring(1));
                char xCoordChar = cellCoords.ToUpper()[0];
                xCoord = xCoordChar - 'A' + 1;
                if(_coordRange.Contains(xCoord) && _coordRange.Contains(yCoord))
                {
                    return;
                }
            }
            catch
            {
                throw new Exception("Invalid Cell coordinate string.  Cell Coordinates must be specified as a 2-3 character string starting with A-J followed by a number 1-10 eg 'B9'");
            }
        }

        private string CoordinatesAsString(int xCoord, int yCoord)
        {
            if (_coordRange.Contains(xCoord) && _coordRange.Contains(xCoord))
            {
                return string.Format("{0}{1}", 'A' + xCoord - 1, yCoord);
            }
            throw new Exception("Internal error - a coordinate was outside the range 1-10");
        }
    }
}
