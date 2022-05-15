.global _start
.intel_syntax noprefix

_start:
  xor rsi, rsi
  xor rdx, rdx
  mov al, 59 // execve
  lea rdi, [rip + filename]
  syscall

filename:
  .byte 'f'
