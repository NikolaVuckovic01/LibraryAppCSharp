using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projekat2knjizara{
    public partial class Form1 : Form{
        KnjizaraDataSet knjizaraDs;
        KnjizaraDataSetTableAdapters.KnjigaTableAdapter knjigaDa;
        KnjizaraDataSetTableAdapters.PripadnostTableAdapter pripadnostDa;
        KnjizaraDataSetTableAdapters.ZanrTableAdapter zanrDa;
        KnjizaraDataSetTableAdapters.RacunTableAdapter racunDa;
        KnjizaraDataSetTableAdapters.Stavka_racunaTableAdapter stavkaDa;
        string knjigaLabel1;
        string knjigaLabel2;
        string knjigaLabel3;

        public Form1(){
            InitializeComponent();
            knjizaraDs = new KnjizaraDataSet();
            
            knjigaDa = new KnjizaraDataSetTableAdapters.KnjigaTableAdapter();
            pripadnostDa = new KnjizaraDataSetTableAdapters.PripadnostTableAdapter();
            zanrDa = new KnjizaraDataSetTableAdapters.ZanrTableAdapter();
            racunDa = new KnjizaraDataSetTableAdapters.RacunTableAdapter();
            stavkaDa = new KnjizaraDataSetTableAdapters.Stavka_racunaTableAdapter();
        }
        private void Form1_Load(object sender, EventArgs e){
            knjigaDa.Fill(knjizaraDs.Knjiga);
            pripadnostDa.Fill(knjizaraDs.Pripadnost);
            zanrDa.Fill(knjizaraDs.Zanr);
            racunDa.Fill(knjizaraDs.Racun);
            stavkaDa.Fill(knjizaraDs.Stavka_racuna);

            comboBox1.DataSource = knjizaraDs.Zanr.Copy();
            comboBox1.DisplayMember = knjizaraDs.Zanr.nazivColumn.ColumnName;
            comboBox1.ValueMember = knjizaraDs.Zanr.id_zanrColumn.ColumnName;
            numericUpDown1.Minimum = 1;

            var rows = (from k in knjizaraDs.Knjiga
                        select k).OrderByDescending(x => x.naziv);

            DataTable filtrirano = knjizaraDs.Knjiga.Copy();
            filtrirano.Clear();
            foreach (DataRow row in rows)
            {
                DataRow newrow = filtrirano.NewRow();
                for (int i = 0; i < filtrirano.Columns.Count; i++)
                    newrow[i] = row[i];
                filtrirano.Rows.Add(newrow);
            }
            dataGridView1.DataSource = filtrirano;

            int[] niz_knjiga = new int[3];
            int[] niz_kolicina = new int[3];
            string st = DateTime.Today.ToShortDateString();
            string st2 = st.Remove(st.Length - 1);
            List<int> lista = new List<int>();
            OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Nikola\Knjizara.accdb");
            OleDbCommand com = new OleDbCommand(@"SELECT id_knjiga FROM Stavka_racuna WHERE(id_racun IN(SELECT id_racun FROM Racun WHERE(datum =?)))
                                                    GROUP BY id_knjiga ORDER BY SUM(Kolicina) DESC", con);
            com.Parameters.Add(new OleDbParameter { Value = st2 });
            con.Open();
            OleDbDataReader citac = com.ExecuteReader();
            while (citac.Read()){
                lista.Add(Convert.ToInt32(citac["id_knjiga"]));
            }
            con.Close();
            if (lista.Count < 3){
                knjigaLabel1 = "Nema rezultata";
                knjigaLabel2 = "Nema rezultata";
                knjigaLabel3 = "Nema rezultata";
            }
            else {
                int knjiga1 = lista[0];
                int knjiga2 = lista[1];
                int knjiga3 = lista[2];
                OleDbCommand com3Label = new OleDbCommand(@"select naziv from knjiga where id_knjiga=?", con);
                com3Label.Parameters.Add(new OleDbParameter { Value = knjiga1 });
                con.Open();
                knjigaLabel1 = Convert.ToString(com3Label.ExecuteScalar());
                con.Close();

                OleDbCommand com4Label = new OleDbCommand(@"select naziv from knjiga where id_knjiga=?", con);
                com4Label.Parameters.Add(new OleDbParameter { Value = knjiga2 });
                con.Open();
                knjigaLabel2 = Convert.ToString(com4Label.ExecuteScalar());
                con.Close();

                OleDbCommand com5 = new OleDbCommand(@"select naziv from knjiga where id_knjiga=?", con);
                com5.Parameters.Add(new OleDbParameter { Value = knjiga3 });
                con.Open();
                knjigaLabel3 = Convert.ToString(com5.ExecuteScalar());
                con.Close();
            }
            Task t = new Task(timer2_Tick);
            t.Start();
        }
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e){
            try{
                var rows = (from k in knjizaraDs.Knjiga
                            join p in knjizaraDs.Pripadnost on k.id_knjiga equals p.id_knjiga
                            where p.id_zanr.Equals(comboBox1.SelectedValue)
                            select k).OrderByDescending(x => x.naziv);
                DataTable filtrirano = knjizaraDs.Knjiga.Copy();
                filtrirano.Clear();
                foreach (DataRow row in rows){
                    DataRow newrow = filtrirano.NewRow();
                    for (int i = 0; i < filtrirano.Columns.Count; i++)
                        newrow[i] = row[i];
                    filtrirano.Rows.Add(newrow);
                }
                dataGridView1.DataSource = filtrirano;
            }
            catch (Exception ex){
                MessageBox.Show(ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e){
            var rows = (from k in knjizaraDs.Knjiga
                        select k).OrderByDescending(x => x.naziv);
            DataTable filtrirano = knjizaraDs.Knjiga.Copy();
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
            DialogResult dialogResult = MessageBox.Show("Dodaj na racun?\nBroj knjiga: " + numericUpDown1.Value, "", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes){
                foreach (DataGridViewRow item in dataGridView1.SelectedRows){
                    if (Convert.ToBoolean(item.Cells[0].Value)){
                        int n = dataGridView2.Rows.Add();
                        dataGridView2.Rows[n].Cells[0].Value = item.Cells[1].Value.ToString();
                        dataGridView2.Rows[n].Cells[1].Value = item.Cells[2].Value.ToString();
                        dataGridView2.Rows[n].Cells[2].Value = item.Cells[3].Value.ToString();
                        dataGridView2.Rows[n].Cells[3].Value = numericUpDown1.Value;
                        dataGridView2.Rows[n].Cells[4].Value = item.Cells[4].Value.ToString();
                        dataGridView2.Rows[n].Cells[5].Value = item.Cells[0].Value.ToString();
                    }
                }
            }
            else if (dialogResult == DialogResult.No){
                MessageBox.Show("Stavka  nije dodana na racun");
            }
            numericUpDown1.Value = 1;
        }
        private void timer1_Tick(object sender, EventArgs e){
            double cenaSaKolicinom = 0;
            double popust = 0;
            double cena3 = 0;
            double rezultat = 0;
            double rezultat2 = 0;
            double rezultat3 = 0;
            for (int i = 0; i < dataGridView2.Rows.Count; i++){
                if ((Convert.ToInt32(dataGridView2.Rows[i].Cells[4].Value)) != 0){
                    popust = (Convert.ToInt32(dataGridView2.Rows[i].Cells[4].Value));
                    popust /= 100;
                    cenaSaKolicinom += ((Convert.ToInt32((dataGridView2.Rows[i].Cells[2].Value))) * (Convert.ToInt32(dataGridView2.Rows[i].Cells[3].Value)));
                    rezultat = cenaSaKolicinom * popust;
                    rezultat2 = cenaSaKolicinom - rezultat;
                }
                else
                    cena3 += ((Convert.ToInt32((dataGridView2.Rows[i].Cells[2].Value))) * (Convert.ToInt32(dataGridView2.Rows[i].Cells[3].Value)));
                rezultat3 = rezultat2 + cena3;
            }
            label4.Text = rezultat3.ToString();
        }
        private void button3_Click(object sender, EventArgs e){
            dataGridView2.DataSource = null;
            dataGridView2.Rows.Clear();
        }
        private void button4_Click(object sender, EventArgs e){
            foreach (DataGridViewRow row in dataGridView2.SelectedRows){
                dataGridView2.Rows.RemoveAt(row.Index);
            }
        }
        private void button5_Click(object sender, EventArgs e){
            OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Nikola\Knjizara.accdb");
            OleDbCommand com2 = new OleDbCommand(@"select max(id_racun) from racun", con);
            con.Open();
            int id_br = Convert.ToInt32(com2.ExecuteScalar()) + 1;
            int id_br2 = Convert.ToInt32(com2.ExecuteScalar()) + 2;
            con.Close();
            int brojac = 0;
            for (int i = 0; i < dataGridView2.Rows.Count - 1; i++){
                for (int j = i + 1; j < dataGridView2.Rows.Count; j++){
                    if (Convert.ToInt32(dataGridView2.Rows[i].Cells[5].Value) == Convert.ToInt32(dataGridView2.Rows[j].Cells[5].Value)){
                        MessageBox.Show("Preklapanje knjiga. Pokusajte ponovo.");
                        brojac = 1;
                        break;
                    }
                }
                break;
            }
            if (brojac == 1)
                return;
            for (int j = 0; j < 1; j++){
                OleDbCommand com = new OleDbCommand(@"insert into Racun(id_racun, datum, ukupna_cena) values(?,?,?) ", con);
                com.Parameters.Add(new OleDbParameter { Value = id_br });
                com.Parameters.Add(new OleDbParameter { Value = DateTime.Now.ToString("dd/M/yyyy") });
                com.Parameters.Add(new OleDbParameter { Value = label4.Text });
                con.Open();
                com.ExecuteNonQuery();
                con.Close();
            }
            for (int j = 0; j < dataGridView2.Rows.Count; j++){
                var id = dataGridView2.Rows[j].Cells[5].Value;
                var pop = dataGridView2.Rows[j].Cells[4].Value;
                var cena = dataGridView2.Rows[j].Cells[2].Value;
                var kol = dataGridView2.Rows[j].Cells[3].Value;
                OleDbCommand com3 = new OleDbCommand(@"insert into Stavka_racuna(id_racun, id_knjiga, cena, popust, kolicina) values(?,?,?,?,?) ", con);
                com3.Parameters.Add(new OleDbParameter { Value = id_br });
                com3.Parameters.Add(new OleDbParameter { Value = id });
                com3.Parameters.Add(new OleDbParameter { Value = cena });
                com3.Parameters.Add(new OleDbParameter { Value = pop });
                com3.Parameters.Add(new OleDbParameter { Value = kol });
                con.Open();
                com3.ExecuteNonQuery();
                con.Close();
            }
            numericUpDown1.Value = 1;
            dataGridView2.Rows.Clear();
            MessageBox.Show("Uspesno potvrdjeno");
        }
        private void button6_Click(object sender, EventArgs e){
            DodajForma df = new DodajForma();
            df.ShowDialog();
        }
        private void button7_Click(object sender, EventArgs e){
            RacunForma rf = new RacunForma();
            rf.ShowDialog();
        }
        private void button8_Click(object sender, EventArgs e){
            GrafForma gf = new GrafForma();
            gf.ShowDialog();
        }
        private void timer2_Tick(){
            label6.Text = knjigaLabel1;
            label7.Text = knjigaLabel2;
            label8.Text = knjigaLabel3;
            while (true){
                label6.ForeColor = Color.Red;
                label7.ForeColor = Color.Red;
                label8.ForeColor = Color.Red;
                Thread.Sleep(2000);
                label6.ForeColor = Color.Blue;
                label7.ForeColor = Color.Blue;
                label8.ForeColor = Color.Blue;
                Thread.Sleep(2000);
                label6.ForeColor = Color.Green;
                label7.ForeColor = Color.Green;
                label8.ForeColor = Color.Green;
                Thread.Sleep(2000);
                label6.ForeColor = Color.Orange;
                label7.ForeColor = Color.Orange;
                label8.ForeColor = Color.Orange;
                Thread.Sleep(2000);
                label6.ForeColor = Color.Purple;
                label7.ForeColor = Color.Purple;
                label8.ForeColor = Color.Purple;
                Thread.Sleep(2000);
                label6.ForeColor = Color.Coral;
                label7.ForeColor = Color.Coral;
                label8.ForeColor = Color.Coral;
                Thread.Sleep(2000);
                label6.ForeColor = Color.Black;
                label7.ForeColor = Color.Black;
                label8.ForeColor = Color.Black;
            }
        }
    }
}
