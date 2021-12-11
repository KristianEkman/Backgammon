#pragma once
#include "Utils.h"
//
//#define White 32
//#define Black 16
#define ushort unsigned short
#define uint unsigned int
#define BUF_SIZE 5000
#define MAX_SETS_LENGTH 1000 // This value is found by running many games and check that it never goes over.

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
	//Unique hash for the game state
	U64 Hash;
} Game;

// The global game variable
Game G;

GameConfig G_Config;

// Switch off Check Count == 15 for some tests
bool CheckerCountAssert;

void StartPosition(Game* g);
void RollDice(Game* g);
int CountAllCheckers(PlayerSide side, Game* game);
void WriteGameString(char* s, Game* g);
void ReadGameString(char* s, Game* g);

void PrintGame(Game* game);
void SetPointsLeft(Game* g);
void InitGameConfig();
