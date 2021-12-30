#pragma once
#include "Utils.h"
#include "mtwister.h"
//
//#define White 32
//#define Black 16
#define ushort unsigned short
#define uint unsigned int
// Size of standard input buffer.
#define BUF_SIZE 5000 

// Max number of sets of moves that can be generated.
// This value is found by running many games and check that it never goes over.
#define MAX_SETS_LENGTH 2000 

typedef enum {
	Black = 16,
	White = 32
} PlayerSide;

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
	//How many times the search should switch sides and calculate probability score.
	int SearchDepth;
	//Set to 4 for a normal game. Or 2 for faster training.
	int DiceQuads;
	//Limit for number of Turn changes in a game.
	int MaxTurns;
	// Prints the game and waits for user to press enter between all game changes.
	bool PausePlay;
} GameSettings;

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
	//Unique hash for the game state
	U64 Hash;
	MTRand rand;
	//Counter for turns in a game
	uint Turns;
} Game;

// The global game variable
Game G;

GameSettings Settings;

// Switch off Check Count == 15 for some tests
bool CheckerCountAssert;

void InitSeed(Game* g, int seed);
void StartPosition(Game* g);
void RollDice(Game* g);
int CountAllCheckers(PlayerSide side, Game* game);
void WriteGameString(char* s, Game* g);
void ReadGameString(char* s, Game* g);

void PrintGame(Game* game);
void SetPointsLeft(Game* g);
void InitHashes();
