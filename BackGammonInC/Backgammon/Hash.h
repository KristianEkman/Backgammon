#pragma once
#include "Utils.h"
#include "GameTypes.h"

#define PosCount 26

typedef enum {
	PV_Set = 1 // Set is stored at a cut off
} EntryType;

typedef struct {
	uint Key;
	ubyte from;
	ubyte to;
} HashEntry;

typedef struct {
	uint EntryCount;
	//set idx: 11bits
	//depth: 3bit
	//type: 2bits
	ushort* Entries;	
} HashTable; 

HashTable H_Table;

// Unique numbers for all actions on the board
// Color, position index and number of checkers in that position.
U64 PositionHash[2][PosCount][15];

U64 GameStartHash;
U64 SidesHash;
U64 DiceHash[2][6];

void InitHashes();

void AllocateHashTable(uint megabytes);

void AddHashEntry(U64 gameHash, ushort setIdx, ubyte depth, ubyte type);

bool ProbeHashTable(U64 gameHash, ushort* setIdx, ubyte depth);
