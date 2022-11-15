using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projekat2knjizara{
    public partial class GrafForma : Form{
        KnjizaraDataSet ds;
        KnjizaraDataSetTableAdapters.RacunTableAdapter ra;
        KnjizaraDataSetTableAdapters.Stavka_racunaTableAdapter sr;
        public GrafForma(){
            InitializeComponent();
            ds = new KnjizaraDataSet();
            ra = new KnjizaraDataSetTableAdapters.RacunTableAdapter();
            sr = new KnjizaraDataSetTableAdapters.Stavka_racunaTableAdapter();
        }
        private void GrafForma_Load(object sender, EventArgs e){
            ra.Fill(ds.Racun);
            sr.Fill(ds.Stavka_racuna);
        }
        private void button1_Click(object sender, EventArgs e){
            try{
                OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Nikola\Knjizara.accdb");
                OleDbCommand com1 = new OleDbCommand("select id_knjiga from knjiga where naziv=?", con);
                string str = textBox1.Text;
                com1.Parameters.Add(new OleDbParameter { Value = str });
                con.Open();
                int br = Convert.ToInt32(com1.ExecuteScalar());
                con.Close();
                OleDbCommand com2 = new OleDbCommand(@"SELECT SUM(Kolicina) FROM Stavka_racuna WHERE (id_knjiga = ?)", con);
                com2.Parameters.Add(new OleDbParameter { Value = br });
                OleDbCommand com3 = new OleDbCommand(@"SELECT SUM(Kolicina) FROM Stavka_racuna WHERE (id_knjiga <> ?)", con);
                com3.Parameters.Add(new OleDbParameter { Value = br });
                con.Open();
                if (br > 0){
                    chart1.Series["Izabrana knjiga"].Points.Clear();
                    chart1.Series["Ukupna kolicina"].Points.Clear();
                    int broj = Convert.ToInt32(com2.ExecuteScalar());
                    chart1.Series["Izabrana knjiga"].Points.AddY(broj);
                    con.Close();
                    con.Open();
                    int broj2 = Convert.ToInt32(com3.ExecuteScalar());
                    chart1.Series["Ukupna kolicina"].Points.AddY(broj2);
                    con.Close();
                }
            }
            catch{
                MessageBox.Show("Rezultati nisu pronadjeni");
            }
        }
    }
}
