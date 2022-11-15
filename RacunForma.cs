using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projekat2knjizara{
    public partial class RacunForma : Form{
        KnjizaraDataSet dsKnj;
        KnjizaraDataSetTableAdapters.RacunTableAdapter r1Da;
        KnjizaraDataSetTableAdapters.Stavka_racunaTableAdapter sr1Da;
        KnjizaraDataSetTableAdapters.KnjigaTableAdapter k1Da;
        public RacunForma(){
            InitializeComponent();
            dsKnj = new KnjizaraDataSet();
            r1Da = new KnjizaraDataSetTableAdapters.RacunTableAdapter();
            sr1Da = new KnjizaraDataSetTableAdapters.Stavka_racunaTableAdapter();
            k1Da = new KnjizaraDataSetTableAdapters.KnjigaTableAdapter();
        }
        private void RacunForma_Load(object sender, EventArgs e){
            r1Da.Fill(dsKnj.Racun);
            sr1Da.Fill(dsKnj.Stavka_racuna);
            k1Da.Fill(dsKnj.Knjiga);
            dataGridView1.DataSource = dsKnj.Stavka_racuna;
        }
        private void button1_Click(object sender, EventArgs e){
            string datum = monthCalendar1.SelectionRange.Start.ToShortDateString();
            string rez = datum.Remove(datum.Length - 1);
            var rows = (from sr in dsKnj.Stavka_racuna
                        join r in dsKnj.Racun on sr.id_racun equals r.id_racun
                        where r.datum.StartsWith(rez)
                        select sr);
            DataTable filtrirano = dsKnj.Stavka_racuna.Copy();
            filtrirano.Clear();
            foreach (DataRow row in rows){
                DataRow newrow = filtrirano.NewRow();
                for (int i = 0; i < filtrirano.Columns.Count; i++)
                    newrow[i] = row[i];
                filtrirano.Rows.Add(newrow);
            }
            dataGridView1.DataSource = filtrirano;
        }
        private void button2_Click(object sender, EventArgs e){
            int knjiga = Int32.Parse(textBox1.Text);
            var rows = (from k in dsKnj.Knjiga
                        where k.id_knjiga.Equals(knjiga)
                        select k);
            DataTable filtrirano = dsKnj.Knjiga.Copy();
            filtrirano.Clear();
            foreach (DataRow row in rows) {
                DataRow newrow = filtrirano.NewRow();
                for (int i = 0; i < filtrirano.Columns.Count; i++)
                    newrow[i] = row[i];
                filtrirano.Rows.Add(newrow);
            }
            dataGridView2.DataSource = filtrirano;
        }
    }
}
