using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.Api.Formatters
{
    public class LivrosCsvFormatter : TextOutputFormatter
    {
        public LivrosCsvFormatter()
        {
            var textCsvMediaType = MediaTypeHeaderValue.Parse("text/csv");
            var applicationCsvMediaType = MediaTypeHeaderValue.Parse("application/csv");
            SupportedMediaTypes.Add(textCsvMediaType);
            SupportedMediaTypes.Add(applicationCsvMediaType);
            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanWriteType(Type type)
        {
            return type == typeof(LivroApi);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var livroEmCsv = string.Empty;

            if (context.Object is LivroApi)
            {
                var livro = context.Object as LivroApi;

                livroEmCsv = $"{livro.Titulo}; {livro.Subtitulo}; {livro.Autor}; {livro.Lista}";
            }

            using (var escritor = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            {
                return escritor.WriteAsync(livroEmCsv);
            }// escritor.Close();
        }
    }
}
