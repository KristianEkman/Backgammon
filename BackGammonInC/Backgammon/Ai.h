#pragma once
#include "Game.h"

typedef enum  {
	EnableHashing = 1,
	EnableAlphaBetaPruning = 2
} AiFlags;

typedef struct  {
	AiFlags Flags;
	ubyte SearchDepth;
} AiConfig;

AiConfig AIs[2];

#define AI(color) AIs[(color)>>5]

//Negative score for leaving a blot on the board.
double BlotFactors[2][26];
//It is good to connect blocks since in the future the opponent might be blocked by them.
double ConnectedBlocksFactor[2][26];

#define DiceCombos 21
int AllDices[DiceCombos][2];

double EvaluateCheckers(Game* g, PlayerSide color);
void InitAi(bool constant);
double GetScore(Game* g);
void PlayGame(Game* g, bool pausePlay);
int FindBestMoveSet(Game* g, MoveSet* bestSet, int depth);
void AutoPlay();
double RecursiveScore(Game* g, int depth, double best_black, double best_white);