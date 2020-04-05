# Natas 03


The idea here is not so different from the Natas 02, there is nothing in the fron page, take a look at the source and will see the following message

***\<!-- No more information leaks!! Not even Google will find it this time... -->***

It's easy to get that, Google, Yahoo, Bing, Yandex... all the search engines use Bots to find new pages in the web, but they ***MUST*** follow some rules, imagine that you have an website and you don't want the /login page be indexed by google, you can specify that in a file called [robots.txt](https://en.wikipedia.org/wiki/Robots_exclusion_standard) within this file you probably can see not allowed pages, now go to /robots.txt file


```
User-agent: *
Disallow: /s3cr3t/
```


The syntax it's very easy to undestand, Disallow any [user agent](https://en.wikipedia.org/wiki/User_agent) to access /s3cr3t/ path, this rule will **NOT** block your browser, but when a legal Bot read that file, he must accept that and let /s3cr3t path away from the search, but we will not leave that :) 


flag is in user.txt file inside that path

Flag is: Z9tkRkWmpt9Qr7XrR5jWRkgOU901swEZ
