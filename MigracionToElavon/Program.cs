using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using MigracionToElavon.DAL;
using MigracionToElavon.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MigracionToElavon
{
    class Program
    {

        private static IConfiguration _iconfiguration;
        static HttpClient client = new HttpClient();

        static void GetAppSettingsFile()
        {
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();
        }

        static async Task<bool> SendFiles(string name, string noar, int idar)
        {
            var foto = new FotoAttachDal(_iconfiguration);
            bool respuesta = false;
            HttpClient client = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();
            HttpContent content = new StringContent("fileToUpload");
            HttpContent contentNoar = new StringContent(noar);
            form.Add(content, "fileToUpload");
            form.Add(contentNoar, "noar");

            var stream = new FileStream("C:\\inetpub\\wwwroot\\MIC3\\UPLOADER\\ARCHIVOS\\" + name, FileMode.Open);
            content = new StreamContent(stream);
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "archivos",
                FileName = name
            };

            form.Add(content);

            HttpResponseMessage response = null;

            try
            {
                response = (await client.PostAsync("https://localhost:44382/api/files/ODT", form));
                foto.insertDatos(name,noar,idar,"ENVIO EXITOSO");
                respuesta = true;
            }
            catch (Exception ex)
            {
                foto.insertDatos(name, noar, idar, "A OCURRIDO UN ERROR AL ENVIAR EL ARCHIVO: " + ex.ToString());
                Console.WriteLine(ex.Message);
            }

            var k = response.Content.ReadAsStringAsync().Result;

            return respuesta;
        }

        static void Main(string[] args)
        {
            GetAppSettingsFile();
            var foto = new FotoAttachDal(_iconfiguration);
            var listaFotos = foto.GetList();

            listaFotos.ForEach(async item =>
            {
                await SendFiles(item.archivo, "REING-RETIRO51", item.idar);
                Console.WriteLine(item.archivo);
            });
            Console.ReadKey();
            
        }
    }
}

