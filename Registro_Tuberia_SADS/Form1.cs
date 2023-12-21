using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Reflection;

namespace Registro_Tuberia_SADS
{
    public partial class frmPrincipal : Form
    {
        //Variable para las solicitudes o consultas a al servidor 
        private static readonly HttpClient cliente = new HttpClient();
        public static  new string ProductVersion { get; }
        public int var_temporal;
        public string p_fecha_guardar, path_archivo_txt,p_formato_hora, p_hora_registro;
        public string p_ruta_archivo_respaldo;
        public bool p_tubo_repetido=false;
        public int p_numero_registros_actual;
        public long p_numero_archivo_respaldo;
        public char[] p_permitidas_tubo_placa = { '-', '0','1', '2', '3', '4', '5', '6', '7', '8', '9', (char)Keys.Back };
        public char[] p_permitidas_lote_alambre = { '/','-', '0', '1','2', '3', '4', '5', '6', '7', '8', '9', (char)Keys.Back };
        //Directory.GetCurrentDirectory() @"C:\"
        public string version_app = "1.0.2.0";

        public frmPrincipal()
        {
            InitializeComponent();
        }

        #region funciones para API REST con la db del SADS
        //funcion parar mandar las consultas a la API echa en php en el servidor o host

        public void insertApiData_tubo(string url,string id_tubo)
        {
            
            p_fecha_guardar= DateTime.Now.ToString("dd/MM/yyyy");
            string fecha_envio, hora_envio, hora_db;
            DateTime hora_db_dt;
            if ((txbHora.Enabled==true)&&(txbFecha.Enabled==true))
            {
                fecha_envio = txbFecha.Text;
                hora_envio = txbHora.Text;

            }
            else
            {
                fecha_envio = p_fecha_guardar;
                hora_envio = p_hora_registro.Replace(".","");
            }
            
            hora_db_dt = Convert.ToDateTime(fecha_envio+" "+ hora_envio);
            hora_db = hora_db_dt.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {

                var cultureInfo = new CultureInfo("es-MX");
                var datos_de_envio = new Tabla_exin
                {
                    T_ID_tubo = id_tubo,
                    T_No_tubo = txbNoTubo.Text,
                    T_No_placa = txbNoPlaca.Text,
                    T_ID_proyecto = lblIDproyecto.Text,
                    T_Lote_fundente = txbLoteFund.Text,
                    T_Lote_alambre = txbLoteAlam.Text,
                    T_Maquina = lblMaquina.Text,
                    T_FolioOperador = txbFolio.Text,
                    T_Fecha = fecha_envio,
                    T_Hora = hora_envio,
                    T_Hora_db =hora_db,
                    T_Archivos_excel = "",
                    T_Observaciones = txbObservaciones.Text
                };


                var json = JObject.FromObject(datos_de_envio);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var response = Consultas.Post_API(url, content);

            }
            catch (Exception err)
            {
                MessageBox.Show("Error en enviar a BD, se guardo respaldo en PC\n error: "+err.Message);

            }
            
        }

        //funcion para saber si hay datos del tubo a registrados
        //si no hay se hace registro nuevo
        //si hay se envia a la tabla estra
        void registro_tuberia(string urlm, string id_tubo)
        {
            
            insertApiData_tubo(urlm, id_tubo);

        }

