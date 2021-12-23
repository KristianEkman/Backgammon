#pragma once
#include "Game.h"
#include "Ai.h"

//Number of AIs competing
#define TrainedSetCount 8

//Number of paralell games
#define ThreadCount 8

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