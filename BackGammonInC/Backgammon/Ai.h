#pragma once
#include "Game.h"

//Negative score for leaving a blot on the board.
double BlotFactors[2][26];

//It is good to connect blocks since in the future the opponent might be blocked by them.
double ConnectedBlocksFactor[2][26];

#define DiceCombos 21
int AllDices[DiceCombos][2];

double EvaluateCheckers(Game* g, PlayerSide color);
void InitAi(bool constant);
double GetScore(Game* g);
void PlayGame(Game* g);
MoveSet FindBestMoveSet(Game* g, double* bestScoreOut, int depth);
void AutoPlay();