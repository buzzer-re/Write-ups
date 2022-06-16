.global _start
.intel_syntax noprefix

_start:
        push rax
        pop rdi
        push rdx
        pop rsi
        syscall

