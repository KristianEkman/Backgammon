#pragma once
#include "Utils.h"

#define White 32
#define Black 16
#define ushort unsigned short
#define BUF_SIZE 5000
#define MAX_SETS_LENGTH 500


typedef struct {
	ushort from;
	ushort to;
	ushort color;
} Move;

typedef struct {
	char CurrentPlayer;
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

void StartPosition();
void RollDice();
void WriteGameString(char* s);
void ReadGameString(char* s);
void RemoveShorterSets(int maxSetLength);
void CreateMoves(Game* g);

bool DoMove(Move move);
void UndoMove(Move move, bool hit);
bool IsBlockedFor(ushort pos, ushort color);


