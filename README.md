# .net-amazon-review-scraper
A .NET console application for scraping Amazon reviews parrallely using Multithreading,HtmlAgilityPack

<b> Packages used </b>

- <a href="https://www.nuget.org/packages/HtmlAgilityPack/" target="_blank">HtmlAgilityPack</a>

<b> Approach </b>

1.  Initial request to the first page of review to extract the number of pages
2.  Run a <b> Parrallel.For() </b> on the <b> ScrapePage(pagenum) </b> function

<b>ScrapePage(pagenum):</b>
- fetches the page document using WebClient
- if the webclient request fails due to some reason, the function will retry the same request for 3 times before giving up
- if response is received,extracts the review text using HtmlAgility's SelectNodes
- writes the extracted review to a csv file in append mode

The csv file can then be used to perform text mining/analysis
