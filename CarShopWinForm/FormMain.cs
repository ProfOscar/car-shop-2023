using CarShopLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarShopWinForm
{
    public partial class FormMain : Form
    {
        List<Veicolo> parcoMezzi;
        DbTools dbTools;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            dbTools = new DbTools(AppDomain.CurrentDomain.BaseDirectory + "ParcoMezzi.mdf");
            parcoMezzi = dbTools.CaricaDati();
            lbxVeicoli.DataSource = parcoMezzi;
            gbMoto.Location = gbAuto.Location;
        }

        private void lbxVeicoli_SelectedIndexChanged(object sender, EventArgs e)
        {
            Veicolo selezionato = (Veicolo)lbxVeicoli.SelectedItem;
            txtMarca.Text = selezionato.Marca;
            txtModello.Text = selezionato.Modello;
            if (selezionato is Auto)
            {
                gbAuto.Visible = true;
                gbMoto.Visible = false;
                Auto a = (Auto)selezionato;
                numPorte.Value = a.NumPorte;
                numDimCerchi.Value = a.DimCerchi;
            }
            else if (selezionato is Moto)
            {
                gbAuto.Visible = false;
                gbMoto.Visible = true;
                Moto m = (Moto)selezionato;
                cmbTipoMoto.SelectedItem = m.Tipo;
                numTempi.Value = m.NumTempi;
            }
        }
    }
}
