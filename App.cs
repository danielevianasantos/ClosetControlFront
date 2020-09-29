
using Closetcontrol.Models;
using ClothesValidation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

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
           
                try
                {
                    var roupa = new Roupa()
                    {
                        Id = Math.Abs(Guid.NewGuid().GetHashCode()),
                        Tipo = clothestype.SelectedItem.ToString() ?? throw new ArgumentNullException("Escolha um tipo"),
                        Estilo = txtclothestyle.Text,
                        Tecido = typesoffabric.SelectedItem.ToString() ?? throw new ArgumentNullException("Escolha um tipo de tecido"),
                        Cor = colorsset.SelectedItem.ToString() ?? throw new ArgumentNullException("Escolha uma cor"),
                        Observacao = txtobsform.Text,
                        DatadeUso = $"{txtyear.Text}-{txtmonth.Text}-{txtday.Text}" ?? throw new ArgumentNullException("Insira uma data válida"),
                    };
                if (Validations.ValidaRoupa(roupa).IsValid) 
                {
                    var Url = "http://localhost:59603/WardrobeControl/postone";
                    var httpClient = new HttpClient();
                    var request = httpClient.PostAsync(Url,
                        new StringContent(JsonConvert.SerializeObject(roupa),
                        Encoding.UTF8, "application/json"));
                    request.Wait();

                    var result = request.Result.Content.ReadAsStringAsync();
                    result.Wait();
                    if (result == null)
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
                {
                    switch(Validations.ValidaRoupa(roupa).ErrorType)
                    {
                        case -1:
                            {
                                txtlistofclothesform.Text = "Corrija aí bonitinha, a data está incorreta";
                                break;
                            }
                        case -2:
                            {
                                txtlistofclothesform.Text = "Corrija aí bonitinha, insira um tipo registrado";
                                break;
                            }
                        case -3:
                            {
                                txtlistofclothesform.Text = "Corrija aí bonitinha, insira um tecido registrado";
                                break;
                            }
                    }
                }
                }
                catch (NullReferenceException) 
                { txtlistofclothesform.Text ="Corrija aí bonitinha, algum campo está errado";}
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
                txtlistofclothesform.Text += "Desculpe bonitinha, mas você não tem essa roupa ou o tipo informado está incorreto!";
        }

        private void btnalteraform_Click(object sender, EventArgs e)
        {
            int aux;
            if (Int32.TryParse(txtiddaprocuradaform.Text, out aux))
            {
                var roupa = new Roupa()
                {
                    Id = aux,
                    Tipo = null,
                    Estilo = null,
                    Tecido = null,
                    Cor = null,
                    Observacao = txtobsform.Text,
                    DatadeUso = $"{txtyear.Text}-{txtmonth.Text}-{txtday.Text}" ?? throw new ArgumentNullException("Insira uma data válida"),
                };
                var test = Validations.ValidaRoupa(roupa);
                if (test.IsValid)
                { 
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
                }
                else if(test.ErrorType == -1)
                {
                    txtlistofclothesform.Text = "Corrija aí bonitinha, a data está incorreta";

                }
            }
            else
                txtlistofclothesform.Text = "Desculpe bonitinha, verifique se escreveu o Id corretamente"+Environment.NewLine+"Ou corrija a data, escrevendo os campos na ordem:\n DIA MÊS e ANO";

        }

        private void btndelroupaform_Click(object sender, EventArgs e)
        {
            var URL = "http://localhost:59603/WardrobeControl/deleteallwiththatid";
            var httpClient = new HttpClient();
            var resultRequestDelete = httpClient.DeleteAsync($"{URL}?IddaRoupa={txtiddaprocuradaform.Text}");
            resultRequestDelete.Wait();

            var resultDelete = resultRequestDelete.Result.Content.ReadAsStringAsync();
            resultDelete.Wait();
            txtlistofclothesform.Text = (resultDelete.Result.Length==0)?"Id incorreto":"Roupa removida do seu guarda-roupas!";
        }
    }
}
