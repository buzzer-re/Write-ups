As the challenge leaks a file descriptor that we can choose, we can send the old `/` as argument then use this file descriptor to change directory using the `fchdir` syscall.

Then, we can just use the old read code.

```
mov rax, 81 ; fchdir
mov rdi, 3  ; our provided file is open with fd 3
syscall
```

