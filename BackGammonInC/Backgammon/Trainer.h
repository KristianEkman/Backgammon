#pragma once
#include "Game.h"
#include "Ai.h"

//Number of AIs competing
#define TrainedSetCount 10

//Number of paralell games
#define ThreadCount 8

typedef struct {
	int Generation;
	AiConfig Set[TrainedSetCount];
} TrainerStruct;

TrainerStruct Trainer;
Game* ThreadGames;

bool LoadTrainedSet();
void SaveTrainedSet(int generation);
void InitTrainer();
void Train();
void NewGeneration();