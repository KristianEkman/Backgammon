#pragma once
#include "Game.h"
#include "Ai.h"

//Number of AIs competing
#define TrainedSetCount 10
#define ThreadCount 8

Game* ThreadGames;

AiConfig TrainedSet[TrainedSetCount];

void Train();