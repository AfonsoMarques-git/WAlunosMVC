using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace WebAlunoMVC.Models
{
    public class ConexaoBD
    {
        private string host;
        private int porta;
        private string bd;
        private string utilizador;
        private string password;
        private MySqlConnection conn = null;

        public ConexaoBD(string host, int porta, string utilizador, string password, string bd)
        {
            this.host = host;
            this.porta = porta;
            this.utilizador = utilizador;
            this.password = password;
            this.bd = bd;
        }

        public MySqlConnection ObterConexao()
        {
            try
            {
                string connectionInfo = "datasource=" + host + ";port=" + porta + ";username=" + utilizador + ";password=" + password + ";database=" + bd + ";SslMode = none";
                conn = new MySqlConnection(connectionInfo);
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            return null;
        }
    }
}