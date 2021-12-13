#pragma once
#include "Utils.h"
#define MAX_SETS_LENGTH 1500 // This value is found by running many games and check that it never goes over.

typedef enum {
	Black = 16,
	White = 32
} PlayerSide;

typedef enum {
	// When autoplaying, game is printed and paused until return key is pressed after eache move.
	EnablePlayPause = 1,
	//Double dice generates four moves as in a standard game.
	EnableQuads = 2
} GameFlags;

typedef struct {
	GameFlags Flags;
	ubyte ThreadsCount;
} GameConfig;

typedef struct {
	ubyte from;
	ubyte to;
	PlayerSide color;
} Move;

typedef struct {
	ubyte Length;
	Move Moves[4];
	// Unique hash for for the set, independent of order of moves.
	U64 Hash;
	bool Duplicate;
	int score;
} MoveSet;

typedef struct {
	PlayerSide CurrentPlayer;
	short Dice[2];
	// 11 1111, count & 15, color & 15
	// 0 Black Bar, 1 - 24 common, 25 White Bar
	short Position[26];
	//Nmbr of checkers in white home
	short WhiteHome;
	//Nmbr of checkers in black home
	short BlackHome;

	ushort BlackLeft;
	ushort WhiteLeft;

	//List of generated possible sets of moves.
	MoveSet PossibleMoveSets[MAX_SETS_LENGTH];
	//Length of the list
	ushort MoveSetsCount;
	//Number of Evaluations in a search
	uint EvalCounts;
	//Well enough unique hash for the placement of checkers and side to move.
	U64 Hash;
	//The dice also defines the state of the game, since they decide valid move sets.
	U64 DiceHash;
} Game;