Solve by just reusing chal 08 solution that is already sorted 


Compile stage1.s


> $ gcc -static -nostdlib stage1.s -o stage1
> $ objcopy --dump-section .text=shellcode_stage1 stage1

Compile stage2.c

> $ make stage2
