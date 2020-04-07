# CaaS - Compile as a Service (100 points)


# Main porpuse

Caas was a simple text editor with the button ```compile```, then the code is going to be sent to a server that will same the file and run ```gcc``` in it.


# Solution

It was no so difficult at all, i lose time trying to do some kind of RCE inside of it, but then i realized that i can try DoS the server with the ```#include``` (Sorry fireshell, i literally crash your server for a few minutes :D)


### Possible payloads

```#include <../../../../../../dev/zero>```

This will make our compiler look for that file, but that file never ends :) the server crashed trying to read it all (Yep, this is a DoS kind attack) then, i tried to load our flag an force our compiler to show the errors


```#include <../../../../flag>```

Then in the error output, the flag will be there :)

Flag: F#{D1d_y0u_1nclud3_th3_fl4g?}