#pragma once
#include "Utils.h"
#include "GameTypes.h"
//
//#define White 32
//#define Black 16

#define BUF_SIZE 5000


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
