#include <stdio.h>

const char* flag = "/flag";

int main() {
  FILE* f = fopen(flag, "r");

  if (!f) return 1;

  char flag[1024];
  fread(flag, 1024, 1, f);

  puts(flag);

  fclose(f);

  return 0;
}
