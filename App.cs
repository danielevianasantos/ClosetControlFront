
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Closetcontrol.Models;

namespace WardrobApp
{
    public partial class App : Form
    {
        public App()
        {
            InitializeComponent();
        }

        private void btnaddform_Click(object sender, EventArgs e)
        {
            if (Regex.IsMatch($"{txtmonth.Text}-{txtday.Text}-{txtyear.Text}", 
                @"^(0[1-9]|1[012])[- -.](0[1-9]|[12][0-9]|3[01])[- -.](19|20)+\d\d$"))
            {
                var roupa = new Roupa()
                {
                    Id = Math.Abs(Guid.NewGuid().GetHashCode()),
                    Tipo = clothestype.SelectedItem.ToString(),
                    Estilo = txtclothestyle.Text,
                    Tecido = typesoffabric.SelectedItem.ToString(),
                    Cor = colorsset.SelectedItem.ToString(),
                    Observacao = txtobsform.Text,
                    DatadeUso = $"{txtyear.Text}-{txtmonth.Text}-{txtday.Text}",
                };
                var Url = "http://localhost:59603/WardrobeControl/postone";
                var httpClient = new HttpClient();
                var request = httpClient.PostAsync(Url,
                    new StringContent(JsonConvert.SerializeObject(roupa),
                    Encoding.UTF8, "application/json"));
                request.Wait();

                var result = request.Result.Content.ReadAsStringAsync();
                result.Wait();
                if(result == null)
                    txtlistofclothesform.Text = "Desculpa bonitinha, mas você não tem essa roupa!";
                List<Roupa> minhalista = new List<Roupa>();
                minhalista = JsonConvert.DeserializeObject<List<Roupa>>(result.Result);
                txtlistofclothesform.Text = "";
                minhalista.ForEach(s => {
                    txtlistofclothesform.Text += s.Id + " " + s.Tipo + " " + s.Estilo + " "
                    + s.Cor + " " + s.Tecido + " " + s.Observacao + " " +
                    s.DatadeUso + Environment.NewLine;
                });
            }
            else
              txtlistofclothesform.Text = "Corrija a data, escreva os campos na ordem:\n DIA MÊS e ANO";
        }

        private void btnshowallform_Click(object sender, EventArgs e)
        {

            var Url = "http://localhost:59603/WardrobeControl/getallofthem";
            var httpClient = new HttpClient();
            var resultRequest = httpClient.GetAsync(Url);
            resultRequest.Wait();

            var result = resultRequest.Result.Content.ReadAsStringAsync();
            result.Wait();

            List<Roupa> minhalista = new List<Roupa>();
            minhalista = JsonConvert.DeserializeObject<List<Roupa>>(result.Result);
            txtlistofclothesform.Text = "";
            minhalista.ForEach(s => {
                txtlistofclothesform.Text += s.Id + " " + s.Tipo + " " + s.Estilo + " "
                + s.Cor + " " + s.Tecido + " " + s.Observacao + " " + DateTime.Parse(s.DatadeUso).ToString("dd/MM/yyyy")
                 + Environment.NewLine;
            });

        }

        private void btnseekform_Click(object sender, EventArgs e)
        {
            var Url = "http://localhost:59603/WardrobeControl/seekonebytype";
            var httpClient = new HttpClient();
            var resultRequest = httpClient.GetAsync($"{Url}?TipodaRoupa={clothestype.SelectedItem}");
            resultRequest.Wait();

            var result = resultRequest.Result.Content.ReadAsStringAsync();
            result.Wait();

            List<Roupa> minhalista = new List<Roupa>();
            minhalista = JsonConvert.DeserializeObject<List<Roupa>>(result.Result);
            txtlistofclothesform.Text = "";
            if(minhalista.Count!=0)
            {
                minhalista.ForEach(s =>
                {
                    txtlistofclothesform.Text += s.Id + " " + s.Tipo + " " + s.Estilo + " "
                    + s.Cor + " " + s.Tecido + " " + s.Observacao + " " +
                    s.DatadeUso + Environment.NewLine;
                });
            }
            else
                txtlistofclothesform.Text += "Desculpe bonitinha, mas você não tem essa roupa!";
        }

        private void btnalteraform_Click(object sender, EventArgs e)
        {
            if (Regex.IsMatch($"{txtmonth.Text}-{txtday.Text}-{txtyear.Text}",
                @"^(0[1-9]|1[012])[- -.](0[1-9]|[12][0-9]|3[01])[- -.](19|20)+\d\d$"))
            {
                var roupa = new Roupa()
                {
                    Id = Int32.Parse(txtiddaprocuradaform.Text),
                    Tipo = null,
                    Estilo = null,
                    Tecido = null,
                    Cor = null,
                    Observacao = txtobsform.Text,
                    DatadeUso = $"{txtyear.Text}-{txtmonth.Text}-{txtday.Text}",
                };
                var Url = "http://localhost:59603/WardrobeControl/changeinfoofone";
                var httpClient = new HttpClient();
                var request = httpClient.PostAsync(Url,
                    new StringContent(JsonConvert.SerializeObject(roupa),
                    Encoding.UTF8, "application/json"));
                request.Wait();
                var result = request.Result.Content.ReadAsStringAsync();
                result.Wait();

                txtlistofclothesform.Text = "";
                if (result.AsyncState != null)
                {
                    txtlistofclothesform.Text = bool.Parse(result.Result) ? "A informação da roupa foi atualizada!" :
                         "Desculpe bonitinha, mas você não tem essa roupa!";
                }
                else
                    txtlistofclothesform.Text = "Desculpe bonitinha, mas você não tem essa roupa!";
            }
            else
                txtlistofclothesform.Text = "Corrija a data, escreva os campos na ordem:\n DIA MÊS e ANO";

        }

        private void btndelroupaform_Click(object sender, EventArgs e)
        {
            var URL = "http://localhost:59603/WardrobeControl/deleteallwiththatid";
            var httpClient = new HttpClient();
            var resultRequestDelete = httpClient.DeleteAsync($"{URL}?IddaRoupa={txtiddaprocuradaform.Text}");
            resultRequestDelete.Wait();

            var resultDelete = resultRequestDelete.Result.Content.ReadAsStringAsync();
            resultDelete.Wait();
            txtlistofclothesform.Text = "Roupa removida do seu guarda-roupas!";
        }
    }
}
