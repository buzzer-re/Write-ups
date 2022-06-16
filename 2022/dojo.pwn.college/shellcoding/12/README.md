Solve by replacing the 11 solving with push and pops, the first `push rax` rely that rax is always 0 because before call our shellcode, is 
moved 0 to rax in the main function


Compile stage1.s


> $ gcc -static -nostdlib stage1.s -o stage1
> $ objcopy --dump-section .text=shellcode_stage1 stage1

Compile stage2.c

> $ make stage2