        //Respaldo que realiza antes de enviar los datos a la API
        //Si la conexion falla el respaldo guarda los datos del tubo
        public void archivo_txt_respaldo()
        {
            string fecha_envio_txt,hora_envio_txt,version_archivo ="";
            p_fecha_guardar = DateTime.Now.ToString("dd/MM/yyyy");
            if ((txbHora.Enabled == true) && (txbFecha.Enabled == true))
            {
                fecha_envio_txt = txbFecha.Text;
                hora_envio_txt = txbHora.Text;

            }
            else
            {
                fecha_envio_txt = p_fecha_guardar;
                hora_envio_txt = p_hora_registro.Replace(".", "");
            }
            string linea_encabezado = "ID_tubo,No_tubo,No_placa,ID_proyecto,Maquina,"+
                                      "Lote_alambre,Lote_fundente,"+
                                       "Folio_operador,Fecha,Hora,Observaciones";

            string linea_registro =  txbNoPlaca.Text + txbNoTubo.Text + "," +
                                  txbNoTubo.Text + "," +
                                  txbNoPlaca.Text + "," +
                                  lblIDproyecto.Text + "," +
                                  lblMaquina.Text + "," +
                                  txbLoteAlam.Text + "," +
                                  txbLoteFund.Text + "," +
                                  txbFolio.Text + "," +
                                  fecha_envio_txt + "," +
                                  hora_envio_txt + "," +
                                  txbObservaciones.Text;

            //path_archivo_txt = @"C:\\Users\\Public\\Respaldo_registro_Maquina_" + lblMaquina.Text + "_v2.txt";

            version_archivo = $"_v2-{p_numero_archivo_respaldo.ToString()}.txt";
            //para no crear archivos taaaan grandes de registros se crean archvivos de solo 500 registros

            if (p_numero_registros_actual > 500)
            {
                p_numero_archivo_respaldo += 1;
                version_archivo = $"_v2-{p_numero_archivo_respaldo.ToString()}.txt";
                p_numero_registros_actual = 0;
                
            }

            //ruta donde guardaran los archivos de respaldo
            path_archivo_txt = p_ruta_archivo_respaldo + "\\Respaldo_registro_Maquina_" + lblMaquina.Text + version_archivo;
        
            if (!File.Exists(path_archivo_txt))
            {
                crear_archivo_txt(linea_encabezado, linea_registro, path_archivo_txt);
            }
            else
            {
                
                using (StreamWriter e_archivo = new StreamWriter(path_archivo_txt, true))
                {
                    e_archivo.WriteLine(linea_registro); //se agrega información al documento

                    e_archivo.Close();
                }
            }
            p_numero_registros_actual += 1;
            
        }

        void crear_archivo_txt(string linea1, string linea2, string ruta_archivo)
        {
            // Create a file to write to.
            using (StreamWriter c_archivo = File.CreateText(ruta_archivo))
            {
                c_archivo.WriteLine(linea1);
                c_archivo.WriteLine(linea2);
                c_archivo.Close();
            }

        }

        #endregion

        #region funcinalidades de formulario
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void tbcPrincipal_MouseMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void tbpOperador_MouseMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void tbpTuberia_MouseMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void PnlTitulo_MouseMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        void Orientacion_vertical()
        {
            //pestañas 
            LblVersion.Location = new Point(430, 26);
            lblTituloTuberia.Location = new Point(100, 3);
            this.Width = 600;
            this.StartPosition = FormStartPosition.WindowsDefaultLocation;
            tbcPrincipal.Width = 590;
            tbcPrincipal.Location = new Point(6, 2);
            //-------Pestaña Operador
            lblFolio.Location = new Point(2, 30);
            txbFolio.Location = new Point(100, 30);
            btnIngresar.Location = new Point(265, 21);
            btnSalirOperador.Location = new Point(425, 21);
            gpbIngresar.Width = 530;
            btnAjustes.Location = new Point(420, 50);
            btnActivarEdicion.Location = new Point(485, 50);
            lblIDp.Location = new Point(9, 390);
            lblIDproyecto.Location = new Point(169, 390);
            //--------------Pestaña tuberia
            lblNombreProyecto.Width = 520;
            btnDatosTuberia.Location = new Point(475, 250);
            lblHora.Location = new Point(200, 3);
            lblEalambre.Location = new Point(6, 156);
            lblTipoalambre.Location = new Point(126, 156);
            lblTipoalambre.Width = 500;
            lblEfundente.Location = new Point(6,185);
            lblTipoFundente.Location = new Point(126, 185);
            lblTipoFundente.Width = 500;
            lblEmaquina.Location = new Point(6, 214);
            lblMaquina.Location = new Point(106, 214);
            lblEdiametro.Location = new Point(6, 243);
            lblDiametro.Location = new Point(106, 243);
            lblEwps.Location = new Point(6, 272);
            lblWps.Location = new Point(106, 272);
            gpbDatossoldadura.Location = new Point(6, 290);
            gpbDatossoldadura.Width = 540;
            gpbDatossoldadura.Height = 255;
            txbObservaciones.Width = 270;
            txbObservaciones.Height = 58;
            txbObservaciones.Location = new Point(156, 193);
            lblEobservaciones.Location = new Point(7, 193);
            lblEobservaciones.Text = "Obs. :";
            btnGuardar.Location = new Point(393, 100);
            btnGuardar.Height = 75;
            btnGuardar.Width = 130;
            btnGuardar.ImageAlign = ContentAlignment.TopCenter;
            btnGuardar.TextAlign = ContentAlignment.BottomCenter;

        }

