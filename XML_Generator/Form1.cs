using Excel;
using Parser.Core;
using Parser.Core.Habra;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace XML_Generator
{
    public partial class Form1 : Form
    {
        public IList<Product> products = new List<Product>();


        ParserWorker<string> descParser;
        ParserWorker<string[]> picParser;
        ParserWorker<List<Tuple<string, string>>> paramParser;


        public Form1()
        {
             InitializeComponent();

             descParser = new ParserWorker<string>( new DescriptionParser());
             descParser.OnNewData += Parser_OnNewDesc;

             picParser = new ParserWorker<string[]>(new PicturesParser());
             picParser.OnNewData += Parser_OnNewPic;

             paramParser = new ParserWorker<List<Tuple<string, string>>>(new ParamsParser());
             paramParser.OnNewData += Parser_OnNewParam;
        }
    

        public int iterator = 0;
        public int iterator2 = 0;
        public int iterator3 = 0;

        private void Parser_OnNewDesc(object arg1, string arg2)
        {
            iterator++;
            listBox1.Items.Add(arg2);
            products[iterator - 1].Description = arg2;
           
        } 
        private void Parser_OnNewPic(object arg1, string[] arg2)
        {
            iterator2++;
            listBox1.Items.AddRange(arg2);
            
            products[iterator2 - 1].Pictures = arg2.ToList();
        }
        private void Parser_OnNewParam(object arg1, List<Tuple<string, string>> arg2)
        {

            foreach (var pair in arg2)
            {
                string parametres = pair.Item1 + " : " + pair.Item2;
                listBox1.Items.Add(parametres);
            }

            iterator3++;
            products[iterator3 - 1].Params = arg2;

        }


        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add("Процесс начался, подождите..");

            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Excel Workbook|*.xls", ValidateNames = true })
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    FileStream fs = File.Open(ofd.FileName, FileMode.Open, FileAccess.Read);
                    IExcelDataReader reader = ExcelReaderFactory.CreateBinaryReader(fs);
                    reader.IsFirstRowAsColumnNames = true;
                    DataSet result = reader.AsDataSet();
                    DataTable dt = result.Tables[0];
                    label1.Text = ofd.FileName;

                    int id = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        id++;
                        if (!row.IsNull("Наименование  товара"))
                        {

                            products.Add(new Product
                            {
                                Id = id,
                                Name = row["Наименование  товара"].ToString(),
                                Link = row["Ссылка на товар"].ToString(),
                                Price = row["Цена розн., грн."].ToString(),
                            });

                        }

                    }

                    DescParse();
                    PicParse();
                    ParamParse();

                    reader.Close();
                }
        }


        private void button2_Click(object sender, EventArgs e)
        {
              CreateXml();
            
        }

        public async void DescParse()
        {
            int i = 0;
            foreach (var product in products)
            {
                i++;
                descParser.Settings = new SiteSettings(product.Link);
                await descParser.Start();
                progressBar1.Value = i;
                label2.Text = "Обработано " + i + "/" + products.Count.ToString();
            }
        }

        public async void PicParse()
        {
           
            foreach (var product in products)
            {
               
                picParser.Settings = new SiteSettings(product.Link);
                await picParser.Start();
                
            }
        }

        public async void ParamParse()
        {
            foreach (var product in products)
            {
                paramParser.Settings = new SiteSettings(product.Link);
                await paramParser.Start();
            }
        }

        public void CreateXml()
        {

            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
            new XDocumentType("yml_catalog", null, "shops.dtd", null));


            XElement yml = new XElement("yml_catalog");
            XAttribute ymlAttr = new XAttribute("date", DateTime.Now.ToString("yyyy-MM-dd hh:mm"));
            yml.Add(ymlAttr);

            XElement name = new XElement("name", "Sentex");
            XElement company = new XElement("company", "Sentex");
            XElement url = new XElement("url", "https://sentex.com.ua/");
            XElement currencies = new XElement("currencies");
            XElement currency = new XElement("currency");

            XAttribute currencyId = new XAttribute("id", "UAH");
            XAttribute currencyRate = new XAttribute("rate", "1");
            currency.Add(currencyId);
            currency.Add(currencyRate);
            currencies.Add(currency);

            XElement categories = new XElement("categories");
            XElement category = new XElement("category", "Apple");
            XAttribute categoryId = new XAttribute("id", "1");
            category.Add(categoryId);
            categories.Add(category);

            XElement shop = new XElement("shop");

            XElement offers = new XElement("offers");

            foreach (var product in products)
            {


                XElement offer = new XElement("offer");

                XAttribute offerId = new XAttribute("id", product.Id);
                XAttribute available = new XAttribute("available", "true");
                XElement Url = new XElement("url", "https://sentex.com.ua/");
                XElement Price = new XElement("price", product.Price);
                XElement CurrencyId = new XElement("currencyId", "UAH");
                XElement CategoryId = new XElement("categoryId", "1");
                XElement Vendor = new XElement("vendor", "Apple");
                XElement Stock = new XElement("stock_quantity", "50");
                XElement Name = new XElement("price", product.Name);
                string saveDesc = "NO INFO";
                if(product.Description != null)
                {
                    saveDesc = product.Description;
                }
               
                XElement Description = new XElement("description", new XCData(saveDesc));
                offer.Add(offerId);
                offer.Add(available);
                offer.Add(Url);
                offer.Add(Price);
                offer.Add(CurrencyId);
                offer.Add(CategoryId);

                List<string> savePics = new List<string>();
                savePics.Add("NO INFO");

                if (product.Pictures != null)
                {
                    savePics = product.Pictures;
                }

                foreach (var pic in savePics)
                {
                    XElement Picture = new XElement("picture", pic);
                    offer.Add(Picture);
                }

                offer.Add(Vendor);
                offer.Add(Stock);
                offer.Add(Name);
                offer.Add(Description);

                List<Tuple<string, string>> saveParams = new List<Tuple<string, string>>();
                Tuple<string, string> tempParam = Tuple.Create("No", "Info");
                saveParams.Add(tempParam);

                if (product.Params != null)
                {
                    saveParams = product.Params;
                }


                foreach (var param in saveParams)
                {
                    XAttribute ParamName = new XAttribute("name", param.Item1);
                    XElement Param = new XElement("param", param.Item2);
                    Param.Add(ParamName);
                    offer.Add(Param);
                }

                offers.Add(offer);
            }

            shop.Add(name);
            shop.Add(company);
            shop.Add(url);
            shop.Add(currencies);
            shop.Add(categories);
            shop.Add(offers);
            yml.Add(shop);
            xdoc.Add(yml);


            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\sentex.xml";
            xdoc.Save(filePath);

            System.Diagnostics.Process.Start(filePath);


        }

     
    }
}
