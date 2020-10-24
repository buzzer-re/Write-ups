import sys
# Flareon2020 - Report stage 2 decryptor

import binascii

def decrypt_stage(stage2, pos, key, length):
	decrypted = []
	len_key = len(key)
	key_count = 0

	for i in range(0, len(stage2), 4):
		byte_at = int(stage2[i+pos:i+2+pos], base=16)
		b = byte_at ^ key[key_count % len_key]
		
		decrypted.append(b)
		key_count += 1

		if key_count == length:
			break

	return bytes(decrypted)
 

if __name__ == '__main__':
	key = b"\x11\x22\x33\x44\x55\x66\x77\x88\x99\xAA\xBB\xCC\xDD\xEE"

	input_file = sys.argv[1]

	stage2_encrypted = open(input_file, "r").read().strip()
	size = 	168667
	decrypted_stage2 = decrypt_stage(stage2_encrypted, 0, key, size)
	
		
	with open("stage2.dec", "wb") as sfd:
		print(decrypted_stage2[:4])
		sfd.write(decrypted_stage2)
