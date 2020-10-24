import re
import sys
import os
from decode_dlit import get_decode_table

SCRIPT_PATH = './script.vbs'



def deobfuscate_content(content, decode_table):
    index_rule = r'Number\(\" \d+ \"\)'
    searcher = re.compile(index_rule)

    number_table = re.finditer(index_rule, content)
    decode_table_len = len(decode_table)
    tokens = {}

    # Find and classify tokens
    for number in number_table:
        real_number = int(number.group().split("\"")[1])
        
        if real_number != 0:
            real_number -= 1
        
        variable_name = content[number.start()-14:number.start()-3]
        
        if real_number >= decode_table_len:
            tokens[variable_name] = {
                'real_value': '',
                'number': str(real_number+1),
                'is_encoded': False
            }
            continue

        real_value = decode_table[real_number]
        tokens[variable_name] = {
            'real_value': real_value.decode(),
            'number': str(real_number+1),
            'is_encoded': True
        }

    # Replace the values
    for token, value in tokens.items():
        if value['is_encoded']: # If has a value in decode_table
            replace_var = "$os[{}]".format(token)
            content = content.replace(replace_var, '\"{}\"'.format(value['real_value']))
        
        content = content.replace(token, value['number'])




    return content        
    


if __name__ == '__main__':
    script_content = None
    path = sys.argv[1]
    path_save = sys.argv[2]
    if not os.path.exists(path):
        print("Unable to find {}".format(path))

    with open(path, "r") as script_fd:
        script_content = script_fd.read()
    
    decode_table = get_decode_table()

    print("Starting...")
    deobfuscated = deobfuscate_content(script_content, decode_table)


    print("Saving...")
    with open("{}.new.vbs".format(path_save), "w") as script_fd:
        script_fd.write(deobfuscated)

