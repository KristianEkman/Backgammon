#pragma once
#include "Game.h"
#include "Ai.h"

//Number of AIs competing
#define TrainedSetCount 10
//Number of paralell games
#define ThreadCount 8

Game* ThreadGames;

AiConfig TrainedSet[TrainedSetCount];

void LoadTrainedSet();
void SaveTrainedSet();
void InitTrainer();

void Train();

void NewGeneration();