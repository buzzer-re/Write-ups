## Level 01


Let's first run this file

```sh
level1@io:/levels$ ./level01
Enter the 3 digit passcode to enter: 111
```
So you need a password for this, this look just like a simple password discovery crackme, just run ***objdump*** against it to get the main function.

```sh
level1@io:/levels$ objdump -DM intel level01 | less
/_start
```
Notice that this binary don't have a ***main*** function and is static linked, you can notice that by looking into functions like ***fscanf*** that are in plain code here.

```
level01:     file format elf32-i386


Disassembly of section .text:

08048080 <_start>:
 8048080:       68 28 91 04 08          push   0x8049128
 8048085:       e8 85 00 00 00          call   804810f <puts>
 804808a:       e8 10 00 00 00          call   804809f <fscanf>
 804808f:       3d 0f 01 00 00          cmp    eax,0x10f
 8048094:       0f 84 42 00 00 00       je     80480dc <YouWin>
 804809a:       e8 64 00 00 00          call   8048103 <exit>
 ```

Yep, good and old ***cmp*** the password is ***0x10f***, if this is saved in ***eax*** register a conditional jump is called (je).

Converting to decimal using bc:

```
level1@io:/levels$ echo "ibase=16; 10F" | bc
271
level1@io:/levels$ ./level01 
Enter the 3 digit passcode to enter: 271
Congrats you found it, now read the password for level2 from /home/level2/.pass

```
XNWFtWKWHhaaXoKI

Grab your level 2 password and continue!
