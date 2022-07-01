This challenge uses `seccomp` to block some system calls, but allows us to use `openat`, `write` and `read`. 

So we just need to open the `flag` using `openat` which receives a relative path as a file descriptor, and this will be our first parameter with `/`.

That way we open the `flag` outside the jail because `chroot` does not close any file descriptor!
