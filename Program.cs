using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PlaintextWikipedia;
using Lucene.Net;
using Lucene;
using Lucene.Net.Analysis.Snowball;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;
namespace wikiProcessor
{

    internal class Program
    {
      static public FSDirectory WorkingDir= FSDirectory.Open(new DirectoryInfo(@"E:/index"));
        private static void Main(string[] args)
        {
            WikipediaArticle w = new WikipediaArticle();

            // var result = Tools.Wiki.WikiProvider.ConvertToHtml(sourceText);
            var wikiarticles =
                WikipediaArticle.ReadArticlesFromXmlDump(@"enwiki-20160204-pages-articles.xml");

            IndexSearcher searcher = new IndexSearcher(WorkingDir, true);
            long i=searcher.MaxDoc;
            searcher.Dispose();
            StandardAnalyzer snowballAnalyzer= new StandardAnalyzer(Version.LUCENE_30);

            using (
                IndexWriter iw = new IndexWriter(WorkingDir, snowballAnalyzer, true,
                    IndexWriter.MaxFieldLength.UNLIMITED))
            {
                Console.WriteLine();
                
                foreach (WikipediaArticle item in wikiarticles)
                {
                   
                    Document dc= new Document();
                    dc.Add(new Field("id",i.ToString(),Field.Store.YES,Field.Index.NO,Field.TermVector.NO));
                    dc.Add(new Field("title",item.Title,Field.Store.YES,Field.Index.ANALYZED,Field.TermVector.WITH_POSITIONS_OFFSETS));
                    dc.Add(new Field("text", item.Text, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
                    dc.Add(new Field("plainText", item.Plaintext, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
                   
                    iw.AddDocument(dc);
                   Console.WriteLine(i);
                    i++;

                    if (i==1000000)
                    {
                        
                  
                        Console.WriteLine("optimization start -" + DateTime.Now);
                        iw.Optimize();
                        Console.WriteLine("optimization close -" + DateTime.Now);
                        } 

                    
                }
                Console.WriteLine("optimization start -"+DateTime.Now);
                iw.Optimize();
                iw.Dispose();
                Console.WriteLine("optimization close -" + DateTime.Now);

            }
            Console.WriteLine("processing closed");
            Console.ReadLine();
        }
    }
}