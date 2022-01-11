#pragma once
#include "Game.h"
#include "Ai.h"

//Number of AIs competing
#define TrainedSetCount 12

//Number of paralell games
#define ThreadCount 12

typedef struct {
	int Generation;
	AiConfig Set[TrainedSetCount];
} TrainerStruct;

TrainerStruct Trainer;
Game* ThreadGames;

bool LoadTrainedSet(char* name);
void SaveTrainedSet(int generation, char* name);
void InitTrainer();
void Train();
void NewGeneration();
double CompareAIs(AiConfig trained, AiConfig untrained);