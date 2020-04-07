# URL_TO_PDF (500 Points)


# Main porpuse
It's a simple application that get an HTML page an transforms into a PDF document,

# Solving

The first thing that came up in my mind was [SSRF](https://portswigger.net/web-security/ssrf), i needed to find a way to force the HTML to PDF application pull information from where it cannot be pulled.

Here a list of HTML tags i tried:
* ```<iframe src="file:///flag"/>```
* ```<embed src="file:///flag"/>```
* ```<object width="400" height="400" data="file:///flag"/>```

***I uploaded my HTML in an simple free file hosting and put the url in the input***

### Detecting the PDF creator

I looked at the PDF headers looking for the software that created that (For 1 minute i trought to look for exploits and load inside an HTML page, but i remembered that this was a Web challenge and not a PWN :p)

```
$ pdfinfo c7b920f57e553df2bb68272f61570210.pdf
Title:          Google
Subject:        Search the world's information, including webpages, images, videos and more. Google has many special features to help you find exactly what you're looking for.
Keywords:       
Author:         
Producer:       cairo 1.16.0 (https://cairographics.org)
````

So, its an application that uses Cairo as lib... that doesn't helped at all, so after looking in a lot of SSRF sources, i found a simple solution!

Here is the HTML tag that worked for me:

```<link rel="attachment" href="file:///flag">```

After that, i downloaded the PDF and used ```binwalk -e <pdf>``` to extract any "files" that he may found inside, then i found the flag inside the _<pdf> folder created.

I don't remember the flag itself(i lose the files, sorry), but it's was there :)

