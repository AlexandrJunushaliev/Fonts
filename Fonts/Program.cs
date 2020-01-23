using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using HtmlAgilityPack;

namespace Fonts
{
    class Program
    {
        static void Main(string[] args)
        {
            var enumStringBuilder = new StringBuilder("public enum StdIconic\n{\n\tNone,\r");
            var switchCaseStringBuilder = new StringBuilder("switch(value)\n{\n");
            var web = new HtmlWeb();
            //загрузка документа и пробег по нодам
            var document = web.Load("https://fontawesome.com/v4.7.0/cheatsheet/");
            foreach (var node in document.DocumentNode.Descendants()
                .Where(x => x.Attributes["class"] != null && x.Attributes["class"].Value == "row").First().ChildNodes
                .Where(x => x.Name == "div"))
            {
                var name = node.ChildNodes.Where(x => x.Name == "#text").Select(x => x.InnerText)
                    .Where(x => !string.IsNullOrWhiteSpace(x)).First().Trim();
                var nameWords = name.Remove(0, 3).Split("-");
                var className = string.Concat(nameWords.Select(x => x[0].ToString().ToUpper() + x.Remove(0, 1)));
                className = char.IsDigit(className[0]) ? "_" + className : className;
                enumStringBuilder.Append($"\t{className},\n");
                switchCaseStringBuilder.Append($"\tcase StdIconic.{className}:\n\t\treturn \"{name}\";\n");
            }

            Console.WriteLine(enumStringBuilder.Remove(enumStringBuilder.Length - 2, 1).Append("}"));
            Console.WriteLine(switchCaseStringBuilder.Append(
                "\tdefault:\n\t\tthrow new ArgumentOutOfRangeException(new Guid(\"73FE4B0B-B8DD-48A7-ACD4-61AF4A926FBA\"));\n}"));

            //вывод в файл
            /*using (StreamWriter sw = new StreamWriter("E:\\hw.txt", false, System.Text.Encoding.Default))
            {
                sw.WriteLine(enumStringBuilder);
                sw.WriteLine(switchCaseStringBuilder);
            }*/
        }
    }
}