using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Snowball;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Microsoft.SqlServer.Server;
using Version = Lucene.Net.Util.Version;

namespace WebApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var dt = new DataTable("result");
            dt.Columns.Add("id", typeof(Int64));
            dt.Columns.Add("title", typeof(string));
            dt.Columns.Add("Text", typeof(string));
            var isIndexSearcher = Application["indexSearcher"] as IndexSearcher;

            var analyzer = new SnowballAnalyzer(Version.LUCENE_30, "myname");

            var saAnalyzer = new StandardAnalyzer(Version.LUCENE_30);

            var query = TextBox1.Text.Replace(' ', '*');
           var  queryParser = new MultiFieldQueryParser(Version.LUCENE_30,new string[] { "title", "plainText" }, saAnalyzer);
            var qury = queryParser.Parse(query);

            // code highlighting
            // SimpleHTMLFormatter formatter = new SimpleHTMLFormatter("<span style=\"background:yellow;\">", "</span>");
            var formatter = new SimpleHTMLFormatter("<b>", "</b>");
            var fragmenter = new SimpleFragmenter(50);
            IScorer eScorer = new QueryScorer(qury);

            IEncoder sEncoder = new SimpleHTMLEncoder();
            var hl = new Highlighter(formatter, sEncoder, eScorer);
            //for (int i = 0; i < hits.Length(); i++)
            //{
            //    Lucene.Net.Documents.Document doc = hits.Doc(i);
            //    Lucene.Net.Analysis.TokenStream stream = analyzer.TokenStream("", new StringReader(doc.Get("text")));
            //    string highlightedText = highlighter.GetBestFragments(stream, doc.Get("text"), 1, "...");
            //    Console.WriteLine("--> " + highlightedText);
            //}


            if (isIndexSearcher != null)
            {
                var result = isIndexSearcher.Search(qury, 2000).ScoreDocs;
                count.InnerHtml = result.Length.ToString();


                for (var i = 0; i < (result.Count() >= 10 ? result.Count() : result.Count()); i++)
                {
                    var doc = isIndexSearcher.Doc(result[i].Doc);
                    if (doc.Get("title") != null && doc.Get("plainText") !=null)
                    {
                       var stream = saAnalyzer.TokenStream("title", new StringReader(doc.Get("title")));
                        var highlightedText = hl.GetBestFragments(stream, doc.Get("title"), 1, "...");

                        var stream1 = saAnalyzer.TokenStream("plainText", new StringReader(doc.Get("plainText")));
                        var htxt = hl.GetBestFragments(stream1, doc.Get("plainText"), 1, "...");

                        dt.Rows.Add(doc.Get("id"), highlightedText,htxt);
                    }


                }

                gd.DataSource = dt;
                gd.DataBind();
            }


        }

    }

}