import socket
import os
import sys




class C2:
	def __init__(self, ip, port, sender = True):
		self.sender = sender
		self.ip = ip
		self.port = port
		self.s = socket.socket()
		self.s.bind((self.ip, self.port))
		self.bytes_to_send = b''
	
	def set_host_data(self, data_path):
		if not os.path.exists(data_path):
			raise Exception("Unable to find file {}".format(data_path))
		
		print("Loading {}".format(data_path))
		with open(data_path, "rb") as data_fd:
			self.bytes_to_send = data_fd.read()
		
		print("Data loaded, ready to serve!");


	def listen(self):
		self.s.listen(1)
		print("Wainting connections at {}:{}...".format(self.ip, self.port))

		while True:
			conn, addr = self.s.accept()
			if self.sender:
				print("Received connection, sending bytes...")
				conn.send(self.bytes_to_send)
				print("Bytes sent!, closing...")
			else:
				print("Received connection, waiting data...")
				data = conn.recv(4096)
				save_file = "dump.bin"
				print("Received!, saving to {}".format(save_file))
				with open(save_file, "wb") as save:
					save.write(data)

				print("Saved")


			conn.close()





if __name__ == '__main__':
	port   = int(sys.argv[1])
	file_to_server = sys.argv[2]
	sender = sys.argv[3]
	
	c2 = C2("0.0.0.0", port, sender == "sender")
	c2.set_host_data(file_to_server)

	c2.listen()