        void Orientacion_horizontal()
        {
            //pestañas
            LblVersion.Location = new Point(455, 26);
            lblTituloTuberia.Location = new Point(110, 3);
            this.Width = 720;
            this.StartPosition = FormStartPosition.CenterScreen;
            tbcPrincipal.Width = 710;
            tbcPrincipal.Location = new Point(10, 2);
            //----------Pestaña Operador
            lblFolio.Location = new Point(19, 30);
            txbFolio.Location = new Point(136, 30);
            btnIngresar.Location = new Point(311, 21);
            btnSalirOperador.Location = new Point(470, 21);
            gpbIngresar.Width = 588;
            btnAjustes.Location = new Point(470, 43);
            btnActivarEdicion.Location = new Point(534, 43);
            lblIDp.Location = new Point(9, 286);
            lblIDproyecto.Location = new Point(169, 286);
            //------- Pestaña tuberia
            lblNombreProyecto.Width = 260;
            btnDatosTuberia.Location = new Point(622, 113);
            lblHora.Location = new Point(279, 5);
            lblEalambre.Location = new Point(372, 40);
            lblTipoalambre.Location = new Point(500, 40);
            lblTipoalambre.Width = 167;
            lblEfundente.Location = new Point(372, 69);
            lblTipoFundente.Location = new Point(500, 69);
            lblTipoFundente.Width = 167;
            lblEmaquina.Location = new Point(372, 98);
            lblMaquina.Location = new Point(500, 98);
            lblEdiametro.Location = new Point(372, 127);
            lblDiametro.Location = new Point(500, 127);
            lblEwps.Location = new Point(6, 152);
            lblWps.Location = new Point(86, 152);
            gpbDatossoldadura.Location = new Point(7, 172);
            gpbDatossoldadura.Width = 660;
            txbObservaciones.Width = 259;
            txbObservaciones.Height = 75;
            txbObservaciones.Location = new Point(395, 58);
            lblEobservaciones.Location = new Point(392, 24);
            lblEobservaciones.Text = "Observaciones:";
            btnGuardar.Location = new Point(398, 142);
            btnGuardar.Height = 39;
            btnGuardar.Width = 158;
            btnGuardar.ImageAlign = ContentAlignment.MiddleLeft;
            btnGuardar.TextAlign = ContentAlignment.MiddleRight;
        }

        #endregion

        #region funciones para iniciar formulario y pestañas
        void Desabilitar_tuberia()
        {

            gpbDatossoldadura.Enabled = false;

        }

        void Iniciar_formulario()
        {
            //cargar variables de la app  guardadas la sesion pasada
            p_formato_hora = Properties.Settings.Default.Gformato_hora;
            tmrFechaHora.Enabled = true;
            lblMaquina.Text = Properties.Settings.Default.Gmaquina;
            lblNombreProyecto.Text = Properties.Settings.Default.Gproyecto;
            string orientacion = Properties.Settings.Default.Gfrmp_orientacion;
            p_numero_registros_actual = Properties.Settings.Default.Gnumero_registros_respaldo;
            p_numero_archivo_respaldo = Properties.Settings.Default.Gnumero_archivo_respaldo;
            p_ruta_archivo_respaldo = Properties.Settings.Default.Gpath_archivotexto;
            //si no se tien ruta para el respaldo se aigna una de default
            if(p_ruta_archivo_respaldo == "")
                p_ruta_archivo_respaldo = "C:\\Users\\Public";

            //iniciar objetos en el formulario
            LblVersion.Text = version_app;
            Cargar_datos_proyectos();
            btnGuardar.Enabled = false;
            this.Height = 300;
            tbcPrincipal.Height = 295;
            tbcPrincipal.Location = new Point(6, 5);
            Desabilitar_tuberia();
            //btnActivarEdicion.Visible = false;
            btnActivarEdicion.Enabled = false;
            btnDatosTuberia.Visible = false;
            txbFecha.Enabled = false;
            txbFecha.Visible = false;
            txbHora.Enabled = false;
            txbHora.Visible = false;
            txbFolio.Enabled = true;
            txbFolio.Text = "";
            btnIngresar.Enabled = true;
            btnAjustes.Enabled = true;
            lblNombre.Text = "XXXXXXXXXXXXXXX";
            lblClaveSold.Text = "XXXXXXXXXXX";
            lblMensajeerror.Text = "Ingrese Folio";
            if (orientacion == "VERTICAL")
            {
                Orientacion_vertical();
            }else
            {
                Orientacion_horizontal();
            }
            txbFolio.Select();
        }

        
        void Habilitar_tuberia()
        {
            gpbDatossoldadura.Enabled = true;
            btnDatosTuberia.Visible = true;
        }

        

