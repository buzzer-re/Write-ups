# 07


In this one, we are able to call the `chroot` syscall itself, so in order to escape we can create a directory inside the jail, chroot to it and `chdir` to outside the main root.

The manual talks about that:

> man 2 chroot

```
This  call  does not change the current working directory, so that after the call '.' can be outside the tree rooted at '/'.
       In particular, the superuser can escape from a "chroot jail" by doing:

           mkdir foo; chroot foo; cd ..

```

- Call mkdir syscall
- Call chroot syscall
- Call chdir syscall with "../../../../../"
- Read the flag
