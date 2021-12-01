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

typedef struct {
	ushort from;
	ushort to;
	PlayerSide color;
} Move;

typedef struct {
	char Length;
	Move Moves[4];
	U64 Hash;
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
	//Length of each set.
	ushort SetLengths[MAX_SETS_LENGTH];
} Game;

// The global game variable
Game G;

void StartPosition(Game* g);
void RollDice(Game* g);
int CountAllCheckers(PlayerSide side, Game* game);
void WriteGameString(char* s, Game* g);
void ReadGameString(char* s, Game* g);
void RemoveShorterSets(int maxSetLength, Game* g);
void CreateMoves(Game* g);

bool DoMove(Move move, Game *g);
void UndoMove(Move move, bool hit, Game *g);
bool IsBlockedFor(ushort pos, ushort color, Game* g);
void PrintGame(Game* game);
void SetPointsLeft(Game* g);



