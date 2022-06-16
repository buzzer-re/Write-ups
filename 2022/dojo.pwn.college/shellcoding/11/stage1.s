.global _start
.intel_syntax noprefix

_start:
  xor rsi, rsi
  xor rdx, rdx
  mov al, 59
  lea rdi, [rip + filename]
  syscall

filename:
  .ascii "stage2"
