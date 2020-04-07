# Rev (500 Points)

# Main porpuse
Rev is a simple reverse enginner task, we need to undestand the cryptographic function and reverse it. 


# Reversing

The binary provided to us was stripped, so we did't have any debug symbols, to find our main funcion quicly as possible i used Radare2 framework to it, so let's find all symbols possible.


```
$ r2 -A chall
$ fs symbols;f 
$ s main
$ pdf
```

You can jump until the offset 0x00401d2f, its when our loop (aka crypt loop) starts, take a look:

```
  ,=< 0x00401d2f      eb3b           jmp 0x401d6c
|       |      ; JMP XREF from 0x00401d8b (main)
|      .--> 0x00401d31      0fb645e6       movzx eax, byte [local_1ah]
|      :|   0x00401d35      c0e807         shr al, 7 
|      :|   0x00401d38      8845e7         mov byte [local_19h], al
|      :|   0x00401d3b      0fb645e6       movzx eax, byte [local_1ah]
|      :|   0x00401d3f      01c0           add eax, eax # value * 2
|      :|   0x00401d41      8845e6         mov byte [local_1ah], al
|      :|   0x00401d44      0fb645e6       movzx eax, byte [local_1ah]
|      :|   0x00401d48      0a45e7         or al, byte [local_19h]
|      :|   0x00401d4b      8845e6         mov byte [local_1ah], al
|      :|   0x00401d4e      0fb645e6       movzx eax, byte [local_1ah]
|      :|   0x00401d52      f7d0           not eax # ~value
|      :|   0x00401d54      8845e6         mov byte [local_1ah], al
|      :|   0x00401d57      0fb645e6       movzx eax, byte [local_1ah]
|      :|   0x00401d5b      0fb6c0         movzx eax, al
|      :|   0x00401d5e      488b55f0       mov rdx, qword [local_10h]
|      :|   0x00401d62      4889d6         mov rsi, rdx
|      :|   0x00401d65      89c7           mov edi, eax
|      :|   0x00401d67      e8c4fd0100     call fcn.00421b30
|      :|      ; JMP XREF from 0x00401d2f (main)
|      :`-> 0x00401d6c      488d55e6       lea rdx, qword [local_1ah]
|      :    0x00401d70      488b45e8       mov rax, qword [local_18h]
|      :    0x00401d74      488d3527d40a.  lea rsi, qword [0x004af1a2] ; "%c"
|      :    0x00401d7b      4889c7         mov rdi, rax
|      :    0x00401d7e      b800000000     mov eax, 0
|      :    0x00401d83      e898ec0000     call fcn.00410a20           ; fcn.00410960+0xc0
|      :    0x00401d88      83f8ff         cmp eax, 0xff               ; 255
|      `==< 0x00401d8b      75a4           jne 0x401d31
```

Of course i run this binary with breakpoints in that offset with gdb + [gef](https://gef.readthedocs.io/en/master/) to see what was really happening, the important process was, at address ```0x00401d3b``` we get the index of some thing (guess what ? our input) the next address we sum with itself ```(s[i] * 2)``` and at address ```0x00401d52``` we make a not operation ```( ~(s[i]*2) )```.


### Rewriting

After strugle to understand and write a equivalent C code, i came up with this:

```C
#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>




int main(int argc, char** argv)
{
	
	if (argc < 3) {
		fprintf(stderr, "%s <encrypt_text> <size>\n", argv[0]);
		return 1;
	}

	const char* in = argv[1];
	int size = atoi(argv[2]);

	unsigned char enc;

	for (ssize_t i = 0; i < size; ++i) {
	
		enc = ~(in[i] * 2);	

		printf("0x%x ", enc);
	}


	return 0;
 }

```

Lets test that

```
$ echo 'Cool text here' > test_file
$ ./chall test_file test_file.enc
$ ./encrypt "$(cat test_file) 15 # We need pass the length, just get that with wc -c
0x79 0x21 0x21 0x27 0xbf 0x17 0x35 0xf 0x17 0xbf 0x2f 0x35 0x1b 0x35 0xff
$ xxd test_file.enc
00000000: 7921 2127 bf17 350f 17bf 2f35 1b35 eb    y!!'..5.../5.5.
```

Yep, feels good to me (there is a garbage in my output, but i just ignored it :D)


# Decrypting

Now its easy, we just need to make the reverse operation to create a decryption code!

Change the loop of encryption:
```C
unsigned char dec;
for (ssize_t i = 0; i < size; ++i) {
	dec = ~( in[i] );
	dec /= 2;
	printf("%c", dec);
}
```

```
./decrypt "$(cat flag.enc)" 129
Congratulations!

I hope you liked this small challenge.

The flag you are looking for is F#{S1mpl3_encr1pt10n_f0und_0n_g1thub!}
```


That's it