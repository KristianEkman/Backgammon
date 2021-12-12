#pragma once
#include "Game.h"

typedef enum  {
	EnableAlphaBetaPruning = 1
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

int EvaluateCheckers(Game* g, PlayerSide color);
void InitAi(bool constant);
int GetScore(Game* g);
void PlayGame(Game* g, bool pausePlay);
int FindBestMoveSet(Game* g, MoveSet* bestSet, int depth);
void AutoPlay();
int RecursiveScore(Game* g, int depth, int best_black, int best_white);

void RemoveShorterSets(int maxSetLength, Game* g);
void CreateMoves(Game* g);
void CreateBlackMoveSets(int fromPos, int diceIdx, int diceCount, int* maxSetLength, Game* g);
void CreateWhiteMoveSets(int fromPos, int diceIdx, int diceCount, int* maxSetLength, Game* g);

bool DoMove(Move move, Game* g);
void UndoMove(Move move, bool hit, Game* g, U64 prevHash);
bool IsBlockedFor(ushort pos, ushort color, Game* g);