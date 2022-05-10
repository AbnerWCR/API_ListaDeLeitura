﻿using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Seguranca;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivroApiClient
    {
        #region "Variaveis de leitura"

        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _accessor;

        #endregion

        #region "Construtor"

        public LivroApiClient(HttpClient httpClient, IHttpContextAccessor accessor)
        {
            _httpClient = httpClient;
            _accessor = accessor;
        }

        #endregion

        #region "Metodos Privados"

        private void AddBearerToken()
        {
            var token = _accessor.HttpContext.User.Claims.First(user => user.Type == "Token").Value;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.ToString());
        }

        private HttpContent CreateMultipartFormDataContent(LivroUpload model)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.Titulo), EnvolveComAspasDuplas("titulo"));
            content.Add(new StringContent(model.Lista.ParaString()), EnvolveComAspasDuplas("lista"));

            if (!string.IsNullOrEmpty(model.Subtitulo))
            {
                content.Add(new StringContent(model.Subtitulo), EnvolveComAspasDuplas("subtitulo"));
            }

            if (!string.IsNullOrEmpty(model.Resumo))
            {
                content.Add(new StringContent(model.Resumo), EnvolveComAspasDuplas("resumo"));
            }

            if (!string.IsNullOrEmpty(model.Autor))
            {
                content.Add(new StringContent(model.Autor), EnvolveComAspasDuplas("autor"));
            }

            if (model.Id > 0)
            {
                content.Add(new StringContent(model.Id.ToString()), EnvolveComAspasDuplas("id"));
            }

            if (model.Capa != null)
            {
                var imageContent = new ByteArrayContent(model.Capa.ConvertToBytes());
                imageContent.Headers.Add("content-type", "image/png");
                content.Add(
                    imageContent,
                    EnvolveComAspasDuplas("capa"),
                    EnvolveComAspasDuplas("capa.png")
                    );
            }

            return content;
        }

        private string EnvolveComAspasDuplas(string valor)
        {
            return $"\"{valor}\"";
        }

        #endregion

        #region "Metodos Publicos"

        public async Task<Lista> GetListaLeituraAsync(TipoListaLeitura tipo)
        {
            //var token = await _authApiClient.PostLoginAsync(new LoginModel { Login = "abner", Password = "123"});
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.ToString());

            AddBearerToken();
            var response = await _httpClient.GetAsync($"listasleitura/{tipo}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<Lista>();
        }

        public async Task DeleteLivroAsync(int id)
        {
            AddBearerToken();
            HttpResponseMessage response = await _httpClient.DeleteAsync($"livros/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<byte[]> GetCapaLivroAsync(int id)
        {
            //var token = await _authApiClient.PostLoginAsync(new LoginModel { Login = "abner", Password = "123"});
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.ToString());

            AddBearerToken();
            HttpResponseMessage response = await _httpClient.GetAsync($"livros/{id}/capa");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<LivroApi> GetLivroApiAsync(int id)
        {
            //http://localhost:6000/api/livros/{id}
            //http://localhost:6000/api/listasleitura/paraler
            //http://localhost:6000/api/livros/{id}/capa

            AddBearerToken();
            HttpResponseMessage response = await _httpClient.GetAsync($"livros/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<LivroApi>();
        }

        public async Task PostLivroAsync(LivroUpload livro)
        {
            AddBearerToken();
            HttpContent content = CreateMultipartFormDataContent(livro);
            var response = await _httpClient.PostAsync("livros", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task PutLivroAsync(LivroUpload livro)
        {
            AddBearerToken();
            HttpContent content = CreateMultipartFormDataContent(livro);
            var response = await _httpClient.PutAsync("livros", content);
            response.EnsureSuccessStatusCode();
        }

        #endregion
    }
}
