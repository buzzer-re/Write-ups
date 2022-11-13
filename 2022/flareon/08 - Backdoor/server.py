from dnslib.server import DNSServer, DNSRecord, RR, QTYPE
from dnslib.dns import A

reply_list = [
    # 2
    "128.0.0.2",
    "43.50.0.0",
    # 10
    "128.0.0.3",
    "43.49.48.0",
    #8
    "128.0.0.2",
    "43.56.0.0",
    #19
    "128.0.0.3",
    "43.49.57.0",
    #11
    "128.0.0.3",
    "43.49.49.0",
    #1
    "128.0.0.2",
    "43.49.0.0",
    #15
    "128.0.0.3",
    "43.49.53.0",
    #13
    "128.0.0.3",
    "43.49.51.0",
    #22
    "128.0.0.3",
    "43.50.50.0",
    #16
    "128.0.0.3",
    "43.49.54.0",
    #5
    "128.0.0.2",
    "43.53.0.0",
    #12
    "128.0.0.3",
    "43.49.50.0",
    #21    
    "128.0.0.3",
    "43.50.49.0",
    #3
    "128.0.0.2",
    "43.51.0.0",
    #18
    "128.0.0.3",
    "43.49.56.0",
    #17
    "128.0.0.3",
    "43.49.55.0",
    #20
    "128.0.0.3",
    "43.50.48.0",
    #14
    "128.0.0.3",
    "43.49.52.0",
    #9
    "128.0.0.2",
    "43.57.0.0",
    #7
    "128.0.0.2",
    "43.55.0.0",
    #4
    "128.0.0.2",
    "43.52.0.0",
]

class C2Handler(object):
    def resolve(self,request: DNSRecord ,handler):
        reply = request.reply()
        if "flare-on" in request.__str__():
            print(reply_list)
            reply.add_answer(RR("abc.com",QTYPE.A,rdata=A(reply_list.pop(0)),ttl=60))
            print("Sending bot data")

        else:
            reply.add_answer(RR("abc.com",QTYPE.A,rdata=A("127.0.0.1"),ttl=60))

        return reply


s = DNSServer(C2Handler(), address="127.0.0.1", port=53)
s.start()

