#pragma once
#include "Game.h"

struct AiPlayer {
	//Negative score for leaving a blot on the board.
	double BlotFactors[25];

	//It is good to connect blocks since in the future the opponent might be blocked by them.
	double ConnectedBlocksFactor[25];
};

double EvaluateCheckers(Game* g, char color);
void InitAi(bool constant);
double GetScore(Game* g);
void PlayGame(Game* g);
int FindBestMoveSet(Game* g);