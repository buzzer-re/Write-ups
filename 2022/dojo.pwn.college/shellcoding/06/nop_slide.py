import sys

if __name__ == '__main__':
    if len(sys.argv) < 2:
        print(f"{sys.argv[0]} <shellcode>")
        sys.exit(1)

    print("Generating NOP slide...")
    shellcode_file = sys.argv[1]
    NOP = b'\x90' * 4096
    shellcode = b''
    with open(shellcode_file, 'rb') as shell_fd:
        shellcode = shell_fd.read()

    if shellcode:
        with open(f'{shellcode_file}.slide', 'wb') as shell_fd:
            shell_fd.write(NOP + shellcode)

    print("Written!")
