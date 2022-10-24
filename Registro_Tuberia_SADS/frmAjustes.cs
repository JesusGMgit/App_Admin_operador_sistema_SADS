﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;


namespace Registro_Tuberia_SADS
{
    public partial class frmAjustes : Form
    {
        //Variable para las solicitudes o consultas a al servidor 
        private static readonly HttpClient cliente_ajustes = new HttpClient();
        string P_nombre_proyecto;
        //funcion parar mandar las consultas a la API echa en php en el servidor o host
        public string GetApiData(string url)
        {
            string responseString = "";
            try
            {
                var response = cliente_ajustes.GetStringAsync(url);
                responseString = response.Result;

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);

            }
            return responseString;
        }
        public frmAjustes()
        {
            InitializeComponent();
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            if ((cmbProyecto.Text!="") && (cmbMaquina.Text!="") && (cmbOrientacion.Text != ""))
            {
                Properties.Settings.Default.Gcp = true;
                Properties.Settings.Default.Gmaquina = cmbMaquina.Text;
                Properties.Settings.Default.Gproyecto = cmbProyecto.Text;
                Properties.Settings.Default.Gfrmp_orientacion = cmbOrientacion.Text;
                MessageBox.Show("Ajustes guardados.");
                cmbProyecto.Enabled = false;
                cmbMaquina.Enabled = false;
                cmbOrientacion.Enabled = false;
                btnAplicar.Enabled = false;
                Task.Delay(2000);
                btnCerrar2.PerformClick();
            }
            else
            {
                MessageBox.Show("Selecciona proyecto y maquina");
            }
            

        }

        private void btnCerrar2_Click(object sender, EventArgs e)
        {

            if(Properties.Settings.Default.Gcp == false)
            {
                MessageBox.Show("LOS CAMBIOS NO FUERON GUARDADOS");
            }

            this.Close();


        }

        private void frmAjustes_Load(object sender, EventArgs e)
        {
            //carga los datos a los combobox de maquina y orientacion
            //opciones de maquinas
            cmbMaquina.Items.Add("EXTERNA1");
            cmbMaquina.Items.Add("EXTERNA2");
            cmbMaquina.Items.Add("EXTERNA3");
            cmbMaquina.Items.Add("INTERNA1");
            cmbMaquina.Items.Add("INTERNA2");
            cmbMaquina.Items.Add("INTERNA3");
            //opciones de orientacion
            cmbOrientacion.Items.Add("HORIZONTAL");
            cmbOrientacion.Items.Add("VERTICAL");
            cmbOrientacion.Text = "HORIZONTAL";
            //carga la opcion selecionada anteriormente
            cmbMaquina.Text = Properties.Settings.Default.Gmaquina;
            cmbOrientacion.Text = Properties.Settings.Default.Gfrmp_orientacion;
            try
            {
                

                var output = GetApiData("http://10.10.20.15/api/rq_tProyectos.php?id=");

                //operadores_tabla myobj = JsonConvert.DeserializeObject<operadores_tabla>(output.Substring(1, output.Length - 2));
                List<proyectos_tabla> results = JsonConvert.DeserializeObject<List<proyectos_tabla>>(output);

                foreach (var r in results)
                {
                    cmbProyecto.Items.Add(r.Pro_ID + "-" + r.Pro_Ordentrabajo);
                    
                }
                
                cmbProyecto.Text = Properties.Settings.Default.Gordentrabajo;


            }
            catch (Exception err)
            {
                MessageBox.Show("Error cargar proyecto: " + err.Message +
                        "\n CHEQUE CONEXION CON LA BASE DE DATOS SADS.");
            }

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbProyecto_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