        void Cargar_datos_proyectos()
        {
            string nombre_proyecto = lblNombreProyecto.Text;
            Properties.Settings.Default.Gproyecto = nombre_proyecto;
            
            if (nombre_proyecto!="")
            {
                int po = nombre_proyecto.IndexOf("-");
                lblIDproyecto.Text = nombre_proyecto.Substring(0, po);
                //colocar try ra ver si hay conexion, si no hay recibir y mandar a paantalla 
                //mensaje de error
                try
                {
                    var output_p = Consultas.Get_API("http://10.10.20.15/backend/api/ar_tProyectos.php?Pro_ID=" + lblIDproyecto.Text);
                    List<proyectos_tabla> results_p = JsonConvert.DeserializeObject<List<proyectos_tabla>>(output_p);
                    foreach (var r in results_p)
                    {
                        lblNombreProyecto.Text = r.Pro_Nombre;
                        lblOrdentrabajo.Text = r.Pro_OrdenTrabajo;
                        lblTipoalambre.Text = r.Pro_Alambre;
                        lblTipoFundente.Text = r.Pro_Fundente;
                        lblEspecificacion.Text = r.Pro_Especificacion;
                        lblDiametro.Text = r.Pro_Diametro.ToString();
                        lblEspesor.Text = r.Pro_Espesor.ToString();
                        lblWps.Text = r.Pro_WPS;

                    }
                }
                catch (Exception err_proyecto)
                {

                    MessageBox.Show("Error cargar proyecto: " + err_proyecto.Message+
                        "\n CHEQUE CONEXION CON LA BASE DE DATOS." );
                }
                
            }
            
        }
        //rutina de cerrar
        void cerrar_botones()
        {
            DialogResult r = MessageBox.Show("¿Esta seguro que quiere salir?",
                "Registro de Tuberia", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (r == DialogResult.Yes)
            {
                
                Properties.Settings.Default.Gmaquina = lblMaquina.Text;
                Properties.Settings.Default.Gproyecto = lblIDproyecto.Text+"-"+ lblNombreProyecto.Text;
                Properties.Settings.Default.Gordentrabajo = lblIDproyecto.Text + "-" + lblOrdentrabajo.Text;
                Properties.Settings.Default.Gformato_hora = p_formato_hora;
                Properties.Settings.Default.Gpath_archivotexto = p_ruta_archivo_respaldo;
                Properties.Settings.Default.Gnumero_registros_respaldo = p_numero_registros_actual;
                Properties.Settings.Default.Gnumero_archivo_respaldo = p_numero_archivo_respaldo;
                Properties.Settings.Default.Save();
                this.Close();
            }
        }

        #endregion

        #region cuadro de texto
        private void txbNoPlaca_KeyPress(object sender, KeyPressEventArgs e)
        {
            //agregar pra solo letras y numeros

        }

        private void txbNoTubo_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (!p_permitidas_tubo_placa.Contains(e.KeyChar))
            {
                e.Handled = true;
            }
           
            
        }

