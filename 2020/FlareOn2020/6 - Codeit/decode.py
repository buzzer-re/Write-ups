import sys
import hashlib

sprite_content = open("sprite.bmp", "rb").read()
hostname = [ord(x) for x in sys.argv[1].lower()]

sprite_content = sprite_content[54:] # Remove header

out = ""
count = 0
extracted = ""


flag = []
for i in range(len(hostname)):
	number_at = hostname[i]
	real_value = 0
	print("Picking {} ".format(number_at))
	for j in range(6, -1, -1):
		f = (sprite_content[count] & 1) << j
		print("({} & 1) << {} = {}".format(sprite_content[count], j, f))
		number_at += f
		real_value += f
		count += 1
	
	flag.append(chr(real_value))
	print("Result {}".format(number_at))

		
	sum_lsb = (number_at >> 1) + ((number_at & 1) << 7)
	print("Appling ({0} >> 1) + (({0} & 1)) << 7) = {1}".format(number_at, sum_lsb))
	out += chr(sum_lsb)



print(''.join(flag))
print(out)
print(' '.join([ hex(ord(o)) for o in out ]))
print(len(out))
