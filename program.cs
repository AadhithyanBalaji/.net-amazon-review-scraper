 public static StringBuilder csvbuilder = new StringBuilder();
        public static int pagepending = 0;
        static void Main(string[] args)
        {
           
            try
            {
                Stopwatch s = new Stopwatch();
                s.Start();
                var client = new WebClient();
                client.UseDefaultCredentials = true; // If you are using a proxy
                string result = client.DownloadString("https://www.amazon.in/product-reviews/B072LPF91D");
                result.Replace('\x00', ' ');
                HtmlDocument reviewoverview = new HtmlDocument();
                reviewoverview.LoadHtml(result);
                var reviewpagebuttonlist = reviewoverview.DocumentNode.SelectNodes("//li[@class='page-button']");
                var reviewpagecountchar = reviewpagebuttonlist[reviewpagebuttonlist.Count - 1].InnerText;
                reviewpagecountchar = reviewpagecountchar.Replace(",", string.Empty);
                var reviewpagecount = Convert.ToInt32(reviewpagecountchar);
                var reviewcount = new List<string>();


                //for(int i=0; i< reviewpagecount; i++)
                //{

                //    Thread thread = new Thread(() => Scraper.ScrapePage(i));
                //    thread.Name = "Thread - " + i;
                //    thread.Start();
                //}
                for (int i = 0; i < reviewpagecount; i += 51)
                {
                    Parallel.For(i, i + 50, index => Scraper.ScrapePage(index));
                }
            }
            catch (Exception e)
            {
                
            }
            
        }
        public static class Scraper
        {
            public static void ScrapePage(int i)
            {
                var client = new WebClient();
                client.UseDefaultCredentials = true;
                try
                {
                    pagepending++;
                    var reviewpagestring = client.DownloadString("https://www.amazon.in/product-reviews/B072LPF91D/ref=cm_cr_getr_d_paging_btm_2?showViewpoints=1&pageNumber=" + (i + 1));
                    HtmlDocument reviewpage = new HtmlDocument();
                    reviewpage.LoadHtml(reviewpagestring);
                    var reviewspans = reviewpage.DocumentNode.SelectNodes("//span[@class='a-size-base review-text']");
                    var reviewusername = reviewpage.DocumentNode.SelectNodes("//span[@class='a-profile-name']");
                    for (int j = 0; reviewspans != null && j < reviewspans.Count; j++)
                    {
                        if (reviewspans[j] != null && reviewspans[j].InnerText != null)
                        {
                            //Console.WriteLine("\r\n User : "+ reviewusername[j].InnerText+"\n Comment:"+ reviewspans[j].InnerText);
                            string cleanedreview = reviewspans[j].InnerText.ToLower()
                                .Replace('\n', ' ')
                                .Replace("\r\n", " ")
                                .Replace("\t", " ")
                                .Replace(","," ");
                            csvbuilder.AppendLine(reviewusername[j].InnerText + "," + cleanedreview);
                        }
                    }
                    var exception = false;
                    var trycount = 1;
                    do
                    {
                        try
                        {
                            File.WriteAllText(@"\review.csv", csvbuilder.ToString());
                        }
                        catch (Exception e)
                        {
                            exception = true;
                        }
                        trycount++;
                    } while (exception && trycount <= 3);


                    pagepending--;
                    Console.Clear();
                    Console.WriteLine("Pages in queue : "+ pagepending);
                    
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception at " + Thread.CurrentThread.Name + " : " + e.InnerException.Message);
                }
            }
        }
