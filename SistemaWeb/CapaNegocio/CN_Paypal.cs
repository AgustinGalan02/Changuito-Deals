using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using CapaEntidad.Paypal;

namespace CapaNegocio
{
    public class CN_Paypal
    {

        private static string urlpaypal = ConfigurationManager.AppSettings["UrlPaypal"];
        private static string clientId = ConfigurationManager.AppSettings["ClientId"];
        private static string secret = ConfigurationManager.AppSettings["Secret"];

        // CREAR SOLICITUD DE PAGO
        public async Task<Response_Paypal<Response_Checkout>> CrearSolicitud(Checkout_Order orden)
        {
            Response_Paypal<Response_Checkout> response_paypal = new Response_Paypal<Response_Checkout>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(urlpaypal); // se manda la url base para ejecutar la API

                var authToken = Encoding.ASCII.GetBytes($"{clientId}:{secret}"); // se crea autentificacion con estas 2 credenciales
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",Convert.ToBase64String(authToken));

                var json = JsonConvert.SerializeObject(orden); // request. Se convierte la clase en un objeto JSON
                var data = new StringContent(json, Encoding.UTF8, "application/json"); // convertir en el tipo de contenido que necesita la API

                HttpResponseMessage response = await client.PostAsync("v2/checkout/orders", data); // EJECUTAR API y OBTENER RESPUESTA

                response_paypal.Status = response.IsSuccessStatusCode;

                if (response.IsSuccessStatusCode)
                {
                    string jsonRespuesta = response.Content.ReadAsStringAsync().Result; // SE OBTIENE TODA LA INFO DE LA RESPUESTA

                    Response_Checkout checkout = JsonConvert.DeserializeObject<Response_Checkout>(jsonRespuesta); // se convierte a JSON

                    response_paypal.Response = checkout;
                }

                return response_paypal;
            }
        }



        // CREAR VALIDACION DE PAGO
        public async Task<Response_Paypal<Response_Capture>> AprobarPago(string token)
        {
            Response_Paypal<Response_Capture> response_paypal = new Response_Paypal<Response_Capture>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(urlpaypal); // se manda la url base para ejecutar la API

                var authToken = Encoding.ASCII.GetBytes($"{clientId}:{secret}"); // se crea autentificacion con estas 2 credenciales
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                var data = new StringContent("{}", Encoding.UTF8, "application/json"); 

                HttpResponseMessage response = await client.PostAsync($"v2/checkout/orders/{token}/capture", data); 

                response_paypal.Status = response.IsSuccessStatusCode;

                if (response.IsSuccessStatusCode)
                {
                    string jsonRespuesta = response.Content.ReadAsStringAsync().Result; // SE OBTIENE TODA LA INFO DE LA RESPUESTA

                    Response_Capture capture = JsonConvert.DeserializeObject<Response_Capture>(jsonRespuesta); // se convierte a JSON

                    response_paypal.Response = capture;
                }

                return response_paypal;
            }
        }

    }
}
