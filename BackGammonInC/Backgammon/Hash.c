#include "Utils.h"
#include "Hash.h"

void InitHashes() {
	for (int i = 0; i < posCount; i++)
		PositionHash[i] = LlrandShift();

	MoveStartHash = LlrandShift();
}