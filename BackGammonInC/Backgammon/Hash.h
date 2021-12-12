#pragma once

#define PosCount 26

// Unique numbers for all actions on the board
// Color, position index and number of checkers in that position.
U64 PositionHash[2][PosCount][15];

U64 GameStartHash;

void InitHashes();