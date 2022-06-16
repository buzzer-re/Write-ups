Final chall

We just have 6 bytes to use, so in order to win this chall we must reuse existing registers!

rax => Have 0, so we already have access to the read syscall 
rdx => Have the shellcode address, so we can use this to write the stage1 


push rax ; push 0 to stack
pop rdi ; pop 0 in rdi, literally we just use 2 bytes 
push rdx ; push the shellcode address
push rsi ; pop in rsi, we used 4 bytes so far
syscall ; 2 bytes, great! we used 6 bytes only


The stage2 starts with 6 nops (but can be anything actually) just to fill the first 6 bytes, and then comes the first shellcode to get the flag

Amazing :)