        private void txbFolio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                btnIngresar.PerformClick();
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                
                e.Handled = true;
            }

            
        }

        private void txbLoteAlam_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!p_permitidas_lote_alambre.Contains(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        #endregion

        #region formuralio y tick del reloj

        private void tmrFechaHora_Tick(object sender, EventArgs e)
        {
            lblHora.Text = DateTime.Now.ToString("hh:mm:ss tt");
            p_hora_registro = DateTime.Now.ToString("hh:mm:ss tt");
            if (p_formato_hora == "24horas")
            {
                lblHora.Text = DateTime.Now.ToString("HH:mm:ss");
            }

            lblFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            Iniciar_formulario();
            //<a href="https://www.flaticon.com/free-icons/minus" title="minus icons">Minus icons created by Aldo Cervantes - Flaticon</a>
            //<a href="https://www.flaticon.com/free-icons/cross" title="cross icons">Cross icons created by Freepik - Flaticon</a>
        }

              
        private void frmPrincipal_Activated(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Gcp == true)
            {
                string orientacion = Properties.Settings.Default.Gfrmp_orientacion;
                if(orientacion=="VERTICAL")
                {

                    Orientacion_vertical();
                    
                }
                else
                {
                    Orientacion_horizontal();
                    
                }
                lblNombreProyecto.Text = Properties.Settings.Default.Gproyecto;
                Cargar_datos_proyectos();
                lblMaquina.Text = Properties.Settings.Default.Gmaquina;
                p_formato_hora = Properties.Settings.Default.Gformato_hora;
                Properties.Settings.Default.Gcp = false;
                p_ruta_archivo_respaldo = Properties.Settings.Default.Gpath_archivotexto;
                if (p_ruta_archivo_respaldo == "")
                    p_ruta_archivo_respaldo = "C:\\Users\\Public";

            }
            
        }

        private void frmPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {
     
        }
        #endregion

        private void btnAPIs_Click(object sender, EventArgs e)
        {
            /*string valor = "CONTRASEÑA";
            string contraseña = inputboxvb.InputBox("CONTRASEÑA", "ESCRIBE", ref valor);

            if (contraseña == "SADS2023")
            {
                frmAPIs frm = new frmAPIs();
                frm.ShowDialog();
            }*/

            frmAPIs frm = new frmAPIs();
            frm.ShowDialog();

        } 
        //_________________________________________________________________
        #region funciones de los botones
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            cerrar_botones();
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var_temporal = 1;
            if ((txbLoteAlam.Text != "") && (txbLoteFund.Text != "") && (txbNoPlaca.Text != "") && (txbNoTubo.Text != ""))
            {

                string no_placa_g = txbNoPlaca.Text.Replace("-", "");
                string no_tubo_g = txbNoTubo.Text.Replace("-", "");
                string id_tubo = no_placa_g + no_tubo_g, urlm;
                //crear archivo de texto
                archivo_txt_respaldo();

                string opcion_maquina = lblMaquina.Text;

                switch (opcion_maquina)
                {
                    case "INTERNA1":
                        urlm = "http://10.10.20.15/backend/api/ar_tTuberiaInterna_1.php";
                        registro_tuberia(urlm, id_tubo);
                        break;
                    case "INTERNA2":
                        urlm = "http://10.10.20.15/backend/api/ar_tTuberiaInterna_2.php";
                        registro_tuberia(urlm, id_tubo);
                        break;
                    case "INTERNA3":
                        urlm = "http://10.10.20.15/backend/api/ar_tTuberiaInterna_3.php";
                        registro_tuberia(urlm, id_tubo);
                        break;
                    case "EXTERNA1":
                        urlm = "http://10.10.20.15/backend/api/ar_tTuberiaExterna_1.php";
                        registro_tuberia(urlm, id_tubo);
                        break;
                    case "EXTERNA2":
                        urlm = "http://10.10.20.15/backend/api/ar_tTuberiaExterna_2.php";
                        registro_tuberia(urlm, id_tubo);
                        break;
                    case "EXTERNA3":
                        urlm = "http://10.10.20.15/backend/api/ar_tTuberiaExterna_3.php";
                        registro_tuberia(urlm, id_tubo);
                        break;

                    default:
                        break;
                }


                txbNoPlaca.Text = "";
                txbNoTubo.Text = "";
                txbObservaciones.Text = "";
            }
            else
            {
                MessageBox.Show("Dejo sin llenar un cuadro de texto");
            }

        }

        /*private void btnCerrar2_Click(object sender, EventArgs e)
        {
            cerrar_botones();
        }*/
        private void button1_Click(object sender, EventArgs e)
        {
            string folio = txbFolio.Text;
            string url_api_operador = @"http://10.10.20.15/backend/api/ar_tOperadores.php?Op_Folio=";
            var output = Consultas.Get_API(url_api_operador + folio);

            List<operadores_tabla> results = JsonConvert.DeserializeObject<List<operadores_tabla>>(output);

            if ((output != "null") && (folio != ""))
            {
                foreach (var r in results)
                {
                    lblNombre.Text = r.Op_Nombre;
                    lblClaveSold.Text = "sc-" + r.Op_Clave_soldador;

                }

                if (lblNombreProyecto.Text != "")
                {

                    //Cargar_datos_proyectos();

                    btnIngresar.Enabled = false;
                    txbFolio.Enabled = false;
                    btnAjustes.Enabled = false;
                    btnGuardar.Enabled = true;
                    Habilitar_tuberia();
                    lblMensajeerror.Text = "Hablitada pestaña de registro de Tuberia";
                    btnActivarEdicion.Enabled = true;
                    btnActivarEdicion.Visible = true;
                    btnAPIs.Enabled = true;
                    btnAPIs.Visible = true;

                }
                else
                {
                    MessageBox.Show("Proyecto no configurado");
                    lblMensajeerror.Text = "Pulse el engrane y configure proyecto y maquina,\n despues de nuevo pulse ingresar";
                }

                if (Properties.Settings.Default.Gfrmp_orientacion == "VERTICAL")
                {
                    this.Height = 600;
                    tbcPrincipal.Height = 595;
                    tbcPrincipal.Location = new Point(6, 2);
                }
                else
                {
                    this.Height = 420;
                    tbcPrincipal.Height = 415;
                    tbcPrincipal.Location = new Point(6, 2);
                }


            }
            else
            {
                MessageBox.Show("Operador no registrado");
                lblMensajeerror.Text = "Folio no registrado";
            }

           /* if (txbFolio.Text == "554433")
            {
                

            }
            else
            {
                btnActivarEdicion.Enabled = false;
                btnActivarEdicion.Visible = false;
                btnAPIs.Enabled = false;
                btnAPIs.Visible = false;
            }*/
        }
        private void btnSalirOperador_Click(object sender, EventArgs e)
        {

            Iniciar_formulario();
        }

        private void btnAjustes_Click(object sender, EventArgs e)
        {
            //groupBox1.Enabled = false;
            //btnCerrar2.Enabled = false;
            //tbcPrincipal.Enabled = false;
            frmAjustes frm = new frmAjustes();
            frm.ShowDialog();
        }
        private void btnDatosTuberia_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Proyecto: " + lblNombreProyecto.Text + "\n" +
                            "Orden de trabajo: " + lblOrdentrabajo.Text + "\n" +
                            "Especificacion: " + lblEspecificacion.Text + ".\n" +
                            "Espesor: " + lblEspesor.Text + "\n" +
                            "Diametro: " + lblDiametro.Text + "\n" +
                            "Tipo de alambre: " + lblTipoalambre.Text + "\n" +
                            "Tipo de fundente: " + lblTipoFundente.Text + "\n" +
                            "Maquina: " + lblMaquina.Text + "\n" +
                            "WPS: " + lblWps.Text + "\n" +
                            "version app: " + version_app + "\n"
                            );
        }


       /* private void btnMinimizar2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }*/

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnActivarEdicion_Click(object sender, EventArgs e)
        {
            if ((txbFecha.Enabled == false) && (txbHora.Enabled == false))
            {
                txbFecha.Enabled = true;
                txbFecha.Visible = true;
                txbHora.Enabled = true;
                txbHora.Visible = true;
                lblFecha.Visible = false;
                lblHora.Visible = false;
            }
            else
            {
                txbFecha.Enabled = false;
                txbFecha.Visible = false;
                txbHora.Enabled = false;
                txbHora.Visible = false;
                lblFecha.Visible = true;
                lblHora.Visible = true;
            }

        }
        #endregion

        #region funciones no usadas y que si borro no sirve el formulario
       
        private void frmPrincipal_Validated(object sender, EventArgs e)
        {
            
        }

        private void frmPrincipal_Enter(object sender, EventArgs e)
        {
            
        }

        private void lblNombreProyecto_Click(object sender, EventArgs e)
        {

        }

        private void txbFolio_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void tbpOperador_Click(object sender, EventArgs e)
        {
            
        }

        private void tbpTuberia_Click(object sender, EventArgs e)
        {
            
        }

        private void txbNoPlaca_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbpOperador_Enter(object sender, EventArgs e)
        {
            
        }

        private void lblHora_Click(object sender, EventArgs e)
        {

        }

       

        private void tbcPrincipal_Enter(object sender, EventArgs e)
        {
            

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txbNoTubo_TextChanged(object sender, EventArgs e)
        {

        }
        #endregion


    }
}
