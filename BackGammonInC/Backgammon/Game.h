#pragma once
#include "Utils.h"
//
//#define White 32
//#define Black 16
#define ushort unsigned short
#define BUF_SIZE 5000
#define MAX_SETS_LENGTH 500


typedef struct {
	ushort from;
	ushort to;
	ushort color;
} Move;

typedef enum {
	Black = 16,
	White = 32
} PlayerSide;

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
	Move PossibleMoveSets[MAX_SETS_LENGTH][4];
	//Length of the list
	ushort MoveSetsCount;
	//Length of each set.
	ushort SetLengths[MAX_SETS_LENGTH];
} Game;

// The global game variable
Game G;

void StartPosition(Game* g);
void RollDice(Game* g);
void WriteGameString(char* s, Game* g);
void ReadGameString(char* s, Game* g);
void RemoveShorterSets(int maxSetLength, Game* g);
void CreateMoves(Game* g);

bool DoMove(Move move, Game *g);
void UndoMove(Move move, bool hit, Game *g);
bool IsBlockedFor(ushort pos, ushort color, Game* g);
void PrintGame(Game* game);
void SetPointsLeft(Game* g);



