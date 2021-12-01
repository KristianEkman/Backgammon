#pragma once
#include "Game.h"

//Negative score for leaving a blot on the board.
double BlotFactors[2][26];

//It is good to connect blocks since in the future the opponent might be blocked by them.
double ConnectedBlocksFactor[2][26];

double EvaluateCheckers(Game* g, char color);
void InitAi(bool constant);
double GetScore(Game* g);
void PlayGame(Game* g);
int FindBestMoveSet(Game* g);
void AutoPlay();