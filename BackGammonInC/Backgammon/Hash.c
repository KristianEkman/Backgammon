#include <stdlib.h>
#include "Utils.h"
#include "Hash.h"
#include <Windows.h>

void InitHashes() {
	for (int p = 0; p < 2; p++)
		for (int i = 0; i < PosCount; i++)
			for (int j = 0; j < 15; j++)
				PositionHash[p][i][j] = LlrandShift();

	for (int d = 0; d < 2; d++)
		for (int n = 0; n < 6; n++)
			DiceHash[d][n] = LlrandShift();

	GameStartHash = LlrandShift();
	SidesHash = LlrandShift();
}

void AllocateHashTable(uint megabytes) {
	free(H_Table.Entries);
	H_Table.EntryCount = (megabytes * 0x100000ULL) / sizeof(ushort);
	H_Table.Entries = malloc(H_Table.EntryCount * sizeof(ushort));
	if (H_Table.Entries > 0)
		memset(H_Table.Entries, 0, H_Table.EntryCount * sizeof(ushort));
}

void AddHashEntry(U64 gameHash, ushort setIdx, ubyte depth, ubyte type) {
	ASSERT_DBG(setIdx < 2047); //Only 11 bits stored
	ASSERT_DBG(depth < 7); //Only 3bits stored
	ASSERT_DBG(type > 0); //Only 2bits stored
	ASSERT_DBG(type < 4); //Only 2bits stored

	uint index = (uint)(gameHash % H_Table.EntryCount);
	ushort currentValue = H_Table.Entries[index];
	if ( currentValue != 0) {
		ubyte type = currentValue & 0x3;
		currentValue = currentValue >> 2;
		ubyte dep = currentValue & 0x7;
		// Values with lower depths are less interesting and not saved.
		if (dep < depth)
			return;
	}

	ushort value = setIdx;
	value = value << 3;	
	value |= depth;
	value = value << 2;
	value |= type;
	H_Table.Entries[index] = value;
}

// Finds the previous best MoveSet index at this game position and depth, or greater depth.
bool ProbeHashTable(U64 gameHash, ushort* setIdx, ubyte depth) {
	ASSERT_DBG(depth < 7); //Only 3bit stored
	int index = gameHash % H_Table.EntryCount;
	ushort value = H_Table.Entries[index];
	
	if (value == 0)
		return false;
	ubyte type = value & 0x3;
	value = value >> 2;
	ubyte dep = value & 0x7;

	if (dep < depth)
		return false;
	value = value >> 3;
	*setIdx = value;
	return true;
}
