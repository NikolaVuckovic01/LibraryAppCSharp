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
    public partial class DodajForma : Form{
        KnjizaraDataSet dataSet;
        KnjizaraDataSetTableAdapters.ZanrTableAdapter ZanrTableAdapter;
        public DodajForma(){
            InitializeComponent();
            dataSet = new KnjizaraDataSet();
            ZanrTableAdapter = new KnjizaraDataSetTableAdapters.ZanrTableAdapter();
        }

        private void button2_Click(object sender, EventArgs e){
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e){
            if (textBox1.Text.Length == 0 || textBox2.Text.Length == 0 || numericUpDown1.Value == 0){
                MessageBox.Show("Popunite sva polja!");
            }
            else{
                OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Nikola\Knjizara.accdb");
                OleDbCommand com = new OleDbCommand(@"select max(id_knjiga) from knjiga", con);
                OleDbCommand com3 = new OleDbCommand(@"select id_zanr from zanr where naziv='" + comboBox1.SelectedValue + "'", con);
                con.Open();
                int br = Convert.ToInt32(com.ExecuteScalar()) + 1;
                con.Close();
                con.Open();
                int zanr = Convert.ToInt32(com3.ExecuteScalar());
                con.Close();
                for (int i = 0; i < 1; i++){
                    OleDbCommand com2 = new OleDbCommand(@"insert into knjiga(id_knjiga,autor,naziv,cena,popust,broj_strana) values(?,?,?,?,?,?)", con);
                    com2.Parameters.Add(new OleDbParameter { Value = br });
                    com2.Parameters.Add(new OleDbParameter { Value = textBox1.Text });
                    com2.Parameters.Add(new OleDbParameter { Value = textBox2.Text });
                    com2.Parameters.Add(new OleDbParameter { Value = numericUpDown1.Value });
                    com2.Parameters.Add(new OleDbParameter { Value = numericUpDown2.Value });
                    com2.Parameters.Add(new OleDbParameter { Value = numericUpDown3.Value });
                    con.Open();
                    com2.ExecuteNonQuery();
                    con.Close();
                    OleDbCommand com4 = new OleDbCommand(@"insert into Pripadnost(id_knjiga,id_zanr) values(?,?)", con);
                    com4.Parameters.Add(new OleDbParameter { Value = br });
                    com4.Parameters.Add(new OleDbParameter { Value = zanr });
                    con.Open();
                    com4.ExecuteNonQuery();
                    con.Close();
                }
                MessageBox.Show("Knjiga je dodana u bazu");
            }
        }
        private void DodajForma_Load(object sender, EventArgs e){
            ZanrTableAdapter.Fill(dataSet.Zanr);
            comboBox1.DataSource = dataSet.Zanr.Copy();
            comboBox1.DisplayMember = dataSet.Zanr.nazivColumn.ColumnName;
            comboBox1.ValueMember = dataSet.Zanr.nazivColumn.ColumnName;
            numericUpDown1.Maximum = 10000;
            numericUpDown2.Maximum = 100;
            numericUpDown3.Maximum = 10000;
            numericUpDown1.Minimum = 1;
            numericUpDown2.Minimum = 0;
            numericUpDown3.Minimum = 1;
        }
    }
}
