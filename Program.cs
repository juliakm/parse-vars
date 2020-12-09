using System;
using System.IO;
using HtmlAgilityPack;

namespace parse_vars
{

    class Program
    {
        static void Main(string[] args)
        {
        // From Web
        var html = @"https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables";
        HtmlWeb web = new HtmlWeb();
        var htmlDoc = web.Load(html);
        var node = htmlDoc.DocumentNode.SelectSingleNode("//head/title");
        Console.WriteLine("Node Name: " + node.Name + "\n" + node.OuterHtml);

        var heading = htmlDoc.DocumentNode.SelectSingleNode("//h2");

        Console.WriteLine("Node Name: " + heading.Name + "\n" + heading.InnerHtml);

        
        // init File and clear out existing text
        StreamWriter w = new StreamWriter("WriteVars.txt", false);

        //grabbing all tables
        foreach (var row in htmlDoc.DocumentNode.SelectNodes("//*[@id='main']//tr"))
        {
            var nodes = row.SelectNodes("td");
            if (nodes != null)
            {

                var variable = nodes[0].InnerText;
                var definition = nodes[1].InnerText;
                //var templates = nodes[2].InnerText;
                var separator = "--";
                var descriptionHeading = "## Description";

                //Console.WriteLine(variable);
                //Console.WriteLine(description);

               string[] lines = {variable};
                
                // Write to file
                w.WriteLine("# " + variable);
                w.WriteLine();
                w.WriteLine(descriptionHeading);
                w.WriteLine(definition);
                //w.WriteLine(templates);
                w.WriteLine(separator);
                    //}

            }
        }

            //Test to write the file (imaginary)
            string message = "Booya";
            writeFile.Writer(message);



        }
    }


}
