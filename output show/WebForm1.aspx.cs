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
            DataTable dt = new DataTable("result");
            dt.Columns.Add("id", typeof(Int64));
            dt.Columns.Add("title", typeof(string));
            IndexSearcher isIndexSearcher = Application["indexSearcher"] as IndexSearcher;

            SnowballAnalyzer analyzer = new SnowballAnalyzer(Version.LUCENE_30, "myname");

            StandardAnalyzer saAnalyzer = new StandardAnalyzer(Version.LUCENE_30);

            string query = TextBox1.Text.Replace(' ', '*');
            QueryParser queryParser = new QueryParser(Version.LUCENE_30, "title", saAnalyzer);
            Query qury = queryParser.Parse(query);

            // code highlighting
            SimpleHTMLFormatter formatter = new SimpleHTMLFormatter("<span style=\"background:yellow;\">", "</span>");
            var fragmenter = new SimpleFragmenter(50);
            IScorer eScorer = new QueryScorer(qury);

            IEncoder sEncoder = new SimpleHTMLEncoder();
            Highlighter hl = new Highlighter(formatter, sEncoder, eScorer);
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


                for (int i = 0; i < (result.Count() >= 10 ? result.Count() : result.Count()); i++)
                {
                    Document doc = isIndexSearcher.Doc(result[i].Doc);
                    if (doc.Get("title") != null)
                    {
                       TokenStream stream = saAnalyzer.TokenStream("title", new StringReader(doc.Get("title")));
                        string highlightedText = hl.GetBestFragments(stream, doc.Get("title"), 1, "...");
                        dt.Rows.Add(doc.Get("id"), highlightedText);
                    }


                }

                gd.DataSource = dt;
                gd.DataBind();
            }


        }

    }

}