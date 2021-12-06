#pragma once

//#define false 0
//#define true 1
#define max(x, y) (((x) > (y)) ? (x) : (y))
#define min(x, y) (((x) < (y)) ? (x) : (y))

#define CheckerCount(x) ((x) & 15)
#define OtherColor(x) (~(x) & 48)


typedef enum { false, true } bool;
typedef unsigned long long U64;
typedef unsigned char ubyte;

U64 Llrand();

U64 LlrandShift();

int RandomInt(int lower, int upper);
double RandomDouble(double lower, double upper);

bool Streq(char s1[], char s2[]);

bool StartsWith(char a[], char b[]);

void Stdout_wl(char* text);

bool Contains(char a[], char b[]);

int IndexOf(char * a, char * b);

void ConsoleWriteLine(char* text);

void SubString(char s[], char sub[], int start, int length);

void SetCursorPosition(int x, int y);

void FailAssert();

#ifdef _DEBUG
	#define ASSERT_DBG(condition) if (!(condition)) FailAssert();
#else
	#define ASSERT_DBG(condition) // does nothing
#endif // _DEBUG

#define VERIFY(condition) if (!(condition)) {printf("\n Condition (%s) failed verification in function %s\n", ""#condition"", __func__); exit(0);}

typedef enum {
	black,
	blue,
	green,
	marine,
	red,
	purple,
	lightbrown,
	lightgray,
	gray,
	lightblue,
	lightgreen,
	seagreen,
	orange,
	lightpurple,
	yellow,
	white,
} ConsoleColor;

void PrintRed(char* msg);

void PrintGreen(char* msg);
void PrintBlue(char* msg);
void PrintYellow(char* msg);

void PrintInverted(char* msg);

void ColorPrint(char* text, ConsoleColor textColor, ConsoleColor background);
