global _start

%define SYS_open 2
%define SYS_write 1
%define SYS_read 0
%define SYS_openat 257

_start:
    mov rbp, rsp
    sub rsp, 100
    mov r11, 0
loop:
    mov byte[rsp + r11], 0
    inc r11
    cmp r11, 100
    jne loop

    mov rax, SYS_openat
    mov rdi, 3
    lea rsi, [rel filename]
    xor rdx, rdx
    mov r10, 1
    syscall

    mov rdi, rax
    mov rax, SYS_read
    lea rsi, [rsp]
    mov rdx, 100
    syscall

    mov rax, SYS_write
    lea rsi, [rsp]
    mov rdi, 1
    mov rdx, 100
    syscall

filename:
    db "flag", 0
