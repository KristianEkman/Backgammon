// source from https://github.com/ESultanik/mtwister.git
#pragma once
#include "Utils.h"


#define STATE_VECTOR_LENGTH 624
#define STATE_VECTOR_M      397 /* changes to STATE_VECTOR_LENGTH also require changes to this */

typedef struct tagMTRand {
  unsigned long mt[STATE_VECTOR_LENGTH];
  int index;
} MTRand;

MTRand g_Rand;

void seedRand(unsigned long seed, MTRand* rand);
unsigned long genRandLong(MTRand* rand);
double genRand(MTRand* rand);
unsigned long genDice(MTRand* rand);

int RandomInt(MTRand* rand, int lower, int upper);

double RandomDouble(MTRand* rand, double lower, double upper);

U64 Random64(MTRand* rand);
