using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using HtmlAgilityPack;

namespace Fonts
{
    
    class Program
    {
        static void AppendToSbs(StringBuilder enumStringBuilder,
            StringBuilder switchCaseStringBuilder,
            string htmlClass, string style = null)
        {
            var name = htmlClass.Split(" ");
            var firstPartNameWords = name[1].Remove(0, 3).Split("-");
            var nameWords = new List<string>(firstPartNameWords);

            if (style != null)
            {
                nameWords.Add(style);
            }

            var className = string.Concat(nameWords.Select(x => x[0].ToString().ToUpper() + x.Remove(0, 1)));
            className = char.IsDigit(className[0]) ? "_" + className : className;
            enumStringBuilder.Append($"\t{className},{Environment.NewLine}");
            switchCaseStringBuilder.Append(
                $"\tcase StdIcon.{className}:{Environment.NewLine}\t\treturn \"{htmlClass}\";{Environment.NewLine}");
        }

        static void Main(string[] args)
        {
            //суть та же самая осталасть
            var classStylePairs = new Dictionary<string, List<string>>();
            var enumStringBuilder = new StringBuilder($"public enum StdIcon{Environment.NewLine}{{{Environment.NewLine}\tNone,{Environment.NewLine}");
            var switchCaseStringBuilder = new StringBuilder($"switch(value){Environment.NewLine}{{{Environment.NewLine}");
            //пришлось загружать файл, так как на сайте постзагрузка (видимо что-то на реактивке)
            //поэтому загружает только самую раннюю версию, на которой нет иконок
            var path = @"LocationOfYourFile\Free Icons _ Font Awesome.html";
            //загрузка документа и пробег по нодам
            var document = new HtmlDocument();
            document.Load(path);
            foreach (var node in document.DocumentNode
                .Descendants().First(x => x.Name == "main").Descendants().Where(x => x.Name == "li")
                .Select(x => x.Descendants().First(y => y.Name == "i")))
            {
                var name = node.Attributes["class"].Value.Split(" ");
                var className = name[1];
                if (!classStylePairs.ContainsKey(className))
                {
                    classStylePairs.Add(className, new List<string>());
                }

                classStylePairs[className].Add(name[0]);
            }

            foreach (var htmlClassWithoutStyle in classStylePairs.Keys)
            {
                var styles = classStylePairs[htmlClassWithoutStyle];
                if (styles.Count >= 2)
                {
                    foreach (var style in styles.OrderBy(x => x))
                    {
                        if (style == "far")
                        {
                            AppendToSbs(enumStringBuilder, switchCaseStringBuilder,
                                string.Concat(style, " ", htmlClassWithoutStyle));
                        }
                        else
                        {
                            //Brand выглядит как Solid, поэтому, в принципе, взаимозаменяемы в виде Bold
                            AppendToSbs(enumStringBuilder, switchCaseStringBuilder,
                                string.Concat(style, " ", htmlClassWithoutStyle), "Bold");
                        }
                    }
                }
                else
                {
                    AppendToSbs(enumStringBuilder, switchCaseStringBuilder,
                        string.Concat(styles[0], " ", htmlClassWithoutStyle));
                }
            }

            enumStringBuilder.Remove(enumStringBuilder.Length - 2, 1).Append("}");
            switchCaseStringBuilder.Append(
                $"\tdefault:{Environment.NewLine}\t\tthrow new ArgumentOutOfRangeException(new Guid(\"73FE4B0B-B8DD-48A7-ACD4-61AF4A926FBA\").ToString());{Environment.NewLine}}}");
            Console.WriteLine(enumStringBuilder);
            Console.WriteLine(switchCaseStringBuilder);

            //вывод в файл
            using (StreamWriter sw = new StreamWriter("E:\\hw.txt", false, System.Text.Encoding.Default))
            {
                sw.WriteLine(enumStringBuilder);
                sw.WriteLine(switchCaseStringBuilder);
            }
        }
    }
}