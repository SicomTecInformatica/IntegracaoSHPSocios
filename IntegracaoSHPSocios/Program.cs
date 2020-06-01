
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net.Http.Headers;
using Newtonsoft.Json.Bson;
using RestSharp;
using RestSharp.Authenticators;
using System.IO;
//using NUnit.Framework;
using System.Web;
using System.Diagnostics;
using RestSharp.Authenticators.OAuth;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Runtime.Serialization.Json;

namespace IntegracaoSHPSocios
{
    class Program
    {
        static void Main(string[] args)
        {
            ImportaSocio();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GET()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //KEYS

            //const string consumerKey = "f69a90cbedeca911176fb94fe698844d8d9c454e0ecee0199fc684c9b904c1eb";
            //const string consumerSecret = "3507739ded4db7a084866c8d321f210f3e32782ee6ab7684a824f7862bc6594a";
            //const string accessToken = "3b9b59f3279861339769a191148139c82622f7dead96992a247f8fb3051ec0a7";
            //const string tokenSecret = "6c56bc39d230c5ceaa5e59ef2da41214f3fd39edb7a8dd46c05711edae601eba";
            const string consumerKey = "f705c6b2a43a7acfe9e6dda2a24389b139daab3195c6eb6418031c44e5c465ab";
            const string consumerSecret = "10e254e3158b31eea1e3f8034cbe0a3c3c0c4553639a8b81047e55afb06c8f83";
            const string accessToken = "62e84ef0542bdaf8343b469d9c703c59007577659be31a6b48928aed9132ea8a";
            const string tokenSecret = "3157071cdee78d4a438c062db9b8134d0da1ffb376957e7e8c3b6bfeec4b248d";

            //Endereço base

            const string baseUrl = "https://5022901.restlets.api.netsuite.com/app/site/hosting/restlet.nl";
            //const string baseUrl = "https://5022901-sb1.restlets.api.netsuite.com/app/site/hosting/restlet.nl";
            //https://5022901.restlets.api.netsuite.com/app/site/hosting/restlet.nl?script=511&deploy=1&pag_start=2&pag_end=4
            //https://5022901-sb1.restlets.api.netsuite.com/app/site/hosting/restlet.nl?script=469&deploy=1&pag_start=2&pag_end=4

            //criando a chamada com seus parametros
            var client = new RestClient(baseUrl);
/*            client.AddDefaultQueryParameter("script", "469");
            client.AddDefaultQueryParameter("deploy", "1");
            //client.AddDefaultQueryParameter("qtd", "7000");
            client.AddDefaultQueryParameter("pag_start", "2");
            client.AddDefaultQueryParameter("pag_end", "4");
*/
            client.AddDefaultQueryParameter("script", "511");
            client.AddDefaultQueryParameter("deploy", "1");
            client.AddDefaultQueryParameter("pag_start", "2");
            client.AddDefaultQueryParameter("pag_end", "4");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");

            //usando as chaves para criar o authorization
            OAuth1Authenticator auth = OAuth1Authenticator.ForProtectedResource(consumerKey, consumerSecret, accessToken, tokenSecret);
            auth.SignatureMethod = OAuthSignatureMethod.HmacSha1;
            auth.Realm = "5022901";
//            auth.Realm = "5022901_SB1";
            auth.ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader;
            client.Authenticator = auth;

            //chamada da api

            IRestResponse response = client.Execute(request);
            var _resposta = response.Content;
            //return  JsonConvert.DeserializeObject<List<Clientes>>(_resposta);

            return response.Content;

        }
        public static void ImportaSocio()
        {
            string retornoget = GET();
            if (retornoget != null)
            {
                SqlConnection dataConnection = new SqlConnection();
                String wCon = @"Server=JAIRSICOM\SQL2019;Database=PDVSHP;User ID=sa;Password=sicom4713; Connect Timeout=30; MultipleActiveResultSets=True";
                dataConnection.ConnectionString = wCon;
                dataConnection.Open();

                var rootObj = JsonConvert.DeserializeObject<RootObject>(retornoget);
                foreach (var row in rootObj.Clientes)
                {

                    SqlCommand cmdSQL = new SqlCommand();
                    cmdSQL.CommandType = System.Data.CommandType.Text;
                    cmdSQL.Connection = dataConnection;
                    //Console.WriteLine(row.id);
                    //Console.WriteLine(row.name);
                    //Console.WriteLine(row.cpf);
                    //Console.WriteLine(row.email);
                    //Console.WriteLine(row.titulo);
                    //Console.WriteLine(row.nascimento);
                    //Console.WriteLine(row.telefone);
                    //Console.WriteLine(row.socio);
                    if (row.name.Contains(":"))
                    {
                        int inicioDoSegundoNome = row.name.IndexOf(":");
                        string nome = row.name.Substring(inicioDoSegundoNome+2);
                        cmdSQL.Parameters.Clear();
                        string wsql = "INSERT INTO Clientes_Socios_Dependentes (Codigo_Cliente, " +
                            "ID_Socio, CPF, Nome, Data_Nascimento, flgSMS) Values (" +
                            "@codcli, @id, @cpf, @nome, @nasc, 0)";
                        cmdSQL.CommandText = wsql;
                        cmdSQL.Parameters.Clear();
                        cmdSQL.Parameters.AddWithValue("@codcli", row.titulo);
                        cmdSQL.Parameters.AddWithValue("@id", row.id);
                        cmdSQL.Parameters.AddWithValue("@cpf", 0);
                        cmdSQL.Parameters.AddWithValue("@nome", nome);
                        cmdSQL.Parameters.AddWithValue("@nasc", row.nascimento);
                        cmdSQL.Parameters.AddWithValue("@saldo", 0);
                        cmdSQL.ExecuteNonQuery();
                    }
                    else
                    {
                        cmdSQL.CommandText = @"INSERT INTO clientes (
                                    Codigo_Cliente, cgc_cpf_cliente, Nome, nLoja, Complemento, telefone, rg, 
                                    e_mail, Data_Nascimento, SaldoCredor) VALUES (
                                    @codcli, @cpf, @nome, @nloja, 
                                    @compl, @tel, @rg, 
                                    @email, @nasc, @saldo)";
                        cmdSQL.Parameters.Clear();
                        cmdSQL.Parameters.AddWithValue("@codcli", row.titulo);
                        cmdSQL.Parameters.AddWithValue("@cpf", 0);
                        cmdSQL.Parameters.AddWithValue("@nome", row.name);
                        cmdSQL.Parameters.AddWithValue("@nloja", row.id);
                        cmdSQL.Parameters.AddWithValue("@compl", "");
                        cmdSQL.Parameters.AddWithValue("@tel", row.telefone);
                        cmdSQL.Parameters.AddWithValue("@rg", row.id);
                        cmdSQL.Parameters.AddWithValue("@email", row.email);
                        cmdSQL.Parameters.AddWithValue("@nasc", row.nascimento);
                        cmdSQL.Parameters.AddWithValue("@saldo", 0);
                        cmdSQL.ExecuteNonQuery();
                        string wsql = "INSERT INTO Clientes_Socios_Dependentes (Codigo_Cliente, " +
                            "ID_Socio, CPF, Nome, Data_Nascimento, flgSMS) Values (" +
                            "@codcli, @id, @cpf, @nome, @nasc, 0)";
                        cmdSQL.CommandText = wsql;
                        cmdSQL.Parameters.Clear();
                        cmdSQL.Parameters.AddWithValue("@codcli", row.titulo);
                        cmdSQL.Parameters.AddWithValue("@id", row.id);
                        cmdSQL.Parameters.AddWithValue("@cpf", 0);
                        cmdSQL.Parameters.AddWithValue("@nome", row.name);
                        cmdSQL.Parameters.AddWithValue("@nasc", row.nascimento);
                        cmdSQL.Parameters.AddWithValue("@saldo", 0);
                        cmdSQL.ExecuteNonQuery();
                    }
                    //cmdSQL.Connection.Close();

                }
                dataConnection.Close();
            }



        }
    }
}
