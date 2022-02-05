#pragma once
#include "Game.h"

typedef enum  {
	EnableAlphaBetaPruning = 1
} AiFlags;

typedef struct  {
	int Id;
	int SearchDepth;
	int Score;
	//Negative score for leaving a blot on the board.
	double BlotFactors[26];
	//It is good to connect blocks since in the future the opponent might be blocked by them.
	double ConnectedBlocksFactor[26];

} AiConfig;

AiConfig AIs[2];

#define AI(color) AIs[(color)>>5]

void SetDiceCombinations();
#define DiceCombos 21
// [ComboIndex][DiceValue]
int AllDices[DiceCombos][2];

int EvaluateCheckers(Game* g, PlayerSide color);
int EvaluateCheckers2(Game* g, PlayerSide color);
void InitAi(AiConfig* config, bool constant);
void InitAiManual(AiConfig * config);
int GetScore(Game* g);
void PlayGame(Game* g);
int FindBestMoveSet(Game* g, MoveSet* bestSet, int depth);
void WatchGame();
int RecursiveScore(Game* g, int depth, int best_black, int best_white);

void RemoveShorterSets(int maxSetLength, Game* g);
void CreateMoves(Game* g);
void CreateBlackMoveSets(int fromPos, int diceIdx, int diceCount, int* maxSetLength, Game* g);
void CreateWhiteMoveSets(int fromPos, int diceIdx, int diceCount, int* maxSetLength, Game* g);

bool DoMove(Move move, Game* g);
void UndoMove(Move move, bool hit, Game* g, U64 prevHash);
bool IsBlockedFor(ushort pos, ushort color, Game* g);
void PrintSet(MoveSet set);