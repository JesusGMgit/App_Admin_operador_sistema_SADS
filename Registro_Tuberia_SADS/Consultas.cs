﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace Registro_Tuberia_SADS
{
    public static class Consultas
    {
        private static readonly HttpClient cliente_consulta = new HttpClient();

        public static string Get_API(string url)
        {
            string responseString = "";
            try
            {
                var response = cliente_consulta.GetStringAsync(url);
                responseString = response.Result;

            }
            catch (Exception err)
            {
                responseString= "error_conexion: " + err.Message;
                

            }
            return responseString;
        }

        public static string Post_API(string url, StringContent contenido)
        {
            string respuesta_post;

           return respuesta_post= cliente_consulta.PostAsync(url, contenido).Result.ToString();
        }

        
    }
}