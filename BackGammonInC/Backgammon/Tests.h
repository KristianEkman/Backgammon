#pragma once

void RunAll();

#define TEST(name, testbody) void name() { printf("\nRunning %s took ", ""#name""); clock_t start = clock(); testbody printf("%fms", (float)(clock() - start)  * 1000 / CLOCKS_PER_SEC); }
