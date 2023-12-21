using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Data;
using System.Linq;

namespace Registro_Tuberia_SADS
{
    public partial class frmAPIs : Form
    {
        DataTable P_Tuberia_datatable = new DataTable();
        DataTable P_Operadores_datatable = new DataTable();
        string P_url_fecha = "";
        public char[] Admin_permitidas_fecha = { '/', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', (char)Keys.Back };
        public char[] Admin_permitidas_hora = { ':',' ', 'a', 'm', 'p', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', (char)Keys.Back };
        public char[] Admin_permitidas_tubo_placa = { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', (char)Keys.Back };
        public char[] Admin_permitidas_lote_alambre = { '/', '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', (char)Keys.Back };

        #region Admin proyectos

        private void btnCreateProyectos_Click(object sender, EventArgs e)
        {
            string url_crearpro = "http://10.10.20.15/backend/api/ar_tProyectos.php";
            try
            {
                var datos_de_envio = new proyectos_tabla
                {
                    Pro_Nombre = txbCrProNombre.Text,
                    Pro_OrdenTrabajo = txbCrProOT.Text,
                    Pro_WPS = txbCrProWPS.Text,
                    Pro_Alambre = txbCrProAlambre.Text,
                    Pro_Fundente = txbCrProFundente.Text,
                    Pro_Diametro = txbCrProDiametro.Text,
                    Pro_Espesor = txbCrProEspesor.Text,
                    Pro_Especificacion = txbCrProEspecificacion.Text
                };


                var json = JObject.FromObject(datos_de_envio);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = Consultas.Post_API(url_crearpro, content);
                Iniciar_combobox_proyectos();
                Limpiar_textbox_CrearProyecto();
                MessageBox.Show("Se registro con exito");

            }
            catch (Exception err)
            {
                MessageBox.Show("Error en enviar a BD\n error: " + err.Message);

            }

        }

        void Iniciar_combobox_proyectos()
        {
            try
            {
                var output = Consultas.Get_API("http://10.10.20.15/backend/api/ar_tProyectos.php");
                //operadores_tabla myobj = JsonConvert.DeserializeObject<operadores_tabla>(output.Substring(1, output.Length - 2));
                List<proyectos_tabla> results = JsonConvert.DeserializeObject<List<proyectos_tabla>>(output);
                Limpiar_combobox_Proyecto();
                foreach (var r in results)
                {
                    cmbProyectosAPI.Items.Add(r.Pro_ID + "-" + r.Pro_OrdenTrabajo);
                    cmbActualizarPro.Items.Add(r.Pro_ID + "-" + r.Pro_OrdenTrabajo);
                    cmbBorrarPro.Items.Add(r.Pro_ID + "-" + r.Pro_OrdenTrabajo);
                    cmbCrTuboIDproyecto.Items.Add(r.Pro_ID + "-" + r.Pro_OrdenTrabajo);
                    cmbReTuboIDproyecto.Items.Add(r.Pro_ID + "-" + r.Pro_OrdenTrabajo);
                    cmbUpTuboIDproyecto.Items.Add(r.Pro_ID + "-" + r.Pro_OrdenTrabajo);
                }

            }
            catch (Exception err)
            {
                MessageBox.Show("Error cargar proyecto: " + err.Message +
                        "\n CHEQUE CONEXION CON LA BASE DE DATOS SADS.");
            }
        }

        void Limpiar_combobox_Proyecto()
        {
            cmbProyectosAPI.Items.Clear();
            cmbBorrarPro.Items.Clear();
            cmbActualizarPro.Items.Clear();
        }
        void Limpiar_textbox_CrearProyecto()
        {
            txbCrProOT.Text = txbCrProWPS.Text = txbCrProNombre.Text = "";
            txbCrProDiametro.Text = txbCrProEspesor.Text = txbCrProAlambre.Text = "";
            txbCrProFundente.Text = txbCrProEspecificacion.Text = "";

        }
        private void btnGetProyectos_Click(object sender, EventArgs e)
        {

            foreach (var r in Obtener_datos_proyecto(cmbProyectosAPI.Text))
            {
                lblIDProyecto.Text = r.Pro_ID.ToString();
                lblNombreProyecto.Text = r.Pro_Nombre;
                lblOTProyecto.Text = r.Pro_OrdenTrabajo;
                lblAlambreProyecto.Text = r.Pro_Alambre;
                lblFundenteProyecto.Text = r.Pro_Fundente;
                lblEspecificacionProyecto.Text = r.Pro_Especificacion;
                lblDiametroProyecto.Text = r.Pro_Diametro.ToString();
                lblEspesorProyecto.Text = r.Pro_Espesor.ToString();
                lblWPSProyecto.Text = r.Pro_WPS;

            }

        }

        private void btnActualizarPro_Click(object sender, EventArgs e)
        {
            foreach (var r in Obtener_datos_proyecto(cmbActualizarPro.Text))
            {
                txbAcProID.Text = r.Pro_ID.ToString();
                txbAcProNombre.Text = r.Pro_Nombre;
                txbAcProOT.Text = r.Pro_OrdenTrabajo;
                txbAcProAlambre.Text = r.Pro_Alambre;
                txbAcProFundente.Text = r.Pro_Fundente;
                txbAcProEspecificacion.Text = r.Pro_Especificacion;
                txbAcProDiametro.Text = r.Pro_Diametro.ToString();
                txbAcProEspesor.Text = r.Pro_Espesor.ToString();
                txbAcProWPS.Text = r.Pro_WPS;

            }
            
        }

        private void btnPutProyectos_Click(object sender, EventArgs e)
        {
            string url_acpro = "http://10.10.20.15/backend/api/ar_tProyectos.php";
            try
            {
                var datos_de_envio = new proyectos_tabla
                {
                    Pro_ID = Convert.ToInt32(txbAcProID.Text),
                    Pro_Nombre = txbAcProNombre.Text,
                    Pro_OrdenTrabajo = txbAcProOT.Text,
                    Pro_WPS = txbAcProWPS.Text,
                    Pro_Alambre = txbAcProAlambre.Text,
                    Pro_Fundente = txbAcProFundente.Text,
                    Pro_Diametro = txbAcProDiametro.Text,
                    Pro_Espesor = txbAcProEspesor.Text,
                    Pro_Especificacion = txbAcProEspecificacion.Text
                };


                var json = JObject.FromObject(datos_de_envio);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = Consultas.Update_API(url_acpro, content);
                Iniciar_combobox_proyectos();
                MessageBox.Show("Registro actualizado");

            }
            catch (Exception err)
            {
                MessageBox.Show("Error en enviar a BD, \n error: " + err.Message);

            }
            
        }

        private void btnBoProVer_Click(object sender, EventArgs e)
        {
            foreach (var r in Obtener_datos_proyecto(cmbBorrarPro.Text))
            {
                lblBoProID.Text = r.Pro_ID.ToString();
                lblBoProNombre.Text = r.Pro_Nombre;
                lblBoProOT.Text = r.Pro_OrdenTrabajo;
                lblBoProAlambre.Text = r.Pro_Alambre;
                lblBoProFundente.Text = r.Pro_Fundente;
                lblBoProEspecificacion.Text = r.Pro_Especificacion;
                lblBoProDiametro.Text = r.Pro_Diametro.ToString();
                lblBoProEspesor.Text = r.Pro_Espesor.ToString();
                lblBoProWPS.Text = r.Pro_WPS;

            }
            btnDeletePro.Enabled = true;
        }
        private void btnDeletePro_Click(object sender, EventArgs e)
        {
            string url_bopro = "http://10.10.20.15/backend/api/ar_tProyectos.php";
            DialogResult r = MessageBox.Show("¿Esta seguro que quiere borrar el registro de proyecto?",
                "Registro de Tuberia", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (r == DialogResult.Yes)
            {
                try
                {
                    int po = cmbBorrarPro.Text.IndexOf("-");
                    string ID_proyecto = cmbBorrarPro.Text.Substring(0, po);
                    var output_p = Consultas.Delete_API(url_bopro + "?Pro_ID=" + ID_proyecto);
                    Iniciar_combobox_proyectos();

                    MessageBox.Show("Se elimino el registro");
                }
                catch (Exception err)
                {
                    MessageBox.Show("Error al borrar en BD\n error: " + err.Message);

                }

            }
            else
            {
                MessageBox.Show("No elimino el registro");
            }
        }

        List<proyectos_tabla> Obtener_datos_proyecto(string cmb_proyecto)
        {
            List<proyectos_tabla> datos_p = new List<proyectos_tabla>();
            try
            {
                int po = cmb_proyecto.IndexOf("-");
                string ID_proyecto = cmb_proyecto.Substring(0, po);
                var output_p = Consultas.Get_API("http://10.10.20.15/backend/api/ar_tProyectos.php?Pro_ID=" + ID_proyecto);
                datos_p = JsonConvert.DeserializeObject<List<proyectos_tabla>>(output_p);
            }
            catch (Exception err_proyecto)
            {

                MessageBox.Show("Error cargar proyecto: " + err_proyecto.Message +
                    "\n CHEQUE CONEXION CON LA BASE DE DATOS.");
            }

            return datos_p;
        }

        
        #endregion

        #region Admin operadores
       
        void Iniciar_tabla_operadores()
        {
            P_Operadores_datatable.Columns.Add("Op_Folio");
            P_Operadores_datatable.Columns.Add("Op_Nombre");
            string url_operadores = "http://10.10.20.15/backend/api/ar_tOperadores.php";
            rellenar_tabla_operadores(url_operadores);
            dgvOperadores.DataSource = P_Operadores_datatable;
        }

        public void rellenar_tabla_operadores(string url)
        {
            //llena la tabla de datos para tuberia de los registros solicitados por fecha dada
            P_Operadores_datatable.Rows.Clear();
            try
            {

                var output = Consultas.Get_API(url);

                List<operadores_tabla> temporal_results = JsonConvert.DeserializeObject<List<operadores_tabla>>(output);
                foreach (var r in temporal_results)
                {
                    P_Operadores_datatable.Rows.Add(r.Op_Folio, r.Op_Nombre);

                }

            }
            catch (Exception err)
            {
                MessageBox.Show("Rellenar tabla Operadores:" + err.Message);

            }
        }

        private void btnCreateOperador_Click(object sender, EventArgs e)
        {
            string url_crearop = "http://10.10.20.15/backend/api/ar_tOperadores.php";
            try
            {
                var datos_de_envio = new operadores_tabla
                {
                    Op_Folio = Convert.ToInt32(txbCrOpFolio.Text),
                    Op_Clave_soldador = txbCrOpClaveSoldador.Text,
                    Op_Nombre = txbCrOpNombre.Text,
                    Op_Puesto = "Operador"
                };


                var json = JObject.FromObject(datos_de_envio);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = Consultas.Post_API(url_crearop, content);
                rellenar_tabla_operadores(url_crearop);
                MessageBox.Show("Se registro con exito");

            }
            catch (Exception err)
            {
                MessageBox.Show("Error en enviar a BD\n error: " + err.Message);

            }

        }

        private void btnGetOperador_Click(object sender, EventArgs e)
        {
            if(txbFolioOperador.Text != "Escribe el Folio")
            {
                var output = Consultas.Get_API("http://10.10.20.15/backend/api/ar_tOperadores.php?Op_Folio=" + txbFolioOperador.Text);

                //operadores_tabla myobj = JsonConvert.DeserializeObject<operadores_tabla>(output.Substring(1, output.Length - 2));
                List<operadores_tabla> results = JsonConvert.DeserializeObject<List<operadores_tabla>>(output);

                if (output != "null")
                {
                    foreach (var r in results)
                    {
                        lblIDOperador.Text = r.Op_ID.ToString();
                        lblFolioOperador.Text = r.Op_Folio.ToString();
                        lblNombreOperador.Text = r.Op_Nombre;
                        lblClaveSoldadorOperador.Text = r.Op_Clave_soldador;

                    }

                }
                else
                {
                    MessageBox.Show("Operador no registrado");
                }
            }
            
        }

        private void btnActualizarOp_Click(object sender, EventArgs e)
        {
            if(txbAcOpFolio.Text != "Escribe el Folio")
            {
                var output = Consultas.Get_API("http://10.10.20.15/backend/api/ar_tOperadores.php?Op_Folio=" + txbAcOpFolio.Text);

                //operadores_tabla myobj = JsonConvert.DeserializeObject<operadores_tabla>(output.Substring(1, output.Length - 2));
                List<operadores_tabla> results = JsonConvert.DeserializeObject<List<operadores_tabla>>(output);

                if (output != "null")
                {
                    foreach (var r in results)
                    {
                        txbAcOpNombre.Text = r.Op_Nombre;
                        txbAcOpClaveSoldador.Text = "sc-" + r.Op_Clave_soldador;

                    }

                }
                else
                {
                    MessageBox.Show("Operador no registrado");
                }
            }
            
        }

        private void btnPutOperador_Click(object sender, EventArgs e)
        {
            string url_acop = "http://10.10.20.15/backend/api/ar_tOperadores.php";
            try
            {
                var datos_de_envio = new operadores_tabla
                {
                    Op_Folio = Convert.ToInt32(txbAcOpFolio.Text),
                    Op_Clave_soldador = txbAcOpClaveSoldador.Text,
                    Op_Nombre = txbAcOpNombre.Text,
                    Op_Puesto = "Operador"
                };


                var json = JObject.FromObject(datos_de_envio);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = Consultas.Update_API(url_acop, content);
                rellenar_tabla_operadores(url_acop);
                MessageBox.Show("Registro actualizado");

            }
            catch (Exception err)
            {
                MessageBox.Show("Error en enviar a BD, \n error: " + err.Message);

            }
        }

        
        private void btnBorrarOp_Click(object sender, EventArgs e)
        {
            if (txbDeOpFolio.Text != "Escribe el Folio")
            {
                var output = Consultas.Get_API("http://10.10.20.15/backend/api/ar_tOperadores.php?Op_Folio=" + txbDeOpFolio.Text);

                //operadores_tabla myobj = JsonConvert.DeserializeObject<operadores_tabla>(output.Substring(1, output.Length - 2));
                List<operadores_tabla> results = JsonConvert.DeserializeObject<List<operadores_tabla>>(output);

                if (output != "null")
                {
                    foreach (var r in results)
                    {
                        lblDeOpID.Text = r.Op_ID.ToString();
                        lblDeOpFolio.Text = r.Op_Folio.ToString();
                        lblDeOpNombre.Text = r.Op_Nombre;
                        lblDeOpClaveSoldador.Text = r.Op_Clave_soldador;

                    }

                }
                else
                {
                    MessageBox.Show("Ingrese un Folio");
                }
            }
        }

        private void btnDeleteOp_Click(object sender, EventArgs e)
        {
            string url_boop = "http://10.10.20.15/backend/api/ar_tOperadores.php";
            DialogResult r = MessageBox.Show("¿Esta seguro que quiere borrar el registro del operador?",
                "Registro de Tuberia", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (r == DialogResult.Yes)
            {
                try
                {
                    string Folio_operador = lblDeOpFolio.Text;
                    var output_p = Consultas.Delete_API(url_boop + "?Op_Folio=" + Folio_operador);
                    rellenar_tabla_operadores(url_boop);
                    MessageBox.Show("Se elimino el registro");
                }
                catch (Exception err)
                {
                    MessageBox.Show("Error al borrar en BD\n error: " + err.Message);

                }

            }
            else
            {
                MessageBox.Show("No elimino el registro");
            }
        }

        private void tbpOperadores_Leave(object sender, EventArgs e)
        {
            txbFolioOperador.Text = txbAcOpFolio.Text = txbDeOpFolio.Text = "Escribe el Folio";
        }

        private void txbFolioOperador_Enter(object sender, EventArgs e)
        {
            if (txbFolioOperador.Text == "Escribe el Folio")
            {
                txbFolioOperador.Text = "";
            }
        }

        private void txbAcOpFolio_Enter(object sender, EventArgs e)
        {
            if (txbAcOpFolio.Text == "Escribe el Folio")
            {
                txbAcOpFolio.Text = "";
            }
        }

        private void txbDeOpFolio_Enter(object sender, EventArgs e)
        {
            if (txbDeOpFolio.Text == "Escribe el Folio")
            {
                txbDeOpFolio.Text = "";
            }
        }
        #endregion

        #region Admin Tubo

        private void btnCreateTubo_Click(object sender, EventArgs e)
        {
            if (txbCrTuboNotubo.Text != "" && txbCrTuboNoplaca.Text != "" &&
                txbCrTuboFecha.Text != "" && txbCrTuboHora.Text != "" &&
                txbCrTuboFolioOperador.Text != "" && txbCrTuboLotealambre.Text != "" &&
                txbCrTuboLotefundente.Text != "" && cmbCrTuboIDproyecto.Text != "" &&
                cmbCrTuboMaquina.Text != "")
            {
                string fecha_envio, hora_envio, hora_db;
                string id_tubo = txbCrTuboNoplaca.Text + txbCrTuboNotubo.Text;
                string url_crtubo = "";
                DateTime hora_db_dt;
                fecha_envio = txbCrTuboFecha.Text;
                hora_envio = txbCrTuboHora.Text;
                hora_db_dt = Convert.ToDateTime(fecha_envio + " " + hora_envio);
                hora_db = hora_db_dt.ToString("yyyy-MM-dd HH:mm:ss");

                string opcion_maquina = cmbCrTuboMaquina.Text;

                int po = cmbCrTuboIDproyecto.Text.IndexOf("-");
                string ID_proyecto = cmbCrTuboIDproyecto.Text.Substring(0, po);

                switch (opcion_maquina)
                {
                    case "INTERNA1":
                        url_crtubo = "http://10.10.20.15/backend/api/ar_tTuberiaInterna_1.php";
                        break;
                    case "INTERNA2":
                        url_crtubo = "http://10.10.20.15/backend/api/ar_tTuberiaInterna_2.php";
                        break;
                    case "INTERNA3":
                        url_crtubo = "http://10.10.20.15/backend/api/ar_tTuberiaInterna_3.php";
                        break;
                    case "EXTERNA1":
                        url_crtubo = "http://10.10.20.15/backend/api/ar_tTuberiaExterna_1.php";
                        break;
                    case "EXTERNA2":
                        url_crtubo = "http://10.10.20.15/backend/api/ar_tTuberiaExterna_2.php";
                        break;
                    case "EXTERNA3":
                        url_crtubo = "http://10.10.20.15/backend/api/ar_tTuberiaExterna_3.php";
                        break;

                    default:
                        break;
                }

                try
                {
                    var datos_de_envio = new Tabla_exin
                    {
                        T_ID_tubo = id_tubo,
                        T_No_tubo = txbCrTuboNotubo.Text,
                        T_No_placa = txbCrTuboNoplaca.Text,
                        T_ID_proyecto = ID_proyecto,
                        T_Lote_fundente = txbCrTuboLotefundente.Text,
                        T_Lote_alambre = txbCrTuboLotealambre.Text,
                        T_Maquina = opcion_maquina,
                        T_FolioOperador = txbCrTuboFolioOperador.Text,
                        T_Fecha = fecha_envio,
                        T_Hora = hora_envio,
                        T_Hora_db = hora_db,
                        T_Archivos_excel = "",
                        T_Observaciones = txbCrTuboObs.Text
                    };


                    var json = JObject.FromObject(datos_de_envio);
                    var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                    var response = Consultas.Post_API(url_crtubo, content);
                    MessageBox.Show("Se guardo el registro");

                }
                catch (Exception err)
                {
                    MessageBox.Show("Error en enviar a BD\n error: " + err.Message);

                }
            }
            
        }

        private void btnConsultaTubo_Click(object sender, EventArgs e)
        {
            if(txbIDtubo.Text != "" && cmbReTuboIDproyecto.Text != "")
            {
                //busqueda en la tablas de soldadura interna del tubo
                string url_tubo;
                List<Tabla_exin> datos = new List<Tabla_exin>();

                int po = cmbReTuboIDproyecto.Text.IndexOf("-");
                string ID_proyecto = cmbReTuboIDproyecto.Text.Substring(0, po);

                url_tubo = "http://10.10.20.15/backend/api/ar_tTuberiaInterna_";

                datos = Obtener_datos_tubo(url_tubo, txbIDtubo.Text, ID_proyecto, "INTERNA");

                //Si hay un elemento dentro de la lista, la función List.Any() devuelve true; de lo contrario, false
                if (datos.Any())
                {
                    foreach (var r in datos)
                    {
                        lblDatosTuboInterna.Text = "No Tubo: " + r.T_No_tubo + "\n" +
                                                   "No Placa: " + r.T_No_placa + "\n" +
                                                   "Maquina: " + r.T_Maquina + "\n" +
                                                   "Fecha: " + r.T_Fecha + "\n" +
                                                   "Hora: " + r.T_Hora + "\n" +
                                                   "Folio operador: " + r.T_FolioOperador + "\n" +
                                                   "Reporte: " + r.T_Reporte_excel;
                    }

                }
                else
                {
                    lblDatosTuboInterna.Text = "no hay datos en soldadura interna";
                }

                url_tubo = "http://10.10.20.15/backend/api/ar_tTuberiaExterna_";

                datos = Obtener_datos_tubo(url_tubo, txbIDtubo.Text,ID_proyecto, "EXTERNA");

                if (datos.Any())
                {
                    foreach (var r in datos)
                    {
                        lblDatosTuboExterna.Text = "No Tubo: " + r.T_No_tubo + "\n" +
                                                   "No Placa: " + r.T_No_placa + "\n" +
                                                   "Maquina: " + r.T_Maquina + "\n" +
                                                   "Fecha: " + r.T_Fecha + "\n" +
                                                   "Hora: " + r.T_Hora + "\n" +
                                                   "Folio operador: " + r.T_FolioOperador + "\n" +
                                                   "Reporte: " + r.T_Reporte_excel;

                    }
                }
                else
                {
                    lblDatosTuboExterna.Text = "no hay datos en soldadura externa";
                }
            }
            


        }

        private void btnUpTuboBuscar_Click(object sender, EventArgs e)
        {
            if (txbUpTuboNotubo.Text != "" && cmbUpTuboSoldadura.Text != "" && cmbUpTuboIDproyecto.Text != "")
            {
                string url_tubo;
                List<Tabla_exin> datos = new List<Tabla_exin>();
                int po = cmbUpTuboIDproyecto.Text.IndexOf("-");
                string ID_proyecto = cmbUpTuboIDproyecto.Text.Substring(0, po);

                if (cmbUpTuboSoldadura.Text == "INTERNA")
                {
                    url_tubo = "http://10.10.20.15/backend/api/ar_tTuberiaInterna_";
                    datos = Obtener_datos_tubo(url_tubo, txbUpTuboNotubo.Text, ID_proyecto, "INTERNA");

                }
                else
                {
                    url_tubo = "http://10.10.20.15/backend/api/ar_tTuberiaExterna_";
                    datos = Obtener_datos_tubo(url_tubo, txbUpTuboNotubo.Text, ID_proyecto, "EXTERNA");
                }

                txbUpTuboFecha.Text = datos[0].T_Fecha;
                txbUpTuboHora.Text = datos[0].T_Hora;
                txbUpTuboFoliooperador.Text = datos[0].T_FolioOperador;
                cmbUpTuboIDproyecto.Text = datos[0].T_ID_proyecto + "-";
                cmbUpTuboMaquina.Text = datos[0].T_Maquina;
                txbUpTuboLotefundente.Text = datos[0].T_Lote_fundente;
                txbUpTuboLotealambre.Text = datos[0].T_Lote_alambre;
                txbUpTuboObservaciones.Text = datos[0].T_Observaciones;
                txbUpTuboNoplaca.Text = datos[0].T_No_placa;
            }

        }

        List<Tabla_exin> Obtener_datos_tubo(string url_inex, string numero_tubo, string ID_proyecto, string exin)
        {
            List<Tabla_exin> datos_tubo = new List<Tabla_exin>();
            string url_tubo, tubo_datos;
            for (int i = 1; i <= 3; i++)
            {
                url_tubo = url_inex + i.ToString() + ".php" + "?T_No_tubo=" + numero_tubo + "&T_ID_proyecto=" + ID_proyecto;

                tubo_datos = Consultas.Get_API(url_tubo);
                if (tubo_datos != "null")
                {
                    datos_tubo = JsonConvert.DeserializeObject<List<Tabla_exin>>(tubo_datos);
                    datos_tubo[0].T_Maquina = exin + i.ToString();
                }

            }
            return datos_tubo;
        }

        private void btnUpdateTubo_Click(object sender, EventArgs e)
        {
            string url_actubo, hora_db;
            int po = cmbUpTuboIDproyecto.Text.IndexOf("-");
            string ID_proyecto = cmbUpTuboIDproyecto.Text.Substring(0, po);
            DateTime hora_db_dt;

            po = cmbUpTuboMaquina.Text.IndexOf("A");
            string no_maquina = cmbUpTuboMaquina.Text.Substring(po + 1);

            hora_db_dt = Convert.ToDateTime(txbUpTuboFecha.Text + " " + txbUpTuboHora.Text);
            hora_db = hora_db_dt.ToString("yyyy-MM-dd HH:mm:ss");

            if (cmbUpTuboSoldadura.Text == "INTERNA")
            {
                url_actubo = "http://10.10.20.15/backend/api/ar_tTuberiaInterna_" + no_maquina + ".php";
            }
            else
            {
                url_actubo = "http://10.10.20.15/backend/api/ar_tTuberiaExterna_" + no_maquina + ".php";
            }

            

            try
            {
                var datos_de_envio = new Tabla_exin
                {
                    T_No_tubo = txbUpTuboNotubo.Text,
                    T_No_placa = txbUpTuboNoplaca.Text,
                    T_ID_proyecto = ID_proyecto,
                    T_Lote_alambre = txbUpTuboLotealambre.Text,
                    T_Lote_fundente = txbUpTuboLotefundente.Text,
                    T_FolioOperador = txbUpTuboFoliooperador.Text,
                    T_Fecha = txbUpTuboFecha.Text,
                    T_Hora = txbUpTuboHora.Text,
                    T_Hora_db = hora_db,
                    T_Observaciones = txbUpTuboObservaciones.Text
                };


                var json = JObject.FromObject(datos_de_envio);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = Consultas.Update_API(url_actubo, content);

                MessageBox.Show("Registro actualizado");

            }
            catch (Exception err)
            {
                MessageBox.Show("Error en enviar a BD, \n error: " + err.Message);

            }
        }


        private void txbCrTuboFolioOperador_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Admin_permitidas_tubo_placa.Contains(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txbCrTuboLotealambre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Admin_permitidas_lote_alambre.Contains(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txbCrTuboNotubo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Admin_permitidas_tubo_placa.Contains(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txbCrTuboFecha_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Admin_permitidas_fecha.Contains(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txbCrTuboHora_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Admin_permitidas_hora.Contains(e.KeyChar))
            {
                e.Handled = true;
            }
        }


        #endregion

        #region Tuberia

        void Iniciar_tabla_tuberia()
        {
            //P_Tuberia_datatable.Columns.Add("T_id_tubo");
            P_Tuberia_datatable.Columns.Add("No_tubo");
            P_Tuberia_datatable.Columns.Add("No placa");
            P_Tuberia_datatable.Columns.Add("Fecha");
            P_Tuberia_datatable.Columns.Add("Hora");
            P_Tuberia_datatable.Columns.Add("Reporte excel");
            P_Tuberia_datatable.Columns.Add("Archivos excel");
            P_Tuberia_datatable.Columns.Add("ID proyecto");
            P_Tuberia_datatable.Columns.Add("Lote alambre");
            P_Tuberia_datatable.Columns.Add("Lote fundente");
            P_Tuberia_datatable.Columns.Add("Folio Operador");
            P_Tuberia_datatable.Columns.Add("Hora DB");
            P_Tuberia_datatable.Columns.Add("Observaciones");

        }

        public void Rellenar_tabla_datos(string url)
        {
            //llena la tabla de datos para tuberia de los registros solicitados por fecha dada
            P_Tuberia_datatable.Rows.Clear();
            try
            {

                var output = Consultas.Get_API(url + "?T_Fecha=" + dtpFechaTuberia.Text);

                List<Tabla_exin> temporal_results = JsonConvert.DeserializeObject<List<Tabla_exin>>(output);
                foreach (var r in temporal_results)
                {
                    P_Tuberia_datatable.Rows.Add(r.T_No_tubo, r.T_No_placa, r.T_Fecha, r.T_Hora, r.T_Reporte_excel,
                                                 r.T_Archivos_excel, r.T_ID_proyecto, r.T_Lote_alambre,
                                                 r.T_Lote_fundente, r.T_FolioOperador, r.T_Hora_db, r.T_Observaciones);

                }

            }
            catch (Exception err)
            {
                MessageBox.Show("Rellenar tabla:" + err.Message);

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DgvTablatuberia.DataSource = null;
            P_Tuberia_datatable.DefaultView.RowFilter = "No_tubo NOT IN (.)";
            P_Tuberia_datatable.Rows.Clear();
            DgvTablatuberia.DataSource = P_Tuberia_datatable;

            //P_extra = false;
            //designar a que tabla de soldadura sera guardado el dato del archivo excel
            if (cmbMaquina.Text != "")
            {
                if (cmbMaquina.Text == "INTERNA1")
                {
                    P_url_fecha = "http://10.10.20.15/backend/api/ar_tTuberiaInterna_1.php";
                }
                else if (cmbMaquina.Text == "INTERNA2")
                {
                    P_url_fecha = "http://10.10.20.15/backend/api/ar_tTuberiaInterna_2.php";
                }
                else if (cmbMaquina.Text == "INTERNA3")
                {
                    P_url_fecha = "http://10.10.20.15/backend/api/ar_tTuberiaInterna_3.php";
                }
                else if (cmbMaquina.Text == "EXTERNA1")
                {
                    P_url_fecha = "http://10.10.20.15/backend/api/ar_tTuberiaExterna_1.php";
                }
                else if (cmbMaquina.Text == "EXTERNA2")
                {
                    P_url_fecha = "http://10.10.20.15/backend/api/ar_tTuberiaExterna_2.php";
                }
                else if (cmbMaquina.Text == "EXTERNA3")
                {
                    P_url_fecha = "http://10.10.20.15/backend/api/ar_tTuberiaExterna_3.php";
                }

                Rellenar_tabla_datos(P_url_fecha);
                DgvTablatuberia.DataSource = P_Tuberia_datatable;
            }
            else
            {
                MessageBox.Show("Seleccione maquina");
            }

        }

        #endregion

        #region JSON sepa
        private void btnConvertir_Click(object sender, EventArgs e)
        {
            string url = "http://10.10.20.15/backend/api/ar_tTuberiaInterna_1.php";
            var content = new StringContent(lblJSON.Text, Encoding.UTF8, "application/json");
            var response = Consultas.Post_API(url, content);
            lblRespuesta.Text = response;
        }

        private void btnJSON_Click(object sender, EventArgs e)
        {
            string id_tubo = "54123", Notubo = "5421", Noplaca = "T12", IDproyecto = "8";
            string Lotealambre = "12/34", Lotefundente = "F1234", Maquina = "INTERNA1", Folio = "554433";
            string fecha_envio = "2022/01/22", hora_envio = "07:50:00 am", hora_db = "2022-01-22 07:50:00", observaciones = "test de datos";
            var cultureInfo = new CultureInfo("de-DE");
            var datos_de_envio = new Tabla_exin
            {
                T_ID_tubo = id_tubo,
                T_No_tubo = Notubo,
                T_No_placa = Noplaca,
                T_ID_proyecto = IDproyecto,
                T_Lote_fundente = Lotefundente,
                T_Lote_alambre = Lotealambre,
                T_Maquina = Maquina,
                T_FolioOperador = Folio,
                T_Fecha = fecha_envio,
                T_Hora = hora_envio,
                T_Hora_db = hora_db,
                T_Archivos_excel = "",
                T_Observaciones = observaciones
            };

            //string jsonString = JsonSerializer.Serialize(datos_de_envio);
            var json = JObject.FromObject(datos_de_envio);
            lblJSON.Text = json.ToString();
        }

        #endregion

        public frmAPIs()
        {
            InitializeComponent();
        }

        private void frmAPIs_Load(object sender, EventArgs e)
        {
            for (int i = 1; i <= 3; i++)
            {
                cmbMaquina.Items.Add("EXTERNA" + i);
                cmbCrTuboMaquina.Items.Add("EXTERNA" + i);
            }

            for (int i = 1; i <= 3; i++)
            {
                cmbMaquina.Items.Add("INTERNA" + i);
                cmbCrTuboMaquina.Items.Add("INTERNA" + i);
            }

            //combo de update tubo
            cmbUpTuboSoldadura.Items.Add("INTERNA");
            cmbUpTuboSoldadura.Items.Add("EXTERNA");

            btnDeletePro.Enabled = false;
            Iniciar_combobox_proyectos();
            Iniciar_tabla_operadores();
            txbFolioOperador.Text = "Escribe el Folio";
            Iniciar_tabla_tuberia();
            
        }


        #region funciones no usadas

        private void btnAsignarfechadb_Click(object sender, EventArgs e)
        {
            int numero_filas = DgvTablatuberia.Rows.Count;

            string id_tubo, hora_db, fecha_envio, hora_envio;
            DateTime hora_db_dt;

            for (int i = 0; i < numero_filas - 1; i++)
            {
                id_tubo = DgvTablatuberia.Rows[i].Cells[0].Value.ToString();
                fecha_envio = DgvTablatuberia.Rows[i].Cells[7].Value.ToString();
                hora_envio = DgvTablatuberia.Rows[i].Cells[8].Value.ToString();
                hora_db_dt = Convert.ToDateTime(fecha_envio + " " + hora_envio);
                hora_db = hora_db_dt.ToString("yyyy-MM-dd HH:mm:ss");
                Dictionary<string, string> diccionario = new Dictionary<string, string>
                {
                    {"T_ID_tubo", id_tubo },
                    {"T_Hora_db",hora_db }

                };
                var json = JsonConvert.SerializeObject(diccionario);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                Consultas.Update_API(P_url_fecha, content);
            }

        }

        private void btnModificarUno_Click(object sender, EventArgs e)
        {

            int numero_filas = DgvTablatuberia.Rows.Count;
            int i = Convert.ToInt16(txbTuboModificar.Text);
            string id_tubo, hora_db, fecha_envio, hora_envio;
            DateTime hora_db_dt;
            id_tubo = DgvTablatuberia.Rows[i].Cells[0].Value.ToString();
            fecha_envio = DgvTablatuberia.Rows[i].Cells[7].Value.ToString();
            hora_envio = DgvTablatuberia.Rows[i].Cells[8].Value.ToString();
            hora_db_dt = Convert.ToDateTime(fecha_envio + " " + hora_envio);
            hora_db = hora_db_dt.ToString("yyyy-MM-dd HH:mm:ss");
            Dictionary<string, string> diccionario = new Dictionary<string, string>
                {
                    {"T_ID_tubo", id_tubo },
                    {"T_Hora_db",hora_db }

                };
            var json = JsonConvert.SerializeObject(diccionario);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            Consultas.Update_API(P_url_fecha, content);
        }

        #endregion


        private void tbpOperadores_Click(object sender, EventArgs e)
        {

        }

        
    }
}
