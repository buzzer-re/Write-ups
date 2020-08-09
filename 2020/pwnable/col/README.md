# Col


Col it's the second challenge, it's explore collision in hash implementations.

Code:

```c
#include <stdio.h>
#include <string.h>
unsigned long hashcode = 0x21DD09EC;
unsigned long check_password(const char* p){
	int* ip = (int*)p;
	int i;
	int res=0;
	for(i=0; i<5; i++){
		res += ip[i];
	}
	return res;
}

int main(int argc, char* argv[]){
	if(argc<2){
		printf("usage : %s [passcode]\n", argv[0]);
		return 0;
	}
	if(strlen(argv[1]) != 20){
		printf("passcode length should be 20 bytes\n");
		return 0;
	}

	if(hashcode == check_password( argv[1] )){
		system("/bin/cat flag");
		return 0;
	}
	else
		printf("wrong passcode.\n");
	return 0;
}
```

We need to provide a passcode that his hash matches ***0x21DD09EC***, the hash function it's a pretty simple one, it take 5 blocks of 4 bytes each and sum everything.

```c
int* ip = (int*)p;
...
res += i[i]
```

As the passcode is passed as a ***char**** and it's created a integer pointer, our pointer will get the size of an integer (4 bytes) of our string, so if we have something like AABB, our integer will be ***0x41414242***, so we just need to create 5 blocks that his sum give us ***0x21DD09EC***.


## Collision

Here is a handcrafted collision passcode:


* ***0xCF052F08*** - First block

* ***0x0F010F0B*** - Second block

* ***0x01011F0B*** - Third block

* ***0x0B010101*** - Fourth block

* ***0x02017F02*** - Fifth block

I just brute-forced manually until find a sum that matches the hash code.



## Beating

```
col@pwnable:~$ ./col $(echo -en '\xCF\x5\x2F\x8\xF\x1\xF\xB\x1\x1\x1F\xB\xB\x1\x1\x1\x2\x1\x7F\x2')
daddy! I just managed to create a hash collision :)
```

## Automatic collision algorithm

I decided to create an algorithm to beat this challenge by getting all integers between 1-255, getting all 4 blocks combination possible and getting the sum of each block until hit a collision:


```python
import itertools

hashcode = b'\x21\xDD\x09\xEC'



badnumbers = [0xA, 0x9, 0x0]
numbers = [x for x in range(1, 0xFF+1) if x not in badnumbers]


collision = [0] * 20


for n, h in enumerate(hashcode):
	
	if h < 10:
		list_numbers = [x for x in [i for i in range(0,9) if i not in badnumbers]*5]
	else:
		list_numbers = numbers[:h-1]		

	block_combination = itertools.combinations(list_numbers, 5)
	
	print("Finding collisions to block {}...".format(hex(h)))
	for i in block_combination:

		if sum(i) == h:
			collision[n]  =	i[0]
			collision[n+4]  =	i[1]
			collision[n+8]  =	i[2]
			collision[n+12] =   i[3]
			collision[n+16]  = 	i[4]
			print("Found {}\n".format([hex(x) for x in i]))
			break



collision.reverse() # Little endian



echo_fmt = "".join([hex(i).replace("0","\\") for i in collision])

print("./col $(echo -en '{}')".format(echo_fmt)))
```

### Beating with script

```
col@pwnable:~$ ./col $(echo -en '\xe2\x2\xd3\x17\x4\x1\x4\x4\x3\x3\x3\x3\x2\x2\x2\x2\x1\x1\x1\x1')
daddy! I just managed to create a hash collision :)
```









