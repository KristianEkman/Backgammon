#pragma once
#include "Game.h"

//Negative score for leaving a blot on the board.
double BlotFactors[2][26];

//It is good to connect blocks since in the future the opponent might be blocked by them.
double ConnectedBlocksFactor[2][26];

double EvaluateCheckers(int g, char color);
void InitAi(bool constant);
double GetScore(int g);
void PlayGame(int g);
int FindBestMoveSet(int g);
void AutoPlay();