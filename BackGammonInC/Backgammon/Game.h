#pragma once
#include "Utils.h"
//
//#define White 32
//#define Black 16
#define ushort unsigned short
#define uint unsigned int
#define BUF_SIZE 5000
#define MAX_SETS_LENGTH 6000

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
	// Unique hash for for the set, independent of order of moves.
	/*U64 Hash;
	bool Duplicate;*/
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
} Game;

// The global game variable
Game Games[1];

void StartPosition(int g);
void RollDice(int g);
int CountAllCheckers(PlayerSide side, int g);
void WriteGameString(char* s, int g);
void ReadGameString(char* s, int g);
void RemoveShorterSets(int maxSetLength, int g);
void CreateMoves(int g);
void CreateBlackMoveSets(int fromPos, int diceIdx, int diceCount, int* maxSetLength, int g);
void CreateWhiteMoveSets(int fromPos, int diceIdx, int diceCount, int* maxSetLength, int g);

bool DoMove(Move move, int g);
void UndoMove(Move move, bool hit, int g);
bool IsBlockedFor(ushort pos, ushort color, int g);
void PrintGame(int game);
void SetPointsLeft(int g);



