#pragma once
#include "Utils.h"
//
//#define White 32
//#define Black 16
#define ushort unsigned short
#define uint unsigned int
#define BUF_SIZE 5000
#define MAX_SETS_LENGTH 10000

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
	ushort from;
	ushort to;
	PlayerSide color;
} Move;

typedef struct {
	char Length;
	Move Moves[4];
	// Unique hash for for the set, independent of order of moves.
	U64 Hash;
	bool Duplicate;
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
void RemoveShorterSets(int maxSetLength, Game* g);
void CreateMoves(Game* g, bool doHashing);
void CreateBlackMoveSets(int fromPos, int diceIdx, int diceCount, int* maxSetLength, Game* g, bool doHashing);
void CreateWhiteMoveSets(int fromPos, int diceIdx, int diceCount, int* maxSetLength, Game* g, bool doHashing);

bool DoMove(Move move, Game *g);
void UndoMove(Move move, bool hit, Game *g, U64 prevHash);
bool IsBlockedFor(ushort pos, ushort color, Game* g);
void PrintGame(Game* game);
void SetPointsLeft(Game* g);
void InitGameConfig();
