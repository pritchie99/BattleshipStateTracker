# BattleshipStateTracker 
A Battleship State Tracker demo application with tests

## Overview

This project comprises a Battleship state-tracker for a single player, and a series of unit test methods which support the development and maintenance of required functionality.

The main Battleship State Tracker is implemented as a .NET Standard class library incorporating 2 main classes:-
* a small CellState class used to store the state of each cell, and 
* the state tracker itself, which at it's core stores the state of the game in a "map" implemented as a sparse, 2-dimensional array of CellState objects.

The task is to implement a Battleship state-tracker for a single player that must support the following logic:-
* Create a board,
* Add a battleship to the board,
* Take an “attack” at a given position, and report back whether the attack resulted in a hit or a miss,
* Return whether the player has lost the game yet (i.e. all battleships are sunk).

## Requirements
This implementation includes a number of features to support the requested attributes as follows.

**Readability**
* Methods and variables are named to reflect either the verb that they execute eg "Place", or the noun that they represent eg "_map".
* Convention is followed such as prepending private class fields with "_".
* Complex structures which degrade readibility are cached into a reusable variable - eg "Enumerable.Range(1, 10)" is cached as "_validCoordinateRange".

**Maintainability**
* Code is commented with the goal of documenting the _intention_ rather than the implementation.  Coding errors are more easily identified rather than if the comment only specifies what the code does, and future changes are more likely to continue to implement intended functionality.
* Alternatively if intended functionality is to change, so should the comments outlining the intention and this will also improve the uptake of the incoming maintainer of the code.
* Methods are written to do one thing thereby reducing the chances or use of confusing side-effects which can reduce the success of future maintenance coding.

**Testability**
* All public methods in the tracker class raise exceptions which make tests to check for error conditions easier to write.
* Public/internal methods and their parameters are named so that their intended function is clear, eg AddShip(ship, alignment, cellCoordinates).

**Extensibility**
* Future extensibility will depend on the requirement at the time.  This may involve splitting the state from the state configuration methods by use of a helper, extension, or abstract class.  It is a fine line whether implementing this sort of structure here for an unknown feature request might offset the Readability requirement above.

